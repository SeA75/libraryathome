using LibraryAtHomeProvider;
using LibraryAtHomeRepositoryDriver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibraryAtHomeTracerFileMetadataExtractor;
using LibraryAtHomeTracer;

namespace BooksParser
{
    /// <summary>
    /// 
    /// </summary>
    public class LibraryCataloguer
    {

        private IProgress<double> Progress { get; set; }

        private readonly IMongodbConnection Connection;

        private readonly BooksCollectedDataMapper BooksInLibrary;

        private readonly BookToBeReviewedDataMapper BookToReview;

        private readonly LibraryStatisticsDataMapper LibStatistics;

        private readonly IBookParserTrace _tracer;

        private ConcurrentQueue<Exception> _exceptions;

        private readonly Dictionary<String, Delegate> GetMetadataFromFileDictionaryDelegate;
       
        private IList<string> Files { get; set; }

        private BookParserConfig Configuration { get; set; }


        public LibraryCataloguer(BookParserConfig configuration, ConcurrentQueue<Exception> exceptions, 
            IBookParserTrace tracer, IProgress<double> progress)
        {
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            GetMetadataFromFileDictionaryDelegate = new Dictionary<String, Delegate>() {
                            {".pdf",  new Func<string, BookatHome>(new PdfTextReaderFileInfoExtractor().GetPocoBook) },
                            {".txt",  new Func<string, BookatHome>(new FileInfoExtractor().GetPocoBook )},
                            {".rtf",  new Func<string, BookatHome>(new RtfFileInfoExtractor().GetPocoBook )},
                            {".epub", new Func<string, BookatHome>(new EpubFileInfoExtractor().GetPocoBook )  },
                            {".lit", new Func<string, BookatHome>(new LitFileInfoExtractor().GetPocoBook ) },
                             {".doc", new Func<string, BookatHome>(new FileInfoExtractor().GetPocoBook) }};


            Connection = new MongodbConnection();

            BooksInLibrary = new BooksCollectedDataMapper(Connection);

            BookToReview = new BookToBeReviewedDataMapper(Connection);

            LibStatistics = new LibraryStatisticsDataMapper(Connection);

            _tracer = tracer;

            _exceptions = exceptions;

            Init();

        }

        public async Task CatalogBooksAsync()
        {
            _tracer.TraceInfo("Start CatalogBooksAsync");

            var (collectedBooks, discardedBooks) = CollectBooks();

            await PutBookOnDatabase(collectedBooks, discardedBooks).ConfigureAwait(false);

            DeleteAlreadyCatalogedBooks();

            _tracer.TraceInfo("End CatalogBooksAsync");
        }

        public void CreateStatistics(TimeSpan elapsedTime)
        {
            LibStatistics.Write(new LibraryStatistics(BookToReview.Count() + BooksInLibrary.Count(), BooksInLibrary.Count(), elapsedTime, Configuration.ebookdirectory));
        }

        private void Init()
        {
            Regex reSearchPattern = new Regex(string.Join("|", Configuration.ebookformat.ToArray()), RegexOptions.IgnoreCase);

            IList<string> fileintofolder = Directory.EnumerateFiles(Configuration.ebookdirectory, "*.*", SearchOption.AllDirectories)
                .Where(x => reSearchPattern.IsMatch(Path.GetExtension(x).ToLowerInvariant())).ToList<string>();

            var filesCataloged = from cataloged in BooksInLibrary.Read()
                                 select cataloged.File;

            Files = fileintofolder.Except<string>(filesCataloged).ToList();

        }

        private BookatHome SearchBookInfoOfFile(string file)
        {
            _tracer.TraceInfo("SearchBookInfoOfFile start for file {0}", file);
            
            try
            {
                PocoBook minimalbookinfo = GetMetadataFromFileDictionaryDelegate[Utils.GetExtension(file)].DynamicInvoke(file) as PocoBook;

                BooksProvider google = new GoogleBookProvider(_tracer);
                var booksFromProvider = google.FetchInfoOfBook(minimalbookinfo);

                return FileInfoExtractor.AnalyzeResults(minimalbookinfo, booksFromProvider, _tracer);
            }

            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is InvalidOperationException || ex is ArgumentNullException || ex is System.Reflection.TargetInvocationException)
                {
                    return new BookToBeReviewed(file, ex.Message);
                }
                throw;
            }
        }


        private async Task PutBookOnDatabase(IEnumerable<PocoBook> collectedBooks, List<BookToBeReviewed> discardedBooks)
        {
            var result = await BooksInLibrary.BulkAsync(collectedBooks).ConfigureAwait(true);

            if (result != null && result.Any())
                discardedBooks.AddRange(result.Select((x => new BookToBeReviewed(x.File, ""))));

            await BookToReview.BulkAsync(discardedBooks).ConfigureAwait(true);
        }


        private (List<PocoBook> collectedBooks, List<BookToBeReviewed> discardedBooks) CollectBooks()
        {
            double progressCounter = 0;
            var collectedBooks = new List<PocoBook>();
            var discardedBooks = new List<BookToBeReviewed>();


            Parallel.ForEach(Files, file =>
            {
                try
                {
                    Progress.Report(progressCounter++ / Files.Count);

                    var book = SearchBookInfoOfFile(file);

                    AddToCollections(book, collectedBooks, discardedBooks);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    _exceptions.Enqueue(e);
                }
            });

            return (collectedBooks, discardedBooks);
        }

        private static void AddToCollections(BookatHome book, List<PocoBook> collectedBooks, List<BookToBeReviewed> discardedBooks)
        {
            if (book != null)
            {
                switch (book)
                {
                    case PocoBook bookfound:
                        collectedBooks.Add(bookfound);
                        break;
                    case BookToBeReviewed booktoreview:
                        discardedBooks.Add(booktoreview);
                        break;
                }
            }
        }

       

        private void DeleteAlreadyCatalogedBooks()
        {
            var filesCataloged = from cataloged in BooksInLibrary.Read()
                select cataloged.File;

            var fileDiscartedPresentInCataloged = from discarted in BookToReview.Read()
                where filesCataloged.Contains(discarted.File)
                select discarted;

            foreach (var filenowcataloged in fileDiscartedPresentInCataloged)
            {
                BookToReview.Delete(filenowcataloged);
            }
        }
    }
}





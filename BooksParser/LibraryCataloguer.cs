using PdfSharp.Pdf.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BookatHomeProvider;
using LibraryAtHomeRepositoryDriver;


namespace BooksParser
{
    /// <summary>
    /// 
    /// </summary>
    public class LibraryCataloguer
    {
        public LibraryCataloguer(BookParserConfig configuration, IProgress<double> progress)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));


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

            Configuration = configuration;

        }

        IProgress<double> Progress { get; set; }

        private readonly IMongodbConnection Connection;

        private readonly BooksCollectedDataMapper BooksInLibrary;

        private readonly BookToBeReviewedDataMapper BookToReview;

        private readonly LibraryStatisticsDataMapper LibStatistics;

        private bool _configured;



        public void ConfigureFilesToSearch()
        {
            Regex reSearchPattern = new Regex(string.Join("|", Configuration.ebookformat.ToArray()), RegexOptions.IgnoreCase);


            IList<string> fileintofolder = Directory.EnumerateFiles(Configuration.ebookdirectory, "*.*", SearchOption.AllDirectories)
                .Where(x => reSearchPattern.IsMatch(Path.GetExtension(x).ToLowerInvariant())).ToList<string>();


            var filesCataloged = from cataloged in BooksInLibrary.Read()
                                 select cataloged.File;

            Files = fileintofolder.Except<string>(filesCataloged).ToList();

            _configured = true;
        }

        private readonly Dictionary<String, Delegate> GetMetadataFromFileDictionaryDelegate;

        private IList<string> Files { get; set; }

        BookParserConfig Configuration { get; set; }

        public async Task CatalogBooksAsync(ConcurrentQueue<Exception> exceptions, IBookParserTrace tracer)
        {
            tracer.TraceInfo("Start CatalogBooksAsync");

            if (!_configured)
                throw new ArgumentException("Library is not configured.");

            var (collectedBooks, discardedBooks) = CollectBooks(exceptions, tracer);

            await PutBookOnDatabase(collectedBooks, discardedBooks);

            DeleteAlreadyCatalogedBooks();

            tracer.TraceInfo("End CatalogBooksAsync");
        }

        private BookatHome SearchBook(string file, IBookParserTrace trace)
        {
            trace.TraceInfo("SearchBook start for file {0}", file);
            string fileextension = Path.GetExtension(file).ToLowerInvariant();
            try
            {
                PocoBook minimalbookinfo = GetMetadataFromFileDictionaryDelegate[fileextension].DynamicInvoke(file) as PocoBook;

                BooksProvider google = new GoogleBookProvider();
                var booksFromProvider = google.FetchBooksInfo(minimalbookinfo, trace);

                return FileInfoExtractor.AnalyzeResults(minimalbookinfo, booksFromProvider, trace);
            }

            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is PdfReaderException || ex is InvalidOperationException || ex is ArgumentNullException || ex is System.Reflection.TargetInvocationException)
                {
                    return new BookToBeReviewed(file, ex.Message);
                }
                throw;
            }
        }

        private async Task PutBookOnDatabase(List<PocoBook> collectedBooks, List<BookToBeReviewed> discardedBooks)
        {
            var result = await BooksInLibrary.BulkAsync(collectedBooks).ConfigureAwait(true);

            if (result != null && result.Any())
                discardedBooks.AddRange(result.Select((x => new BookToBeReviewed(x.File, ""))));

            await BookToReview.BulkAsync(discardedBooks).ConfigureAwait(true);
        }

        private (List<PocoBook> collectedBooks, List<BookToBeReviewed> discardedBooks) CollectBooks(ConcurrentQueue<Exception> exceptions, IBookParserTrace tracer)
        {
            double progressCounter = 0;
            var collectedBooks = new List<PocoBook>();
            var discardedBooks = new List<BookToBeReviewed>();


            Parallel.ForEach(Files, file =>
            {
                try
                {
                    Progress.Report(progressCounter++ / Files.Count);

                    var book = SearchBook(file, tracer);

                    AddToCollections(book, collectedBooks, discardedBooks);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    exceptions.Enqueue(e);
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

        public void CreateStatistics(TimeSpan elapsedTime)
        {
            LibStatistics.Write(new LibraryStatistics(BookToReview.Count() + BooksInLibrary.Count(), BooksInLibrary.Count(), elapsedTime, Configuration.ebookdirectory));
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





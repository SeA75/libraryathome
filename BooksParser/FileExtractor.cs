using System;
using System.Collections.Generic;
using System.Linq;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    public abstract class FileExtractor
    {
        public abstract BookatHome GetPocoBook(string filepath);

        public static BookatHome AnalyzeResults(PocoBook minimalbookinfo, List<PocoBook> booksFromProvider, IBookParserTrace trace)
        {
            trace?.TraceInfo("AnalyzeResults start");
            if (booksFromProvider == null || booksFromProvider.Count == 0)
            {
                if (!String.IsNullOrEmpty(minimalbookinfo?.Title) && minimalbookinfo.Authors.Length != 0)
                {
                    minimalbookinfo.BookReliability = PocoBook.Reliability.Medium;
                    return minimalbookinfo;
                }

                throw new ArgumentNullException(nameof(booksFromProvider));
            }

            if (booksFromProvider.Count == 1)
            {
                booksFromProvider[0].BookReliability = PocoBook.Reliability.High;
                return booksFromProvider.First();
            }

            return FindTheBookInfoFromCollection(minimalbookinfo.File, booksFromProvider, minimalbookinfo.SearchTitle, trace);
        }

        private static BookatHome FindTheBookInfoFromCollection(string fileunderanalysis, List<PocoBook> booksFromProvider,
            string maytitle, IBookParserTrace trace)
        {
            var titlecomparer = new TitleCompareBookFinder();
            var levenshteincomparer = new LevenshteinBookFinder();

            titlecomparer.SetNext(levenshteincomparer);

            return titlecomparer.HandleTheBook(fileunderanalysis, booksFromProvider, maytitle, trace);
        }
    }
}

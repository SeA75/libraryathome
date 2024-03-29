﻿using System;
using System.Collections.Generic;
using System.Linq;
using LibraryAtHomeRepositoryDriver;
using LibraryAtHomeTracer;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public abstract class FileExtractor
    {
        public abstract BookAtHome GetPocoBook(string filepath);

        public static BookAtHome AnalyzeResults(PocoBook minimalbookinfo, List<PocoBook> booksFromProvider, IBookParserTrace trace)
        {
            trace?.TraceInfo("AnalyzeResults start");
            if (booksFromProvider == null || booksFromProvider.Count == 0)
            {
                if (!String.IsNullOrEmpty(minimalbookinfo?.Title) && minimalbookinfo.Authors.Count != 0)
                {
                    minimalbookinfo.BookReliability = PocoBook.Reliability.Medium;
                    return minimalbookinfo;
                }

                throw new ArgumentNullException(nameof(booksFromProvider));
            }

            if (booksFromProvider.Count == 1 )
            {
                booksFromProvider.First().BookReliability = PocoBook.Reliability.High;
                return booksFromProvider.First();
            }

            if ( !string.IsNullOrEmpty(minimalbookinfo.SearchPhrase))
            {
                booksFromProvider.First().BookReliability = PocoBook.Reliability.Medium;
                return booksFromProvider.First();
            }

            return FindTheBookInfoFromCollection(minimalbookinfo.File, booksFromProvider, minimalbookinfo.SearchTitle, trace);
        }

        private static BookAtHome FindTheBookInfoFromCollection(string fileunderanalysis, List<PocoBook> booksFromProvider,
            string maytitle, IBookParserTrace trace)
        {
            var titlecomparer = new TitleCompareBookFinder(fileunderanalysis, trace);
            var levenshteincomparer = new LevenshteinBookFinder(fileunderanalysis, trace);

            titlecomparer.SetNext(levenshteincomparer);

            return titlecomparer.HandleTheBookFromList(maytitle, booksFromProvider);
        }
    }
}

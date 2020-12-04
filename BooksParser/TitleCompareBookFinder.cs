using LibraryAtHomeRepositoryDriver;
using System;
using System.Collections.Generic;

namespace BooksParser
{

    public class TitleCompareBookFinder : AbstractBookFinderHandlerr
    {
        public override BookatHome HandleTheBook(string fileunderanalysis, List<PocoBook> booklist, string maybetitle, IBookParserTrace trace)
        {
            trace.TraceInfo("TitleCompareBookFinder --> HandleTheBook start");
            foreach (var book in booklist)
            {
                if (book.Title.Contains(maybetitle, StringComparison.OrdinalIgnoreCase))
                {
                    return book;
                }
            }
            return base.HandleTheBook(fileunderanalysis, booklist, maybetitle, trace);
        }

    }
}

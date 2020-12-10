using LibraryAtHomeRepositoryDriver;
using System;
using System.Collections.Generic;
using LibraryAtHomeTracer;

namespace LibraryAtHomeTracerFileMetadataExtractor
{

    public class TitleCompareBookFinder : AbstractBookFinderHandlerr
    {
        private IBookParserTrace _trace;

        public TitleCompareBookFinder(string fileunderanalysis, IBookParserTrace trace) : base(fileunderanalysis)
        {
            _trace = trace;
        }

        public override BookatHome HandleTheBookFromList(string maybetitle, List<PocoBook> booklist)
        {
            _trace.TraceInfo("TitleCompareBookFinder --> HandleTheBook start");
            foreach (var book in booklist)
            {
                if (book.Title.Contains(maybetitle, StringComparison.OrdinalIgnoreCase))
                {
                    return book;
                }
            }
            return base.HandleTheBookFromList(maybetitle, booklist);
        }

    }
}

using System.Collections.Generic;
using LibraryAtHomeRepositoryDriver;
using LibraryAtHomeTracer;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public class LevenshteinBookFinder : AbstractBookFinderHandlerr
    {
        const int Levenshteinthreshold = 12;

        private readonly IBookParserTrace _trace;
        public LevenshteinBookFinder(string fileunderanalysis, IBookParserTrace trace) : base(fileunderanalysis)
        {
            _trace = trace;
        }

        public override BookAtHome HandleTheBookFromList(string maybetitle, List<PocoBook> booklist)
        {
            _trace?.TraceInfo("LevenshteinBookFinder --> HandleTheBook start");

            int bookindex = -1;
            int lddistance = int.MaxValue;
            int i = 0;
            foreach (var book in booklist)
            {
                if (string.Compare(book.Title, maybetitle, true) != 0)
                {
                    // non trovato il libro
                    int retval = LevenshteinDistance.Compute(book.Title, maybetitle);
                    if (retval < lddistance)
                    {
                        lddistance = retval;
                        bookindex = i;
                    }

                }
                else
                {
                    lddistance = 0;
                    bookindex = i;
                    break;
                }
                i++;
            }
            if (lddistance < Levenshteinthreshold)
            {
                if (lddistance < 4)
                {
                    booklist[bookindex].BookReliability = PocoBook.Reliability.High;
                }
                else
                    booklist[bookindex].BookReliability = PocoBook.Reliability.Low;

                return booklist[bookindex];
            }            
            return base.HandleTheBookFromList(maybetitle, booklist);
        }

    }
}

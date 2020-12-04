using System.Collections.Generic;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    public class LevenshteinBookFinder : AbstractBookFinderHandlerr
    {
        const int Levenshteinthreshold = 12;
        public override BookatHome HandleTheBook(string fileunderanalysis, List<PocoBook> booklist, string maybetitle, IBookParserTrace trace)
        {
            trace.TraceInfo("LevenshteinBookFinder --> HandleTheBook start");

            int bookindex = -1;
            int lddistance = int.MaxValue;
            int i = 0;
            foreach (var book in booklist)
            {
                if (string.Compare(book.Title, maybetitle, true) != 0)
                {
                    // non trovato il libro
                    LevenshteinDistance d = new LevenshteinDistance();
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
            return base.HandleTheBook(fileunderanalysis, booklist, maybetitle, trace);
        }

    }
}

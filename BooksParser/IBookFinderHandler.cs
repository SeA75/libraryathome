using System.Collections.Generic;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    public interface IBookFinderHandler
    {
        BookatHome HandleTheBook(string fileunderanalysis, List<PocoBook> booklist, string maybetitle, IBookParserTrace trace);

        IBookFinderHandler SetNext(IBookFinderHandler handler);
    }
}

using System.Collections.Generic;
using LibraryAtHomeRepositoryDriver;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public interface IBookFinderHandler
    {
        BookAtHome HandleTheBookFromList(string title, List<PocoBook> booklist);

        IBookFinderHandler SetNext(IBookFinderHandler handler);
    }
}

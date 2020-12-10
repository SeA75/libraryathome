using LibraryAtHomeRepositoryDriver;
using System.Collections.Generic;

namespace LibraryAtHomeProvider
{
    public abstract class BooksProvider
    {
        public abstract List<PocoBook> FetchInfoOfBook(PocoBook book);
    }
}

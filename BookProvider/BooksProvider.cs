using System.Collections.Generic;
using BooksParser;
using LibraryAtHomeRepositoryDriver;

namespace BookatHomeProvider
{
    public abstract class BooksProvider
    {
        public abstract List<PocoBook> FetchInfoOfBook(PocoBook book);
    }
}

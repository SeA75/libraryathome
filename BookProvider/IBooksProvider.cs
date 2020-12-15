using LibraryAtHomeRepositoryDriver;
using System.Collections.Generic;

namespace LibraryAtHomeProvider
{
    public interface IBooksProvider
    {
        List<PocoBook> FetchInfoOfBook(PocoBook book);
    }
}

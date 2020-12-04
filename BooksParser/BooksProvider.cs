using RestSharp;
using System.Collections.Generic;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    public abstract class BooksProvider
    {
        public abstract List<PocoBook> GetBooks(PocoBook book, IRestClient restclient, IBookParserTrace trace);
    }
}

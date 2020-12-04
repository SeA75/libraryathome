using System;
using System.Collections.Generic;

namespace BooksParser
{
    public interface IBookDataMapper<TCOLLECTION>
    {
        public IMongodbConnection Connection { get;  }

        public void InsertBook(PocoBook abook);

        public List<PocoBook> GetAllBooksInCollections(string collectionName);

        public PocoBook GetBookByIsbn(string isbn);

        public List<PocoBook> GetBookByTitle(string title);

        public List<PocoBook> GetBookByAuthor(string author);

        public List<PocoBook> GetBookByLanguage(string lang);

        public List<PocoBook> GetBookByCategory(string cat);

        public List<PocoBook> GetBookByReliability(string rel);

        public List<PocoBook> GetBookByFormaty(string format);

        public PocoBook UpdateBook(PocoBook newBook);

        public void DeleteBook(string isbn);

        public void DropDatabase(string databasename);

        public void DropCollection(string name);

        public void CreateStatistics(TimeSpan elapsedtime, string librarydir);
    }

}
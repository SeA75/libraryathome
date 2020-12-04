using MongoDB.Driver;

namespace BooksParser
{
    public class MongodbConnection : IMongodbConnection
    {

        public IMongoDatabase Database { get; private set; }

        public MongoClient Client { get; private set; }

     

        public MongodbConnection(BookParserConfig configuration)
        {
            var mongoUrl = MongoUrl.Create(configuration.libraryContext.connectionstring);

            Client = new MongoClient(mongoUrl);
          
            Database = Client.GetDatabase(configuration.libraryContext.databasename);
        }

    }
}


//public IMongoCollection<PocoBook> BooksNotCataloged
//{
//    get
//    {
//        return Connection.Database.GetCollection<PocoBook>("booksnotcataloged");
//    }
//}

//public IMongoCollection<LibraryStatistics> Statistics
//{
//    get
//    {
//        return Connection.Database.GetCollection<LibraryStatistics>("statistics");
//    }
//}

///// <summary>
///// 
///// </summary>
///// <param name="databasename"></param>
//public void DropDatabase(string databasename)
//{
//    Connection.Client.DropDatabase(databasename);
//}

//public void DropCollection(string name)
//{
//    Connection.Database.DropCollection(name);
//}

//public void InsertBook(PocoBook abook)
//{
//    try
//    {
//        lock (_object)
//        {
//            if (abook == null)
//            {
//                string message = string.Format("Isbn cannot be null or empty for book {0}", abook);
//                throw new ArgumentNullException(message);
//            }

//            switch (abook.BookReliability)
//            {
//                case PocoBook.Reliability.Discarded:
//                    BooksNotCataloged.InsertOne(abook);
//                    break;
//                case PocoBook.Reliability.Low:
//                case PocoBook.Reliability.Medium:
//                case PocoBook.Reliability.High:
//                    {
//                        if (string.IsNullOrEmpty(abook.Isbn))
//                        {
//                            string message = string.Format("Isbn cannot be null or empty for book {0}", abook);
//                            throw new ArgumentNullException(message);
//                        }
//                        Books.InsertOne(abook);
//                        break;
//                    }
//                default:
//                    break;
//            }
//        }
//    }
//    catch(MongoWriteException mwe)
//    {
//        Console.WriteLine("Error Dunring Insert operation for book {0}. Error Message {1}", abook.ToString(), mwe.Message);
//        abook.BookReliability = PocoBook.Reliability.Discarded;
//        InsertBook(abook);

//    }

//}

///// <summary>
///// get the book you want to update and change only the filed values you want to change
///// </summary>
///// <param name="newBook"></param>
///// <returns></returns>
//public PocoBook UpdateBook(PocoBook newBook)
//{
//    FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Eq("Isbn", newBook.Isbn);

//    UpdateDefinition<PocoBook> update = null;
//    PocoBook oldBook = GetBookByIsbn(newBook.Isbn);

//    update = UpdateValue(newBook.Title, oldBook.Title, "Title", update);
//    update = UpdateValue(newBook.Language, oldBook.Language, "Language", update);
//    update = UpdateValue(newBook.PageCount, oldBook.PageCount, "PageCount", update);
//    update = UpdateVectorValue(newBook.Authors, oldBook.Authors, "Authors", update);
//    update = UpdateValue(newBook.BookReliability, oldBook.BookReliability, "BookReliability", update);
//    update = UpdateVectorValue(newBook.Categories, oldBook.Categories, "Categories", update);
//    update = UpdateValue(newBook.Description, oldBook.Description, "Description", update);
//    update = UpdateValue(newBook.File, oldBook.File, "File", update);
//    update = UpdateValue(newBook.PublishedDate, oldBook.PublishedDate, "PublishedDate", update);
//    update = UpdateValue(newBook.Publisher, oldBook.Publisher, "Publisher", update);

//    var opts = new FindOneAndUpdateOptions<PocoBook>()
//    {
//        IsUpsert = true,
//        ReturnDocument = ReturnDocument.After
//    };

//    var model = Books.FindOneAndUpdate<PocoBook>(filter, update, opts);
//    return model;
//}


//public void DeleteBook(string isbn)
//{
//    Books.DeleteOne(x => x.Isbn == isbn);
//}


//public PocoBook GetBookByIsbn(string isbn)
//{
//    FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Eq("Isbn", isbn);

//    try
//    {
//        return Books.Find<PocoBook>(filter).First<PocoBook>();
//    }
//    catch (InvalidOperationException e)
//    {
//        return null;
//    }
//}


//public List<PocoBook> GetBookByTitle(string title)
//{
//    return GetBookBy("Title", title);
//}

//public List<PocoBook> GetBookByAuthor(string author)
//{
//    return GetBookBy("Authors", author);
//}

//public List<PocoBook> GetBookByLanguage(string lang)
//{
//    return GetBookBy("Language", lang);
//}

//public List<PocoBook> GetBookByCategory(string cat)
//{
//    return GetBookBy("Categories", cat);
//}

//public List<PocoBook> GetBookByReliability(string rel)
//{
//    return GetBookBy("BookReliability", rel);
//}

//public List<PocoBook> GetBookByFormaty(string format)
//{
//    return GetBookBy("Format", format);
//}

//public List<PocoBook> GetAllBooksInCollections(string collectionName)
//{
//    switch (collectionName)
//    {
//        case "books":
//            return Books.Find<PocoBook>(_ => true).ToList<PocoBook>();
//        case "booksnotcataloged":
//            return BooksNotCataloged.Find<PocoBook>(_ => true).ToList<PocoBook>();
//        default:
//            return new List<PocoBook>();
//    }
//}


//public void CreateStatistics(TimeSpan elapsedtime, string librarydir)
//{
//    LibraryStatistics stat = new LibraryStatistics();

//    string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", elapsedtime.Hours, elapsedtime.Minutes,
//        elapsedtime.Seconds, elapsedtime.Milliseconds / 10);

//    stat.NumberOfCollectedBook = Books.CountDocuments<PocoBook>(x => true);
//    stat.TotalBooks = BooksNotCataloged.CountDocuments<PocoBook>(x => true) + stat.NumberOfCollectedBook;
//    stat.ElapsedTime = elapsedTime;

//    stat.LibraryDirectory = librarydir;
//    stat.Timestamp = DateTime.Now.ToString();
//    long successratio = (100 * stat.NumberOfCollectedBook) / (stat.TotalBooks);

//    stat.SuccessRatio = successratio.ToString() + "%";

//    Statistics.InsertOne(stat);
//}



//private static bool CheckEquality<T>(T[] first, T[] second)
//{
//    if (first.SequenceEqual<T>(second))
//        return true;
//    return false;
//}

//private static UpdateDefinition<PocoBook> UpdateValue<T>(T newValue, T oldValue, string fieldname, UpdateDefinition<PocoBook> update)
//{
//    if (!EqualityComparer<T>.Default.Equals(newValue, oldValue))
//        update = Builders<PocoBook>.Update.Set(fieldname, newValue);
//    return update;
//}

//private static UpdateDefinition<PocoBook> UpdateVectorValue<T>(T[] newValue, T[] oldValue, string fieldname, UpdateDefinition<PocoBook> update)
//{
//    if (!CheckEquality<T>(oldValue, newValue))
//        update = Builders<PocoBook>.Update.Set(fieldname, newValue);
//    return update;
//}


//private void CreateIndexes()
//{
//    Books.Indexes.CreateMany(new[]
//    {
//         new CreateIndexModel<PocoBook>(
//            Builders<PocoBook>.IndexKeys.Ascending(x => x.Isbn),
//            new CreateIndexOptions
//            {
//                Unique = true
//            }),

//        new CreateIndexModel<PocoBook>(
//            Builders<PocoBook>.IndexKeys.Ascending(x => x.Authors),
//            new CreateIndexOptions
//            {
//                Unique = false
//            }),
//         new CreateIndexModel<PocoBook>(
//            Builders<PocoBook>.IndexKeys.Ascending(x => x.Language),
//            new CreateIndexOptions
//            {
//                Unique = false
//            }),
//         new CreateIndexModel<PocoBook>(
//            Builders<PocoBook>.IndexKeys.Ascending(x => x.Publisher),
//            new CreateIndexOptions
//            {
//                Unique = false
//            }),

//        new CreateIndexModel<PocoBook>(
//            Builders<PocoBook>.IndexKeys.Ascending(x => x.BookReliability),
//            new CreateIndexOptions
//            {
//                Background = false
//            }),

//        new CreateIndexModel<PocoBook>(
//            Builders<PocoBook>.IndexKeys.Ascending(x => x.Categories),
//            new CreateIndexOptions
//            {
//                Background = false
//            })
//    });

//    BooksNotCataloged.Indexes.CreateOne(
//        new CreateIndexModel<PocoBook>(Builders<PocoBook>.IndexKeys.Ascending(x => x.File),
//        new CreateIndexOptions
//        { Unique = true }
//        ));
//}
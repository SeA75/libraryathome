using MongoDB.Driver;

namespace BooksParser
{
    public interface IMongodbConnection
    {
        public IMongoDatabase Database { get; }

        public MongoClient Client { get; }
    }
}

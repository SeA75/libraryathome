using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public interface IMongodbConnection
    {
        IMongoDatabase Database { get; }

        MongoClient Client { get; }
    }
}

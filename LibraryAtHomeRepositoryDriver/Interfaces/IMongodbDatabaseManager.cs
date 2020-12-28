using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public interface IMongodbDatabaseManager
    {
        bool CollectionExists(string name);

        void CreateCollection(string name);

        IMongoCollection<TCOLL> GetCollection<TCOLL>(string name);

        void DropCollection(string name);
    }
}
using System.Linq;
using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public class MongodbDatabaseManager : IMongodbDatabaseManager
    {

        private readonly string _server;

        private readonly string _database;
        
        public MongodbDatabaseManager(string server, string database)
        {
            _server = server;
            _database = database;
        }

        public bool CollectionExists(string name)
        {
            var currentDatabase = GetCurrentDatabase();

            using (var cursor = currentDatabase.ListCollectionNames())
            {
                if (cursor.ToEnumerable().FirstOrDefault(x => x == name) == name)
                    return true;
            }

            return false;
        }

        public void CreateCollection(string name)
        {
            var currentDatabase = GetCurrentDatabase();
            currentDatabase.CreateCollection(name);
        }

        public IMongoCollection<TCOLL> GetCollection<TCOLL>(string name)
        {
            var currentDatabase = GetCurrentDatabase();
            return currentDatabase.GetCollection<TCOLL>(name);
        }

        public void DropCollection(string name)
        {
            var currentDatabase = GetCurrentDatabase();
            currentDatabase.DropCollection(name);
        }

        private IMongoDatabase GetCurrentDatabase()
        {
            IMongodbConnection connection = new MongodbConnection();
            connection.ConnectToServer(_server);
            IMongoDatabase currentDatabase = connection.ConnectToDatabase(_database);
            return currentDatabase;
        }
    }
}
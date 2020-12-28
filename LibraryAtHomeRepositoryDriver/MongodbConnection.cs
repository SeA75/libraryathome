using System;
using System.Configuration;
using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public class MongodbConnection : IMongodbConnection
    {
        private MongoClient _client;

        public void ConnectToServer(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Server name cannot be null or empty.");
            }

            string connectionString = $"mongodb://{name}:27017/";

            var url = MongoUrl.Create(connectionString);

            _client = new MongoClient(url);
        }

        public IMongoDatabase ConnectToDatabase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Database name cannot be null or empty.");
            }

            return _client?.GetDatabase(name);
        }
    }
}

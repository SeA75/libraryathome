using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public class MongodbServerManager : IMongodbServerManager
    {
        private readonly MongoClient _client;

        public MongodbServerManager(string server)
        {
            string connectionString = $"mongodb://{server}:27017/";

            var mongodbUrl = MongoUrl.Create(connectionString);

            _client = new MongoClient(mongodbUrl);
        }

        public void DropDatabase(string name)
        {
            _client.DropDatabase(name);
        }

        public List<string> ListDatabases()
        {

            List<string> databases = new List<string>();

            using (var cursor = _client.ListDatabases())
            {
                foreach (var document in cursor.ToEnumerable())
                {
                    databases.Add(document.GetElement(0).Value.AsString); //TODO filter for libathome database 
                }
            }
            return databases;
        }

       
    }
}
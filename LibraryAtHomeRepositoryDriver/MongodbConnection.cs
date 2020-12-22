using System.Configuration;
using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public class MongodbConnection : IMongodbConnection
    {
        public IMongoDatabase Database { get; private set; }

        public MongoClient Client { get; private set; }

        public MongodbConnection(string connectionstring, string databasename)
        {
            var mongoUrl = MongoUrl.Create(connectionstring);

            Client = new MongoClient(mongoUrl);
          
            Database = Client.GetDatabase(databasename);
        }

    }
}

using System.Configuration;
using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public class MongodbConnection : IMongodbConnection
    {
        public IMongoDatabase Database { get; private set; }

        public MongoClient Client { get; private set; }

        public MongodbConnection()
        {

            var mongoUrl = MongoUrl.Create(ConfigurationManager.ConnectionStrings["DefaultMongoConnection"].ConnectionString);

            Client = new MongoClient(mongoUrl);
          
            Database = Client.GetDatabase(ConfigurationManager.AppSettings.Get("MongoDbName"));
        }

    }
}

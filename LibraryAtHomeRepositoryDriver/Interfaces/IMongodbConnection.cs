using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    internal interface IMongodbConnection
    {
        void ConnectToServer(string name);

        IMongoDatabase ConnectToDatabase(string name);

    }
}

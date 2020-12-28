using System.Collections.Generic;

namespace LibraryAtHomeRepositoryDriver
{
    public interface IMongodbServerManager
    {
       void DropDatabase(string name);

        List<string> ListDatabases();
    }
}
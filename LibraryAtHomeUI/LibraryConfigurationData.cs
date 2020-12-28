using System.Collections.Generic;

namespace LibraryAtHomeUI
{
    public class LibraryConfigurationData
    {
        public LibraryConfigurationData()
        {
            BookFormatsCheckStatus = new Dictionary<string, bool>();
            RepositoryHost = "localhost";
            ConnectionString = $"mongodb://{RepositoryHost}:27017/";
        }

        public Dictionary<string, bool> BookFormatsCheckStatus { get; set; }

        public string EbookFolder { get; set; }

        public string DatabaseName { get; set; }

        public string ConnectionString { get; set; }

        public string RepositoryHost { get; set; }

        public string PlugInFolder { get; set; }

        public string PluginAssemblyName { get; set; }

        public bool LibraryExits { get; set; }

    }
}

using System.CodeDom;
using System.Collections.Generic;

namespace BooksParser
{
    public partial class BookParserConfig
    {

        public BookParserConfig()
        {
            ebookformat = new List<string>();
            libraryContext = new LibraryContextConfig();
            providerPlugin = new BookProviderPlugin();
        }

        public List<string> ebookformat { get; set; }

       
        public string ebookdirectory { get; set; }

      
        public string statisticdirectory { get; set; }

       
        public string tracefile { get; set; }

     
        public string tracelevel { get; set; }

      
        public LibraryContextConfig libraryContext { get; set; }

        
        public BookProviderPlugin providerPlugin { get; set; }
    }

    public partial class LibraryContextConfig
    {

       
        public string jsonlibraryfile { get; set; }

      
        public string databasename { get; set; }

      
        public string hostname { get; set; }

    }

    public partial class BookProviderPlugin
    {
       
        public string pluginfolder { get; set; }

       
        public string pluginassemblyname { get; set; }
    }


}





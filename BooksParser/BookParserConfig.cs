using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;

namespace BooksParser
{
    public partial class BookParserConfig
    {


        [JsonProperty("ebookformat")]
        public List<string> ebookformat { get; set; }

        [JsonProperty("ebookdirectory")]
        public string ebookdirectory { get; set; }

        [JsonProperty("statisticdirectory")]
        public string statisticdirectory { get; set; }

        [JsonProperty("tracefile")]
        public string tracefile { get; set; }

        [JsonProperty("tracelevel")]
        public string tracelevel { get; set; }

        [JsonProperty("libraryContext")]
        public LibraryContextConfig libraryContext { get; set; }
    }

    public partial class LibraryContextConfig
    {

        [JsonProperty("jsonlibraryfile")]
        public string jsonlibraryfile { get; set; }

        [JsonProperty("databasename")]
        public string databasename { get; set; }


        [JsonProperty("connectionstring")]
        public string connectionstring { get; set; }

    }

    public partial class BookParserConfig
    {
        public static BookParserConfig FromJson(string json) => JsonConvert.DeserializeObject<BookParserConfig>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this BookParserConfig self) => JsonConvert.SerializeObject(self, BooksParser.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }


}





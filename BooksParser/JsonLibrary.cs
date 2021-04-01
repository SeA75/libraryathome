using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    class JsonLibrary
    {

        static readonly object _object = new object();


        public JsonLibrary()
        {
            Library = new List<PocoBook>();
            JsonWriterSettings.Defaults.Indent = true;
        }

        public List<PocoBook> Library { get; set; }

        public void Append(PocoBook abook)
        {
            lock (_object)
            {
                Library.Add(abook);
            }

        }

        public void SaveLibrary(string filename)
        {
            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                Converters = new List<Newtonsoft.Json.JsonConverter>() { new Newtonsoft.Json.Converters.StringEnumConverter() }
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Library, Formatting.Indented);

            File.WriteAllText(filename, json);
        }

        public string ToJson()
        {
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };

            return Library.ToJson(jsonWriterSettings);
        }
    }



}

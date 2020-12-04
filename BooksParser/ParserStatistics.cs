using Newtonsoft.Json;
using System.Collections.Generic;

namespace BooksParser
{
    public class SkippedFile
    {
        public SkippedFile(string filename, string reason)
        {
            Filename = filename;
            Reason = reason;
        }

        [JsonProperty("Filename")]
        string Filename { get; }

        [JsonProperty("Reason")]
        string Reason { get; }
    }

    public class ParserStatistics
    {
        static readonly object _object = new object();
        public ParserStatistics(int count, List<string> extensions)
        {
            Succedbytype = new Dictionary<string, int>(count);
            Failedbytype = new Dictionary<string, int>(count);
            FilesSkipped = new List<SkippedFile>(count);
            CatalogedFiles = new List<string>(count);

            foreach (var mykey in extensions)
            {
                Failedbytype[mykey] = 0;
                Succedbytype[mykey] = 0;
            }
            BookCount = count;
        }

        [JsonProperty("BookCount")]
        public int BookCount { get; }


        public void AddStats(StatResult res, string extension, string searchedfile, string reason = null)
        {
            lock (_object)
            {
                if (res == StatResult.Success)
                {
                    Succedbytype[extension]++;
                    CatalogedFiles.Add(searchedfile);
                }
                else
                {
                    Failedbytype[extension]++;
                    SkippedFile sk = new SkippedFile(searchedfile, reason);
                    FilesSkipped.Add(sk);
                }
            }

        }

        public enum StatResult
        {
            Success,
            Fail
        }

        [JsonProperty("Succedbytype")]
        public Dictionary<string, int> Succedbytype { get; set; }

        [JsonProperty("Failedbytype")]
        public Dictionary<string, int> Failedbytype { get; set; }

        [JsonProperty("FilesSkipped")]
        public List<SkippedFile> FilesSkipped { get; set; }

        [JsonProperty("CatalogedFiles")]
        public List<string> CatalogedFiles { get; set; }

        [JsonProperty("ProcessTime")]
        public string ProcessTime { get; set; }


        [JsonProperty("SuccessRatio")]
        public string SuccessRatio
        {
            get
            {
                return ((CatalogedFiles.Count * 100) / BookCount).ToString();
            }

            set { }
        }

    }
}

using BooksParser;
using LibraryAtHomeRepositoryDriver;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;


namespace BookatHomeProvider
{
    public class GoogleBookProvider : BooksProvider
    {
        private static readonly SHA256 Sha256 = SHA256.Create();

        private readonly IBookParserTrace _trace;

        private PocoBook _requestedBook;

        public GoogleBookProvider(IBookParserTrace trace)
        {
            _trace = trace;
        }

        public override List<PocoBook> FetchInfoOfBook(PocoBook book)
        {
            _requestedBook = book;

            Rootobject rootresponse = ExecuteGoogleRequest();

            return FetchBooksInfoFromResponse(rootresponse);
        }

        public static byte[] GetHashSha256(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return Sha256.ComputeHash(stream);
            }
        }
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        private Rootobject ExecuteGoogleRequest()
        {
            _trace.TraceInfo("FetchInfoOfBook start for book {0}", _requestedBook.Title);
            
            string url = String.Format("https://www.googleapis.com/books/v1/volumes?q={0}&maxResults=40&country=IT", BuildQuery());

            Rootobject root = ExecuteGoogleRequestWithRetry(url);
            if (root == null)
            {
                string query = string.Format("intitle:{0}", _requestedBook.Title);
                url = String.Format("https://www.googleapis.com/books/v1/volumes?q={0}&maxResults=40&country=IT", query);

                return ExecuteGoogleRequestWithRetry(url);
            }
            return root;
        }


        private List<PocoBook> FetchBooksInfoFromResponse(Rootobject root)
        {
            if(root == null || root.items == null)
            {
                return new List<PocoBook>();
            }

            List<PocoBook> retval = new List<PocoBook>();

            for (int i = 0;  i < root.items.Length; i++)
            {
                PocoBook abook = new PocoBook(_requestedBook.File, root.items[i].volumeInfo.title,
                    root.items[i].volumeInfo.authors, string.Empty, string.Empty)
                {
                    Categories = root.items[i].volumeInfo.categories,
                    Description = root.items[i].volumeInfo.description,
                    Language = root.items[i].volumeInfo.language,
                    Publisher = root.items[i].volumeInfo.publisher,
                    ImageLink = root.items[i].volumeInfo.imageLinks?.smallThumbnail,
                    PublishedDate = root.items[i].volumeInfo.publishedDate.ToDateTime(),
                    Isbn = root.items[i].ToIsbn() ?? Extensions.GenerateUniqueFileId(_requestedBook.File)
                };

                retval.Add(abook);
            }

            return retval;

        }

       

        private static Rootobject ExecuteGoogleRequestWithRetry(string url)
        {
            IRestClient restclient = new RestClient();
            restclient.BaseUrl = new Uri(url);
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse response = restclient.Execute(request);
            int retry = 3;
            Rootobject root;

            do
            {
                root = JsonConvert.DeserializeObject<Rootobject>(response.Content);
                retry--;
                Thread.Sleep(2000);
            } while ((root == null || root.items == null || root.items.Length == 0 ) && retry != 0);

            return root;
        }



        private string BuildQuery()
        {
            string query;
            if (!string.IsNullOrEmpty(_requestedBook.Isbnsearch))
            {
                query = "isbn:" + _requestedBook.Isbnsearch.Replace("-", "").Replace("ISBN", "", true, null).Replace(" ", "");
            }
            else
            {
                if (_requestedBook.Authors == null || _requestedBook.Authors.Length == 0)
                {
                    query = string.Format("intitle:{0}", _requestedBook.SearchTitle);
                }
                else
                {
                    query = string.Format("intitle:{0}&inauthor:{1}", _requestedBook.SearchTitle, string.Join(", ", _requestedBook.Authors));
                }
            }

            return query;
        }

    }
}

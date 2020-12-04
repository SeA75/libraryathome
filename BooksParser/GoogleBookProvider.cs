using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    public class GoogleBookProvider : BooksProvider
    {
        private readonly SHA256 Sha256 = SHA256.Create();


        public override List<PocoBook> GetBooks(PocoBook book, IRestClient restclient, IBookParserTrace trace)
        {
            Rootobject rootresponse = ExecuteGoogleRequest(book, restclient, trace);

            return GetBookFromRest(rootresponse, book.File);
        }


        private  Rootobject ExecuteGoogleRequest(PocoBook book, IRestClient restclient, IBookParserTrace trace)
        {
            trace.TraceInfo("GetBooks start for book {0}", book.Title);

            string url = String.Format("https://www.googleapis.com/books/v1/volumes?q={0}&maxResults=40&country=IT", BuildQuery(book));

            Rootobject root = GoogleRestBooksRequest(restclient, url);
            if (root == null)
            {
                string query = string.Format("intitle:{0}", book.Title);
                url = String.Format("https://www.googleapis.com/books/v1/volumes?q={0}&maxResults=40&country=IT", query);

                return GoogleRestBooksRequest(restclient, url);
            }
            else return root;

        }


        private List<PocoBook> GetBookFromRest(Rootobject root, string filename)
        {
            if(root == null || root.items == null)
            {
                return new List<PocoBook>();
            }

            List<PocoBook> retval = new List<PocoBook>();

            for (int i = 0;  i < root.items.Length; i++)
            {

                PocoBook abook = new PocoBook(filename, root.items[i].volumeInfo.title, root.items[i].volumeInfo.authors, string.Empty, string.Empty)
                {
                    Categories = root.items[i].volumeInfo.categories,
                    Description = root.items[i].volumeInfo.description,
                    Language = root.items[i].volumeInfo.language,
                    Publisher = root.items[i].volumeInfo.publisher
                };


                DateTime pubDate;
                if (DateTime.TryParse(root.items[i].volumeInfo.publishedDate, out pubDate) != true)
                {
                    string[] formats = { "yyyy" };

                    DateTime.TryParseExact(root.items[i].volumeInfo.publishedDate, formats,
                        System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out pubDate);
                }

                SetIndustrialIdentifier(root, i, abook);

                retval.Add(abook);
            }

            return retval;

        }

        private void SetIndustrialIdentifier(Rootobject root, int i, PocoBook abook)
        {
            if (root.items[i].volumeInfo.industryIdentifiers != null && root.items[i].volumeInfo.industryIdentifiers.Length == 1)
            {
                abook.Isbn = root.items[i].volumeInfo.industryIdentifiers[0].identifier;
            }

            if (root.items[i].volumeInfo.industryIdentifiers != null && root.items[i].volumeInfo.industryIdentifiers.Length == 2)
            {
                if ((root.items[i].volumeInfo.industryIdentifiers[1].type == "ISBN_13") ||
                        root.items[i].volumeInfo.industryIdentifiers[0].type == "ISBN_13")
                {
                    abook.Isbn = root.items[i].volumeInfo.industryIdentifiers[1].identifier;
                }

                else
                {
                    if ((root.items[i].volumeInfo.industryIdentifiers[1].type == "ISBN_10") ||
                                               root.items[i].volumeInfo.industryIdentifiers[0].type == "ISBN_10")
                    {
                        abook.Isbn = root.items[i].volumeInfo.industryIdentifiers[1].identifier;

                    }
                }

            }
            if (abook.Isbn == null)
            {
                //maybe a personal doc
                string personalId = BytesToString(GetHashSha256(abook.File));
                abook.Isbn = String.Format("PersonalID<{0}>", Guid.NewGuid());
            }
        }

        private static Rootobject GoogleRestBooksRequest(IRestClient restclient, string url)
        {
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



        private static string BuildQuery(PocoBook book)
        {
            string query;
            if (!string.IsNullOrEmpty(book.Isbnsearch))
            {
                query = "isbn:" + book.Isbnsearch.Replace("-", "").Replace("ISBN", "", true, null).Replace(" ", "");
            }
            else
            {
                if (book.Authors == null || book.Authors.Length == 0)
                {
                    query = string.Format("intitle:{0}", book.SearchTitle);
                }
                else
                {
                    query = string.Format("intitle:{0}&inauthor:{1}", book.SearchTitle, string.Join(", ", book.Authors));
                }
            }

            return query;
        }



        private byte[] GetHashSha256(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return Sha256.ComputeHash(stream);
            }
        }
        private static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }


    }
}

using LibraryAtHomeRepositoryDriver;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


namespace LibraryAtHomeProvider
{
    public class GoogleBookProvider : IBooksProvider
    {
        private static readonly SHA256 Sha256 = SHA256.Create();       


        public List<PocoBook> FetchInfoOfBook(PocoBook book)
        {
            Uri myquery = CreateQueryOnBook(book);

            Rootobject root = ExecuteGoogleRequest(myquery);
            root.File = book.File;
            //       Rootobject rootresponse = ExecuteGoogleRequest();

            return FetchBooksInfoFromResponse(root);
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
            StringBuilder result = new StringBuilder();
            foreach (byte b in bytes)
                result.Append(b.ToString("x2"));
           
            return result.ToString();
        }

        //private Rootobject ExecuteGoogleRequest()
        //{
        ////    string url = String.Format("https://www.googleapis.com/books/v1/volumes?q={0}&maxResults=40&country=IT", BuildQuery());

        //    Uri myquery = BuildQuery();

        //    Rootobject root = ExecuteGoogleRequestWithRetry(myquery);
            
            
        //    //if (root == null)
        //    //{
        //    //    string query = string.Format("intitle:{0}", _requestedBook.Title);
        //    //    url = String.Format("https://www.googleapis.com/books/v1/volumes?q={0}&maxResults=40&country=IT", query);

        //    //    return ExecuteGoogleRequestWithRetry(url);
        //    //}
        //    return root;
        //}


        private List<PocoBook> FetchBooksInfoFromResponse(Rootobject root)
        {
            if (root == null || root.items == null)
            {
                return new List<PocoBook>();
            }

            List<PocoBook> retval = new List<PocoBook>();

            for (int i = 0;  i < root.items.Length; i++)
            {
                Collection<string> authors = new Collection<string>();
               
                Array.ForEach(root.items[i].volumeInfo.authors ?? Array.Empty<string>(), str => authors.Add(str));

                PocoBook abook = new PocoBook(root.File, root.items[i].volumeInfo.title,
                    authors , string.Empty, string.Empty)
                {
                    Description = root.items[i].volumeInfo.description,
                    Language = root.items[i].volumeInfo.language,
                    Publisher = root.items[i].volumeInfo.publisher,
                    ImageLink = root.items[i].volumeInfo.imageLinks?.smallThumbnail,
                    PublishedDate = root.items[i].volumeInfo.publishedDate.ToDateTime(),
                    Isbn = root.items[i].ToIsbn() ?? Extensions.GenerateUniqueFileId(root.File),
                    Authors = authors
                };
                Array.ForEach(root.items[i].volumeInfo.categories?? Array.Empty<string>(), str => abook.Categories.Add(str));
                retval.Add(abook);
            }

            return retval;

        }

       

        private static Rootobject ExecuteGoogleRequest(Uri url)
        {
            IRestClient restclient = new RestClient();
            restclient.BaseUrl = url;
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse response = restclient.Execute(request);
            int retry = 3;
            Rootobject root;

            do
            {
                root = JsonConvert.DeserializeObject<Rootobject>(response.Content);
                if(retry < 3)
                    Thread.Sleep(2000);
                retry--;
            } while ((root == null || root.items == null || root.items.Length == 0 ) && retry != 0);

            return root;
        }

        private Uri CreateQueryOnBook(PocoBook book)
        {
            string endpoint = "https://www.googleapis.com/books/v1/volumes?q=###&maxResults=40&country=IT";

            string query;
            if (!string.IsNullOrEmpty(book.Isbnsearch))
            {
                query = "isbn:" + book.Isbnsearch.Replace("-", "").Replace("ISBN", "", true, null).Replace(" ", "");
                endpoint = endpoint.Replace("###", query);
                return new Uri(endpoint);
            }

            if(!string.IsNullOrEmpty(book.SearchPhrase))
            {
                query = string.Format(" \"{0}\" ", book.SearchPhrase);
                endpoint = endpoint.Replace("###", query);
                return new Uri(endpoint);
            }

            if (book.Authors == null || book.Authors.Count == 0)
            {
                query = string.Format("intitle:{0}", book.SearchTitle);
                endpoint = endpoint.Replace("###", query);
                return new Uri(endpoint);
            }
            
            query = string.Format("intitle:{0}&inauthor:{1}", book.SearchTitle, string.Join(", ", book.Authors));
            endpoint = endpoint.Replace("###", query);
            return new Uri(endpoint);
        }


        //private string BuildQuery()
        //{
        //    string query;
        //    if (!string.IsNullOrEmpty(_requestedBook.Isbnsearch))
        //    {
        //        query = "isbn:" + _requestedBook.Isbnsearch.Replace("-", "").Replace("ISBN", "", true, null).Replace(" ", "");
        //    }
        //    else
        //    {
        //        if (_requestedBook.Authors == null || _requestedBook.Authors.Count == 0)
        //        {
        //            query = string.Format("intitle:{0}", _requestedBook.SearchTitle);
        //        }
        //        else
        //        {
        //            query = string.Format("intitle:{0}&inauthor:{1}", _requestedBook.SearchTitle, string.Join(", ", _requestedBook.Authors));
        //        }
        //    }

        //    return query;
        //}

    }
}

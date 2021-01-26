using LibraryAtHomeRepositoryDriver;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;


namespace LibraryAtHomeProvider
{
    public class GoogleBookProvider : IBooksProvider
    {
        private const int retryCounter = 3;
        private const int msRetrySleep = 700;

        private readonly IRestRequestManager restMngr;

        public GoogleBookProvider(IRestRequestManager restMngr)
        {
            this.restMngr = restMngr;
        }

        public List<PocoBook> FetchBookInfo(PocoBook book)
        {
            if (PocoBook.IsNullOrEmpty(book))
            {
                throw new ArgumentNullException(nameof(book), "Book cannot be null.");
            }

            Uri myquery = CreateQueryOnBook(book);

            Rootobject root = ExecuteGoogleRequest(myquery);
            root.File = new FileInfo(book.File);

            return FetchBooksInfoFromResponse(root);
        }


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

                PocoBook abook = new PocoBook(root.File.FullName, root.items[i].volumeInfo.title,
                    authors , string.Empty, string.Empty)
                {
                    Description = root.items[i].volumeInfo.description,
                    Language = root.items[i].volumeInfo.language,
                    Publisher = root.items[i].volumeInfo.publisher,
                    ImageLink = root.items[i].volumeInfo.imageLinks?.smallThumbnail,
                    PublishedDate = root.items[i].volumeInfo.publishedDate.ToDateTime(),
                    Isbn = root.items[i].ToIsbn() ?? root.File.UniqueIdentifier(),
                    Authors = authors
                };
                Array.ForEach(root.items[i].volumeInfo.categories?? Array.Empty<string>(), str => abook.Categories.Add(str));
                retval.Add(abook);
            }

            return retval;

        }

       

        private Rootobject ExecuteGoogleRequest(Uri url)
        {
           
            restMngr.SetBaseUri(url);
       
            int retry = 3;
            IRestResponseManager response;
            do
            {                
                if (retry < retryCounter)
                    Thread.Sleep(msRetrySleep);

                response = restMngr.ExecuteGet();

                retry--;

            } while ((response == null || response.GetResponseContent() == null) && retry != 0);        

            return DeserializeResponse(response);
        }

        private static Rootobject DeserializeResponse(IRestResponseManager response)
        {
            if(response == null || response.GetResponseContent() == null)
            {
                return new Rootobject();
            }   
            
            return JsonConvert.DeserializeObject<Rootobject>(response.GetResponseContent());
        }


        private Uri CreateQueryOnBook(PocoBook book)
        {
            if(PocoBook.IsNullOrEmpty(book))
            {
                throw new ArgumentNullException(nameof(book), "Book cannot be null or empty");
            }

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
    }
}

using BooksParser;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using LibraryAtHomeRepositoryDriver;

namespace NUnitTestBooksParser
{
    public class UnitTestLibraryContext
    {
        BookParserConfig configuration;
        MongodbDataMapper<PocoBook> mybooks;

        public static void SetAssemblyConfig(Assembly assembly)
        {
            Configuration currentConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Configuration assemblyConfiguration = ConfigurationManager.OpenExeConfiguration(new Uri(assembly.CodeBase).LocalPath);
            if (assemblyConfiguration.HasFile && string.Compare(assemblyConfiguration.FilePath, currentConfiguration.FilePath, true) != 0)
            {
                assemblyConfiguration.SaveAs(currentConfiguration.FilePath);
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }

        [OneTimeSetUp]
        public void Setup()
        {
            SetAssemblyConfig(GetType().Assembly);
            var configLocation = Assembly.GetEntryAssembly().Location;
            string configFile = @"C:\Users\vgh8no\source\repos\BooksParser\NUnitTestBooksParser\testConf.json";
            configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<BookParserConfig>(File.ReadAllText(configFile));
            IMongodbConnection connection = new MongodbConnection();

            mybooks = new BooksCollectedDataMapper(connection);
            ImportJsonTestDatabase();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            mybooks?.Drop();
        }


        [Test]
        public void TestInsert_HappyPath()
        {

            PocoBook book = new PocoBook("blabla");
            book.File = @"c:\test\prova.cc";
            book.Title = "mio titolo";
            book.Isbn = System.Guid.NewGuid().ToString();
            book.BookReliability = PocoBook.Reliability.High;

            mybooks.Write(book);
        }


        [Test]

        public void TestInsert_DuplicatedBook_ExpectedMongoWriteException()
        {
            PocoBook book = new PocoBook("blabla");
            book.File = @"c:\test\prova.cc";
            book.Title = "mio titolo";
            book.Isbn = "PSU:000054186286";

            Assert.Throws<MongodbDataMapperException<PocoBook>>(delegate { mybooks.Write(book); });
        }

        [Test]

        public void TestInsert_IsbnEmpty_ExpectedMongoWriteException()
        {
            PocoBook book = new PocoBook("blabla");
            book.File = @"c:\test\prova.cc";
            book.Title = "mio titolo";
            book.Isbn = string.Empty;

            Assert.Throws<ArgumentNullException>(delegate { mybooks.Write(book); });
        }


        [Test]

        public void TestReadBookByTitle_CorrectTitle_ReturnOneBook()
        {
            PocoBook book = new PocoBook();
            book.Title = "Oltre";

            var result = mybooks.Read(book);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Oltre il nulla", result[0].Title);
            Assert.AreEqual("9788831132763", result[0].Isbn);
        }




        [Test]

        public void TestReadBookByAuthor_CorrectAuthor_ReturnBooks()
        {
            PocoBook book = new PocoBook();
            book.Authors = new []{ "Asimov" };

            var result = mybooks.Read(book);

            Assert.AreEqual(3, result.Count);

            var firstbook = result.Where(p => p.Title == "Preludio alla fondazione").First();
            var secondBook = result.Where(p => p.Title == "Fondazione e terra").First();
            var thirdBook = result.Where(p => p.Title == "Cavalieri cosmici").First();

            Assert.IsNotNull(firstbook);
            Assert.AreEqual("9788804401841", firstbook.Isbn);
            Assert.IsNotNull(secondBook);
            Assert.AreEqual("9788804403029", secondBook.Isbn);
            Assert.IsNotNull(thirdBook);
            Assert.AreEqual("9788834700563", thirdBook.Isbn);
        }

        [Test]
        public void TestReadBookByLanguage_Correctlanguage_ReturnManyBooks()
        {
            PocoBook book = new PocoBook();
            book.Language = "it";
            var result = mybooks.Read(book);

            Assert.AreEqual(9, result.Count);
        }

        [Test]
        public void TestReadBookByLanguage_NotExisting_ReturnNoBooks()
        {
            PocoBook book = new PocoBook();
            book.Language = "rr";
            var result = mybooks.Read(book);

            Assert.AreEqual(0, result.Count);
        }

        [Test]

        public void TestReadAllBooksByCollection_CorrectCollection_ReturnAllBooks()
        {
            var book = mybooks.Read();

            Assert.AreEqual(25, book.Count);

        }

        [Test]
        public void TestReadBook_BookNull_ReturnNull()
        {
            Assert.Throws<ArgumentNullException>(() => mybooks.Read(null));
        }


        [Test]
        public void TestReadBook_BookEmpty_ReturnNull()
        {
            PocoBook mybook = new PocoBook();
            Assert.Throws<ArgumentNullException>(() => mybooks.Read(mybook));

        }

        [Test]
        public void TestReadBookByTitle_CorrectTitle_ReturnMultipleBooks()
        {
            PocoBook queryBook = new PocoBook();
            queryBook.Title = "Pattern-Oriented Software Architecture";
            
            var result = mybooks.Read(queryBook);

            Assert.AreEqual(3, result.Count);

            var firstbook = result.Where(p => p.Isbn == "PSU:000054186286").First();
            var secondBook = result.Where(p => p.Isbn == "UCSD:31822028759850").First();
            var thirdBook = result.Where(p => p.Isbn == "9780470512579").First();

            Assert.IsNotNull(firstbook);
            Assert.AreEqual("Pattern-Oriented Software Architecture, A System of Patterns", firstbook.Title);
            Assert.IsNotNull(secondBook);
            Assert.AreEqual("Pattern-Oriented Software Architecture, Patterns for Concurrent and Networked Objects", secondBook.Title);
            Assert.IsNotNull(thirdBook);
            Assert.AreEqual("Pattern-Oriented Software Architecture, On Patterns and Pattern Languages", thirdBook.Title);
        }

        [Test]
        public void TestReadBookByTitle_NotExisting_ReturnEmpty()
        {
            PocoBook queryBook = new PocoBook();
            queryBook.Title = "asdasd";
            var result = mybooks.Read(queryBook);

            Assert.IsEmpty(result);
        }



        [Test]
        public void TestReadBookByIsbn_CorrectIsbnAndVerifyFullFields_FullBookCorrect()
        {
            string expTitle = "Pattern-Oriented Software Architecture, On Patterns and Pattern Languages";
            string expPublisher = "John Wiley & Sons";
            DateTime expPubDate = new DateTime(2007, 4, 29);
            int expPageCount = 0;
            string expDescr = "Software patterns have revolutionized the way developers think about how software is designed, built, and documented, and this unique book offers an in-depth look of what patterns are, what they are not, and how to use them successfully The only book to attempt to develop a comprehensive language that integrates patterns from key literature, it also serves as a reference manual for all pattern-oriented software architecture (POSA) patterns Addresses the question of what a pattern language is and compares various pattern paradigms Developers and programmers operating in an object-oriented environment will find this book to be an invaluable resource";
            string expLang = "en";
            PocoBook.Reliability expBookRel = PocoBook.Reliability.Low;
            string expFormat = "pdf";

            const string isbn = "9780470512579";
            PocoBook querybook = new PocoBook();
            querybook.Isbn = isbn;

            var result = mybooks.Read(querybook).First();


            string[] expAuthors = { "Frank Buschmann", "Kevin Henney", "Douglas C. Schmidt" };
            string[] expCategories = { "Computers" };

            Assert.IsNotNull(result);
            Assert.AreEqual(expTitle, result.Title);

            Assert.AreEqual(expPublisher, result.Publisher);
            Assert.AreEqual(expPubDate, result.PublishedDate);
            Assert.AreEqual(expPageCount, result.PageCount);
            Assert.AreEqual(expDescr, result.Description);
            Assert.AreEqual(expFormat, result.Format);
            Assert.AreEqual(expBookRel, result.BookReliability);
            Assert.AreEqual(expLang, result.Language);
            Assert.AreEqual(expAuthors, result.Authors);
            Assert.AreEqual(expCategories, result.Categories);
        }

        [Test]
        public void TestUpdateTitle_ExistingBook_ReturnTitleUpdated()
        {
            string isbn = "9780321246424";
            PocoBook query = new PocoBook();
            query.Isbn = isbn;


            var toUpdate = mybooks.Read(query).First();

            string newTitle = "Cross-platform Development in Cpp";
            toUpdate.Title = newTitle;

            var updatedBook = mybooks.Update(toUpdate);

            Assert.AreEqual(isbn, updatedBook.Isbn);
            Assert.AreEqual(newTitle, updatedBook.Title);
        }

        [Test]
        public void TestUpdateTitle_AddAuthor_ReturnBookUpdated()
        {
            string isbn = "PSU:000054186286";
            var query = new PocoBook();
            query.Isbn = isbn;

            var book = mybooks.Read(query).First();

            string[] expAuthors = book.Authors.Append("Peppino");


            book.Authors = book.Authors.Append<string>("Peppino");

            var updatedBook = mybooks.Update(book);

            Assert.AreEqual(isbn, updatedBook.Isbn);
            Assert.AreEqual(expAuthors.Length, updatedBook.Authors.Length);
            Assert.AreEqual(expAuthors, updatedBook.Authors);

            //Clean
            book.Authors = book.Authors.Remove<string>("Peppino");
            mybooks.Update(book);
        }


        [Test]
        public void TestReadBookbyCategory_GetComputers_ReturnBooks()
        {
            var query = new PocoBook();
            query.Categories = new[] { "Computers" };
           
            var books = mybooks.Read(query);

            Assert.IsNotNull(books);
            Assert.AreEqual(7, books.Count);
        }

        [Test]
        public void TestReadBookbyReliability_GetLow_ReturnBooks()
        {
            string rel = "Low";
            var query = new PocoBook();
            query.BookReliability = PocoBook.Reliability.Low;

            var books = mybooks.Read(query);

            var res = (from book in books
                       where book.BookReliability == PocoBook.Reliability.Low
                       select book.Isbn).ToList<string>();

            Assert.IsNotNull(books);
            Assert.AreEqual(2, books.Count);
            Assert.Contains("9780470512579", res);
            Assert.Contains("9788831132763", res);
        }

        [Test]
        public void TestReadBookbyFormat_GetPdf_ReturnBooks()
        {
            string format = "pdf";
            var query = new PocoBook();
            query.Format = format;

            var books = mybooks.Read(query);


            var res = (from book in books
                       where book.Format == format
                       select book.Isbn).ToList<string>();


            Assert.IsNotNull(books);
            Assert.AreEqual(12, books.Count);
            Assert.Contains("UCSD:31822028759850", res);
            Assert.Contains("OCLC:1113772535", res);
            Assert.Contains("8852010440", res);
        }



        [Test]
        public void TestUpdateBook_RemoveAuthor_ReturnBookUpdated()
        {
            string isbn = "PSU:000054186286";
            var query = new PocoBook();
            query.Isbn = isbn;

            var book = mybooks.Read(query).First();

            string[] newAuthors = { "Frank Buschmann", "Douglas C. Schmidt", "Regine Meunier", "Hans Rohnert", "Michael Stal" };


            book.Authors = book.Authors.Remove<string>("Peter Sommerlad");

            var updatedBook = mybooks.Update(book);

            Assert.AreEqual(isbn, updatedBook.Isbn);
            Assert.AreEqual(5, updatedBook.Authors.Length);
            Assert.AreEqual(newAuthors, updatedBook.Authors);
        }

        [Test]
        public void TestUpdateBook_AddCategory_ReturnBookUpdated()
        {
            string isbn = "PSU:000054186286";                
            var query = new PocoBook();
            query.Isbn = isbn;

            var book = mybooks.Read(query).First();

            string[] category = { "Computers", "Science" };

            book.Categories = book.Categories.Append<string>("Science");

            var updatedBook = mybooks.Update(book);

            Assert.AreEqual(isbn, updatedBook.Isbn);
            Assert.AreEqual(2, updatedBook.Categories.Length);
            Assert.AreEqual(category, updatedBook.Categories);
        }


        [Test]
        public void TestRemoveBook_CorrectlyRemoved()
        {
            PocoBook myNewBook = new PocoBook();
            var myguid = Guid.NewGuid();
            string title = "asdfasdfa";
            myNewBook.Isbn = myguid.ToString();
            myNewBook.Title = title;
            mybooks.Write(myNewBook);


            var test = mybooks.Read(myNewBook).First();

            Assert.AreEqual(myguid.ToString(), test.Isbn);
            Assert.AreEqual("asdfasdfa", test.Title);

            mybooks.Delete(myNewBook);


            var test2 = mybooks.Read(myNewBook);
            Assert.IsEmpty(test2);

        }

        private bool ImportJsonTestDatabase()
        {
            string result = "";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:/Program Files/MongoDB/Tools/100/bin/mongoimport.exe";
                startInfo.Arguments = @" --db testlibraryathome --collection books --jsonArray --file C:\Users\vgh8no\source\repos\BooksParser\NUnitTestBooksParser\testrepo.json";
                Process proc = new Process();
                proc.StartInfo = startInfo;
                proc.Start();
                result += "ddd";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            if (!result.Equals(""))
            {
                Thread.Sleep(1500);
                return true;
            }
            return false;
        }





    }
}
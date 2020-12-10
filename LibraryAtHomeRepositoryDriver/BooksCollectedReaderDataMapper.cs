using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LibraryAtHomeRepositoryDriver
{
    public class BooksCollectedReaderDataMapper : MongodbReadDataMapper<PocoBook>
    {
        private const string collectionname = "books";

        public IMongoCollection<PocoBook> Books
        {
            get
            {
                return Connection.Database.GetCollection<PocoBook>(collectionname);
            }
        }

        public BooksCollectedReaderDataMapper(IMongodbConnection connection) : base(connection)
        {
            if (Books == null)
                Connection.Database.CreateCollection(collectionname);

            CreateIndexes();
        }

        public override bool Equals(PocoBook x, PocoBook y)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode(PocoBook obj)
        {
            throw new NotImplementedException();
        }

        public override List<PocoBook> Read()
        {
            FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Where(x => true);
            return Books.Find<PocoBook>(filter).ToList<PocoBook>();
        }

        public override List<PocoBook> Read(PocoBook instance)
        {
            if (PocoBook.IsNullOrEmpty(instance))
            {
                throw new ArgumentNullException(nameof(instance), "Book cannot be null or empty."); 
            }


            if (!string.IsNullOrEmpty(instance.Isbn))
            {
                FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Eq("Isbn", instance.Isbn);

                try
                {
                    return Books.Find<PocoBook>(filter).ToList<PocoBook>();
                }
                catch (InvalidOperationException e)
                {
                    return new List<PocoBook>();
                }
            }
            if (!string.IsNullOrEmpty(instance.Title))
            {
                return GetBookBy("Title", instance.Title);
            }

            if (!string.IsNullOrEmpty(instance.Publisher))
            {
                return GetBookBy("Publisher", instance.Publisher);
            }

            if (!string.IsNullOrEmpty(instance.Format))
            {
                return GetBookBy("Format", instance.Format);
            }

            if (!string.IsNullOrEmpty(instance.Language))
            {
                return GetBookBy("Language", instance.Language);
            }

            if (instance.Categories != null && instance.Categories.Count > 0)
            {
                return GetBookBy("Categories", string.Join(", ", instance.Categories));
            }

            if (instance.Authors != null && instance.Authors.Count > 0)
            {
                return GetBookBy("Authors", string.Join(", ", instance.Authors));
            }

            if (instance.BookReliability != PocoBook.Reliability.Empty) 
            {
                return GetBookBy("BookReliability", instance.BookReliability.ToString());
            }

            return new List<PocoBook>();

        }

        public override long Count()
        {
            FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Where(x => true);

            return Books.CountDocuments(filter);
        }

        public override void CreateIndexes()
        {
            Books.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<PocoBook>(
                    Builders<PocoBook>.IndexKeys.Ascending(x => x.Isbn),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }),

                new CreateIndexModel<PocoBook>(
                    Builders<PocoBook>.IndexKeys.Ascending(x => x.Authors),
                    new CreateIndexOptions
                    {
                        Unique = false
                    }),
                new CreateIndexModel<PocoBook>(
                    Builders<PocoBook>.IndexKeys.Ascending(x => x.Language),
                    new CreateIndexOptions
                    {
                        Unique = false
                    }),
                new CreateIndexModel<PocoBook>(
                    Builders<PocoBook>.IndexKeys.Ascending(x => x.Publisher),
                    new CreateIndexOptions
                    {
                        Unique = false
                    }),

                new CreateIndexModel<PocoBook>(
                    Builders<PocoBook>.IndexKeys.Ascending(x => x.BookReliability),
                    new CreateIndexOptions
                    {
                        Background = false
                    }),

                new CreateIndexModel<PocoBook>(
                    Builders<PocoBook>.IndexKeys.Ascending(x => x.Categories),
                    new CreateIndexOptions
                    {
                        Background = false
                    })
            });
        }


        private List<PocoBook> GetBookBy(string fieldname, string fieldvalue)
        {
            if (string.IsNullOrEmpty(fieldvalue))
            {
                throw new ArgumentException("Field value cannot be null or empty", nameof(fieldvalue));
            }
            var filter = Builders<PocoBook>.Filter.Regex(fieldname, new BsonRegularExpression(fieldvalue));
            var result = Books.Find(filter).ToList();

            if (result != null)
                return result;
            return new List<PocoBook>();
        }
    }
}
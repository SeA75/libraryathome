﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Agreement;

namespace BooksParser
{


    public class BooksCollectedDataMapper : MongodbDataMapper<PocoBook> 
    {

        static readonly object _object = new object();

        private const string collectionname = "books";

        public BooksCollectedDataMapper(IMongodbConnection connection) : base(connection)
        {
            if (Books == null)
                Connection.Database.CreateCollection(collectionname);       

            CreateIndexes();
        }

        public override List<PocoBook> Get()
        {
            FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Where(x => true);
            return Books.Find<PocoBook>(filter).ToList<PocoBook>();
        }

        public override List<PocoBook> Get(PocoBook instance)
        {
            if (PocoBook.IsNullOrEmpty(instance))
            {
                throw new ArgumentNullException("Book cannot be null or empty."); ;
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

            if (instance.Categories != null && instance.Categories.Length > 0)
            {
                return GetBookBy("Categories", string.Join(", ", instance.Categories));
            }

            if (instance.Authors != null && instance.Authors.Length > 0)
            {
                return GetBookBy("Authors",  string.Join(", ", instance.Authors));
            }

            if (instance.BookReliability != PocoBook.Reliability.Empty) //TODO da configurare nel ctor
            {
                return GetBookBy("BookReliability", instance.BookReliability.ToString());
            }

            return new List<PocoBook>();

        }


        public override void Put(PocoBook instance)
        {
            List<PocoBook> notWrittenBooks = new List<PocoBook>();
            try
            {
                lock (_object)
                {
                    if (instance == null || string.IsNullOrEmpty(instance.Isbn))
                    {
                        throw new ArgumentNullException(nameof(instance), "Book or Isbn cannot be null or empty.");
                    }
                    
                    Books.InsertOne(instance);
                }
            }
            catch (MongoWriteException mwe)
            {
                
                throw new MongodbDataMapperException<PocoBook>(mwe.Message);
            }
        }


        public override async Task<IEnumerable<PocoBook>> BulkAsync(IEnumerable<PocoBook> instances)
        {
            List<PocoBook> notcataloged = new List<PocoBook>();
            try
            {
               await Books.InsertManyAsync(instances, new InsertManyOptions(){IsOrdered = false}).ConfigureAwait(false);
            }
            catch (MongoBulkWriteException<PocoBook> ex)
            {
                var processedWithErrors = ex.WriteErrors.Select((x => ex.Result.ProcessedRequests[x.Index] as InsertOneModel<PocoBook>))
                    .Select(x => x?.Document).ToList();

                if (ex.UnprocessedRequests.Count != 0)
                {
                    var unProcessed = ex.UnprocessedRequests.Select(x => (x as InsertOneModel<PocoBook>)?.Document).ToList();
                    notcataloged.AddRange(unProcessed);
                }

                notcataloged.AddRange(processedWithErrors);
            }
            return notcataloged;
        }


        public override PocoBook Update(PocoBook newBook)
        {
            if (newBook == null || string.IsNullOrEmpty(newBook.Isbn))
            {
                throw new ArgumentNullException(nameof(newBook), "Book or Isbn cannot be null or empty.");
            }

            FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Eq("Isbn", newBook.Isbn);

            UpdateDefinition<PocoBook> update = null;

            PocoBook queryBook = new PocoBook
            {
                Isbn = newBook.Isbn
            };

            PocoBook oldBook = Get(queryBook).First();

            update = UpdateValue(newBook.Title, oldBook.Title, "Title", update);
            update = UpdateValue(newBook.Language, oldBook.Language, "Language", update);
            update = UpdateValue(newBook.PageCount, oldBook.PageCount, "PageCount", update);
            update = UpdateVectorValue(newBook.Authors, oldBook.Authors, "Authors", update);
            update = UpdateValue(newBook.BookReliability, oldBook.BookReliability, "BookReliability", update);
            update = UpdateVectorValue(newBook.Categories, oldBook.Categories, "Categories", update);
            update = UpdateValue(newBook.Description, oldBook.Description, "Description", update);
            update = UpdateValue(newBook.File, oldBook.File, "File", update);
            update = UpdateValue(newBook.PublishedDate, oldBook.PublishedDate, "PublishedDate", update);
            update = UpdateValue(newBook.Publisher, oldBook.Publisher, "Publisher", update);

            var opts = new FindOneAndUpdateOptions<PocoBook>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            return Books.FindOneAndUpdate<PocoBook>(filter, update, opts);
        }


        public override void Delete(PocoBook instance)
        {
            if (instance == null || string.IsNullOrEmpty(instance.Isbn))
            {
                throw new ArgumentNullException(nameof(instance), "Book or Isbn cannot be null or empty.");
            }

            Books.DeleteOne(x => x.Isbn == instance.Isbn);
        }

        public override void Drop()
        {
            Connection.Database.DropCollection(collectionname);
        }

        public override long Count()
        {
            FilterDefinition<PocoBook> filter = Builders<PocoBook>.Filter.Where(x => true);

            return Books.CountDocuments(filter);
        }

        public IMongoCollection<PocoBook> Books
        {
            get
            {
                return Connection.Database.GetCollection<PocoBook>(collectionname);
            }
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

        public override bool Equals(PocoBook b1, PocoBook b2)
        {
            if (b1 == null && b2 == null)
                return true;
            else if (b1 == null || b2 == null)
                return false;

            return (b1 == b2);
        }

        public override int GetHashCode(PocoBook bx)
        {            
            return bx.GetHashCode();
        }
    }
}

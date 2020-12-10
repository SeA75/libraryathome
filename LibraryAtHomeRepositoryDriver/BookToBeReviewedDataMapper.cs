﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAtHomeRepositoryDriver
{
    public class BookToBeReviewedDataMapper : MongodbDataMapper<BookToBeReviewed>
    {
        static readonly object _object = new object();

        private const string collectionname = "booktobereviewed";

        public BookToBeReviewedDataMapper(IMongodbConnection connection) : base(connection)
        {
            if (BooksToReviewList == null)
                Connection.Database.CreateCollection(collectionname);

            CreateIndexes();
        }

        public override void Delete(BookToBeReviewed instance)
        {
            if (instance == null || string.IsNullOrEmpty(instance.File))
            {
                throw new ArgumentNullException(nameof(instance), "Book to be review cannot be null or empty.");
            }
            
            BooksToReviewList.DeleteOne(x => x.File == instance.File);
        }

        public override List<BookToBeReviewed> Read()
        {
            FilterDefinition<BookToBeReviewed> filter = Builders<BookToBeReviewed>.Filter.Where(x => true);
            return BooksToReviewList.Find<BookToBeReviewed>(filter).ToList<BookToBeReviewed>();
        }

        public override List<BookToBeReviewed> Read(BookToBeReviewed instance)
        {
            if (instance == null || string.IsNullOrEmpty(instance.File))
            {
                throw new ArgumentNullException(nameof(instance), "Book to be reviewed cannot be null or empty.");
            }

            FilterDefinition<BookToBeReviewed> filter = Builders<BookToBeReviewed>.Filter.Eq("File", instance.File);

            try
            {
                return BooksToReviewList.Find<BookToBeReviewed>(filter).ToList<BookToBeReviewed>();
            }
            catch (InvalidOperationException)
            {
                return new List<BookToBeReviewed>();
            }


        }

        public override void Write(BookToBeReviewed instance)
        {
            lock (_object)
            {
                if (instance == null || string.IsNullOrEmpty(instance.File))
                {
                    throw new ArgumentNullException(nameof(instance), "Book cannot be null or empty.");
                }

                BooksToReviewList.InsertOne(instance);
            }
        }

        public override async Task<IEnumerable<BookToBeReviewed>> BulkAsync(IEnumerable<BookToBeReviewed> instances)
        {
            try
            {
                if(instances.Any())
                    await BooksToReviewList.InsertManyAsync(instances,
                        new InsertManyOptions() { IsOrdered = false, BypassDocumentValidation = false }).ConfigureAwait(false);
            }
            catch (MongoBulkWriteException<BookToBeReviewed>)
            {
            }
            return new List<BookToBeReviewed>();
        }

        public override BookToBeReviewed Update(BookToBeReviewed instance)
        {
            if (instance == null || string.IsNullOrEmpty(instance.File))
            {
                throw new ArgumentNullException(nameof(instance), "Book cannot be null or empty.");
            }

            FilterDefinition<BookToBeReviewed> filter = Builders<BookToBeReviewed>.Filter.Eq("File", instance.File);

            UpdateDefinition<BookToBeReviewed> update = null;

            BookToBeReviewed queryBook = new BookToBeReviewed(instance.File, instance.FailureReason);

            BookToBeReviewed oldBook = Read(queryBook).First();

            update = UpdateValue(instance.File, oldBook.File, "File", update);
            update = UpdateValue(instance.FailureReason, oldBook.FailureReason, "FailureReason", update);

            var opts = new FindOneAndUpdateOptions<BookToBeReviewed>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            return BooksToReviewList.FindOneAndUpdate<BookToBeReviewed>(filter, update, opts);
        }

        public override void Drop()
        {
            Connection.Database.DropCollection(collectionname);
        }

        public override long Count()
        {
            FilterDefinition<BookToBeReviewed> filter = Builders<BookToBeReviewed>.Filter.Where(x => true);

            return BooksToReviewList.CountDocuments(filter);
        }

        public IMongoCollection<BookToBeReviewed> BooksToReviewList
        {
            get
            {
                return Connection.Database.GetCollection<BookToBeReviewed>(collectionname);
            }
        }

        public override void CreateIndexes()
        {
            BooksToReviewList.Indexes.CreateOne(
                new CreateIndexModel<BookToBeReviewed>(Builders<BookToBeReviewed>.IndexKeys.Ascending(x => x.File),
                new CreateIndexOptions { Unique = true }));
        }

        public override bool Equals(BookToBeReviewed x, BookToBeReviewed y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            return (x == y);
        }

        public override int GetHashCode(BookToBeReviewed obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Book to be reviewed cannot be null.");
            }
            return obj.GetHashCode();
        }
    }
}


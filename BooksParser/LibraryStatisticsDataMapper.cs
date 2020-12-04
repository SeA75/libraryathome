using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooksParser
{
    public class LibraryStatisticsDataMapper : MongodbDataMapper<LibraryStatistics>
    {

        static readonly object _object = new object();

        private const string collectionname = "statistics";

        public LibraryStatisticsDataMapper(IMongodbConnection connection) : base(connection)
        {
            if (Statistics == null)
               Connection. Database.CreateCollection(collectionname);
            
        }

        public IMongoCollection<LibraryStatistics> Statistics
        {
            get
            {
                return Connection.Database.GetCollection<LibraryStatistics>(collectionname);
            }
        }

        public override void Delete(LibraryStatistics instance)
        {
            if (instance == null )
            {
                throw new ArgumentNullException(nameof(instance), "Statistics cannot be null.");
            }

            Statistics.DeleteOne(x => x.Timestamp == instance.Timestamp);
        }

        public override List<LibraryStatistics> Get()
        {
            FilterDefinition<LibraryStatistics> filter = Builders<LibraryStatistics>.Filter.Where(x => true);
            return Statistics.Find(filter).ToList();
        }

        public override List<LibraryStatistics> Get(LibraryStatistics instance)
        {
            FilterDefinition<LibraryStatistics> filter = Builders<LibraryStatistics>.Filter.Where(x => true);
            return Statistics.Find(filter).ToList();
        }

        public override void Put(LibraryStatistics instance)
        {
            lock (_object)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }
                
                Statistics.InsertOne(instance);                
            }
        }


        public override async Task<IEnumerable<LibraryStatistics>> BulkAsync(IEnumerable<LibraryStatistics> instances)
        {
            try
            {
                await Statistics.InsertManyAsync(instances,
                       new InsertManyOptions() { IsOrdered = false, BypassDocumentValidation = false }).ConfigureAwait(false);
            }
            catch(MongoBulkWriteException<LibraryStatistics> e)
            {

            }
            return new List<LibraryStatistics>();
        }

        public override LibraryStatistics Update(LibraryStatistics instance)
        {
            return instance;
        }

        public override void CreateIndexes()
        {
        }

        public override long Count()
        {
            FilterDefinition<LibraryStatistics> filter = Builders<LibraryStatistics>.Filter.Where(x => true);

            return Statistics.CountDocuments(filter);
        }

        public override void Drop()
        {
            Connection.Database.DropCollection(collectionname);
        }


        public override bool Equals(LibraryStatistics b1, LibraryStatistics b2)
        {
            if (b1 == null && b2 == null)
                return true;
            else if (b1 == null || b2 == null)
                return false;

            return (b1 == b2);
        }

        public override int GetHashCode(LibraryStatistics bx)
        {
            return bx.GetHashCode();
        }
    }
}
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksParser
{


    public abstract class MongodbDataMapper<TCOLL> : EqualityComparer<TCOLL>
    {
        /// <summary>
        /// A .Net database connection (SQL Server, MySql, Oracle, etc.... )
        /// </summary>
        public IMongodbConnection Connection { get; private set; }


        /// <summary>
        /// Reads configuration from the app.config and initializes the data mapper
        /// </summary>
        /// <param name="connection">A .net connection that implements IDbConnection</param>
        protected MongodbDataMapper(IMongodbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("Database connection cannot be null.");

            this.Connection = connection;
        }

        /// <summary>
        /// Default select method for type T
        /// </summary>
        /// <param name="exError">Out exception object</param>
        /// <returns>List of type T</returns>
        public abstract List<TCOLL> Get();

        /// <summary>
        /// Default create method for type T
        /// </summary>
        /// <param name="instance">The instance to create</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Boolean success/failure</returns>
        public abstract void Put(TCOLL instance);


        /// <summary>
        /// Default create method for type T
        /// </summary>
        /// <returns>Boolean success/failure</returns>
        public abstract Task<IEnumerable<TCOLL>> BulkAsync(IEnumerable<TCOLL> instances);

        /// <summary>
        /// Default read method for type T 
        /// </summary>
        /// <param name="instance">Object of instance to read</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Instance of type T</returns>
        public abstract List<TCOLL> Get(TCOLL instance);

        /// <summary>
        /// Default update method for type T
        /// </summary>
        /// <param name="instance">Object of instance to update</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Instance of type T</returns>
        public abstract TCOLL Update(TCOLL instance);

        
        /// <summary>
        /// Default delete method for type T 
        /// </summary>
        /// <param name="instance">Object of instance to delete</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Boolean success/failure</returns>
        public abstract void Delete(TCOLL instance);


        public abstract long Count();

        public abstract void CreateIndexes();

        public abstract void Drop();



        internal static bool CheckEquality<T>(T[] first, T[] second)
        {
            if (first.SequenceEqual<T>(second))
                return true;
            return false;
        }  



        internal static UpdateDefinition<TCOLL> UpdateValue<T>(T newValue, T oldValue, string fieldname, UpdateDefinition<TCOLL> update)
        {
            if (!EqualityComparer<T>.Default.Equals(newValue, oldValue))
                update = Builders<TCOLL>.Update.Set(fieldname, newValue);
            return update;
        }

        internal static UpdateDefinition<TCOLL> UpdateVectorValue<T>(T[] newValue, T[] oldValue, string fieldname, UpdateDefinition<TCOLL> update)
        {
            if (!CheckEquality<T>(oldValue, newValue))
                update = Builders<TCOLL>.Update.Set(fieldname, newValue);
            return update;
        }


    }

}
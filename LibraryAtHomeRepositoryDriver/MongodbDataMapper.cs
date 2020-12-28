using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;



namespace LibraryAtHomeRepositoryDriver
{

    public abstract class MongodbDataMapper<TCOLL> : EqualityComparer<TCOLL>, IMongoDataMapper<TCOLL>/*, IBsonSerializationProvider*/
    {
        /// <summary>
        /// Default select method for type T
        /// </summary>
        /// <param name="exError">Out exception object</param>
        /// <returns>List of type T</returns>
        public abstract List<TCOLL> Read();

        /// <summary>
        /// Default read method for type T 
        /// </summary>
        /// <param name="instance">Object of instance to read</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Instance of type T</returns>
        public abstract List<TCOLL> Read(TCOLL instance);

        /// <summary>
        /// Default create method for type T
        /// </summary>
        /// <param name="instance">The instance to create</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Boolean success/failure</returns>
        public abstract void Write(TCOLL instance);

        /// <summary>
        /// Default create method for type T
        /// </summary>
        /// <returns>Boolean success/failure</returns>
        public abstract Task<IEnumerable<TCOLL>> BulkAsync(IEnumerable<TCOLL> instances);
        

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

        internal static bool CheckEquality<T>(Collection<T> first, Collection<T> second)
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

        internal static UpdateDefinition<TCOLL> UpdateVectorValue<T>(Collection<T> newValue, Collection<T> oldValue, string fieldname, UpdateDefinition<TCOLL> update)
        {
            if (!CheckEquality<T>(oldValue, newValue))
                update = Builders<TCOLL>.Update.Set(fieldname, newValue);
            return update;
        }
    }

}
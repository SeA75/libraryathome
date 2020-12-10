using System;
using System.Collections.Generic;

namespace LibraryAtHomeRepositoryDriver
{
    /// <summary>
    /// Class used for reading operation from web app
    /// </summary>
    /// <typeparam name="TCOLL"></typeparam>
    public abstract class MongodbReadDataMapper<TCOLL> : EqualityComparer<TCOLL>, IMongoDataReadMapper<TCOLL>
    {

        /// <summary>
        /// A .Net database connection (SQL Server, MySql, Oracle, etc.... )
        /// </summary>
        public IMongodbConnection Connection { get; private set; }


        /// <summary>
        /// Reads configuration from the app.config and initializes the data mapper
        /// </summary>
        /// <param name="connection">A .net connection that implements IDbConnection</param>
        protected MongodbReadDataMapper(IMongodbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), "Database connection cannot be null.");

            this.Connection = connection;
        }

        public abstract List<TCOLL> Read();

        public abstract List<TCOLL> Read(TCOLL instance);

        public abstract long Count();

        public abstract void CreateIndexes();
    }
}
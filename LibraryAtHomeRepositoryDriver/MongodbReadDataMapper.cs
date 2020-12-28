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

        public abstract List<TCOLL> Read();

        public abstract List<TCOLL> Read(TCOLL instance);

        public abstract long Count();

        public abstract void CreateIndexes();
    }
}
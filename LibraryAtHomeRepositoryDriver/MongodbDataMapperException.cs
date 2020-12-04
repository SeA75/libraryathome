using System;

namespace LibraryAtHomeRepositoryDriver
{
    public class MongodbDataMapperException<TCOLL> : Exception
    {
        public MongodbDataMapperException()
        {
        }

        public MongodbDataMapperException(string message)
            : base(message)
        {
        }

        public MongodbDataMapperException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
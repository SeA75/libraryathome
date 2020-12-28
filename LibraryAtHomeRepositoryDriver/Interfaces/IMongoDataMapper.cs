namespace LibraryAtHomeRepositoryDriver
{
    public interface IMongoDataMapper<TCOLL> : IMongoDataReadMapper<TCOLL>, IMongoDataWriteMapper<TCOLL>
    {

    }
}
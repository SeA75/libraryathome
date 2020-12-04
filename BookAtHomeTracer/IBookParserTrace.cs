namespace BooksParser
{
    public interface IBookParserTrace
    {
        void TraceInfo(string message, params object[] list);

        void TraceWarning(string message, params object[] list);

        void TraceError(string message, params object[] list);

        TraceLevel MessageLevel { get; set; }

    };

    public enum TraceLevel
    {
        Info,
        Warning,
        Error,
        None
    }
}

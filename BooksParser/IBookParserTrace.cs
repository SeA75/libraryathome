namespace BooksParser
{
    public interface IBookParserTrace
    {
        public void TraceInfo(string message, params object[] list);

        public void TraceWarning(string message, params object[] list);

        public void TraceError(string message, params object[] list);

        public TraceLevel MessageLevel { get; set; }

    };

    public enum TraceLevel
    {
        Info,
        Warning,
        Error,
        None
    }
}

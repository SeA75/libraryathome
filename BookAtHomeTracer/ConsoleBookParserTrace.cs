using System.Diagnostics;

namespace LibraryAtHomeTracer
{
    public class ConsoleBookParserTrace : IBookParserTrace
    {

        public ConsoleBookParserTrace()
        {
            TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);

            Trace.Listeners.Add(tr1);
        }
        public TraceLevel MessageLevel { get; set; }


        public void TraceError(string formatStr, params object[] arguments)
        {
            // Do some checking and apply some logic
            string message = string.Format(formatStr, arguments);
            Trace.WriteLineIf(MessageLevel <= TraceLevel.Error, message);
        }

        public void TraceInfo(string formatStr, params object[] arguments)
        {
            // Do some checking and apply some logic
            string message = string.Format(formatStr, arguments);

            Trace.WriteLineIf(MessageLevel <= TraceLevel.Info, message);
        }

        public void TraceWarning(string formatStr, params object[] arguments)
        {
            // Do some checking and apply some logic
            string message = string.Format(formatStr, arguments);
            Trace.WriteLineIf(MessageLevel <= TraceLevel.Warning, message);
        }
    }
}

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace BooksParser
{
    public class FileBookParserTrace : IBookParserTrace
    {
        ~FileBookParserTrace()  // finalizer
        {
            Trace.Flush();
        }
              

        public FileBookParserTrace(string filename, string tracelevel)
        {
            if(Enum.Parse<TraceLevel>(tracelevel) != TraceLevel.None)
            {
                TextWriterTraceListener tr2 = new TextWriterTraceListener(System.IO.File.CreateText(filename));
                Trace.Listeners.Add(tr2);
            }
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
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture);
            string message = string.Format(formatStr, arguments);

            string toDump = string.Format("Thread id [{0}] Time [{1}] {2}", Thread.CurrentThread.ManagedThreadId, timestamp, message);

            Trace.WriteLineIf(MessageLevel <= TraceLevel.Info, toDump);
        }

        public void TraceWarning(string formatStr, params object[] arguments)
        {
            // Do some checking and apply some logic
            string message = string.Format(formatStr, arguments);
            Trace.WriteLineIf(MessageLevel <= TraceLevel.Warning, message);
        }
    }
}

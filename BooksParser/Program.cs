using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using LibraryAtHomeTracer;


namespace BooksParser
{
    class Program
    {

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string configFile = @"C:\Users\vgh8no\source\repos\BooksParser\BooksParser\config\ebookConf.json";
           
            var configuration = BookParserConfig.FromJson(File.ReadAllText(configFile));
      
            IBookParserTrace trace = new FileBookParserTrace(configuration.tracefile, configuration.tracelevel);

            using var progress = new ProgressBar();

            var exceptions = new ConcurrentQueue<Exception>();

            LibraryCataloguer cataloger = new LibraryCataloguer(configuration, exceptions, trace, progress);
            

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                await cataloger.CatalogBooksAsync().ConfigureAwait(false);

                sw.Stop();

                cataloger.CreateStatistics(sw.Elapsed);

            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.Flatten().InnerExceptions)
                {
                    if (ex is ArgumentException)
                        Console.WriteLine(ex.Message);
                }
            }

        }
    }



}





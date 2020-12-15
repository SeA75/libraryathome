using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LibraryAtHomeProvider;
using LibraryAtHomeTracer;
using NDesk.Options;
using Org.BouncyCastle.Asn1.X509.Qualified;


namespace BooksParser
{
    class Program
    {
        static int verbosity;
        static async System.Threading.Tasks.Task Main(string[] args)
        {
           

            var configFile = GetConfigFileFromCommandLine(args);

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

        private static string GetConfigFileFromCommandLine(string[] args)
        {
            bool show_help = false;
            string configFile = string.Empty; //@"C:\Users\vgh8no\source\repos\BooksParser\BooksParser\config\ebookConf.json";
            int repeat = 1;

            var p = new OptionSet()
            {
                {
                    "c|configfile=", "path to the {CONFIGFILE}.",
                    v => configFile = v
                },
                {
                    "v", "increase debug message verbosity",
                    v =>
                    {
                        if (v != null) ++verbosity;
                    }
                },
                {
                    "h|help", "show this message and exit",
                    v => show_help = v != null
                },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("BookParser: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `BookParser --help' for more information.");
                return configFile;
            }

            if (show_help)
            {
                ShowHelp(p);
                return configFile;
            }

            return configFile;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: BookParser [OPTIONS]+ message");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        

    }



}





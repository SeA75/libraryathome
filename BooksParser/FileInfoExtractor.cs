using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibraryAtHomeRepositoryDriver;


namespace BooksParser
{

    public class FileInfoExtractor : FileExtractor
    {
        public override BookatHome GetPocoBook(string filepath)
        {
            string maybetitle;
            
            maybetitle = Path.GetFileNameWithoutExtension(filepath).Replace("_", " ").Replace("ebook", "").Replace("'", "");

            return new PocoBook(filepath, string.Empty, null, string.Empty, maybetitle);
        }
    }
}

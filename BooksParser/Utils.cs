using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BooksParser
{
    public static class Utils
    {
        public static Func<string, string> GetExtension = s => Path.GetExtension(s).ToLowerInvariant();
    }
}

   

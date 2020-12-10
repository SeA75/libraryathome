using System;
using System.IO;

namespace BooksParser
{
    public static class Utils
    {
        public static Func<string, string> GetExtension = s => Path.GetExtension(s).ToLowerInvariant();
    }
}

   

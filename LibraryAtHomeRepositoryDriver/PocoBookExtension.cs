using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAtHomeRepositoryDriver
{
    public static class PocoBookExtension
    {
        public static Func<string, string> FileNameToTitle = s => 
                Path.GetFileNameWithoutExtension(s).Replace("_", "' ").Replace("ebook", "").Trim().Trim();
    }
}

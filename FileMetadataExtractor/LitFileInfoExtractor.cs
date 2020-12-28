using System.IO;
using LibraryAtHomeRepositoryDriver;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public class LitFileInfoExtractor : FileInfoExtractor
    {
        public override BookAtHome GetPocoBook(string filepath)
        {
            string maybetitle = Path.GetFileNameWithoutExtension(filepath).Replace("_", " ");
            return new PocoBook(filepath, string.Empty, null, string.Empty, maybetitle);
        }
    }
}

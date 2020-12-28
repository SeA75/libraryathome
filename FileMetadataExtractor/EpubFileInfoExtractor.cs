using System.Collections.ObjectModel;
using EpubSharp;
using System.Linq;
using LibraryAtHomeRepositoryDriver;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public class EpubFileInfoExtractor : FileInfoExtractor
    {
        public override BookAtHome GetPocoBook(string filepath)
        {
            EpubBook book = EpubReader.Read(filepath);

            if (string.IsNullOrEmpty(book.Title))
            {
                return base.GetPocoBook(filepath);
            }
            
            return new PocoBook(filepath, book.Title, new Collection<string>(book.Authors.ToList()), string.Empty, book.Title);
        }
    }
}

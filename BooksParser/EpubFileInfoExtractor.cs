using EpubSharp;
using System.Linq;
using LibraryAtHomeRepositoryDriver;

namespace BooksParser
{
    public class EpubFileInfoExtractor : FileInfoExtractor
    {
        public override BookatHome GetPocoBook(string filepath)
        {
            EpubBook book = EpubReader.Read(filepath);

            if (string.IsNullOrEmpty(book.Title))
            {
                return base.GetPocoBook(filepath);
            }
            
            return new PocoBook(filepath, book.Title, book.Authors.ToArray(), string.Empty, book.Title);
        }
    }
}

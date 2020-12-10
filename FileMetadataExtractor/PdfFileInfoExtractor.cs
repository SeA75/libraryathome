using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.ObjectModel;
using LibraryAtHomeRepositoryDriver;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public class PdfFileInfoExtractor : FileInfoExtractor
    {
        public override BookatHome GetPocoBook(string filepath)
        {
            PdfDocument document = new PdfDocument();

            try
            {
                document = PdfReader.Open(filepath, PdfDocumentOpenMode.ReadOnly);

                if (string.IsNullOrEmpty(document.Info.Title))
                {
                    return base.GetPocoBook(filepath);
                }

                return new PocoBook(filepath, document.Info.Title, new  Collection<string>(){document.Info.Author}, string.Empty, document.Info.Title);
            }
            catch(Exception ex)
            {
                if(ex is PdfReaderException || ex is InvalidOperationException)
                {
                    return base.GetPocoBook(filepath);
                }
                throw;
            }           
            finally
            {
                document?.Dispose();
            }
        }
    }
}

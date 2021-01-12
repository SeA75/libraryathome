using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LibraryAtHomeRepositoryDriver;

namespace LibraryAtHomeTracerFileMetadataExtractor
{
    public class PdfTextReaderFileInfoExtractor : PdfFileInfoExtractor
    {
        public override BookAtHome GetPocoBook(string filepath)
        {
            iText.Kernel.Pdf.PdfReader reader = null;
            iText.Kernel.Pdf.PdfDocument pDoc = null;
            const int extracted_phrase_len = 50;
            try
            {

                if (File.Exists(filepath))
                {
                    reader = new iText.Kernel.Pdf.PdfReader(filepath);
                    pDoc = new iText.Kernel.Pdf.PdfDocument(reader);
                    int nPages = pDoc.GetNumberOfPages();
                    int maxsearch = nPages < 15 ? nPages : 10;
                    string currentText = string.Empty;
                    for (int i = 1; i <= maxsearch; i++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        currentText = PdfTextExtractor.GetTextFromPage(pDoc.GetPage(i), strategy);

                        currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));

                        if (currentText.ToLower().Contains("isbn", System.StringComparison.InvariantCulture))
                        {
                            string pattern = @"ISBN(-1(?:(0)|3))?:?\x20(\s)*[0-9]+[- ][0-9]+[- ][0-9]+[- ][0-9]*[- ]*[xX0-9]";

                            Match m = Regex.Match(currentText.Replace(":", "").Replace("-", " "), pattern);

                            if (m.Success)
                            {
                                PocoBook retBook = base.GetPocoBook(filepath) as PocoBook;
                                retBook.Isbnsearch = m.Value;                               
                                return retBook;
                            }
                        }
                    }
                    PocoBook abook = base.GetPocoBook(filepath) as PocoBook;
                    int len = currentText.Length < extracted_phrase_len ? currentText.Length : extracted_phrase_len;
                    abook.SearchPhrase = currentText.Substring(0, len);

                    return abook;

                }

            }
            catch (IOException)
            {
                return new PocoBook(filepath);
            }
            finally
            {
                ((IDisposable)reader)?.Dispose();
                ((IDisposable)pDoc)?.Dispose();
                reader?.Close();
            }
            
            return base.GetPocoBook(filepath);

        }
    }
}

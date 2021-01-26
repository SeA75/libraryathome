using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LibraryAtHomeProvider
{
    public static class Extensions
    {
        public static DateTime ToDateTime(this String dateString)
        {
            if (!DateTime.TryParse(dateString, out var pubDate))
            {
                string[] formats = { "yyyy" };

                DateTime.TryParseExact(dateString, formats,
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out pubDate);
            }

            return pubDate;
        }

        public static string ToIsbn(this Item item)
        {
            if (item.volumeInfo.industryIdentifiers != null && item.volumeInfo.industryIdentifiers.Length == 1)
            {
                return item.volumeInfo.industryIdentifiers[0].identifier;
            }

            if (item.volumeInfo.industryIdentifiers != null && item.volumeInfo.industryIdentifiers.Length == 2)
            {
                if ((item.volumeInfo.industryIdentifiers[1].type == "ISBN_13") ||
                    item.volumeInfo.industryIdentifiers[0].type == "ISBN_13")
                {
                    return item.volumeInfo.industryIdentifiers[1].identifier;
                }

                else
                {
                    if ((item.volumeInfo.industryIdentifiers[1].type == "ISBN_10") ||
                        item.volumeInfo.industryIdentifiers[0].type == "ISBN_10")
                    {
                        return item.volumeInfo.industryIdentifiers[1].identifier;
                    }
                }
            }

            return null;
        }


        public static string UniqueIdentifier(this FileInfo file)
        {
            SHA256 Sha256 = SHA256.Create();
            byte[] myHash;

            using (FileStream stream = File.OpenRead(file.FullName))
            {
                myHash = Sha256.ComputeHash(stream);
            }

            StringBuilder result = new StringBuilder();
            foreach (byte b in myHash)
                result.Append(b.ToString("x2"));

            return result.ToString();
        }       

    }
}

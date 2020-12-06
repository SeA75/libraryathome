using System;
using System.Collections.Generic;
using System.Text;

namespace BookatHomeProvider
{
    public static class Extensions
    {
        public static DateTime ToDateTime(this String dateString)
        {
            DateTime pubDate;
            if (!DateTime.TryParse(dateString, out pubDate))
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


        public static Func<string, string> GenerateUniqueFileId = s =>
            $"PersonalID {GoogleBookProvider.BytesToString(GoogleBookProvider.GetHashSha256(s))}";
       
    }
}

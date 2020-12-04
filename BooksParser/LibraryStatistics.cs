using System;

namespace BooksParser
{
    public class LibraryStatistics
    {
        public LibraryStatistics(long totbooks, long bookscollected, TimeSpan elapsedTime, string folder)
        {
            TotalBooks = totbooks;
            NumberOfCollectedBook = bookscollected;        


            ElapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", elapsedTime.Hours, elapsedTime.Minutes,
                    elapsedTime.Seconds, elapsedTime.Milliseconds / 10); 

            LibraryDirectory = folder;
            Timestamp = DateTime.Now.ToString();
            long successratio = (100 * NumberOfCollectedBook) / (TotalBooks);
            SuccessRatio = successratio.ToString() + "%";
            BooksPerSeconds = Convert.ToDouble(TotalBooks) / Convert.ToDouble(elapsedTime.Seconds);
        }
        public long TotalBooks { get; set; }

        public long NumberOfCollectedBook { get; set; }

        public string ElapsedTime { get; set; } //TODO da convertire in ISODate

        public string LibraryDirectory { get; set; }

        public string Timestamp { get; set; }

        public string SuccessRatio { get; set; }

        public double BooksPerSeconds { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BookatHome);
        }


        public override string ToString()
        {
            return "Statistics for library: " + LibraryDirectory + " Timestamp: " + Timestamp + " Total books: " + TotalBooks + " Collected book in library: " + NumberOfCollectedBook +
                "Process time " + ElapsedTime + " Success ratio: " + SuccessRatio;
        }

        public bool Equals(LibraryStatistics p)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            if (this.ToString() == p.ToString())
                return true;
            return false;
        }



        public override int GetHashCode()
        {
            return string.GetHashCode(this.ToString());
        }


        public static bool operator ==(LibraryStatistics lhs, LibraryStatistics rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(LibraryStatistics lhs, LibraryStatistics rhs)
        {
            return !(lhs == rhs);
        }
    }
}
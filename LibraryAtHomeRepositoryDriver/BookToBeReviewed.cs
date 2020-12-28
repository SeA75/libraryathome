using MongoDB.Bson.Serialization.Attributes;
using System;

namespace LibraryAtHomeRepositoryDriver
{
    //TODO elimanare la dipendenza
    [BsonIgnoreExtraElements]
    public class BookToBeReviewed : BookAtHome
    {
        public BookToBeReviewed(string filepath, string failureReason)
        {
            File = filepath;
            FailureReason = failureReason;
        }

        public string FailureReason { get; set; }


        public override bool Equals(object obj)
        {
            return this.Equals(obj as BookToBeReviewed);
        }

        public bool Equals(BookToBeReviewed p)
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

            if (File == p.File && FailureReason == p.FailureReason)
                return true;
            return false;
        }



        public override int GetHashCode()
        {           
            return string.GetHashCode(File+FailureReason);
        }


        public static bool operator ==(BookToBeReviewed lhs, BookToBeReviewed rhs)
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

        public static bool operator !=(BookToBeReviewed lhs, BookToBeReviewed rhs)
        {
            return !(lhs == rhs);
        }
    }
}


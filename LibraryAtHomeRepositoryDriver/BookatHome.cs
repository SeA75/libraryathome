using System;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryAtHomeRepositoryDriver
{

    public interface IBookatHome
    {
        string File { get; set; }
    }

    //TODO elimanare la dipendenza
    [BsonIgnoreExtraElements]
    public class BookatHome : IBookatHome
    {
        public string File { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BookatHome);
        }

        public bool Equals(BookatHome p)
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

            if (File == p.File)
                return true;
            return false;
        }



        public override int GetHashCode()
        {
            return string.GetHashCode(File);
        }


        public static bool operator ==(BookatHome lhs, BookatHome rhs)
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

        public static bool operator !=(BookatHome lhs, BookatHome rhs)
        {
            return !(lhs == rhs);
        }
    }
}


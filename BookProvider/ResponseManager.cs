using RestSharp;
using System;

namespace LibraryAtHomeProvider
{
    public class ResponseManager : IRestResponseManager
    {
        private readonly IRestResponse response;

        public ResponseManager()
        {
            response = new RestResponse();
        }

        public ResponseManager(IRestResponse response)
        {
            this.response = response ?? throw new ArgumentNullException(nameof(response), "responde cannot be null");
        }

        public string GetResponseContent()
        {
            return response.Content;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ResponseManager);
        }

        public bool Equals(ResponseManager p)
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

            return (this.ToString() == p.ToString());
        }



        public override int GetHashCode()
        {
            return response.GetHashCode();
        }


        public static bool IsNullOrEmpty(ResponseManager instance)
        {
            if (instance == null)
                return true;
            return (instance == new ResponseManager());
        }

        public static bool operator ==(ResponseManager lhs, ResponseManager rhs)
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

        public static bool operator !=(ResponseManager lhs, ResponseManager rhs)
        {
            return !(lhs == rhs);
        }

        
    }
}
using RestSharp;
using System;

namespace LibraryAtHomeProvider
{
    public class RestRequestManager : IRestRequestManager
    {
        private readonly IRestClient _client;

        public RestRequestManager(RestClient client)
        {
            if(client == null)
            {
                throw new ArgumentNullException(nameof(client), "client cannot be null");
            } 
            
            _client = client;
        }

        public IRestResponseManager ExecuteGet()
        {
            var res = _client.Execute(new RestRequest(Method.GET));

            if(res != null)
            {
                IRestResponseManager retval = new ResponseManager(res);

                return retval;
            }

            return new ResponseManager();
        }

        public void SetBaseUri(Uri url)
        {
            _client.BaseUrl = url;
        }
    }
}
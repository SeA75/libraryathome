using System;

namespace LibraryAtHomeProvider
{
    public interface IRestRequestManager
    {
        IRestResponseManager ExecuteGet();

        void SetBaseUri(Uri url);
    }
}
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core
{
    public class ClientContext : Microsoft.SharePoint.Client.ClientContext
    {
        private int _maxRetries = 1;
        private int _retryDelay = 500;
        private Microsoft.SharePoint.Client.ClientContext _clientContext;

        public ClientContext(string url, int retryCount = 1, int delay = 500) : base(url)
        {
            if (retryCount <= 0)
                throw new ArgumentException("Provide a retry count greater than zero.");

            if (delay <= 0)
                throw new ArgumentException("Provide a delay greater than zero.");

            this._maxRetries = retryCount;
            this._retryDelay = delay;
        }

        public ClientContext(Uri uri, int retryCount = 0, int delay = 500)
            : base(uri)
        {
            if (retryCount <= 0)
                throw new ArgumentException("Provide a retry count greater than zero.");

            if (delay <= 0)
                throw new ArgumentException("Provide a delay greater than zero.");

            this._maxRetries = retryCount;
            this._retryDelay = delay;
        }

        public new void ExecuteQuery()
        {
            int retryCount = 0;
            int delay = _retryDelay;

            while (retryCount < _maxRetries)
            {
                try
                {
                    base.ExecuteQuery();
                    return;

                }
                catch (WebException wex)
                {
                    var response = wex.Response as HttpWebResponse;

                    // Check if request was throttled - http status code 429
                    if (response != null && response.StatusCode == (HttpStatusCode)429)
                    {
                        LoggingUtility.Internal.TraceWarning((int)EventId.RequestFrequencyExceeded, CoreResources.ClientContext_RequestFrequencyExceed, delay);

                        System.Threading.Thread.Sleep(delay);

                        retryCount++;
                        delay = delay * 2;
                    }
                }
            }
            throw new MaximumRetryAttemptedException(string.Format(CoreResources.ClientContext_MaxRetryAttemptsExceeded, retryCount));
        }



        public class MaximumRetryAttemptedException : Exception
        {
            public MaximumRetryAttemptedException(string message)
                : base(message)
            {

            }
        }
    }
}

using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Enums;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    public class SPOnlineConnection
    {
        private ClientContext _initialContext;

        internal static SPOnlineConnection CurrentConnection { get; set; }
        public ConnectionType ConnectionType { get; protected set; }
        public int MinimalHealthScore { get; protected set; }
        public int RetryCount { get; protected set; }
        public int RetryWait { get; protected set; }
        public PSCredential PSCredential { get; protected set; }

        public string Url { get; protected set; }

        public ClientContext Context { get; set; }

        public SPOnlineConnection(ClientContext context, ConnectionType connectionType, int minimalHealthScore, int retryCount, int retryWait, PSCredential credential, string url)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;
            _initialContext = context;
            ConnectionType = connectionType;
            MinimalHealthScore = minimalHealthScore;
            RetryCount = retryCount;
            RetryWait = retryWait;
            PSCredential = credential;
            Url = url;
        }

        public void RestoreCachedContext()
        {
            Context = _initialContext;
        }

        internal void CacheContext()
        {
            _initialContext = Context;
        }
    }
}

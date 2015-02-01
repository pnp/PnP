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

        private string _url;
      
        public string Url
        {
            get
            {
                return this._url;
                //return this.Context.Url;
            }
            protected set
            {
                this._url = value;
            }
        }

        public ClientContext Context { get; set; }

        public SPOnlineConnection(ClientContext context, ConnectionType connectionType, int minimalHealthScore, int retryCount, int retryWait, PSCredential credential, string url)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            this.Context = context;
            this._initialContext = context;
            this.ConnectionType = connectionType;
            this.MinimalHealthScore = minimalHealthScore;
            this.RetryCount = retryCount;
            this.RetryWait = retryWait;
            this.PSCredential = credential;
            this.Url = url;
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

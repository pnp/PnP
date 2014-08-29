using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    public class SPOnlineConnection
    {
        internal static SPOnlineConnection CurrentConnection { get; set; }
        public ConnectionTypes ConnectionType { get; protected set; }
        public int MinimalHealthScore { get; protected set; }
        public int RetryCount { get; protected set; }
        public int RetryWait { get; protected set; }
        public PSCredential PSCredential { get; protected set; }
        public string Url
        {
            get
            {
                return this.Context.Url;
            }
        }

        public ClientContext Context { get; protected set; }

        public SPOnlineConnection(ClientContext context, ConnectionTypes connectionType, int minimalHealthScore, int retryCount, int retryWait, PSCredential credential)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            this.Context = context;
            this.ConnectionType = connectionType;
            this.MinimalHealthScore = minimalHealthScore;
            this.RetryCount = retryCount;
            this.RetryWait = retryWait;
            this.PSCredential = credential;
        }

        public enum ConnectionTypes
        {
            OnPrem = 0,
            O365 = 1,
            TenantAdmin = 2
        }
    }
}

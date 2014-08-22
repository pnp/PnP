using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Core;
using System;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Net;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    internal class SPOnlineConnectionHelper
    {
        static SPOnlineConnectionHelper()
        {
        }

        internal static SPOnlineConnection InstantiateSPOnlineConnection(Uri url, PSCredential credentials, PSHost host, bool currentCredentials, bool onPrem, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            CmdLetContext context = new CmdLetContext(url.AbsoluteUri, host);
            context.ApplicationName = Properties.Resources.ApplicationName;
            context.RequestTimeout = requestTimeout;
            if (!currentCredentials)
            {
                if (onPrem)
                {
                    context.Credentials = new System.Net.NetworkCredential(credentials.UserName, credentials.Password);
                }
                else
                {
                    SharePointOnlineCredentials onlineCredentials = new SharePointOnlineCredentials(credentials.UserName, credentials.Password);
                    context.Credentials = (ICredentials)onlineCredentials;
                }
            }
            else
            {
                if (credentials != null)
                {
                    context.Credentials = new System.Net.NetworkCredential(credentials.UserName, credentials.Password);
                }
            }
            var connectionType = SPOnlineConnection.ConnectionTypes.OnPrem;
            if (url.Host.ToLower().EndsWith("sharepoint.com"))
            {
                connectionType = SPOnlineConnection.ConnectionTypes.O365;
            }
            if (skipAdminCheck == false)
            {
                if (SPOAdmin.IsTenantAdminSite(context))
                {
                    connectionType = SPOnlineConnection.ConnectionTypes.TenantAdmin;
                }
            }
            return new SPOnlineConnection(context, connectionType, minimalHealthScore, retryCount, retryWait, credentials, onPrem);
        }


    }
}

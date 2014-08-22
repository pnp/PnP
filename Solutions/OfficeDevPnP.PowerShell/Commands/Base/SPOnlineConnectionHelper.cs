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

        internal static SPOnlineConnection InstantiateSPOnlineConnection(Uri url, PSCredential credentials, PSHost host, bool currentCredentials, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            CmdLetContext context = new CmdLetContext(url.AbsoluteUri, host);
            context.ApplicationName = Properties.Resources.ApplicationName;
            context.RequestTimeout = requestTimeout;
            if (!currentCredentials)
            {
                try
                {
                    SharePointOnlineCredentials onlineCredentials = new SharePointOnlineCredentials(credentials.UserName, credentials.Password);
                    context.Credentials = (ICredentials)onlineCredentials;
                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Microsoft.SharePoint.Client.ClientRequestException)
                    {
                        context.Credentials = new System.Net.NetworkCredential(credentials.UserName, credentials.Password);
                    }
                    catch (Microsoft.SharePoint.Client.ServerException)
                    {
                        context.Credentials = new System.Net.NetworkCredential(credentials.UserName, credentials.Password);
                    }
                }
                catch (ArgumentException)
                {
                    // OnPrem?
                    context.Credentials = new System.Net.NetworkCredential(credentials.UserName, credentials.Password);
                    try
                    {
                        context.ExecuteQuery();
                    }
                    catch (Microsoft.SharePoint.Client.ClientRequestException ex)
                    {
                        throw new Exception("Error establishing a connection", ex);
                    }
                    catch (Microsoft.SharePoint.Client.ServerException ex)
                    {
                        throw new Exception("Error establishing a connection", ex);
                    }
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
            if (url.Host.ToUpperInvariant().EndsWith("SHAREPOINT.COM"))
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
            return new SPOnlineConnection(context, connectionType, minimalHealthScore, retryCount, retryWait, credentials);
        }


    }
}

using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Enums;
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

        internal static SPOnlineConnection InstantiateSPOnlineConnection(Uri url, string realm, string clientId, string clientSecret, PSHost host, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            Core.AuthenticationManager authManager = new Core.AuthenticationManager();
            if (realm == null)
            {
                realm = GetRealmFromTargetUrl(url);
            }

            var context = authManager.GetAppOnlyAuthenticatedContext(url.ToString(), realm, clientId, clientSecret);
            context.ApplicationName = Properties.Resources.ApplicationName;
            context.RequestTimeout = requestTimeout;

            var connectionType = ConnectionType.OnPrem;
            if (url.Host.ToUpperInvariant().EndsWith("SHAREPOINT.COM"))
            {
                connectionType = ConnectionType.O365;
            }
            if (skipAdminCheck == false)
            {
                if (IsTenantAdminSite(context))
                {
                    connectionType = ConnectionType.TenantAdmin;
                }
            }
            return new SPOnlineConnection(context, connectionType, minimalHealthScore, retryCount, retryWait, null, url.ToString());
        }

        internal static SPOnlineConnection InstantiateSPOnlineConnection(Uri url, PSCredential credentials, PSHost host, bool currentCredentials, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            ClientContext context = new ClientContext(url.AbsoluteUri);
            context.ApplicationName = Properties.Resources.ApplicationName;
            context.RequestTimeout = requestTimeout;
            if (!currentCredentials)
            {
                try
                {
                    SharePointOnlineCredentials onlineCredentials = new SharePointOnlineCredentials(credentials.UserName, credentials.Password);
                    context.Credentials = onlineCredentials;
                    try
                    {
                        context.ExecuteQueryRetry();
                    }
                    catch (ClientRequestException)
                    {
                        context.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                    }
                    catch (ServerException)
                    {
                        context.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                    }
                }
                catch (ArgumentException)
                {
                    // OnPrem?
                    context.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                    try
                    {
                        context.ExecuteQueryRetry();
                    }
                    catch (ClientRequestException ex)
                    {
                        throw new Exception("Error establishing a connection", ex);
                    }
                    catch (ServerException ex)
                    {
                        throw new Exception("Error establishing a connection", ex);
                    }
                }

            }
            else
            {
                if (credentials != null)
                {
                    context.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);
                }
            }
            var connectionType = ConnectionType.OnPrem;
            if (url.Host.ToUpperInvariant().EndsWith("SHAREPOINT.COM"))
            {
                connectionType = ConnectionType.O365;
            }
            if (skipAdminCheck == false)
            {
                if (IsTenantAdminSite(context))
                {
                    connectionType = ConnectionType.TenantAdmin;
                }
            }
            return new SPOnlineConnection(context, connectionType, minimalHealthScore, retryCount, retryWait, credentials, url.ToString());
        }

        public static string GetRealmFromTargetUrl(Uri targetApplicationUri)
        {
            WebRequest request = WebRequest.Create(targetApplicationUri + "/_vti_bin/client.svc");
            request.Headers.Add("Authorization: Bearer ");

            try
            {
                using (request.GetResponse())
                {
                }
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    return null;
                }

                string bearerResponseHeader = e.Response.Headers["WWW-Authenticate"];
                if (string.IsNullOrEmpty(bearerResponseHeader))
                {
                    return null;
                }

                const string bearer = "Bearer realm=\"";
                int bearerIndex = bearerResponseHeader.IndexOf(bearer, StringComparison.Ordinal);
                if (bearerIndex < 0)
                {
                    return null;
                }

                int realmIndex = bearerIndex + bearer.Length;

                if (bearerResponseHeader.Length >= realmIndex + 36)
                {
                    string targetRealm = bearerResponseHeader.Substring(realmIndex, 36);

                    Guid realmGuid;

                    if (Guid.TryParse(targetRealm, out realmGuid))
                    {
                        return targetRealm;
                    }
                }
            }
            return null;
        }

        private static bool IsTenantAdminSite(ClientContext clientContext)
        {
            try
            {
                var tenant = new Tenant(clientContext);
                clientContext.ExecuteQueryRetry();
                return true;
            }
            catch (ClientRequestException)
            {
                return false;
            }
            catch (ServerException)
            {
                return false;
            }
        }

    }
}

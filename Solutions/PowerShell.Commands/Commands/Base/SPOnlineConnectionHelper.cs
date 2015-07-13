using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Enums;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Net;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    internal class SPOnlineConnectionHelper
    {
        private static readonly string AdalClientId = "e6a41c50-27bf-4ac1-b379-7992722a1a87";
        private static readonly Uri AdalReturnUri = new Uri("http://pnp");
        private const string CommonAuthority = "https://login.windows.net/Common";
        public static AuthenticationContext AuthContext { get; set; }
        private static string ContextUrl { get; set; }

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

        internal static SPOnlineConnection InstantiateAdalConnection(Uri url, string clientID, PSHost host, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            var context = new ClientContext(url);
            ContextUrl = url.ToString();
            context.ExecutingWebRequest += ctx_ExecutingWebRequest;
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

        private static string GetSharePointHost(string url)
        {
            Uri theHost = new Uri(url);
            return theHost.Scheme + "://" + theHost.Host + "/";
        }

        async static void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            AuthenticationResult ar = await AcquireTokenAsync(CommonAuthority, GetSharePointHost(ContextUrl));

            if (ar != null)
            {
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + ar.AccessToken;
            }
        }

        private static async Task<AuthenticationResult> AcquireTokenAsync(string authContextUrl, string resourceId)
        {
            AuthenticationResult ar = null;
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFile = Path.Combine(appDataFolder, "OfficeDevPnP.PowerShell\\tokencache.dat");
            FileTokenCache cache = new FileTokenCache(configFile);
            try
            {

                //create a new authentication context for our app
                AuthContext = new AuthenticationContext(authContextUrl, cache);

                //look to see if we have an authentication context in cache already
                //we would have gotten this when we authenticated previously
                if (AuthContext.TokenCache.ReadItems().Any())
                {

                    //re-bind AuthenticationContext to the authority source of the cached token.
                    //this is needed for the cache to work when asking for a token from that authority.
                    string cachedAuthority =
                        AuthContext.TokenCache.ReadItems().First().Authority;

                    AuthContext = new AuthenticationContext(cachedAuthority, cache);
                }

                //try to get the AccessToken silently using the resourceId that was passed in
                //and the client ID of the application.
                ar = (await AuthContext.AcquireTokenSilentAsync(resourceId, AdalClientId));
            }
            catch (Exception)
            {
                //not in cache; we'll get it with the full oauth flow
            }

            if (ar == null)
            {
                try
                {
                    ar = AuthContext.AcquireToken(resourceId, AdalClientId, AdalReturnUri, PromptBehavior.Auto);

                }
                catch (Exception acquireEx)
                {
                    //utter failure here, we need let the user know we just can't do it
                   throw new Exception("Error trying to acquire authentication result: " + acquireEx.Message);
                }
            }

            return ar;
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

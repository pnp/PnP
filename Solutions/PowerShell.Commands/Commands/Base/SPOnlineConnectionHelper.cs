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
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    internal class SPOnlineConnectionHelper
    {
        private const string CommonAuthority = "https://login.windows.net/Common";
        public static AuthenticationContext AuthContext { get; set; }
        private static string ContextUrl { get; set; }

        static SPOnlineConnectionHelper()
        {
        }

        internal static Uri RedirectUri;
        internal static string ClientId;

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

#if !CLIENTSDKV15
        internal static SPOnlineConnection InitiateAzureADNativeApplicationConnection(Uri url, string clientId, Uri redirectUri, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            Core.AuthenticationManager authManager = new Core.AuthenticationManager();


            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFile = Path.Combine(appDataFolder, "OfficeDevPnP.PowerShell\\tokencache.dat");
            FileTokenCache cache = new FileTokenCache(configFile);

            var context = authManager.GetAzureADNativeApplicationAuthenticatedContext(url.ToString(), clientId, redirectUri, cache);

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

        internal static SPOnlineConnection InitiateAzureADAppOnlyConnection(Uri url, string clientId, string tenant, string certificatePath, SecureString certificatePassword, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            Core.AuthenticationManager authManager = new Core.AuthenticationManager();
            var context = authManager.GetAzureADAppOnlyAuthenticatedContext(url.ToString(), clientId, tenant, certificatePath, certificatePassword);

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
#endif

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

        internal static SPOnlineConnection InstantiateAdfsConnection(Uri url, PSCredential credentials, PSHost host, int minimalHealthScore, int retryCount, int retryWait, int requestTimeout, bool skipAdminCheck = false)
        {
            Core.AuthenticationManager authManager = new Core.AuthenticationManager();

            var networkCredentials = credentials.GetNetworkCredential();

            string adfsHost;
            string adfsRelyingParty;
            GetAdfsConfigurationFromTargetUri(url, out adfsHost, out adfsRelyingParty);

            if (string.IsNullOrEmpty(adfsHost) || string.IsNullOrEmpty(adfsRelyingParty))
            {
                throw new Exception("Cannot retrieve ADFS settings.");
            }

            var context = authManager.GetADFSUserNameMixedAuthenticatedContext(url.ToString(), networkCredentials.UserName, networkCredentials.Password, networkCredentials.Domain, adfsHost, adfsRelyingParty);

            context.ApplicationName = Properties.Resources.ApplicationName;
            context.RequestTimeout = requestTimeout;

            var connectionType = ConnectionType.OnPrem;

            if (skipAdminCheck == false)
            {
                if (IsTenantAdminSite(context))
                {
                    connectionType = ConnectionType.TenantAdmin;
                }
            }
            return new SPOnlineConnection(context, connectionType, minimalHealthScore, retryCount, retryWait, null, url.ToString());
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

        public static void GetAdfsConfigurationFromTargetUri(Uri targetApplicationUri, out string adfsHost, out string adfsRelyingParty)
        {
            adfsHost = "";
            adfsRelyingParty = "";

            var trustEndpoint = new Uri(new Uri(targetApplicationUri.GetLeftPart(UriPartial.Authority)), "/_trust/");
            var request = (HttpWebRequest)WebRequest.Create(trustEndpoint);
            request.AllowAutoRedirect = false;

            try
            {
                using (var response = request.GetResponse())
                {
                    var locationHeader = response.Headers["Location"];
                    if (locationHeader != null)
                    {
                        var redirectUri = new Uri(locationHeader);
                        Dictionary<string, string> queryParameters = Regex.Matches(redirectUri.Query, "([^?=&]+)(=([^&]*))?").Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => Uri.UnescapeDataString(x.Groups[3].Value));
                        adfsHost = redirectUri.Host;
                        adfsRelyingParty = queryParameters["wtrealm"];
                    }
                }
            } catch(WebException ex)
            {
                throw new Exception("Endpoint does not use ADFS for authentication.", ex);
            }
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

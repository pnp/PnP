using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.IdentityModel.TokenProviders.ADFS;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core
{
    /// <summary>
    /// This manager class can be used to obtain a SharePointContext object
    /// </summary>
    public class AuthenticationManager
    {
        private const string SHAREPOINT_PRINCIPAL = "00000003-0000-0ff1-ce00-000000000000";

        private SharePointOnlineCredentials sharepointOnlineCredentials;
        private string appOnlyAccessToken;
        private object tokenLock = new object();
        private CookieContainer fedAuth = null;
        private string _contextUrl;
        private TokenCache _tokenCache;
        private const string _commonAuthority = "https://login.windows.net/Common";
        private static AuthenticationContext _authContext = null;
        private string _clientId;
        private Uri _redirectUri;

        /// <summary>
        /// Returns a SharePointOnline ClientContext object 
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="tenantUser">User to be used to instantiate the ClientContext object</param>
        /// <param name="tenantUserPassword">Password of the user used to instantiate the ClientContext object</param>
        /// <returns>ClientContext to be used by CSOM code</returns>
        public ClientContext GetSharePointOnlineAuthenticatedContextTenant(string siteUrl, string tenantUser, string tenantUserPassword)
        {
            var spoPassword = EncryptionUtility.ToSecureString(tenantUserPassword);
            return GetSharePointOnlineAuthenticatedContextTenant(siteUrl, tenantUser, spoPassword);
        }

        /// <summary>
        /// Returns a SharePointOnline ClientContext object 
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="tenantUser">User to be used to instantiate the ClientContext object</param>
        /// <param name="tenantUserPassword">Password (SecureString) of the user used to instantiate the ClientContext object</param>
        /// <returns>ClientContext to be used by CSOM code</returns>
        public ClientContext GetSharePointOnlineAuthenticatedContextTenant(string siteUrl, string tenantUser, SecureString tenantUserPassword)
        {
            Log.Info(Constants.LOGGING_SOURCE, CoreResources.AuthenticationManager_GetContext, siteUrl);
            Log.Debug(Constants.LOGGING_SOURCE, CoreResources.AuthenticationManager_TenantUser, tenantUser);

            if (sharepointOnlineCredentials == null)
            {
                sharepointOnlineCredentials = new SharePointOnlineCredentials(tenantUser, tenantUserPassword);
            }

            var ctx = new ClientContext(siteUrl);
            ctx.Credentials = sharepointOnlineCredentials;

            return ctx;
        }

        /// <summary>
        /// Returns an app only ClientContext object
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="realm">Realm of the environment (tenant) that requests the ClientContext object</param>
        /// <param name="appId">Application ID which is requesting the ClientContext object</param>
        /// <param name="appSecret">Application secret of the Application which is requesting the ClientContext object</param>
        /// <returns>ClientContext to be used by CSOM code</returns>
        public ClientContext GetAppOnlyAuthenticatedContext(string siteUrl, string realm, string appId, string appSecret)
        {
            EnsureToken(siteUrl, realm, appId, appSecret);
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, appOnlyAccessToken);
            return clientContext;
        }

        /// <summary>
        /// Returns a SharePoint on-premises / SharePoint Online Dedicated ClientContext object
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="user">User to be used to instantiate the ClientContext object</param>
        /// <param name="password">Password of the user used to instantiate the ClientContext object</param>
        /// <param name="domain">Domain of the user used to instantiate the ClientContext object</param>
        /// <returns>ClientContext to be used by CSOM code</returns>
        public ClientContext GetNetworkCredentialAuthenticatedContext(string siteUrl, string user, string password, string domain)
        {
            ClientContext clientContext = new ClientContext(siteUrl);
            clientContext.Credentials = new NetworkCredential(user, password, domain);
            return clientContext;
        }


        /// <summary>
        /// Returns a SharePoint ClientContext using Azure Active Directory authentication. This requires that you have a Azure AD Native Application registered. The user will be prompted for authentication.
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="clientId">The Azure AD Native Application Client ID</param>
        /// <param name="redirectUrl">The Azure AD Native Application Redirect Uri as a string</param>
        /// <param name="tokenCache">Optional token cache. If not specified an in-memory token cache will be used</param>
        /// <returns></returns>
        public ClientContext GetAzureADNativeApplicationAuthenticatedContext(string siteUrl, string clientId, string redirectUrl, TokenCache tokenCache = null)
        {
            return GetAzureADNativeApplicationAuthenticatedContext(siteUrl, clientId, new Uri(redirectUrl), tokenCache);
        }

        /// <summary>
        /// Returns a SharePoint ClientContext using Azure Active Directory authentication. This requires that you have a Azure AD Native Application registered. The user will be prompted for authentication.
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="clientId">The Azure AD Native Application Client ID</param>
        /// <param name="redirectUri">The Azure AD Native Application Redirect Uri</param>
        /// <param name="tokenCache">Optional token cache. If not specified an in-memory token cache will be used</param>
        /// <returns></returns>
        public ClientContext GetAzureADNativeApplicationAuthenticatedContext(string siteUrl, string clientId, Uri redirectUri, TokenCache tokenCache = null)
        {
            var clientContext = new ClientContext(siteUrl);
            _contextUrl = siteUrl;
            _tokenCache = tokenCache;
            _clientId = clientId;
            _redirectUri = redirectUri;
            clientContext.ExecutingWebRequest += clientContext_NativeApplicationExecutingWebRequest;

            return clientContext;
        }

        async void clientContext_NativeApplicationExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            var host = new Uri(_contextUrl);
            var ar = await AcquireNativeApplicationTokenAsync(_commonAuthority, host.Scheme + "://" + host.Host + "/");

            if (ar != null)
            {
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + ar.AccessToken;
            }
        }

        private async Task<AuthenticationResult> AcquireNativeApplicationTokenAsync(string authContextUrl, string resourceId)
        {
            AuthenticationResult ar = null;

            try
            {
                if (_tokenCache != null)
                {
                    _authContext = new AuthenticationContext(authContextUrl, _tokenCache);
                }
                else
                {

                    _authContext = new AuthenticationContext(authContextUrl);
                }

                if (_authContext.TokenCache.ReadItems().Any())
                {
                    string cachedAuthority =
                        _authContext.TokenCache.ReadItems().First().Authority;

                    if (_tokenCache != null)
                    {
                        _authContext = new AuthenticationContext(cachedAuthority, _tokenCache);
                    }
                    else
                    {
                        _authContext = new AuthenticationContext(cachedAuthority);
                    }
                }
                ar = (await _authContext.AcquireTokenSilentAsync(resourceId, _clientId));
            }
            catch (Exception)
            {
                //not in cache; we'll get it with the full oauth flow
            }

            if (ar == null)
            {
                try
                {
                    ar = _authContext.AcquireToken(resourceId, _clientId, _redirectUri, PromptBehavior.Always);

                }
                catch (Exception acquireEx)
                {
                    throw new Exception("Error trying to acquire authentication result: " + acquireEx.Message);
                }
            }

            return ar;
        }

        /// <summary>
        /// Returns a SharePoint ClientContext using Azure Active Directory App Only Authentication. This requires that you have a certificated created, and updated the key credentials key in the application manifest in the azure AD accordingly.
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="clientId">The Azure AD Application Client ID</param>
        /// <param name="tenant">The Azure AD Tenant, e.g. mycompany.onmicrosoft.com</param>
        /// <param name="storeName">The name of the store for the certificate</param>
        /// <param name="storeLocation">The location of the store for the certificate</param>
        /// <param name="thumbPrint">The thumbprint of the certificate to locate in the store</param>
        /// <param name="tokenCache">Optional token cache. If not specified an in-memory token cache will be used</param>
        /// <returns></returns>
        public ClientContext GetAzureADAppOnlyAuthenticatedContext(string siteUrl, string clientId, string tenant, StoreName storeName, StoreLocation storeLocation, string thumbPrint)
        {
            var cert = X509CertificateUtility.LoadCertificate(storeName, storeLocation, thumbPrint);

            return GetAzureADAppOnlyAuthenticatedContext(siteUrl, clientId, tenant, cert);
        }

        /// <summary>
        /// Returns a SharePoint ClientContext using Azure Active Directory App Only Authentication. This requires that you have a certificated created, and updated the key credentials key in the application manifest in the azure AD accordingly.
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="clientId">The Azure AD Application Client ID</param>
        /// <param name="tenant">The Azure AD Tenant, e.g. mycompany.onmicrosoft.com</param>
        /// <param name="certificatePath">The path to the certificate (*.pfx) file on the file system</param>
        /// <param name="certificatePassword">Password to the certificate</param>
        /// <param name="tokenCache">Optional token cache. If not specified an in-memory token cache will be used</param>
        /// <returns></returns>
        public ClientContext GetAzureADAppOnlyAuthenticatedContext(string siteUrl, string clientId, string tenant, string certificatePath, string certificatePassword)
        {
            var certPassword = EncryptionUtility.ToSecureString(certificatePassword);

            return GetAzureADAppOnlyAuthenticatedContext(siteUrl, clientId, tenant, certificatePath, certPassword);
        }

        /// <summary>
        /// Returns a SharePoint ClientContext using Azure Active Directory App Only Authentication. This requires that you have a certificated created, and updated the key credentials key in the application manifest in the azure AD accordingly.
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="clientId">The Azure AD Application Client ID</param>
        /// <param name="tenant">The Azure AD Tenant, e.g. mycompany.onmicrosoft.com</param>
        /// <param name="certificatePath">The path to the certificate (*.pfx) file on the file system</param>
        /// <param name="certificatePassword">Password to the certificate</param>
        /// <param name="tokenCache">Optional token cache. If not specified an in-memory token cache will be used</param>
        /// <returns></returns>
        public ClientContext GetAzureADAppOnlyAuthenticatedContext(string siteUrl, string clientId, string tenant, string certificatePath, SecureString certificatePassword)
        {
            var certfile = System.IO.File.OpenRead(certificatePath);
            var certificateBytes = new byte[certfile.Length];
            certfile.Read(certificateBytes, 0, (int)certfile.Length);
            var cert = new X509Certificate2(
                certificateBytes,
                certificatePassword,
                X509KeyStorageFlags.Exportable |
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet);

            return GetAzureADAppOnlyAuthenticatedContext(siteUrl, clientId, tenant, cert);
        }

        /// <summary>
        /// Returns a SharePoint ClientContext using Azure Active Directory App Only Authentication. This requires that you have a certificated created, and updated the key credentials key in the application manifest in the azure AD accordingly.
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="clientId">The Azure AD Application Client ID</param>
        /// <param name="tenant">The Azure AD Tenant, e.g. mycompany.onmicrosoft.com</param>
        /// <param name="certificate"></param>
        /// <param name="tokenCache">Optional token cache. If not specified an in-memory token cache will be used</param>
        /// <returns></returns>
        public ClientContext GetAzureADAppOnlyAuthenticatedContext(string siteUrl, string clientId, string tenant, X509Certificate2 certificate)
        {

            var clientContext = new ClientContext(siteUrl);

            var authority = string.Format(CultureInfo.InvariantCulture, "https://login.windows.net/{0}/", tenant);

            var authContext = new AuthenticationContext(authority);

            var clientAssertionCertificate = new ClientAssertionCertificate(clientId, certificate);

            var host = new Uri(siteUrl);

            clientContext.ExecutingWebRequest += (sender, args) =>
            {
                var ar = authContext.AcquireToken(host.Scheme + "://" + host.Host + "/", clientAssertionCertificate);
                args.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + ar.AccessToken;
            };

            return clientContext;
        }


        /// <summary>
        /// Returns a SharePoint on-premises / SharePoint Online Dedicated ClientContext object
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="user">User to be used to instantiate the ClientContext object</param>
        /// <param name="password">Password (SecureString) of the user used to instantiate the ClientContext object</param>
        /// <param name="domain">Domain of the user used to instantiate the ClientContext object</param>
        /// <returns>ClientContext to be used by CSOM code</returns>
        public ClientContext GetNetworkCredentialAuthenticatedContext(string siteUrl, string user, SecureString password, string domain)
        {
            ClientContext clientContext = new ClientContext(siteUrl);
            clientContext.Credentials = new NetworkCredential(user, password, domain);
            return clientContext;
        }

        /// <summary>
        /// Returns a SharePoint on-premises ClientContext for sites secured via ADFS
        /// </summary>
        /// <param name="siteUrl">Url of the SharePoint site that's secured via ADFS</param>
        /// <param name="user">Name of the user (e.g. administrator) </param>
        /// <param name="password">Password of the user</param>
        /// <param name="domain">Windows domain of the user</param>
        /// <param name="sts">Hostname of the ADFS server (e.g. sts.company.com)</param>
        /// <param name="idpId">Identifier of the ADFS relying party that we're hitting</param>
        /// <param name="logonTokenCacheExpirationWindow">Optioanlly provide the value of the SharePoint STS logonTokenCacheExpirationWindow. Defaults to 10 minutes.</param>
        /// <returns>ClientContext to be used by CSOM code</returns>
        public ClientContext GetADFSUserNameMixedAuthenticatedContext(string siteUrl, string user, string password, string domain, string sts, string idpId, int logonTokenCacheExpirationWindow = 10)
        {

            ClientContext clientContext = new ClientContext(siteUrl);
            clientContext.ExecutingWebRequest += delegate(object oSender, WebRequestEventArgs webRequestEventArgs)
            {
                if (fedAuth != null)
                {
                    Cookie fedAuthCookie = fedAuth.GetCookies(new Uri(siteUrl))["FedAuth"];
                    // If cookie is expired a new fedAuth cookie needs to be requested
                    if (fedAuthCookie == null || fedAuthCookie.Expires < DateTime.Now)
                    {
                        fedAuth = new UsernameMixed().GetFedAuthCookie(siteUrl, String.Format("{0}\\{1}", domain, user), password, new Uri(String.Format("https://{0}/adfs/services/trust/13/usernamemixed", sts)), idpId, logonTokenCacheExpirationWindow);
                    }
                }
                else
                {
                    fedAuth = new UsernameMixed().GetFedAuthCookie(siteUrl, String.Format("{0}\\{1}", domain, user), password, new Uri(String.Format("https://{0}/adfs/services/trust/13/usernamemixed", sts)), idpId, logonTokenCacheExpirationWindow);
                }

                if (fedAuth == null)
                {
                    throw new Exception("No fedAuth cookie acquired");
                }

                webRequestEventArgs.WebRequestExecutor.WebRequest.CookieContainer = fedAuth;
            };

            return clientContext;
        }

        /// <summary>
        /// Refreshes the SharePoint FedAuth cookie 
        /// </summary>
        /// <param name="siteUrl">Url of the SharePoint site that's secured via ADFS</param>
        /// <param name="user">Name of the user (e.g. administrator) </param>
        /// <param name="password">Password of the user</param>
        /// <param name="domain">Windows domain of the user</param>
        /// <param name="sts">Hostname of the ADFS server (e.g. sts.company.com)</param>
        /// <param name="idpId">Identifier of the ADFS relying party that we're hitting</param>
        /// <param name="logonTokenCacheExpirationWindow">Optioanlly provide the value of the SharePoint STS logonTokenCacheExpirationWindow. Defaults to 10 minutes.</param>
        public void RefreshADFSUserNameMixedAuthenticatedContext(string siteUrl, string user, string password, string domain, string sts, string idpId, int logonTokenCacheExpirationWindow = 10)
        {
            fedAuth = new UsernameMixed().GetFedAuthCookie(siteUrl, String.Format("{0}\\{1}", domain, user), password, new Uri(String.Format("https://{0}/adfs/services/trust/13/usernamemixed", sts)), idpId, logonTokenCacheExpirationWindow);
        }

        /// <summary>
        /// Ensure that AppAccessToken is filled with a valid string representation of the OAuth AccessToken. This method will launch handle with token cleanup after the token expires
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="realm">Realm of the environment (tenant) that requests the ClientContext object</param>
        /// <param name="appId">Application ID which is requesting the ClientContext object</param>
        /// <param name="appSecret">Application secret of the Application which is requesting the ClientContext object</param>
        private void EnsureToken(string siteUrl, string realm, string appId, string appSecret)
        {
            if (appOnlyAccessToken == null)
            {
                lock (tokenLock)
                {
                    Log.Debug(Constants.LOGGING_SOURCE, "AuthenticationManager:EnsureToken(siteUrl:{0},realm:{1},appId:{2},appSecret:PRIVATE)", siteUrl, realm, appId);
                    if (appOnlyAccessToken == null)
                    {
                        TokenHelper.Realm = realm;
                        TokenHelper.ServiceNamespace = realm;
                        TokenHelper.ClientId = appId;
                        TokenHelper.ClientSecret = appSecret;
                        var response = TokenHelper.GetAppOnlyAccessToken(SHAREPOINT_PRINCIPAL, new Uri(siteUrl).Authority, realm);
                        string token = response.AccessToken;
                        ThreadPool.QueueUserWorkItem(obj =>
                        {
                            try
                            {
                                Log.Debug(Constants.LOGGING_SOURCE, "Lease expiration date: {0}", response.ExpiresOn);
                                var lease = GetAccessTokenLease(response.ExpiresOn);
                                lease =
                                    TimeSpan.FromSeconds(
                                        Math.Min(lease.TotalSeconds - TimeSpan.FromMinutes(5).TotalSeconds,
                                                 TimeSpan.FromHours(1).TotalSeconds));
                                Thread.Sleep(lease);
                                appOnlyAccessToken = null;
                            }
                            catch (Exception ex)
                            {
                                Log.Warning(Constants.LOGGING_SOURCE, CoreResources.AuthenticationManger_ProblemDeterminingTokenLease, ex);
                                appOnlyAccessToken = null;
                            }
                        });
                        appOnlyAccessToken = token;
                    }
                }
            }
        }

        /// <summary>
        /// Get the access token lease time span.
        /// </summary>
        /// <param name="expiresOn">The ExpiresOn time of the current access token</param>
        /// <returns>Returns a TimeSpan represents the time interval within which the current access token is valid thru.</returns>
        private TimeSpan GetAccessTokenLease(DateTime expiresOn)
        {
            DateTime now = DateTime.UtcNow;
            DateTime expires = expiresOn.Kind == DateTimeKind.Utc ?
                expiresOn : TimeZoneInfo.ConvertTimeToUtc(expiresOn);
            TimeSpan lease = expires - now;
            return lease;
        }
    }
}

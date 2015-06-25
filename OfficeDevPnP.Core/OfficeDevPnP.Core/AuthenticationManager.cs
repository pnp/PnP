using System;
using System.Net;
using System.Security;
using System.Threading;
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
                    if (fedAuthCookie == null || fedAuthCookie.Expires < DateTime.UtcNow)
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
                                var lease = response.ExpiresOn - DateTime.UtcNow;
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
    }
}

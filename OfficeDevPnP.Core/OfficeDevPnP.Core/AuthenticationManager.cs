using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Returns a SharePointOnline ClientContext object 
        /// </summary>
        /// <param name="siteUrl">Site for which the ClientContext object will be instantiated</param>
        /// <param name="tenantUser">User to be used to instantiate the ClientContext object</param>
        /// <param name="tenantUserPassword">Password of the user used to instantiate the ClientContext object</param>
        /// <returns>ClientContext to be used by CSOM code</returns>
        public ClientContext GetSharePointOnlineAuthenticatedContextTenant(string siteUrl, string tenantUser, string tenantUserPassword)
        {
            LoggingUtility.Internal.TraceInformation((int)EventId.AuthenticationContext, "Getting authentication context for '{0}'", siteUrl);
            LoggingUtility.Internal.TraceVerbose("Tenant user '{0}'", tenantUser);

            if (sharepointOnlineCredentials == null)
            {
                var spoPassword = tenantUserPassword.ToSecureString();
                sharepointOnlineCredentials = new SharePointOnlineCredentials(tenantUser, spoPassword);
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
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, this.appOnlyAccessToken);
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
            clientContext.Credentials = new System.Net.NetworkCredential(user, password, domain);
            return clientContext;
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
            if (this.appOnlyAccessToken == null)
            {
                lock (tokenLock)
                {
                    LoggingUtility.LogVerbose(string.Format("AuthenticationManager:EnsureToken(siteUrl:{0},realm:{1},appId:{2},appSecret:PRIVATE)", siteUrl, realm, appId), EventCategory.Authorization);
                    if (this.appOnlyAccessToken == null)
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
                                LoggingUtility.LogVerbose("Lease expiration date: " + response.ExpiresOn, EventCategory.Authorization);
                                var lease = response.ExpiresOn - DateTime.Now;
                                lease =
                                    TimeSpan.FromSeconds(
                                        Math.Min(lease.TotalSeconds - TimeSpan.FromMinutes(5).TotalSeconds,
                                                 TimeSpan.FromHours(1).TotalSeconds));
                                Thread.Sleep(lease);
                                this.appOnlyAccessToken = null;
                            }
                            catch (Exception ex)
                            {
                                LoggingUtility.LogWarning("Could not determine lease for appOnlyAccessToken.", ex, EventCategory.Authorization);
                                this.appOnlyAccessToken = null;
                            }
                        });
                        this.appOnlyAccessToken = token;
                    }
                }
            }
        }
    }
}

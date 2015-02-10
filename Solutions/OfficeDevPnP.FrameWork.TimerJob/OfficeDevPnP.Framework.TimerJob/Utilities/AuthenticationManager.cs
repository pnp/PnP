using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeDevPnP.Framework.TimerJob.Utilities
{
    /// <summary>
    /// Class that helps in creating ClientContext objects
    /// </summary>
    public class AuthenticationManager
    {
        private const string SHAREPOINT_PRINCIPAL = "00000003-0000-0ff1-ce00-000000000000";
        private const string LOGGING_SOURCE = "Core.TimerJobs";

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
            Log.Info(LOGGING_SOURCE, "Getting authentication context for '{0}'", siteUrl);
            Log.Info(LOGGING_SOURCE, "Tenant user '{0}'", tenantUser);

            if (sharepointOnlineCredentials == null)
            {
                var spoPassword = GetSecureString(tenantUserPassword);
                sharepointOnlineCredentials = new SharePointOnlineCredentials(tenantUser, spoPassword);
            }

            var ctx = new ClientContext(siteUrl);
            ctx.Credentials = sharepointOnlineCredentials;

            return ctx;
        }

        private SecureString GetSecureString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input string is empty and cannot be made into a SecureString", "input");
            }

            var secureString = new SecureString();
            foreach (char c in input.ToCharArray())
            {
                secureString.AppendChar(c);
            }

            return secureString;
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
                    Log.Info(LOGGING_SOURCE, "AuthenticationManager:EnsureToken(siteUrl:{0},realm:{1},appId:{2},appSecret:PRIVATE)", siteUrl, realm, appId);
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
                                Log.Info(LOGGING_SOURCE, "Lease expiration date: {0}", response.ExpiresOn);
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
                                Log.Warning(LOGGING_SOURCE, "Could not determine lease for appOnlyAccessToken. Exception: {0}", ex.Message);
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

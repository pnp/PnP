using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Authentication
{
    /// <summary>
    /// Authentication Class used for working with UserProfiles.
    /// The Class should use a Service Account
    /// </summary>
    public class TenantAccountAuthentication : IAuthentication
    {   
        #region Instance Members
        const string LOGGING_SOURCE = "ProfileAuthentication";
        private static readonly IConfigurationFactory _cf = ConfigurationFactory.GetInstance();
        private static readonly IAppSettingsManager _manager = _cf.GetAppSetingsManager();
        private AppSettings _settings;
        private string _tenantAdminAccount;
        private string _tenantAdminAccountPWD;
        private string _tenantAdminUrl;
        private string _mysiteAdminUrl;
        #endregion

        #region Constructor
        public TenantAccountAuthentication()
        {
            _settings = _manager.GetAppSettings();
        }
        #endregion

        /// <summary>
        /// An Account with Tenant Admin Permissions
        /// By Default this will read from the TenantAdminAccount in your config file of your solution.
        /// </summary>
        public string TenantAdminAccount {

            get
            {
                this._tenantAdminAccount = string.IsNullOrEmpty(this._tenantAdminAccount) ? _settings.TenantAdminAccount : this._tenantAdminAccount;
                return this._tenantAdminAccount;
            }
            set
            {
                this._tenantAdminAccount = value;
            }
        }

        /// <summary>
        /// An Account with Tenant Admin Permissions
        /// By Default this will read from the TenantAdminAccountPWD in your config file of your solution.
        /// </summary>
        public string TenantAdminAccountPWD {

            get
            {
                this._tenantAdminAccountPWD = string.IsNullOrEmpty(this._tenantAdminAccountPWD) ? _settings.TenantAdminAccountPwd : this._tenantAdminAccountPWD;
                return this._tenantAdminAccountPWD;
            }
            set
            {
                this._tenantAdminAccountPWD = value;
            }
        }

        /// <summary>
        /// The Tenant Admin Url for your environment
        /// </summary>
        public string TenantAdminUrl
        {
            get
            {
                this._tenantAdminUrl = string.IsNullOrEmpty(this._tenantAdminUrl) ? _settings.TenantAdminUrl : this._tenantAdminUrl;
                return this._tenantAdminUrl;
            }
            set
            {
                this._mysiteAdminUrl = value;
            }
        }

        /// <summary>
        /// My Site Tenant Url
        /// </summary>
        public string MysiteTenantAdminUrl
        {
            get
            {
                this._mysiteAdminUrl = string.IsNullOrEmpty(this._mysiteAdminUrl) ? _settings.MysiteTenantAdminUrl : this._mysiteAdminUrl;
                return this._mysiteAdminUrl;
            }
            set
            {
                this._tenantAdminUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets Credentials for working with the My Site Tenant Url
        /// </summary>
        public ICredentials Credentials
        {
            get;
            set;
        }

        /// <summary>
        /// Returns an Authenticated ClientContext
        /// </summary>
        /// <returns></returns>
        public ClientContext GetAuthenticatedContext()
        {
            EnsureCredentials();
            var ctx = new ClientContext(this.MysiteTenantAdminUrl);
            ctx.Credentials = this.Credentials;
            return ctx;
        }

        /// <summary>
        /// Returns an HttpWebRequest that is authenticated
        /// This member is not implemented
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpWebRequest GetAuthenticatedWebRequest(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensure that the Credentials are set.
        /// By default this will read the SharePointOnPremises section in the application config.
        /// If SharePointOnPremises is True NetworkCredential is used, if SharePointOnPremises is false, this method will use
        /// SharePointOnlineCredentials.
        /// </summary>
        private void EnsureCredentials()
        {
            if (this.Credentials == null)
            {
                ////check if Onprem if so use networkcreds of not use SharePoint Online Creds
                //if (_settings.SharePointOnPremises)
                //{
                //    NetworkCredential _creds = new NetworkCredential(this.TenantAdminAccount, this.TenantAdminAccountPWD);
                //    this.Credentials = _creds;

                //}
                //else
                //{
                //    SecureString passWord = new SecureString();
                //    foreach (char c in this.TenantAdminAccountPWD.ToCharArray()) passWord.AppendChar(c);
                //    SharePointOnlineCredentials _spoCreds = new SharePointOnlineCredentials(this.TenantAdminAccount, passWord);
                //    this.Credentials = _spoCreds;
                //}
            }
        }
      
        /// <summary>
        /// Gets or sets the Site Url
        /// </summary>
        public string SiteUrl 
        {
            get;
            set;
        }
    }
}

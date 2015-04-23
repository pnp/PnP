using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration.Application
{
    /// <summary>
    /// Internal class that is a facade for getting values from the config. 
    /// Used for returning settings for the application.
    /// </summary>
    internal class AppSetttingsManager : IAppSettingsManager
    {
        #region Instance Members
        private static readonly AppSetttingsManager _instance = new AppSetttingsManager();
        const string LOGGING_SOURCE = "AppSetttingsManager"; 

        const string TENANT_ADMIN_URL_KEY = "TenantAdminUrl";
        const string TENANTURL_KEY = "TenantUrl";
        const string SPHOSTURL_KEY = "SPHost";
        const string CLIENTID_KEY = "ClientId";
        const string CLIENTSECRET_KEY = "ClientSecret";
        const string SUPPORTTEAMNOTIFICATION_KEY = "SupportTeamNotificationEmail";
        const string AUTOAPPROVESITES_KEY = "AutoApproveSites";
        const string REPOSITORYMANGERTYPE_KEY = "RepositoryManagerType";
        const string TEMPLATEPROVIDERTYPE_KEY = "TemplateProviderType";
        const string SHAREPOINTONPREM_KEY = "SharePointOnPremises";
        const string TENANTADMINACCOUNT_KEY = "TenantAdminAccount";
        const string TENANTADMINACCOUNTPWD_KEY = "TenantAdminAccountPWD";
        const string MYSITETENANTADMINURL_KEY = "MysiteTenantAdminUrl";
        #endregion
        
        #region Constructors
        /// <summary>
        /// Static constructor to handle beforefieldinit
        /// </summary>
        static AppSetttingsManager()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        AppSetttingsManager()
        {
        }
        #endregion

        #region Static Members
        internal static IAppSettingsManager GetInstance()
        {
            return _instance;
        }
        #endregion

        #region IAppSettingsManager Members
        public AppSettings GetAppSettings()
        {
            var _appConfig = new AppSettings()
            {
                TenantAdminUrl = ConfigurationHelper.Get(TENANT_ADMIN_URL_KEY),
                SPHostUrl = ConfigurationHelper.Get(SPHOSTURL_KEY),
                ClientID = ConfigurationHelper.Get(CLIENTID_KEY),
                ClientSecret = ConfigurationHelper.Get(CLIENTSECRET_KEY),
                SupportEmailNotification = ConfigurationHelper.Get(SUPPORTTEAMNOTIFICATION_KEY),
                RepositoryManager = ConfigurationHelper.Get(REPOSITORYMANGERTYPE_KEY),
                TemplateProvider = ConfigurationHelper.Get(TEMPLATEPROVIDERTYPE_KEY)
              //  MysiteTenantAdminUrl = ConfigurationHelper.Get(MYSITETENANTADMINURL_KEY)
            };

            ////TODO ENCRYPTION
            //_appConfig.TenantAdminAccount = ConfigurationHelper.Get(TENANTADMINACCOUNT_KEY);
            //_appConfig.TenantAdminAccountPwd = ConfigurationHelper.Get(TENANTADMINACCOUNTPWD_KEY);

            // we need to handle the boolean checks
            var _autoApprove = ConfigurationHelper.Get(AUTOAPPROVESITES_KEY);
          //  var _sharePointOnPremises = ConfigurationHelper.Get(SHAREPOINTONPREM_KEY);

            bool _result = false;
            if(Boolean.TryParse(_autoApprove, out _result)) {
                _appConfig.AutoApprove = _result;
            }
         
            return _appConfig;
        }
        #endregion

    }
}

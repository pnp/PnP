using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Provisioning.Common.Utilities;
using Provisioning.Common.Configuration;
using System.Configuration;

namespace Provisioning.Common.Data.SiteRequests
{
    /// <summary>
    /// Factory for working with the Site Request Repository
    /// <example>
    /// ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
    /// </example>
    /// </summary>
    public sealed class SiteRequestFactory : ISiteRequestFactory
    {
        #region Private Instance Members
        private static readonly SiteRequestFactory _instance = new SiteRequestFactory();
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor to handle beforefieldinit
        /// </summary>
        static SiteRequestFactory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        SiteRequestFactory()
        {
        }
        #endregion

        /// <summary>
        /// Returns an <see cref="Provisioning.Common.Data.ISiteRequestFactory"/> interface for working with the SiteRequest Repository
        /// </summary>
        public static ISiteRequestFactory GetInstance()
        {
            return _instance;
        }

        /// <summary
        /// Returns an <see cref="Provisioning.Common.Data.ISiteRequestManager"/> interface for working with the SiteRequest Repository
        /// Custom implementations must implement <see cref="Provisioning.Common.Data.ISiteRequestManager"/>
        /// This member reads from your configuration file and the app setting RepositoryManagerType must be defined. 
        /// <add key="RepositoryManagerType" value="Provisioning.Common.Data.Impl.SPSiteRequestManagerImpl, Provisioning.Common" />
        /// <returns><see cref="Provisioning.Common.Data.ISiteRequestManager"/></returns>
        /// </summary>
        public ISiteRequestManager GetSiteRequestManager()
        {
            var _configManager = new ConfigManager();
            var _module = _configManager.GetModuleByName(ModuleKeys.REPOSITORYMANGER_KEY);
            var _managerTypeString = _module.ModuleType;

            if (string.IsNullOrEmpty(_managerTypeString)) throw new ConfigurationErrorsException(PCResources.Exception_Template_Provider_Missing_Config_Message);
          
            try { 
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (AbstractModule)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                instance.ConnectionString = _module.ConnectionString;
                instance.Container = _module.Container;
                return (ISiteRequestManager)instance;
            }
            catch(Exception _ex)
            {
                throw new DataStoreException("Exception Occured while Creating Instance" ,_ex);
            }
         
        }
    }
}

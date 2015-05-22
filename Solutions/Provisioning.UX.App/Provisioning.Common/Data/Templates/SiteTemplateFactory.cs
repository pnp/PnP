using Provisioning.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SiteTemplateFactory : ISiteTemplateFactory
    {
        #region Private Instance Members
        private static readonly SiteTemplateFactory _instance = new SiteTemplateFactory();
        #endregion
        
        #region Constructors
        /// <summary>
        /// Static constructor to handle beforefieldinit
        /// </summary>
        static SiteTemplateFactory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        SiteTemplateFactory()
        {
        }
        #endregion

        /// <summary>
        /// Returns an <see cref="Provisioning.Common.Configuration.TemplateISiteTemplateManager"/> interface. This member reads from your configuration file and the app setting TemplateProviderType and must be defined. 
        /// Custom implementations must implement <see cref="Provisioning.Common.Configuration.TemplateISiteTemplateManager"/>
        /// <add key="TemplateProviderType"  value="Provisioning.Common.Configuration.Template.Impl.XMLSiteTemplateManager, Provisioning.Common"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException"></exception>
        public ISiteTemplateManager GetManager()
        {
            var _configManager = new ConfigManager();
            var _module = _configManager.GetModuleByName(ModuleKeys.MASTERTEMPLATEPROVIDER_KEY);
            var _managerTypeString = _module.ModuleType;
            if(string.IsNullOrEmpty(_managerTypeString))
            {
                //TODO LOG
                throw new ConfigurationErrorsException(PCResources.Exception_Template_Provider_Missing_Config_Message);
            }
            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (AbstractModule)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                instance.ConnectionString = _module.ConnectionString;
                instance.Container = _module.Container;
                return (ISiteTemplateManager)instance;
            }
            catch (Exception _ex)
            {
                throw;
               // throw new DataStoreException("Exception Occured while Creating Instance", _ex);
            }
        }

        public static ISiteTemplateFactory GetInstance()
        {
            return _instance;
        }

    }
}

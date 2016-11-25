using Provisioning.Common.Configuration;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC = System.Configuration;


namespace Provisioning.Common.Data.Templates
{
    /// <summary>
    /// Factory Class for working with Site Templates
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
        /// <exception cref="Provisioning.Common.Data.FactoryException">Exception that occurs when the factory encounters an exception.</exception>
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
                if (String.IsNullOrEmpty(_module.ConnectionString))
                {
                    instance.ConnectionString = SC.ConfigurationManager.AppSettings.Get(ModuleKeys.MASTERTEMPLATEPROVIDER_KEY + "_connectionString");
                }
                else
                {
                    instance.ConnectionString = _module.ConnectionString;
                }
                instance.Container = _module.Container;
               Log.Info("Provisioning.Common.Data.Templates", PCResources.SiteTemplate_Factory_Created_Instance, _managerTypeString);
                return (ISiteTemplateManager)instance;
            }
            catch (Exception _ex)
            {
                var _message = String.Format(PCResources.SiteTemplate_Factory_Created_Instance_Exception, _managerTypeString);
                Log.Error("Provisioning.Common.Data.Templates", _message) ;
                throw new FactoryException(_message, _ex);
            }
        }

        public static ISiteTemplateFactory GetInstance()
        {
            return _instance;
        }

    }
}

using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Utilities
{
    public class ReflectionHelper
    {
        const string CONNECTIONSTRING_KEY = "ConnectionString";
        const string CONTAINERSTRING_KEY = "Container";

        private ConfigManager _configManager = new ConfigManager();
        /// <summary>
        /// Returns Connectors
        /// </summary>
        /// <returns></returns>
        public FileConnectorBase GetProvisioningConnector(string moduleKey)
        {
            var _module = _configManager.GetModuleByName(moduleKey);
            var _managerTypeString = _module.ModuleType;
  
            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (FileConnectorBase)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                instance.AddParameter(CONNECTIONSTRING_KEY, _module.ConnectionString);
                instance.AddParameter(CONTAINERSTRING_KEY, _module.Container);
                return instance;
            }
            catch (Exception _ex)
            {
                Log.Error("Provisioning.Common.SiteProvisioningManager", PCResources.FileConnectorBase_Exception, _ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <returns></returns>
        public TemplateProviderBase GetTemplateProvider(string moduleKey)
        {
            var _module = _configManager.GetModuleByName(moduleKey);
            var _managerTypeString = _module.ModuleType;

            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var providerInstance = (TemplateProviderBase)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                var connectorInstance = this.GetProvisioningConnector(ModuleKeys.PROVISIONINGCONNECTORS_KEY);
                connectorInstance.AddParameter(CONNECTIONSTRING_KEY, _module.ConnectionString);
                connectorInstance.AddParameter(CONTAINERSTRING_KEY, _module.Container);

                providerInstance.Connector = connectorInstance;
                return providerInstance;
            }
            catch (Exception _ex)
            {
                Log.Error("Provisioning.Common.SiteProvisioningManager", PCResources.FileConnectorBase_Exception, _ex);
                throw;
            }
        }

    }
}

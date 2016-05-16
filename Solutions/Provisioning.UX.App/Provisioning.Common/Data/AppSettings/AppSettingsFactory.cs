using Provisioning.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC = System.Configuration;


namespace Provisioning.Common.Data.AppSettings
{
    public class AppSettingsFactory: IAppSettingsFactory
    {
        #region Private Instance Members
        private static readonly AppSettingsFactory _instance = new AppSettingsFactory();
        #endregion

        #region Constructors
        
        static AppSettingsFactory()
        {
        }


        AppSettingsFactory()
        {
        }
        #endregion

        
        public static IAppSettingsFactory GetInstance()
        {
            return _instance;
        }

        public IAppSettingsManager GetManager()
        {
            var _configManager = new ConfigManager();
            var _module = _configManager.GetModuleByName(ModuleKeys.APPSETTINGSMANAGER_KEY);
            var _managerTypeString = _module.ModuleType;

            if (string.IsNullOrEmpty(_managerTypeString))
            {
                var _message = "AppSettingsManager is missing from the configuration file.  Please update the configuration file.";
                throw new ConfigurationErrorsException(_message);
            }
            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (AbstractModule)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                if (String.IsNullOrEmpty(_module.ConnectionString))
                {
                    instance.ConnectionString =
                        SC.ConfigurationManager.AppSettings.Get(ModuleKeys.APPSETTINGSMANAGER_KEY + "_connectionString");
                }
                else
                {
                    instance.ConnectionString = _module.ConnectionString;
                }

                instance.Container = _module.Container;
                return (IAppSettingsManager)instance;
            }
            catch (Exception _ex)
            {
                throw new DataStoreException("Exception occured while creating instance", _ex);
            }

        }

    }
}

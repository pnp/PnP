using Provisioning.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC = System.Configuration;


namespace Provisioning.Common.Data.Metadata
{
    public sealed class MetadataFactory : IMetadataFactory
    {
        #region Private Instance Members
        private static readonly MetadataFactory _instance = new MetadataFactory();
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor to handle beforefieldinit
        /// </summary>
        static MetadataFactory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        MetadataFactory()
        {
        }
        #endregion

        /// <summary>
        /// Returns an <see cref="Provisioning.Common.Data.Metadata.IMetadataFactory"/> interface for working with the Metadata Repository
        /// </summary>
        public static IMetadataFactory GetInstance()
        {
            return _instance;
        }

        public IMetadataManager GetManager()
        {
            var _configManager = new ConfigManager();
            var _module = _configManager.GetModuleByName(ModuleKeys.METADATAMANGER_KEY);
            var _managerTypeString = _module.ModuleType;

            if (string.IsNullOrEmpty(_managerTypeString))
            {
                var _message = "MetadataManager is missing from the configuration file.  Please update the configuration file.";
                throw new ConfigurationErrorsException(_message);
            }
            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (AbstractModule)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                if(String.IsNullOrEmpty(_module.ConnectionString))
                {
                    instance.ConnectionString = 
                        SC.ConfigurationManager.AppSettings.Get(ModuleKeys.METADATAMANGER_KEY + "_connectionString");
                }
                else
                {
                    instance.ConnectionString = _module.ConnectionString;
                }
                
                instance.Container = _module.Container;
                return (IMetadataManager)instance;
            }
            catch (Exception _ex)
            {
                throw new DataStoreException("Exception occured while creating instance", _ex);
            }
         
        }
    }
}

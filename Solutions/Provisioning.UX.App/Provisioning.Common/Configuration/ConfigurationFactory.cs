using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Configuration.Template.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
{
    /// <summary>
    /// Factory that is responsible for creating instances of IAppSettingsManager
    /// and ITemplateFactory
    /// </summary>
    public sealed class ConfigurationFactory : IConfigurationFactory
    {
        #region Private Instance Members
        private static readonly ConfigurationFactory _instance = new ConfigurationFactory();
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor to handle beforefieldinit
        /// </summary>
        static ConfigurationFactory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        ConfigurationFactory()
        {
        }
        #endregion

        #region IConfigurationFactory Members

        /// <summary>
        /// Returns an Instance of IConfigurationFactory
        /// </summary>
        /// <returns></returns>
        public static IConfigurationFactory GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Returns an instance of IAppSettingsManager that is responsible for reading from the config files.
        /// </summary>
        /// <returns></returns>
        public IAppSettingsManager GetAppSetingsManager()
        {
            return AppSetttingsManager.GetInstance();
        }

        #endregion

        /// <summary>
        /// Returns an Instance of ITemplateFactory that is responsible for working 
        /// Site Templates
        /// </summary>
        /// <returns></returns>
        public ITemplateFactory GetTemplateFactory()
        {
            XMLTemplateManager.Init();
            return XMLTemplateManager.Instance;
        }
    }
}

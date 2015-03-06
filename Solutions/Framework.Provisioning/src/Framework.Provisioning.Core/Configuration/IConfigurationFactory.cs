using Framework.Provisioning.Core.Configuration.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Configuration
{
    /// <summary>
    /// Interface that is used by the factory that is responsible for creating objects for IAppSettingsManager and ITemplateFactory
    /// </summary>
    public interface IConfigurationFactory
    {
        /// <summary>
        /// Gets the Object that is responsible for returning the Settings of the Applications
        /// </summary>
        /// <returns></returns>
        IAppSettingsManager GetAppSetingsManager();
        /// <summary>
        /// Returns an ITemplateFactory for working with Site Templates
        /// </summary>
        /// <returns></returns>
        ITemplateFactory GetTemplateFactory();
    }
}

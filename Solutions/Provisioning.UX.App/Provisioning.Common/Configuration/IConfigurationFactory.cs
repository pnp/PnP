using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Data.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
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
      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Configuration.Application
{
    /// <summary>
    /// Used to return an Instance of AppSettings
    /// </summary>
    public interface IAppSettingsManager
    {
        /// <summary>
        /// Returns an Instance of AppSettings
        /// </summary>
        /// <returns></returns>
        AppSettings GetAppSettings();
    }
}

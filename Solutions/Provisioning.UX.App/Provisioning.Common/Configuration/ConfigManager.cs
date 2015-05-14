using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
{
    public class ConfigManager
    {
        public ModulesSection ModulesConfiguration
        {
            get { return (ModulesSection)ConfigurationManager.GetSection("modulesSection"); }
        }

        public ModuleElementCollection ModulesElements
        {
            get { return this.ModulesConfiguration.Modules; }
        }

        public Module GetModuleByName(string name)
        {
            return this.ModulesElements[name];
        }

        /// <summary>
        /// Get a value in the Applications Configuration File
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAppSettingsKey(string key)
        {
            string _returnValue = string.Empty;
            Regex r = new Regex(@"(?:(?<=\().+?(?=\))|(?<=\[).+?(?=\]))");
            Regex r1 = new Regex(@"\[(.*?)\]");

            try
            {
                //    Log.Debug(LOGGING_SOURCE, PCResources.AppSettings_GetKey, key);
                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    _returnValue = ConfigurationManager.AppSettings.Get(key);
                    Match _outPut = r.Match(_returnValue);
                    if (_outPut.Success)
                    {
                        var _envPath = Environment.GetEnvironmentVariable(_outPut.Value);
                        _returnValue = r1.Replace(_returnValue, _envPath);
                    }
                }
                else
                {
                    Log.Warning("Provisioning.Common.Configuration.GetAppSettingsKey", PCResources.AppSettings_KeyNotFound, key);
                }
                return _returnValue;
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Fatal("Provisioning.Common.Configuration.GetAppSettingsKey", "Error Occured while reading the configuration file. The Error is {0}", ex);
                throw;
            }
        } 
      
    }
}

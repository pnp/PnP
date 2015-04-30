using Provisioning.Common.Utilities;
using Provisioning.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC = System.Configuration;

namespace Provisioning.Common.Configuration
{
    /// <summary>
    /// Helper class to read from the Config files
    /// </summary>
    internal static class ConfigurationHelper
    {
        const string LOGGING_SOURCE = "ConfigurationHelper";

        #region Public Static Members

        /// <summary>
        /// Helper method to return the a value define in the config file.
        /// </summary>
        /// <param name="key">The key of the value to return</param>
        /// <returns></returns>
        public static string Get(string key)
        {
            string _returnValue = string.Empty;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(PCResources.Exception_Message_EmptyString_Arg, "key");

            try
            {
                if(SC.ConfigurationManager.AppSettings.AllKeys.Contains(key))
                { 
                    _returnValue = SC.ConfigurationManager.AppSettings.Get(key);
                }
                else
                {
                    Log.Warning(LOGGING_SOURCE, PCResources.AppSettings_KeyNotFound, key);
                }
                return _returnValue;
            }
            catch(SC.ConfigurationErrorsException ex)
            {
                Log.Fatal("Provisioning.Common.Configuration.Get", "Error Occured while reading the configruation file. The Error is {0}", ex );
                throw;
            }
        }
        #endregion
    }
}

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions
{
    public class PnPAppConfigReaderTask : Task
    {
        [Required]
        public String ConfigurationFile
        {
            get;
            set;
        }

        [Required]
        public String Configuration
        {
            get;
            set;
        }

        [Output]
        public string PnPBuildConfiguration
        {
            get;
            set;
        }

        [Output]
        public string PnPBranch
        {
            get;
            set;
        }

        public override bool Execute()
        {
            try
            {
                Log.LogMessageFromText(String.Format("PnPAppConfigReaderTask: Reading information from {0} for configuration {1}", ConfigurationFile, Configuration), MessageImportance.Normal);
                PnPAppConfigManager appConfigManager = new PnPAppConfigManager(ConfigurationFile);
                PnPBuildConfiguration = appConfigManager.GetConfigurationElement(Configuration, "PnPBuild");
                PnPBranch = appConfigManager.GetConfigurationElement(Configuration, "PnPBranch");
                return true;
            }
            catch(Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

    }
}

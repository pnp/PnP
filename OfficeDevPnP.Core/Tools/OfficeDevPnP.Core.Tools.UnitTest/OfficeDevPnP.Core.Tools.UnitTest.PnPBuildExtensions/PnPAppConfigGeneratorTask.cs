using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions
{
    public class PnPAppConfigGeneratorTask : Task
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

        [Required]
        public String AppConfigFolder
        {
            get;
            set;
        }


        public override bool Execute()
        {
            try
            {
                Log.LogMessageFromText(String.Format("PnPAppConfigGeneratorTask: Reading information from {0} for configuration {1} to generate app.config in {2}", ConfigurationFile, Configuration, AppConfigFolder), MessageImportance.Normal);
                PnPAppConfigManager appConfigManager = new PnPAppConfigManager(ConfigurationFile);
                appConfigManager.GenerateAppConfig(Configuration, AppConfigFolder);
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

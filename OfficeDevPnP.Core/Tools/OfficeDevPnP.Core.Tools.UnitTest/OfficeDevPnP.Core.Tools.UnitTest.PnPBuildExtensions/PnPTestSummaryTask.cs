using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions
{
    public class PnPTestSummaryTask: Task
    {
        [Required]
        public String TestResultsPath
        {
            get;
            set;
        }

        public override bool Execute()
        {
            try
            {
                Log.LogMessageFromText(String.Format("PnPTestSummaryTask: processing information from folder {0}", TestResultsPath), MessageImportance.Normal);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("MDPath", TestResultsPath);

                PnPTestManager testManager = new PnPTestManager(parameters);
                testManager.GenerateMDSummaryReport();

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

    }
}

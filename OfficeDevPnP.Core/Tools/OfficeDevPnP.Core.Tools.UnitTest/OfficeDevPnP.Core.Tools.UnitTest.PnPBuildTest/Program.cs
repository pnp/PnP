using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //PnPAppConfigManager p = new PnPAppConfigManager(@"C:\Users\bjansen\Documents\Visual Studio 2013\Projects\MSBuildTests\PnPBuildExtensions\mastertestconfiguration.xml");
            ////Console.WriteLine(p.GetConfigurationElement("OnPremAppOnly", "PnPbranch"));
            //p.GenerateAppConfig("OnlineCred", @"c:\temp");

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("MDPath", @"c:\temp");
            parameters.Add("PnPConfigurationToTest", "OnlineCred");
            parameters.Add("PnPBranch", "dev");
            parameters.Add("PnPBuildConfiguration", "debug");

            PnPTestManager t = new PnPTestManager(parameters);

            // Stuff some fake test data
            TestCase tc1 = new TestCase("OfficeDevPnP.Core.Utilities.Tests.JsonUtilityTests.DeserializeListTest", new Uri("http://www.bing.com"), @"c:\GitHub\BertPnP\OfficeDevPnP.Core\OfficeDevPnP.Core.Tests\Utilities\JsonUtilityTests.cs");
            TestResult tr1 = new TestResult(tc1);
            tr1.Outcome = TestOutcome.Passed;
            tr1.DisplayName = "DeserializeListTest";
            tr1.Duration = new TimeSpan(0, 0, 0, 0, 245);
            t.AddTestResult(tr1);

            TestCase tc2 = new TestCase("OfficeDevPnP.Core.Utilities.Tests.JsonUtilityTests.DeserializeListIsNotFixedSizeTest", new Uri("http://www.bing.com"), @"c:\GitHub\BertPnP\OfficeDevPnP.Core\OfficeDevPnP.Core.Tests\Utilities\JsonUtilityTests.cs");
            TestResult tr2 = new TestResult(tc2);
            tr2.Outcome = TestOutcome.Failed;
            tr2.DisplayName = "DeserializeListIsNotFixedSizeTest";
            tr2.Duration = new TimeSpan(0, 0, 0, 1, 749);
            tr2.ErrorMessage = "this is the fake error";
            tr2.ErrorStackTrace = "this is the stack trace of the error that happened";
            t.AddTestResult(tr2);

            Stats s = new Stats();

            //t.TestAreDone(s, false, false, null, null, new TimeSpan(0, 1, 22));
            t.GenerateMDSummaryReport();

        }
    }
}

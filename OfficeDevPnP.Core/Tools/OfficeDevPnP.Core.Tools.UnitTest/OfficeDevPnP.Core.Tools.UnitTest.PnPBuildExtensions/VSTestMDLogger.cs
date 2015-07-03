using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions
{
    [ExtensionUri("logger://MDLogger/v1")] /// Uri used to uniquely identify the MD logger. 
    [FriendlyName("MDLogger")] /// Alternate user friendly string to uniquely identify the logger.
    public class VSTestMDLogger : ITestLoggerWithParameters
    {
        private PnPTestManager testManager;

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            Console.WriteLine("In Initialize");
        }

        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            testManager = new PnPTestManager(parameters);

            foreach (var param in parameters)
            {
                Console.WriteLine("Property: {0}   Value:{1}", param.Key, param.Value);                
            }

            events.TestResult += events_TestResult;
            events.TestRunMessage += events_TestRunMessage;
            events.TestRunComplete += events_TestRunComplete;
        }

        void events_TestRunComplete(object sender, TestRunCompleteEventArgs e)
        {
            try
            {
                testManager.TestAreDone(e.TestRunStatistics, e.IsCanceled, e.IsAborted, e.Error, e.AttachmentSets, e.ElapsedTimeInRunningTests);
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.ToString());
            }
        }

        void events_TestRunMessage(object sender, Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestRunMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        void events_TestResult(object sender, Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestResultEventArgs e)
        {
            testManager.AddTestResult(e.Result);
        }
    }
}

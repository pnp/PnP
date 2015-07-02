using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions
{
    public class PnPTestManager
    {
        #region constants
        private static string TestDate = "%testdate%";
        private static string Configuration = "%configuration%";
        private static string TestTime = "%testtime%";
        private static string PnPBranch = "%pnpbranch%";
        private static string VSBuildConfiguration = "%vsbuildconfiguration%";
        private static string NumberOfTests = "%numberoftests%";
        private static string ElapsedTime = "%elapsedtime%";
        private static string PassedTests = "%passedtests%";
        private static string FailedTests = "%failedtests%";
        private static string SkippedTests = "%skippedtests%";
        private static string TestWasCanceled = "%testwascanceled%";
        private static string TestWasAborted = "%testwasaborted%";
        private static string TestError = "%testerror%";
        private static string FailedTestDetails = "%failedtestdetails%";
        private static string SkippedTestDetails = "%skippedtestdetails%";
        private static string PassedTestDetails = "%passedtestdetails%";
        private static string TestSummary = "%testsummary%";
        private static string ParameterMDPath = "MDPath";
        private static string ParameterPnPConfigurationToTest = "PnPConfigurationToTest";
        private static string ParameterPnPBranch = "PnPBranch";
        private static string ParameterPnPBuildConfiguration = "PnPBuildConfiguration";
        #endregion

        #region Private variables
        private Dictionary<string, string> loggerParameters;
        private List<TestResult> testResults;
        private Collection<Microsoft.VisualStudio.TestPlatform.ObjectModel.AttachmentSet> attachmentSets;
        private TimeSpan elapsedTimeInRunningTests;
        private Exception error;
        private bool isAborted;
        private bool isCanceled;
        private ITestRunStatistics testRunStatistics;
        private DateTimeOffset testStart = DateTimeOffset.MinValue;
        private DateTime testDate;
        #endregion

        #region Constructor
        public PnPTestManager(Dictionary<string, string> parameters)
        {
            loggerParameters = parameters;
            testResults = new List<TestResult>();
        }
        #endregion

        #region public methods
        public void AddTestResult(TestResult testResult)
        {
            testResults.Add(testResult);

            // Log the start time of the first test
            if (testStart == DateTimeOffset.MinValue)
            {
                testStart = testResult.StartTime;
                testDate = DateTime.Now;
            }
        }

        public void TestAreDone(ITestRunStatistics stats, bool isCanceled, bool isAborted, Exception error, Collection<Microsoft.VisualStudio.TestPlatform.ObjectModel.AttachmentSet> attachmentSets, TimeSpan elapsedTime)
        {
            this.testRunStatistics = stats;
            this.isCanceled = isCanceled;
            this.isAborted = isAborted;
            this.error = error;
            this.attachmentSets = attachmentSets;
            this.elapsedTimeInRunningTests = elapsedTime;

            // All tests are done, so generate the report
            PnPTestSummary summary = GenerateMDTestReport();

            // Add this test run to the other test runs in our XML database
            UpdateXMLTestResultFile(Path.Combine(GetParameter(PnPTestManager.ParameterMDPath), "PnPTestResultsSummary.xml"), summary);
        }

        public void GenerateMDSummaryReport()
        {
            int maxItemsInReport = 100;
            string testResultFile = Path.Combine(GetParameter(PnPTestManager.ParameterMDPath), "PnPTestResultsSummary.xml");

            XmlDocument xDoc = new XmlDocument();
            if (File.Exists(testResultFile))
            {
                xDoc.Load(testResultFile);

                // Grab the template from the resource
                string mdTemplate = "";
                using (Stream stream = typeof(PnPTestManager).Assembly.GetManifestResourceStream(String.Format("{0}.MDTestSummaryTemplate.md", typeof(PnPTestManager).Namespace)))
                {
                    StreamReader reader = new StreamReader(stream);
                    mdTemplate = reader.ReadToEnd();
                }

                int resultCounter = 0;

                XmlNodeList results = xDoc.DocumentElement.SelectNodes("/PnPUnitTestResults/PnPTestSummary");
                string lineTemplate = " {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7}";

                StringBuilder lines = new StringBuilder();
                foreach (XmlNode result in results)
                {
                    resultCounter++;

                    if (resultCounter <= maxItemsInReport)
                    {
                        lines.AppendLine(String.Format(lineTemplate, String.Format("[{0}]({1})", result.SelectSingleNode("PnPConfigurationToTest").InnerText, Path.GetFileName(result.SelectSingleNode("MDResultFile").InnerText)),
                                                                     result.SelectSingleNode("TestDate").InnerText,
                                                                     result.SelectSingleNode("TestTime").InnerText,
                                                                     result.SelectSingleNode("PnPBranch").InnerText,
                                                                     result.SelectSingleNode("VSBuildConfiguration").InnerText,
                                                                     result.SelectSingleNode("FailedTests").InnerText,
                                                                     result.SelectSingleNode("SkippedTests").InnerText,
                                                                     result.SelectSingleNode("PassedTests").InnerText));
                    }
                    else
                    {
                        // Remove the nodes which are not needed anymore and delete the MD report file
                        File.Delete(result.SelectSingleNode("MDResultFile").InnerText);
                        xDoc.DocumentElement.RemoveChild(result);
                    }
                }

                mdTemplate = mdTemplate.Replace(PnPTestManager.TestSummary, lines.ToString());

                // Persist the MD file
                File.WriteAllText(Path.Combine(GetParameter(PnPTestManager.ParameterMDPath), "readme.md"), mdTemplate);

                // Persist the updated XML again
                xDoc.Save(testResultFile);

            }
            else
            {
                Console.WriteLine("PnP UnitTest XML test result file {0} was not available.", testResultFile);
                return;
            }
        }
        #endregion

        #region Private methods
        private PnPTestSummary GenerateMDTestReport()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            PnPTestSummary summary = new PnPTestSummary();

            // Grab the template from the resource
            string mdTemplate = "";
            using (Stream stream = typeof(PnPTestManager).Assembly.GetManifestResourceStream(String.Format("{0}.MDTestResultTemplate.md", typeof(PnPTestManager).Namespace)))
            {
                StreamReader reader = new StreamReader(stream);
                mdTemplate = reader.ReadToEnd();
            }

            // replace header strings
            mdTemplate = mdTemplate.Replace(PnPTestManager.Configuration, GetParameter(PnPTestManager.ParameterPnPConfigurationToTest));
            mdTemplate = mdTemplate.Replace(PnPTestManager.TestDate, testDate.ToLongDateString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.TestTime, testStart.UtcDateTime.ToShortTimeString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.PnPBranch, GetParameter(PnPTestManager.ParameterPnPBranch));
            mdTemplate = mdTemplate.Replace(PnPTestManager.VSBuildConfiguration, GetParameter(PnPTestManager.ParameterPnPBuildConfiguration));
            mdTemplate = mdTemplate.Replace(PnPTestManager.NumberOfTests, testRunStatistics.ExecutedTests.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.ElapsedTime, elapsedTimeInRunningTests.ToString("h'h 'm'm 's's'"));
            mdTemplate = mdTemplate.Replace(PnPTestManager.TestWasCanceled, isCanceled.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.TestWasAborted, isAborted.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.TestError, null != error ? error.ToString() : "");

            // fill summary class
            summary.PnPConfigurationToTest = GetParameter(PnPTestManager.ParameterPnPConfigurationToTest);
            summary.TestDate = testDate.ToLongDateString();
            summary.TestTime = testStart.UtcDateTime.ToShortTimeString();
            summary.PnPBranch = GetParameter(PnPTestManager.ParameterPnPBranch);
            summary.VSBuildConfiguration = GetParameter(PnPTestManager.ParameterPnPBuildConfiguration);
            summary.NumberOfTests = testRunStatistics.ExecutedTests;
            summary.ElapsedTime = elapsedTimeInRunningTests.ToString("h'h 'm'm 's's'");

            // Replace test result details
            int passedTests = 0;
            int skippedTests = 0;
            int failedTests = 0;
            StringBuilder failed = new StringBuilder();
            StringBuilder passed = new StringBuilder();
            StringBuilder skipped = new StringBuilder();
            foreach (TestResult test in testResults)
            {
                string testCaseName = !string.IsNullOrEmpty(test.DisplayName) ? test.DisplayName : test.TestCase.FullyQualifiedName;
                testCaseName = testCaseName.Replace("OfficeDevPnP.Core.", "");

                if (test.Outcome == TestOutcome.Failed)
                {
                    failedTests++;
                    failed.AppendLine(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", testCaseName, test.Outcome, test.Duration.ToString("h'h 'm'm 's's'"), test.ErrorMessage));
                }
                else if (test.Outcome == TestOutcome.Skipped)
                {
                    skippedTests++;
                    skipped.AppendLine(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", testCaseName, test.Outcome, test.Duration.ToString("h'h 'm'm 's's'"), test.ErrorMessage));
                }
                else if (test.Outcome == TestOutcome.Passed)
                {
                    passedTests++;
                    passed.AppendLine(String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", testCaseName, test.Outcome, test.Duration.ToString("h'h 'm'm 's's'")));
                }
            }
            mdTemplate = mdTemplate.Replace(PnPTestManager.FailedTestDetails, failed.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.SkippedTestDetails, skipped.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.PassedTestDetails, passed.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.FailedTests, failedTests.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.SkippedTests, skippedTests.ToString());
            mdTemplate = mdTemplate.Replace(PnPTestManager.PassedTests, passedTests.ToString());

            // fill summary class
            summary.FailedTests = failedTests;
            summary.SkippedTests = skippedTests;
            summary.PassedTests = passedTests;

            // save the MD file
            string fileName = string.Format("PnPUnitTestResults-{0}-{1}-{2}.md", testDate.ToString("yyyyMMdd"), GetParameter(PnPTestManager.ParameterPnPConfigurationToTest), testStart.Ticks);
            Directory.CreateDirectory(GetParameter(PnPTestManager.ParameterMDPath));
            File.WriteAllText(Path.Combine(GetParameter(PnPTestManager.ParameterMDPath), fileName), mdTemplate);

            summary.MDResultFile = Path.Combine(GetParameter(PnPTestManager.ParameterMDPath), fileName);

            return summary;
        }

        private void UpdateXMLTestResultFile(string testResultFile, PnPTestSummary summary)
        {
            XmlElement rootNode;

            // Grab the root node
            XmlDocument xDoc = new XmlDocument();
            if (File.Exists(testResultFile))
            {                
                xDoc.Load(testResultFile);                
            }
            else
            {
                XmlNode root = xDoc.CreateNode(XmlNodeType.Element, "PnPUnitTestResults", "");
                xDoc.AppendChild(root);
            }
            rootNode = xDoc.DocumentElement;

            // Serialize the PnPTestSummary class
            XmlSerializerNamespaces xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            XmlSerializer serializer = new XmlSerializer(summary.GetType());
            XmlDocument doc = new XmlDocument();
            XPathNavigator nav = doc.CreateNavigator();
            using (XmlWriter writer = nav.AppendChild())
            {
                writer.WriteStartDocument();
                serializer.Serialize(writer, summary, xns);
                writer.WriteEndDocument();
                writer.Flush();
            }

            // Add to our main XML doc
            XmlNode nodeToAdd = xDoc.ImportNode(doc.FirstChild, true);            
            rootNode.PrependChild(nodeToAdd);

            // persist the XML doc to disk
            xDoc.Save(testResultFile);
        }

        private string GetParameter(string parameter)
        {
            if (loggerParameters.ContainsKey(parameter))
            {
                return loggerParameters[parameter];
            }
            else
            {
                throw new ArgumentException(String.Format("Requested parameter {0} is not defined", parameter));
            }
        }
        #endregion
    }
}

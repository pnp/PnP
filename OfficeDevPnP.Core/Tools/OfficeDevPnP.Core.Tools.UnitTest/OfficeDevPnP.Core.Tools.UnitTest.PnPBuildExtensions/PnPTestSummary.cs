using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions
{
    [Serializable]
    public class PnPTestSummary
    {
        public String PnPConfigurationToTest { get; set; }
        public String TestDate { get; set; }
        public String TestTime { get; set; }
        public String PnPBranch { get; set; }
        public String VSBuildConfiguration { get; set; }
        public long NumberOfTests { get; set; }
        public String ElapsedTime { get; set; }
        public int FailedTests { get; set; }
        public int SkippedTests { get; set; }
        public int PassedTests { get; set; }
        public string MDResultFile { get; set; }
    }
}

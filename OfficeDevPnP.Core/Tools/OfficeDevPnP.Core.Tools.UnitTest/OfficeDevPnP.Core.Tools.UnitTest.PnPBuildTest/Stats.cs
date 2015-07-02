using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tools.UnitTest.PnPBuildTest
{
    class Stats : ITestRunStatistics
    {
        public long ExecutedTests
        {
            get { return 234; }
        }

        public long this[Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome testOutcome]
        {
            get { return 0; }
        }
    }
}

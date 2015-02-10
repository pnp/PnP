using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Framework.TimerJob.Utilities;

namespace OfficeDevPnP.Framework.TimerJob.Samples.Jobs
{
    public class SimpleJob: TimerJob
    {
        public SimpleJob() : base("SimpleJob")
        {
            TimerJobRun += SimpleJob_TimerJobRun;
        }

        void SimpleJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            e.webClientContext.Load(e.webClientContext.Web, p => p.Title);
            e.webClientContext.ExecuteQueryRetry();
            Console.WriteLine("Site {0} has title {1}", e.url, e.webClientContext.Web.Title);
        }
    }
}

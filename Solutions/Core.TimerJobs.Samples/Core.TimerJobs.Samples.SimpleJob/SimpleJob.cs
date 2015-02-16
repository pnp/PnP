using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.TimerJobs;

namespace Core.TimerJobs.Samples.SimpleJob
{
    public class SimpleJob: TimerJob
    {
        public SimpleJob() : base("SimpleJob")
        {
            TimerJobRun += SimpleJob_TimerJobRun;
        }

        void SimpleJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
            e.WebClientContext.ExecuteQueryRetry();
            Console.WriteLine("Site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
        }
    }
}

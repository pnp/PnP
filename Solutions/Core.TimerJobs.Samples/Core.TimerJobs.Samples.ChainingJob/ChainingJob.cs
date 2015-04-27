using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Framework.TimerJobs.Utilities;

namespace Core.TimerJobs.Samples.ChainingJob
{
    public class ChainingJob: TimerJob
    {
        public ChainingJob(): base("ChainingJob")
        {
            TimerJobRun += delegate(object sender, TimerJobRunEventArgs e)
            {
                e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
                e.WebClientContext.ExecuteQueryRetry();
                Console.WriteLine("Site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);

                // Chain another job in this job
                NoThreadingJob noThreadingJob = new NoThreadingJob();
                // Threading inside threaded executions is not supported...override the value set in the original job constructor
                noThreadingJob.UseThreading = false;
                // Take over authentication settings from calling job
                noThreadingJob.Clone(this);
                // Add the site Url we're currently processing in this task
                noThreadingJob.AddSite(e.Url);
                // Run...
                noThreadingJob.Run();

            };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Framework.TimerJob.Samples.Jobs
{
    public class ChainingJob: TimerJob
    {
        public ChainingJob(): base("ChainingJob")
        {
            TimerJobRun += delegate(object sender, TimerJobRunEventArgs e)
            {
                e.webClientContext.Load(e.webClientContext.Web, p => p.Title);
                e.webClientContext.ExecuteQueryRetry();
                Console.WriteLine("Site {0} has title {1}", e.url, e.webClientContext.Web.Title);

                // Chain another job in this job
                NoThreadingJob noThreadingJob = new NoThreadingJob();
                // Threading inside threaded executions is not supported
                noThreadingJob.UseThreading = false;
                // Take over authentication settings from calling job
                noThreadingJob.Clone(this);
                // Add the site url we're currently processing in this task
                noThreadingJob.AddSite(e.url);
                // Run...
                noThreadingJob.Run();

            };
        }

    }
}

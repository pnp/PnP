using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.Threading;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Framework.TimerJobs.Utilities;


namespace Core.TimerJobs.Samples.ExpandJobAppOnly
{
    public class ExpandJob: TimerJob
    {

        public ExpandJob() : base("ExpandJob", "2.0") 
        {
            // We want to operate at sub site level, so let's have the timer framework expand all sub sites
            ExpandSubSites = true;
            // Only use 3 threads instead of the default of 5
            MaximumThreads = 3;
            TimerJobRun += ExpandJob_TimerJobRun;
        }

        void ExpandJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            // Read the title from the site being processed
            e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
            e.WebClientContext.ExecuteQueryRetry();

            // Read the title from the root site of the site being processed
            e.SiteClientContext.Load(e.SiteClientContext.Web, p => p.Title);
            e.SiteClientContext.ExecuteQueryRetry();

            Console.WriteLine("Root site of site {0} has title {1}", e.Url, e.SiteClientContext.Web.Title);
            Console.WriteLine("Sub site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
            
            // Show some threading information
            ThreadingDebugInformation();
        }

        private void ThreadingDebugInformation()
        {
            int maxWorkerThreads;
            int maxCompletionPortThreads;
            int availableWorkerThreads;
            int availableCompletionPortThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);
            ThreadPool.GetAvailableThreads(out availableWorkerThreads, out availableCompletionPortThreads);
            Console.WriteLine("Max threads = {0}, threads in use = {1}", maxWorkerThreads, maxWorkerThreads - availableWorkerThreads);
        }


    }
}

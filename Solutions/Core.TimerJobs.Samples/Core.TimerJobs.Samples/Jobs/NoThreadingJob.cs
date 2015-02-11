using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.Threading;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Framework.TimerJobs.Utilities;


namespace Core.TimerJobs.Samples.Jobs
{
    public class NoThreadingJob: TimerJob
    {

        public NoThreadingJob(): base("NoThreadingJob")
        {
            // Default is to use threading, so explicitely set it to false
            UseThreading = false;
            ExpandSubSites = true;
            
            // Inline delegate
            TimerJobRun += delegate(object sender, TimerJobRunEventArgs e)
            {
                e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
                e.WebClientContext.ExecuteQueryRetry();
                ThreadingDebugInformation();
                Console.WriteLine("NoThreadingJob: Site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
            };
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

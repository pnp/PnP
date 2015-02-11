using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.Threading;

namespace OfficeDevPnP.Framework.TimerJob.Samples.Jobs
{
    public class ExpandJob: TimerJob
    {

        public ExpandJob() : base("ExpandJob", "2.0") 
        {
            ExpandSubSites = true;
            MaximumThreads = 3;
            TimerJobRun += ExpandJob_TimerJobRun;
        }

        void ExpandJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
            e.WebClientContext.ExecuteQueryRetry();
            e.SiteClientContext.Load(e.SiteClientContext.Web, p => p.Title);
            e.SiteClientContext.ExecuteQueryRetry();

            Console.WriteLine("Root site of site {0} has title {1}", e.Url, e.SiteClientContext.Web.Title);
            Console.WriteLine("Sub site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
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

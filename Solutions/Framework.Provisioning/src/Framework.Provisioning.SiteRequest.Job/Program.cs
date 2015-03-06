using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.SiteRequest.Job
{
    /// <summary>
    /// Main Program for the RemoteTimer Job Pattern
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main Entry Point for the RemoteTimer Job Pattern. This is used to process approved Site Requests
        /// in the Site Repository.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Log.Info("Framework.Provisioning.SiteRequest.Job.Main", "Job Execution Starting");
            var _siteRequestJob = new SiteRequestJob();
            _siteRequestJob.ProcessSiteRequests();
            Log.Info("Framework.Provisioning.SiteRequest.Job.Main", "Job Execution Completed.");
        }
    }
}

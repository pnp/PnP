using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Framework.TimerJobs.Utilities;


namespace Core.TimerJobs.Samples.Jobs
{
    public class SiteCollectionScopedJob: TimerJob
    {
        public SiteCollectionScopedJob() : base("SiteCollectionScopedJob")
        {
            // ExpandSites *must* be false as we'll deal with that at TimerJobEvent level
            ExpandSubSites = false;
            TimerJobRun += SiteCollectionScopedJob_TimerJobRun;
        }

        void SiteCollectionScopedJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            // Get all the sub sites in the site we're processing
            IEnumerable<string> expandedSites = GetAllSubSites(e.SiteClientContext.Site);

            // Manually iterate over the content
            foreach (string site in expandedSites)
            {
                // Clone the existing ClientContext for the sub web
                using (ClientContext ccWeb = e.SiteClientContext.Clone(site))
                {
                    // Here's the timer job logic, but now a single site collection is handled in a single thread which 
                    // allows for further optimization or prevents race conditions
                    ccWeb.Load(ccWeb.Web, s => s.Title);
                    ccWeb.ExecuteQueryRetry();
                    Console.WriteLine("Here: {0} - {1}", site, ccWeb.Web.Title);
                }
            }
        }
    }
}

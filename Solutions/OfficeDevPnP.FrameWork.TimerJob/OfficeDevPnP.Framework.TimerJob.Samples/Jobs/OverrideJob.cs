using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Framework.TimerJob.Samples.Jobs
{
    public class OverrideJob: TimerJob
    {

        public OverrideJob(): base("OverrideJob")
        {
            TimerJobRun += OverrideJob_TimerJobRun;
            ExpandSubSites = true;
        }

        public override List<string> UpdateAddedSites(List<string> addedSites)
        {
            // Let's assume we're not happy with the provided list of sites, so first clear it
            addedSites.Clear();
            // Manually adding a new wildcard Url, without an added URL the timer job will do...nothing 
            addedSites.Add("https://bertonline.sharepoint.com/sites/d*");

            // Return the 
            return addedSites;
        }

        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            // Use default TimerJob site resolving
            addedSites = base.ResolveAddedSites(addedSites);

            //Delete the first one from the list...simple change. A real life case could be reading the site scope 
            //from a SQL (Azure) DB to prevent the whole site resolving. 
            addedSites.RemoveAt(0);

            // return the updated list of resolved sites...this list will be processed by the timer job
            return addedSites;
        }


        void OverrideJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
            e.WebClientContext.ExecuteQueryRetry();
            Console.WriteLine("Site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
        }

    }
}

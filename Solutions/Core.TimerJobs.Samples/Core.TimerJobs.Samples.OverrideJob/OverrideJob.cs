using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Framework.TimerJobs.Utilities;


namespace Core.TimerJobs.Samples.OverrideJob
{
    public class OverrideJob: TimerJob
    {

        public OverrideJob(): base("OverrideJob")
        {
            TimerJobRun += OverrideJob_TimerJobRun;
            ExpandSubSites = true;
        }

        /// <summary>
        /// This virtual method is executed by the timerjob framework before it starts with site resolving. 
        /// The idea here is add your own sites at this point instead of the one provided by the TimerJob caller
        /// </summary>
        /// <param name="addedSites">Current list of added sites</param>
        /// <returns>New list of added sites</returns>
        public override List<string> UpdateAddedSites(List<string> addedSites)
        {
            // Let's assume we're not happy with the provided list of sites, so first clear it
            addedSites.Clear();
            // Manually adding a new wildcard Url, without an added URL the timer job will do...nothing 
            addedSites.Add("https://bertonline.sharepoint.com/sites/d*");

            // Return the updated list of sites
            return addedSites;
        }

        /// <summary>
        /// This virtual method is used for resolving sites (= going from wildcard to actual list of sites and/or enumerating 
        /// the sub sites). Use this method to either provide your own list of sites and/or sub sites or for manipulating 
        /// the default generated list (e.g. adding or removing some sites)
        /// </summary>
        /// <param name="addedSites">List of sites to resolve</param>
        /// <returns>Resolved set of sites</returns>
        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            // Use default TimerJob base class site resolving
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

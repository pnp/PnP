using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Governance.TimerJobs.Data;
using Microsoft.SharePoint.Client;
using Microsoft.Online.SharePoint.TenantAdministration;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Utilities;

namespace Governance.TimerJobs
{
    /// <summary>
    /// CleanUpJob is responsible of delete the out-dated site information records where the site has been delete manually from SPO side
    /// </summary>
    public class CleanUpJob : DatabaseTimerJob
    {
       public CleanUpJob(GovernanceDbRepository repository, string url)
            : base("CleanUpJob", repository, url)
        {
        }

        /// <summary>
        /// Go thru each site collection records from DB
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="siteList">The site collection record list</param>
       protected override void ResolveSitesFromDb(GovernanceDbContext dbContext, List<string> siteList)
       {
           int maxPage;
           int page = 1;
           do
           {
               var sites = dbContext.GetAllSites(page, PageSize, out maxPage);
               foreach (var site in sites)
               {
                   siteList.Add(site.Url);
               }
           }
           while (page++ < maxPage);
       }

        /// <summary>
        /// Remove database record if the site is not existing in SharePoint
        /// </summary>
        /// <param name="sender">The current time job instance</param>
        /// <param name="e">Time job event arguments</param>
        protected override void TimerJobRunImpl(object sender, TimerJobRunEventArgs e)
        {
            var tenant = new Tenant(e.TenantClientContext);
            bool existed = tenant.SiteExists(e.Url);
            if (existed)
                return;
            Log.Info(TimerJobsResources.CleanUpJob_RemoveSite, e.Url);
            DbRepository.UsingContext(context => {
                var site = context.GetSite(e.Url);
                context.Sites.Remove(site);
            });
        }
    }
}

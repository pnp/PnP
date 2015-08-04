using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Governance.TimerJobs.Data;
using Governance.TimerJobs.Policy;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.TimerJobs;

namespace Governance.TimerJobs
{
    /// <summary>
    /// GovernanceJob is a concrete DatabaseTimerJob. It queries all incompliant site collections from DB by using the NoncompliancePredictor property of all registered site policies and then run governance workflow. 
    /// </summary>
    public class GovernanceJob : DatabaseTimerJob
    {
        public bool SuppressEmail
        {
            get;
            set;
        }

        /// <summary>
        /// The foreign table relationship collections to be included in the in-compliant sites query
        /// </summary>
        private string[] Includes
        {
            get;
            set;
        }

        /// <summary>
        /// SitePolicyManager instance which handles the policy checking works
        /// </summary>
        private SitePolicyManager PolicyManager
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a new instance of GovernanceJob
        /// </summary>
        /// <param name="repository">db repository</param>
        /// <param name="url">tenant url</param>
        public GovernanceJob(GovernanceDbRepository repository, string url)
            : base("GovernanceJob", repository, url)
        {
            Includes = new string[] { "Administrators", "SiteMetadata" };
            PolicyManager = new SitePolicyManager();
        }

        /// <summary>
        /// Query database by union each policy's noncompliance predictor criteria
        /// </summary>
        /// <param name="dbContext">The database context object</param>
        /// <param name="siteList">The site collection record list</param>
        protected override void ResolveSitesFromDb(GovernanceDbContext dbContext, List<string> siteList)
        {
            int maxPage;
            int page = 1;
            do
            {
                var expressions = from policy in PolicyManager.GetAllGovernancePolicy()
                                  select policy.NoncompliancePredictor;
                var sites = dbContext.GetAllSites(
                    page, PageSize, out maxPage, Includes, expressions.ToArray());
                foreach (var site in sites)
                {
                    siteList.Add(site.Url);
                }
            }
            while (page++ < maxPage);
        }

        /// <summary>
        /// Run policy checking and enforcement for the current site colleciton
        /// </summary>
        /// <param name="sender">The current timer job instance</param>
        /// <param name="e">Time job run event arguments</param>
        protected override void TimerJobRunImpl(object sender, TimerJobRunEventArgs e)
        {
            SiteInformation site = null;
            DbRepository.UsingContext(dbContext =>
                {
                    site = dbContext.GetSite(e.Url);
                    if (site == null)
                        return;
                    PolicyManager.Run(e.TenantClientContext, site, SuppressEmail);
                    if (site.ComplianceState.DeleteDate == DateTime.MaxValue)
                        dbContext.Sites.Remove(site);
                    dbContext.SaveChanges();
                });
        }
    }
}

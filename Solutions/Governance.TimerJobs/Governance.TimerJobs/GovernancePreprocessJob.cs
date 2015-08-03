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
    /// GovernancePreprocessJob is designed to support some special site policies (like HbiBroadAccessPolicy) which requires a customizable site scope query (check all HBI webs) as well as a complex DB status updates process.
    /// </summary>
    public class GovernancePreprocessJob : DatabaseTimerJob
    {
        /// <summary>
        /// The complex site policy attached with this GovernancePreprocessJob
        /// </summary>
        private ISitePolicy Policy
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a new instance of GovernancePreprocessJob
        /// </summary>
        /// <param name="repository">The db repository</param>
        /// <param name="url">The tenant URL</param>
        /// <param name="policy">The complex site policy which requires preprocess</param>
        public GovernancePreprocessJob(GovernanceDbRepository repository, string url, ISitePolicy policy)
            : base("GovernancePreprocessJob", repository, url)
        {
            Policy = policy;
        }

        /// <summary>
        /// Query DB for all root web or sub web records that matches the PreprocessPredictor criteria of the the attached site policy
        /// </summary>
        /// <param name="dbContext">The db context object</param>
        /// <param name="webList">The web records list</param>
        protected override void ResolveSitesFromDb(GovernanceDbContext dbContext, List<string> webList)
        {
            int maxPage;
            int page = 1;
            do
            {
                var webs = dbContext.GetAllWebs(
                    page, PageSize, out maxPage, new[] { Policy.PreprocessPredictor });
                foreach (var web in webs)
                {
                    webList.Add(web.Url);
                }
            }
            while (page++ < maxPage);
        }

        /// <summary>
        /// Run preprocess for the current site
        /// </summary>
        /// <param name="sender">The current timer job instance</param>
        /// <param name="e">The timer job run event arguments</param>
        protected override void TimerJobRunImpl(object sender, TimerJobRunEventArgs e)
        {
            DbRepository.UsingContext(dbContext => {
                var web = dbContext.GetWeb(e.Url);
                var site = dbContext.GetSite(web.SiteUrl);
                Policy.Preprocess(site, web, e);
                dbContext.SaveChanges();
            });            
        }
    }
}

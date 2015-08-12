using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs
{
    /// <summary>
    /// DatabaseTimerJob is an abstract class derived from TenantManagementTimerJob, it replaces the default SharePoint site resolving logic by providing a GovernanceDdContext for concrete classes to query the site information records from DB repository.
    /// </summary>
    public abstract class DatabaseTimerJob : TenantManagementTimerJob
    {
        /// <summary>
        /// The page size for database query result retreival
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// The TenantUrl used to ensure mockup site records from other tenant are removed before we go thru each site
        /// </summary>
        protected string TenantUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct a new instance of DatabaseTimerJob
        /// </summary>
        /// <param name="jobName">Time job name</param>
        /// <param name="repository">db repository</param>
        /// <param name="url">tenant url</param>
        /// <param name="pageSize">Page size for database query result retreival, defaults to 500</param>
        public DatabaseTimerJob(string jobName, GovernanceDbRepository repository, string url, int pageSize = 500)
            : base(jobName, repository)
        {
            AddSite(url);
            TenantUrl = url;
            PageSize = pageSize;
        }

        /// <summary>
        /// Call ResolveSitesFromDb method to get sites to be processed from database
        /// </summary>
        /// <param name="addedSites"></param>
        /// <returns></returns>
        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            base.ResolveAddedSites(addedSites);
            var siteList = new List<string>();
            DbRepository.UsingContext(context =>
            {
                ResolveSitesFromDb(context, siteList);
                // Remove site records which is not of the current tenant
                int length = siteList.Count;
                for (int i = 0; i < length; i++)
                {
                    if (siteList[i].StartsWith(TenantUrl))
                        continue;
                    siteList.RemoveAt(i);
                    i--;
                    length--;
                }
            });
            TotalSites = siteList.Count; // Update timer job workload
            return siteList;
        }

        /// <summary>
        /// Query site collection records as the sites to process for the timer job 
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="siteList">The site collection record list</param>
        protected abstract void ResolveSitesFromDb(GovernanceDbContext dbContext, List<string> siteList);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Governance.TimerJobs.Data;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Framework.TimerJobs;

namespace Governance.TimerJobs
{
    /// <summary>
    /// TenantManagementTimerJob represents an abstract timer job which manages all site collections within a tenant with an app only client context object attached to the Tenant Admin URL. 
    /// </summary>
    public abstract class TenantManagementTimerJob : TimerJob
    {
        /// <summary>
        /// Number of site collections processed
        /// </summary>
        protected volatile int CompletedSites;
        
        /// <summary>
        /// Number of all site collection records in SPO Grid Manager
        /// </summary>
        protected int TotalSites
        {
            get;
            set;
        }

        /// <summary>
        /// The database repository
        /// </summary>
        protected GovernanceDbRepository DbRepository
        {
            get;
            set;
        }        

        /// <summary>
        /// Construct a new instance of TenantManagementTimerJob
        /// </summary>
        /// <param name="jobName">Time job name</param>
        /// <param name="dbRepository">The db repository</param>
        public TenantManagementTimerJob(string jobName, GovernanceDbRepository dbRepository)
            : base(jobName)
        {
            TimerJobRun += ManagementTimerJob_TimerJobRun;
            DbRepository = dbRepository;
        }
        
        /// <summary>
        /// Initalize the timer job workload information
        /// </summary>
        /// <param name="addedSites">The added sites</param>
        /// <returns>The sites to be processed</returns>
        public override List<string> ResolveAddedSites(List<string> addedSites)
        {
            var resolvedSites = base.ResolveAddedSites(addedSites);
            TotalSites = resolvedSites.Count();
            CompletedSites = 0;
            return resolvedSites;
        }
        
        /// <summary>
        /// Timer job run event handler to report the current job progress to console
        /// </summary>
        /// <param name="sender">The current timer job instnace</param>
        /// <param name="e">Timer job run event arguments</param>
        protected void ManagementTimerJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            try
            {
                TimerJobRunImpl(sender, e);
            }
            catch (Exception exception)
            {
                Console.WriteLine(TimerJobsResources.TenantJob_SiteError, Name, e.Url);
                Console.WriteLine(exception.Message);
                throw;
            }
            finally
            {
                e.TenantClientContext.Dispose(); // Dispose the tenant client context object
                Console.WriteLine(TimerJobsResources.TenantJob_Progress, ++CompletedSites, TotalSites, Name);
            }
        }

        /// <summary>
        /// The abstract method which should be implemented in overriden classes to 
        /// </summary>
        /// <param name="sender">The current timer job instnace</param>
        /// <param name="e">Timer job run event arguments</param>
        protected abstract void TimerJobRunImpl(object sender, TimerJobRunEventArgs e);
    }
}

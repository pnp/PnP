using System;
using System.Collections.Generic;
using Governance.TimerJobs.Data;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;

namespace Governance.TimerJobs.Policy
{
    public class SitePolicyManager
    {
        protected ISitePolicyProvider PolicyProvider { get; set; }

        public SitePolicyManager()
        {
            PolicyProvider = new DefaultSitePolicyProvider();
        }
        
        public IEnumerable<ISitePolicy> GetAllGovernancePolicy()
        {
            return PolicyProvider.GetAllGovernancePolicy();
        }

        public void Run(ClientContext tenantClientContext, SiteInformation site, bool suppressEmail)
        {
            if (site == null)
                throw new ArgumentNullException("site");
            if (site.IsSkipGovernance)
                return;
            RunGovernancePolicy(site);
            var executor = new GovernanceWorkflowExecutor(tenantClientContext);
            executor.Enforce(site, suppressEmail);
        }

        private void RunGovernancePolicy(SiteInformation site)
        {
            var plan = PolicyProvider.GetGovernancePlan(site);
            site.ComplianceState.LastCheckDate = GovernanceWorkflowHelper.GetCurrentBusinessTime();
            site.ComplianceState.IsCompliant = true;
            var isSiteDeleted = false;
            try
            {
                foreach (var policy in plan.PolicyCollection)
                {
                    if (isSiteDeleted)
                        break;

                    var isLifeCyclePolicy = policy is LifeCyclePolicy;
                    var isCompliant = policy.IsCompliant(site);
                    if (!isCompliant)
                        site.ComplianceState.IsCompliant = false;
                    if (!isCompliant || isLifeCyclePolicy)
                        policy.Process(site);
                    if (site.ComplianceState.DeleteDate == DateTime.MaxValue)
                    {
                        isSiteDeleted = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("sps", "Policy Checking Error {0}", e);
            }
        }
    }
}
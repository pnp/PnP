using System.Collections.Generic;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    public interface ISitePolicyProvider
    {
        bool StampingSites { get; set; }
        bool EnsureCustomization { get; set; }
        bool UpdateExistingCustomAction { get; set; }
        bool IsShowAlert { get; set; }
        //To turn on/turn off the external sharing feature.
        bool IsShowExternalSharing { get; set; }
        GovernancePlan GetGovernancePlan(SiteInformation site);
        IEnumerable<ISitePolicy> GetAllGovernancePolicy();
    }
}
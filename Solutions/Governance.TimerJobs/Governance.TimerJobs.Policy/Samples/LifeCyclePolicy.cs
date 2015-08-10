using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    /// <summary>
    /// LifeCyclePolicy make sure a site collection will be expire at the end of the default life cycle
    /// </summary>
    public class LifeCyclePolicy : SitePolicy, ILifeCycleHelper
    {
        public LifeCyclePolicy()
        {
        }

        private DateTime ExpireDateLimit
        {
            get { return DateTime.UtcNow.AddMonths(1); }
        }

        public override Expression<Func<SiteInformation, bool>> NoncompliancePredictor
        {
            get { return site => site.ComplianceState.ExpireDate <= ExpireDateLimit; }
        }

        public DateTime GetExpiredDate(SiteInformation site)
        {
            var ret = site.ComplianceState.ExpireDate;
            if (!ret.Equals(DateTime.MinValue)) return ret;
            var lease = GetDefaultLifeTimeInMonth(site);
            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();
            ret = site.CreatedDate;
            while (true)
            {
                if (ret > now)
                    break;
                ret = ret.AddMonths(lease);
            }

            return ret;
        }

        public virtual int GetDefaultLifeTimeInMonth(SiteInformation site)
        {
            switch (site.AudienceScope)
            {
                case "Team":
                case "Project":
                    return 6;
                case "Enterprise":
                case "Organization":
                    return 12;
            }
            return 6;
        }

        public override IEnumerable<NoncomplianceType> GetNoncompliances(SiteInformation site)
        {
            if (IsCompliant(site))
                yield break;
            yield return NoncomplianceType.Expiring;
        }

        public override bool IsCompliant(SiteInformation site)
        {
            var expiredDate = GetExpiredDate(site);
            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();
            if ((expiredDate - now).TotalDays <= GovernanceWorkflowHelper.FirstLockNotificationDays)
            {
                return false;
            }

            return true;
        }

        public override void Process(SiteInformation site)
        {
            var state = site.ComplianceState;
            var now = GovernanceWorkflowHelper.GetCurrentBusinessTime();
            var expiredDate = GetExpiredDate(site);
            if (state.LockedDate > expiredDate || (DateTime.MinValue == state.LockedDate && !state.IsCompliant))
                state.LockedDate = expiredDate;            
            if (DateTime.MinValue == state.ExpireDate)
                state.ExpireDate = expiredDate;
        }       
    }
}
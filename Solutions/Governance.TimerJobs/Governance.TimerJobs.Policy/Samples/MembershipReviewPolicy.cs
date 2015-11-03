using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    /// <summary>
    /// Membership review policy make sure a site collection's owner review its external users in a time period
    /// </summary>
    public class MembershipReviewPolicy : SitePolicy
    {
        /// <summary>
        /// The earliest valid last external user membership review date
        /// </summary>
        private DateTime ReviewDateLimit
        {
            get { return DateTime.UtcNow.AddMonths(-1); }
        }

        /// <summary>
        /// All records with a LastMembershipReviewDate earlier than the ReviewDateLimit will be selected from DB repository as in-cmopliant ones
        /// </summary>
        public override Expression<Func<SiteInformation, bool>> NoncompliancePredictor
        {
            get { 
                return site => site.SharingStatus.HasValue && site.SharingStatus != 0 && site.ComplianceState.LastMembershipReviewDate < ReviewDateLimit; 
            }
        }
        
        public override IEnumerable<NoncomplianceType> GetNoncompliances(SiteInformation site)
        {
            if (IsCompliant(site))
                yield break;
            yield return NoncomplianceType.MembershipReviewDelay;
        }

        /// <summary>
        /// All site information entities with a LastMembershipReviewDate equals to or later than the ReviewDateLimit are compliant
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public override bool IsCompliant(SiteInformation site)
        {
            return !site.IsExternalSharingEnabled || site.ComplianceState.LastMembershipReviewDate >= ReviewDateLimit;
        }
    }
}
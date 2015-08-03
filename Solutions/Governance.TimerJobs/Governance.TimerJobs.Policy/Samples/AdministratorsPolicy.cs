using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    /// <summary>
    /// AdministratorsPolicy make sure a site collection shoulds have 2 administrators at least
    /// </summary>
    public class AdministratorsPolicy : SitePolicy
    {
        /// <summary>
        /// All site collection record with less than 2 administrators will be selected from DB repository for going thru governance workflow
        /// </summary>
        public override Expression<Func<SiteInformation, bool>> NoncompliancePredictor
        {
            get { return site => site.Administrators.Count() < 2; }
        }

        public override IEnumerable<NoncomplianceType> GetNoncompliances(SiteInformation site)
        {
            if (IsCompliant(site))
                yield break;
            yield return NoncomplianceType.NoAdditionalSiteAdmin;
        }

        /// <summary>
        /// Site collection with 2 or more administrators will be marked as compliant
        /// </summary>
        /// <param name="site">Site information entity</param>
        /// <returns>Returns true if the site collection is compliant, otherwise returns false.</returns>
        public override bool IsCompliant(SiteInformation site)
        {
            return site.Administrators.Count() >= 2;
        }
    }
}
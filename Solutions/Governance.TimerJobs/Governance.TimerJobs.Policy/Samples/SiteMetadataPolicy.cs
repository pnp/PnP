using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    /// <summary>
    /// Site Metadata Policy mark sure a site is not missing required metadata.
    /// </summary>
    public class SiteMetadataPolicy : SitePolicy
    {
        /// <summary>
        /// All site collection record does not have required metadata will be selected for governance job.
        /// </summary>
        public override Expression<Func<SiteInformation, bool>> NoncompliancePredictor
        {
            get { return site => string.IsNullOrEmpty(site.BusinessImpact) || string.IsNullOrEmpty(site.AudienceScope); }
        }

        public override IEnumerable<NoncomplianceType> GetNoncompliances(SiteInformation site)
        {
            if (IsCompliant(site))
                yield break;
            yield return NoncomplianceType.MissClassification;
        }

        public override bool IsCompliant(SiteInformation site)
        {
            return !String.IsNullOrWhiteSpace(site.BusinessImpact) &&
                !String.IsNullOrWhiteSpace(site.AudienceScope);
        }
    }
}

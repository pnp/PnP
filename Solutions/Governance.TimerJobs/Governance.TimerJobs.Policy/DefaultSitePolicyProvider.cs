using System;
using System.Collections.Generic;
using Governance.TimerJobs.Data;

namespace Governance.TimerJobs.Policy
{
    public class DefaultSitePolicyProvider : ISitePolicyProvider
    {
        private readonly GovernancePlan extSharedTeamsPlan;
        private readonly GovernancePlan extSharingOffTeamsPlan;

        private List<ISitePolicy> AllSitePolicies;

        public DefaultSitePolicyProvider()
        {
            StampingSites = false;
            EnsureCustomization = false;
            UpdateExistingCustomAction = false;

            #region Site Policy

            AllSitePolicies = new List<ISitePolicy>();

            var lifeCyclePolicy = new LifeCyclePolicy()
            {
                Id = new Guid("{C62F3C53-61CA-4566-AA1C-34AA26D61FB9}"),
                Name = "Check Site Lifecycle",
                Description = "Site must follow the decommission/extending process once provisioned."
            };
            AllSitePolicies.Add(lifeCyclePolicy);

            var administratorsPolicy = new AdministratorsPolicy
            {
                Id = new Guid("{A89D5DDA-8EF4-416A-93A2-73ABCEB5076D}"),
                Name = "Check Site Collection Admins Count",
                Description = "AdministratorsPolicy make sure a site collection have 2 administrators at least"
            };
            AllSitePolicies.Add(administratorsPolicy);

            var hbiBroadAccessPolicy = new HbiBroadAccessPolicy
            {
                Id = new Guid("{950CF798-926B-4A1D-A867-D18D6E293C34}"),
                Name = "Check Broad Access Group on HBI Sites",
                Description =
                    "HbiBroadAccessPolicy make sure there is no permission granted to predefined large groups at the site collection of sub sites level"
            };
            AllSitePolicies.Add(hbiBroadAccessPolicy);

            var membershipReviewPolicy = new MembershipReviewPolicy
            {
                Id = new Guid("{1C4B31C9-B6DE-443A-89CD-9482E9ACD554}"),
                Name = "Check External Users",
                Description =
                    "Membership review policy make sure a site collection's owner review its external users in a time period"
            };
            AllSitePolicies.Add(membershipReviewPolicy);

            var siteMetadataPolicy = new SiteMetadataPolicy
            {
                Id = new Guid("{B2FF8BFA-27CD-4AD4-BBBA-C13FBF507C22}"),
                Name = "Check If Missing Metadata",
                Description = "Site Metadata Policy mark sure a site is not missing required metadata"
            };
            AllSitePolicies.Add(siteMetadataPolicy);

            #endregion

            #region Plan

            extSharingOffTeamsPlan = new GovernancePlan
            {
                Id = new Guid("{1C19DA06-A357-4820-9DF9-BEECA39BB752}"),
                Name = "External Sharing Disabled Team Site Governance Plan",
                Description = "Governance Plan for all SharePoint Team Sites which disable External Sharing",
                PolicyCollection = new ISitePolicy[]
                {
                    siteMetadataPolicy,
                    administratorsPolicy,
                    hbiBroadAccessPolicy,
                    lifeCyclePolicy
                }
            };
            extSharedTeamsPlan = new GovernancePlan
            {
                Id = new Guid("{1D4C0AB9-03F4-4BBB-9DBE-0CE15DBFB805}"),
                Name = "Externally Shared Team Site Governance Plan",
                Description = "Governance Plan for all Shared SharePoint Team Sites which enable External Sharing",
                PolicyCollection = new ISitePolicy[]
                {
                    siteMetadataPolicy,
                    administratorsPolicy,
                    hbiBroadAccessPolicy,
                    membershipReviewPolicy,
                    lifeCyclePolicy
                }
            };

            #endregion
        }

        public bool StampingSites { get; set; }
        public bool EnsureCustomization { get; set; }
        public bool UpdateExistingCustomAction { get; set; }
        public bool IsShowAlert { get; set; }
        public bool IsShowExternalSharing { get; set; }

        public virtual GovernancePlan GetGovernancePlan(SiteInformation site)
        {
            // Add GetGovernnance Plan Logic Here
            var plan = site.IsExternalSharingEnabled ? extSharedTeamsPlan : extSharingOffTeamsPlan;
            return plan;
        }

        public IEnumerable<ISitePolicy> GetAllGovernancePolicy()
        {
            return AllSitePolicies;
        }
    }
}
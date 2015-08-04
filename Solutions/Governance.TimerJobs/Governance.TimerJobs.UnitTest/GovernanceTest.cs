using System;
using Governance.TimerJobs.Data;
using Governance.TimerJobs.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Governance.TimerJobs.UnitTest
{
    [TestClass]
    public class GovernanceTest
    {
        /// <summary>
        ///     Admin Count Test => >1 , =1, <1
        /// </summary>
        [TestMethod]
        public void AdministratorsPolicyIsCompliantTest()
        {
            var site = GetMockUpSiteInformation();
            var administratorsPolicy = new AdministratorsPolicy();
            Assert.IsTrue(administratorsPolicy.IsCompliant(site));
            site.Administrators = GetMockUpSiteUsers(1);
            Assert.IsFalse(administratorsPolicy.IsCompliant(site));
            site.Administrators = GetMockUpSiteUsers(0);
            Assert.IsFalse(administratorsPolicy.IsCompliant(site));
        }

        /// <summary>
        ///     HbiBroadAccessPolicy Test => true / false
        /// </summary>
        [TestMethod]
        public void HbiBroadAccessPolicyIsCompliantTest()
        {
            var hbiBroadAccessPolicy = new HbiBroadAccessPolicy();
            var site = GetMockUpSiteInformation();
            Assert.IsTrue(hbiBroadAccessPolicy.IsCompliant(site));
            site.HasBroadAccess = true;
            Assert.IsFalse(hbiBroadAccessPolicy.IsCompliant(site));
        }

        /// <summary>
        ///     SiteMetadataPolicy Test => Audience Scope x Bussiness Impact combination
        /// </summary>
        [TestMethod]
        public void SiteMetadataPolicyIsCompliantTest()
        {
            var siteMetadataPolicy = new SiteMetadataPolicy();
            var site = GetMockUpSiteInformation();
            Assert.IsTrue(siteMetadataPolicy.IsCompliant(site));
            site.AudienceScope = string.Empty;
            site.BusinessImpact = "HBI";
            Assert.IsFalse(siteMetadataPolicy.IsCompliant(site));
            site.AudienceScope = "Enterprise";
            site.BusinessImpact = string.Empty;
            Assert.IsFalse(siteMetadataPolicy.IsCompliant(site));
            site.AudienceScope = string.Empty;
            site.BusinessImpact = string.Empty;
            Assert.IsFalse(siteMetadataPolicy.IsCompliant(site));
        }

        /// <summary>
        ///     MembershipReviewPolicy test => Ext Enable/Disable x LastReviewData >/</= limit
        /// </summary>
        [TestMethod]
        public void MembershipReviewPolicyIsCompliantTest()
        {
            var membershipReviewPolicy = new MembershipReviewPolicy();
            var site = GetMockUpSiteInformation();
            Assert.IsTrue(membershipReviewPolicy.IsCompliant(site));
            site.SharingStatus = 1;
            Assert.IsFalse(membershipReviewPolicy.IsCompliant(site));
            site.ComplianceState.LastMembershipReviewDate = DateTime.UtcNow.AddDays(-10);
            Assert.IsTrue(membershipReviewPolicy.IsCompliant(site));
            site.ComplianceState.LastMembershipReviewDate = DateTime.UtcNow.AddDays(-30);
            Assert.IsTrue(membershipReviewPolicy.IsCompliant(site));
            site.ComplianceState.LastMembershipReviewDate = DateTime.UtcNow.AddDays(-40);
            Assert.IsFalse(membershipReviewPolicy.IsCompliant(site));
        }


        /// <summary>
        /// life cycle policy is compliant test. ExpireDate => -1 / 30 / 31 /29
        /// </summary>
        [TestMethod]
        public void LifeCyclePolicyIsCompliantTest()
        {
            var lifeCyclePolicy = new LifeCyclePolicy();
            var site = GetMockUpSiteInformation();
            Assert.IsFalse(lifeCyclePolicy.IsCompliant(site));
            site.ComplianceState.ExpireDate = DateTime.UtcNow.AddDays(30);
            Assert.IsFalse(lifeCyclePolicy.IsCompliant(site));
            site.ComplianceState.ExpireDate = DateTime.UtcNow.AddDays(31);
            Assert.IsTrue(lifeCyclePolicy.IsCompliant(site));
            site.ComplianceState.ExpireDate = DateTime.UtcNow.AddDays(29);
            Assert.IsFalse(lifeCyclePolicy.IsCompliant(site));
        }

        /// <summary>
        /// life cycle policy process test. => <LockeDate = Min> x <IsCompliant> x <ExpireDate = Min> combination
        /// </summary>
        [TestMethod]
        public void LifeCyclePolicyProcessTest()
        {
            var lifeCyclePolicy = new LifeCyclePolicy();
            var site = GetMockUpSiteInformation();
            lifeCyclePolicy.Process(site);
            Assert.IsTrue(site.ComplianceState.LockedDate == site.ComplianceState.ExpireDate);
            site.ComplianceState.LockedDate = DateTime.MinValue;
            lifeCyclePolicy.Process(site);
            Assert.IsTrue(site.ComplianceState.LockedDate == site.ComplianceState.ExpireDate);
            site.ComplianceState.LockedDate = DateTime.MinValue;
            site.ComplianceState.IsCompliant = true;
            lifeCyclePolicy.Process(site);
            Assert.IsTrue(site.ComplianceState.LockedDate == DateTime.MinValue);
            site.ComplianceState.ExpireDate = DateTime.MinValue;
            site.ComplianceState.LockedDate = DateTime.MinValue;
            lifeCyclePolicy.Process(site);
            Assert.IsTrue(site.ComplianceState.ExpireDate == site.CreatedDate.AddMonths(6));
        }

        private static SiteInformation GetMockUpSiteInformation()
        {
            return new SiteInformation
            {
                Title = "UTTest",
                Name = "UTTest",
                UrlPath = "/teams/",
                UrlDomain = "https://contoso.sharepoint.com",
                BusinessImpact = "MBI",
                Template = "STS#0",
                StorageMaximumLevel = 500,
                StorageWarningLevel = 400,
                UserCodeMaximumLevel = 100f,
                UserCodeWarningLevel = 90f,
                TimeZoneId = 7,
                Lcid = 1033,
                AudienceScope = "Team",
                Administrators = GetMockUpSiteUsers(2),
                ComplianceState = new ComplianceState
                {
                    DeleteDate = DateTime.MinValue,
                    DeleteNotificationSent = false,
                    DeleteNotificationSentDate = DateTime.MinValue,
                    ExpireDate = DateTime.UtcNow.AddDays(-1),
                    FirstLockNotificationSent = false,
                    FirstLockNotificationSentDate = DateTime.MinValue,
                    IsCompliant = false,
                    IsLocked = false,
                    IsReadonly = false,
                    LastCheckDate = DateTime.Now,
                    SecondLockNotificationSent = false,
                    SecondLockNotificationSentDate = DateTime.MinValue,
                    LockedDate = DateTime.UtcNow.AddDays(10)
                },
                CreatedDate = GovernanceWorkflowHelper.GetCurrentBusinessTime(),
                HasBroadAccess = false
            };
        }

        private static SiteUser[] GetMockUpSiteUsers(int count)
        {
            var siteUsers = new SiteUser[count];
            for (var i = 0; i < count; i++)
            {
                siteUsers[i] = new SiteUser
                {
                    Id = i + 1,
                    Email = string.Format("user{0}@contoso.com", i + 1),
                    DisplayName = string.Format("User {0}", i + 1),
                    IsResolved = false,
                    LoginName = string.Format("User {0}", i + 1),
                    ManagerEmail = ""
                };
            }
            return siteUsers;
        }
    }
}
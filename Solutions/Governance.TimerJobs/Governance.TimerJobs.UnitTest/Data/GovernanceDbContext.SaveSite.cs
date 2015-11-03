using System;
using System.Collections.Generic;
using System.Configuration;
using Governance.TimerJobs.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Governance.TimerJobs.UnitTest
{
    [TestClass]
    public class GovernanceDbContext_DropDbAfterModification
    {
        [TestMethod]
        public void AddOrUpdateSiteTest()
        {
            string connetionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            var context = new GovernanceDbContext(connetionString);
            string url = "https://microsoftspoppe.sharepoint.com/teams/TestSite";
            var existed = context.GetSite(url);
            int existedId = existed == null ? 0 : existed.Id;
            var site = new SiteInformation()
            {
                Administrators = new List<SiteUser>() {
                    new SiteUser()
                    {
                        Email = "ericu@microsoftspoppe.onmicrosoft.com",
                        LoginName = "ericu@microsoftspoppe.onmicrosoft.com",
                    }
                },
                AudienceScope = "Enterprise",
                BusinessImpact = "MBI",
                CreatedBy = "ericu@microsoftspoppe.onmicrosoft.com",
                CreatedDate = DateTime.UtcNow,
                ComplianceState = new ComplianceState(),
                Description = "Test Save Site",
                Guid = Guid.NewGuid(),
                Lcid = 1099,
                LastBusinessImpact = "LBI",
                ModifiedBy = "ericu@microsoftspoppe.onmicrosoft.com",
                ModifiedDate = DateTime.UtcNow,
                SharingStatus = 0,
                SiteMetadata = new SiteMetadata[] {
                    new SiteMetadata()
                    {
                        MetadataKey = "TargetedAudience",
                        MetadataValue = "PM",
                    }
                },
                StorageMaximumLevel = 500,
                StorageWarningLevel = 400,
                Template = "STS#0",
                TimeZoneId = 13,
                Title = "Test Save Site",
                Url = url,
            };
            context.SaveSite(site);
            Assert.AreEqual(existedId+1, site.Id);
        }
    }
}
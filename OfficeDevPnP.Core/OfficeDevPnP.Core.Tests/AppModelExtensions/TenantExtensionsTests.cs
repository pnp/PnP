using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.Online.SharePoint.TenantAdministration;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass()]
    public class TenantExtensionsTests
    {
        [TestMethod()]
        public void GetSiteCollectionsTest()
        {
            using (var tenantContext = TestCommon.CreateTenantClientContext())
            {
                var tenant = new Tenant(tenantContext);
                var siteCols = tenant.GetSiteCollections();

                Assert.IsTrue(siteCols.Any());

            }
        }

        [TestMethod()]
        public void GetOneDriveSiteCollections()
        {
            using (var tenantContext = TestCommon.CreateTenantClientContext())
            {
                var tenant = new Tenant(tenantContext);
                var siteCols = tenant.GetOneDriveSiteCollections();

                Assert.IsTrue(siteCols.Any());

            }
        }

        [TestMethod()]
        public void GetUserProfileServiceClient() {
            using (var tenantContext = TestCommon.CreateTenantClientContext()) {
                var tenant = new Tenant(tenantContext);
                var serviceClient = tenant.GetUserProfileServiceClient();
                tenantContext.Load(tenantContext.Web, w => w.CurrentUser);
                tenantContext.ExecuteQuery();

                var profile = serviceClient.GetUserProfileByName(tenantContext.Web.CurrentUser.LoginName);

                Assert.IsNotNull(profile);
            }
        }

        [TestMethod()]
        public void CheckIfSiteExistsTest() {
            using (var tenantContext = TestCommon.CreateTenantClientContext()) {
                var tenant = new Tenant(tenantContext);
                var siteCollections = tenant.GetSiteCollections();

                var site = siteCollections.First();
                var siteExists1 = tenant.CheckIfSiteExists(site.Url, "Active");
                Assert.IsTrue(siteExists1);

                try {
                    var siteExists2 = tenant.CheckIfSiteExists(site.Url + "sites/aaabbbccc", "Active");
                    Assert.IsFalse(siteExists2, "Invalid site returned as valid.");
                }
                catch (ServerException) { }
            }
        }

        [TestMethod()]
        public void SiteExistsTest()
        {
            using (var tenantContext = TestCommon.CreateTenantClientContext())
            {
                var tenant = new Tenant(tenantContext);
                var siteCollections = tenant.GetSiteCollections();

                var site = siteCollections.First();
                var siteExists1 = tenant.SiteExists(site.Url);
                Assert.IsTrue(siteExists1);

                var siteExists2 = tenant.SiteExists(site.Url + "sites/aaabbbccc");
                Assert.IsFalse(siteExists2, "Invalid site returned as valid.");
            }
        }

        // This works, but is not the best approach to test locking and unlocking sites
        //[TestMethod()]
        //public void SetSiteLockStateTest() {
        //    using (var tenantContext = TestCommon.CreateTenantClientContext()) {
        //        var tenant = new Tenant(tenantContext);
        //        var siteUrl = string.Empty;

        //        var siteCollections = tenant.GetSiteCollections();
        //        siteUrl = siteCollections.Last(s => {
        //            var path = new Uri(s.Url).AbsolutePath;
        //            return path.Length > 1;
        //        }).Url;
        //        tenant.SetSiteLockState(siteUrl, SiteLockState.NoAccess);

        //        var siteProperties = tenant.GetSitePropertiesByUrl(siteUrl, true);
        //        tenantContext.Load(siteProperties);
        //        tenantContext.ExecuteQuery();

        //        Assert.IsTrue(siteProperties.LockState == SiteLockState.NoAccess.ToString());
        //        // delay starting the Unlock test
        //        System.Threading.Thread.Sleep(TimeSpan.FromMinutes(1.5));
        //    }
        //}
    }
}

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

    }
}

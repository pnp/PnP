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
    }
}

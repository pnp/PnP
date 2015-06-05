using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
#if CLIENTSDKV15
    [TestClass()]
    public class Tenant15ExtensionsTests
    {
        private string sitecollectionName = "TestPnPSC_123456789";

        #region Test initialize and cleanup
        [TestInitialize()]
        public void Initialize()
        {
            using (var tenantContext = TestCommon.CreateTenantClientContext())
            {
                //Ensure nothing was left behind before we run our tests
                CleanupCreatedTestSiteCollections(tenantContext);
            }
        }

        [TestCleanup()]
        public void CleanUp()
        {
            using (var tenantContext = TestCommon.CreateTenantClientContext())
            {
                //Cleanup after test run
                CleanupCreatedTestSiteCollections(tenantContext);
            }
        }
        #endregion

        #region Site collection creation and deletion
        [TestMethod]
        public void CreateDeleteSiteCollectionTest()
        {
            using (var tenantContext = TestCommon.CreateTenantClientContext())
            {
                var tenant = new Tenant(tenantContext);

                //Create site collection test
                string siteToCreateUrl = CreateTestSiteCollection(tenant, sitecollectionName);
                var siteExists = tenant.Context.WebExistsFullUrl(siteToCreateUrl);
                Assert.IsTrue(siteExists, "Site collection creation failed");

                //Delete site collection test
                tenant.DeleteSiteCollection(siteToCreateUrl);
                siteExists = tenant.Context.WebExistsFullUrl(siteToCreateUrl);
                Assert.IsFalse(siteExists, "Site collection deletion failed");
            }
        }

        #endregion

        #region Helper methods
        private string GetTestSiteCollectionName(string devSiteUrl, string siteCollection)
        {
            Uri u = new Uri(devSiteUrl);
            string host = String.Format("{0}://{1}:{2}", u.Scheme, u.DnsSafeHost, u.Port);

            string path = u.AbsolutePath;
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            path = path.Substring(0, path.LastIndexOf('/'));

            return string.Format("{0}{1}/{2}", host, path, siteCollection);
        }

        private void CleanupCreatedTestSiteCollections(ClientContext tenantContext)
        {
            string devSiteUrl = ConfigurationManager.AppSettings["SPODevSiteUrl"];
            String testSiteCollection = GetTestSiteCollectionName(devSiteUrl, sitecollectionName);

            //Ensure the test site collection was deleted and removed from recyclebin
            var tenant = new Tenant(tenantContext);
            try
            {
                tenant.DeleteSiteCollection(testSiteCollection);
            }
            catch
            { }
        }

        private string CreateTestSiteCollection(Tenant tenant, string sitecollectionName)
        {
            string devSiteUrl = ConfigurationManager.AppSettings["SPODevSiteUrl"];

            string siteOwnerLogin = string.Format("{0}\\{1}", ConfigurationManager.AppSettings["OnPremDomain"], ConfigurationManager.AppSettings["OnPremUserName"]);
            if (TestCommon.AppOnlyTesting())
            {
                using (var clientContext = TestCommon.CreateClientContext())
                {
                    List<UserEntity> admins = clientContext.Web.GetAdministrators();
                    siteOwnerLogin = admins[0].LoginName.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }

            string siteToCreateUrl = GetTestSiteCollectionName(devSiteUrl, sitecollectionName);
            SiteEntity siteToCreate = new SiteEntity()
            {
                Url = siteToCreateUrl,
                Template = "STS#0",
                Title = "Test",
                Description = "Test site collection",
                SiteOwnerLogin = siteOwnerLogin,
            };

            tenant.CreateSiteCollection(siteToCreate);
            return siteToCreateUrl;
        }
        #endregion

    }
#endif
}

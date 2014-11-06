using System;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class BrandingExtensionsTest
    {
        private static string htmlPublishingPageWithoutExtension= "TestHtmlPublishingPageLayout";
        private static string publishingPageWithoutExtension = "TestPublishingPageLayout";
        private string htmlPublishingPagePath = string.Format("../../Resources/{0}.html", htmlPublishingPageWithoutExtension);
        private string publishingPagePath = string.Format("../../Resources/{0}.aspx", publishingPageWithoutExtension);
        private string pageLayoutTitle = "CustomHtmlPageLayout";

        private string welcomePageContentTypeId =
            "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF390064DEA0F50FC8C147B0B6EA0636C4A7D4";

        private Guid publishingSiteFeatureId = new Guid("f6924d36-2fa8-4f0b-b16d-06b7250180fa");
        private Guid publishingWebFeatureId = new Guid("94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb");

        bool deactivateSiteFeatureOnTeardown = false;
        bool deactivateWebFeatureOnTeardown = false;
        public Web Setup()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                ExceptionHandlingScope scope = new ExceptionHandlingScope(ctx);

                Web web;
                Site site;
                site = ctx.Site;
                web = ctx.Site.RootWeb;
                if (!site.IsFeatureActive(publishingSiteFeatureId))
                {
                    site.ActivateFeature(publishingSiteFeatureId);
                    deactivateSiteFeatureOnTeardown = true;
                }
                if (!web.IsFeatureActive(publishingWebFeatureId))
                {
                    site.RootWeb.ActivateFeature(publishingWebFeatureId);
                    deactivateWebFeatureOnTeardown = true;
                }
                return web;
                
            }
        }

        public void Teardown()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                if (deactivateSiteFeatureOnTeardown)
                {
                    ctx.Site.DeactivateFeature(publishingSiteFeatureId);
                }
                if (deactivateWebFeatureOnTeardown)
                {
                    ctx.Web.DeactivateFeature(publishingWebFeatureId);
                }
            }
        }

        [TestMethod]
        public void CanUploadHtmlPageLayoutAndConvertItToAspxVersion()
        {
            var web = Setup();
            web.Context.Load(web);
            web.DeployHtmlPageLayout(htmlPublishingPagePath, pageLayoutTitle, "", welcomePageContentTypeId);
            web.Context.Load(web, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();
            var item = web.GetPageLayoutListItemByName(htmlPublishingPageWithoutExtension);
            Assert.AreNotEqual(null,item);
            Teardown();
        }

        [TestMethod]
        public void CanUploadPageLayout()
        {
            var web = Setup();
            web.Context.Load(web);
            web.DeployPageLayout(publishingPagePath, pageLayoutTitle, "", welcomePageContentTypeId);
            web.Context.Load(web, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();
            var item = web.GetPageLayoutListItemByName(publishingPageWithoutExtension);
            Assert.AreNotEqual(null, item);
            Teardown();
        }
    }
}
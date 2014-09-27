using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class PageExtensionsTests
    {
        private string folder = "SitePages";
        private string pageName = "Home.aspx";
        private string publishingPageName = "Happy";
        private string publishingPageTemplate = "BlankWebPartPage";
        private Guid publishingSiteFeatureId = new Guid("f6924d36-2fa8-4f0b-b16d-06b7250180fa");
        bool deactivatePublishingOnTearDown = false;
        public Web Setup(string webTemplate = "STS#0", bool enablePublishingInfrastructure = false)
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var name = "WebExtensions";
                ctx.ExecuteQuery();

                ExceptionHandlingScope scope = new ExceptionHandlingScope(ctx);

                Web web;
                Site site;
                site = ctx.Site;
                if (enablePublishingInfrastructure && !site.IsFeatureActive(publishingSiteFeatureId))
                {
                    site.ActivateFeature(publishingSiteFeatureId);
                    deactivatePublishingOnTearDown = true;
                }
                using (scope.StartScope())
                {                    
                    using (scope.StartTry())
                    {
                        web = ctx.Site.OpenWeb(name);
                        web.DeleteObject();
                    }
                    using (scope.StartCatch())
                    {
                        
                        web = ctx.Web.Webs.Add(new WebCreationInformation
                        {
                            Title = name,
                            WebTemplate = webTemplate,
                            Url = name
                        });                        
                    }
                    using (scope.StartFinally())
                    {                        
                        return web;
                    }
                }
            }
        }
        public void Teardown(Web web)
        {
            web.DeleteObject();
            if(deactivatePublishingOnTearDown)
            {
                using (var ctx = TestCommon.CreateClientContext())
                {
                    ctx.Site.DeactivateFeature(publishingSiteFeatureId);
                }
            }
        }
	    [TestMethod]
        public void CanAddLayoutToWikiPage()
        {
            var web = Setup();

            web.AddLayoutToWikiPage(folder, OfficeDevPnP.Core.WikiPageLayout.TwoColumns, pageName);

            Teardown(web);

        }

	[TestMethod]
        public void CanAddHtmlToWikiPage()
        {
            var web = Setup();

            web.AddHtmlToWikiPage(folder, "<h1>I got text</h1>", pageName, 1, 1);

            Teardown(web);

        }
        [TestMethod]
        public void ProveThatWeCanAddHtmlToPageAfterChangingLayout()
        {
            var web = Setup();
            web.AddLayoutToWikiPage(folder, OfficeDevPnP.Core.WikiPageLayout.TwoColumns, pageName);
            web.AddHtmlToWikiPage(folder, "<h1>I got text</h1>", pageName, 1, 1);

            var content = web.GetWikiPageContent(UrlUtility.Combine(UrlUtility.EnsureTrailingSlash(web.ServerRelativeUrl), folder, pageName));

            Assert.IsTrue(content.Contains("<h1>I got text</h1>"));

            Teardown(web);
        }

        [TestMethod]
        public void CanCreatePublishingPage()
        {
            var web = Setup("CMSPUBLISHING#0",true);            
            web.AddPublishingPage(publishingPageName, publishingPageTemplate);
            web.Context.Load(web, w => w.ServerRelativeUrl);
            web.Context.ExecuteQuery();

            var page = web.GetPublishingPage(string.Format(UrlUtility.Combine(UrlUtility.EnsureTrailingSlash(web.ServerRelativeUrl),"Pages", string.Format("{0}.aspx",publishingPageName))));
            web.Context.Load(page.ListItem, i => i["Title"]);
            web.Context.ExecuteQuery();
            
            Assert.AreEqual(page.ListItem["Title"], publishingPageName);

            Teardown(web);
        }
    }
}

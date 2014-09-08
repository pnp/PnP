using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class WebExtensions
    {
        private string folder = "SitePages";
        private string pageName = "Home.aspx";

        public Web Setup()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var name = "WebExtensions";
                ctx.ExecuteQuery();

                ExceptionHandlingScope scope = new ExceptionHandlingScope(ctx);

                Web web;

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
			    WebTemplate = "STS#0",
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
    }
}

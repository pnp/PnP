using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.ObjectHandlers;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class TokenParserTests
    {

        [TestMethod]
        public void ParseTests()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                ctx.Load(ctx.Site, s => s.ServerRelativeUrl);

                var masterCatalog = ctx.Web.GetCatalog((int)ListTemplateType.MasterPageCatalog);
                ctx.Load(masterCatalog, m => m.RootFolder.ServerRelativeUrl);

                var themesCatalog = ctx.Web.GetCatalog((int)ListTemplateType.ThemeCatalog);
                ctx.Load(themesCatalog, t => t.RootFolder.ServerRelativeUrl);


                var parser = new TokenParser(ctx.Web);

                var site1 = parser.Parse("~siTE/test");
                var site2 = parser.Parse("{site}/test");
                var sitecol1 = parser.Parse("~siteCOLLECTION/test");
                var sitecol2 = parser.Parse("{sitecollection}/test");
                var masterUrl1 = parser.Parse("~masterpagecatalog/test");
                var masterUrl2 = parser.Parse("{masterpagecatalog}/test");
                var themeUrl1 = parser.Parse("~themecatalog/test");
                var themeUrl2 = parser.Parse("{themecatalog}/test");

                Assert.IsTrue(site1 == string.Format("{0}/test", ctx.Web.ServerRelativeUrl));
                Assert.IsTrue(site2 == string.Format("{0}/test", ctx.Web.ServerRelativeUrl));
                Assert.IsTrue(sitecol1 == string.Format("{0}/test", ctx.Site.ServerRelativeUrl));
                Assert.IsTrue(sitecol2 == string.Format("{0}/test", ctx.Site.ServerRelativeUrl));
                Assert.IsTrue(masterUrl1 == string.Format("{0}/test", masterCatalog.RootFolder.ServerRelativeUrl));
                Assert.IsTrue(masterUrl2 == string.Format("{0}/test", masterCatalog.RootFolder.ServerRelativeUrl));
                Assert.IsTrue(themeUrl1 == string.Format("{0}/test", themesCatalog.RootFolder.ServerRelativeUrl));
                Assert.IsTrue(themeUrl2 == string.Format("{0}/test", themesCatalog.RootFolder.ServerRelativeUrl));
            }
        }
    }
}

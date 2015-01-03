using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class NavigationExtensionsTests
    {
        #region Add navigation node tests
        [TestMethod]
        public void AddNavigationNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, false);
                
                clientContext.Load(web, w => w.Navigation.TopNavigationBar);
                clientContext.ExecuteQuery();

                Assert.IsTrue(web.Navigation.TopNavigationBar.AreItemsAvailable);

                if (web.Navigation.TopNavigationBar.Any())
                {
                    var navNode = web.Navigation.TopNavigationBar.Where(n => n.Title == "Test Node").FirstOrDefault();
                    Assert.IsNotNull(navNode);
                    navNode.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
        }
        #endregion

        #region Delete navigation node tests
        [TestMethod]
        public void DeleteNavigationNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, false);

                web.DeleteNavigationNode("Test Node", string.Empty, false);

                clientContext.Load(web, w => w.Navigation.TopNavigationBar);
                clientContext.ExecuteQuery();

                if (web.Navigation.TopNavigationBar.Any())
                {
                    var navNode = web.Navigation.TopNavigationBar.Where(n => n.Title == "Test Node").FirstOrDefault();
                    Assert.IsNull(navNode);
                }
            }
        }

        [TestMethod]
        public void DeleteAllQuickLaunchNodesTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                web.DeleteAllQuickLaunchNodes();
                clientContext.Load(web, w => w.Navigation.QuickLaunch);
                clientContext.ExecuteQuery();
                Assert.IsFalse(web.Navigation.QuickLaunch.Any());
            }
        }
        #endregion
    }
}

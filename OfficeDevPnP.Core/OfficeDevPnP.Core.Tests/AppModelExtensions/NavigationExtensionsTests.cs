using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Enums;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class NavigationExtensionsTests
    {
        #region Add navigation node tests
        [TestMethod]
        public void AddTopNavigationNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, NavigationType.TopNavigationBar);

                clientContext.Load(web, w => w.Navigation.TopNavigationBar);
                clientContext.ExecuteQueryRetry();

                Assert.IsTrue(web.Navigation.TopNavigationBar.AreItemsAvailable);

                if (web.Navigation.TopNavigationBar.Any())
                {
                    var navNode = web.Navigation.TopNavigationBar.FirstOrDefault(n => n.Title == "Test Node");
                    Assert.IsNotNull(navNode);
                    navNode.DeleteObject();
                    clientContext.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void AddQuickLaunchNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, NavigationType.QuickLaunch);

                clientContext.Load(web, w => w.Navigation.QuickLaunch);
                clientContext.ExecuteQueryRetry();

                Assert.IsTrue(web.Navigation.QuickLaunch.AreItemsAvailable);

                if (web.Navigation.QuickLaunch.Any())
                {
                    var navNode = web.Navigation.QuickLaunch.FirstOrDefault(n => n.Title == "Test Node");
                    Assert.IsNotNull(navNode);
                    navNode.DeleteObject();
                    clientContext.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void AddSearchNavigationNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, NavigationType.SearchNav);

                NavigationNodeCollection searchNavigation = web.LoadSearchNavigation();

                Assert.IsTrue(searchNavigation.AreItemsAvailable);

                if (searchNavigation.Any())
                {
                    var navNode = searchNavigation.FirstOrDefault(n => n.Title == "Test Node");
                    Assert.IsNotNull(navNode);
                    navNode.DeleteObject();
                    clientContext.ExecuteQueryRetry();
                }
            }
        }
        #endregion

        #region Delete navigation node tests
        [TestMethod]
        public void DeleteTopNavigationNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, NavigationType.TopNavigationBar);

                web.DeleteNavigationNode("Test Node", string.Empty, NavigationType.TopNavigationBar);

                clientContext.Load(web, w => w.Navigation.TopNavigationBar);
                clientContext.ExecuteQueryRetry();

                if (web.Navigation.TopNavigationBar.Any())
                {
                    var navNode = web.Navigation.TopNavigationBar.FirstOrDefault(n => n.Title == "Test Node");
                    Assert.IsNull(navNode);
                }
            }
        }

        [TestMethod]
        public void DeleteQuickLaunchNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, NavigationType.QuickLaunch);

                web.DeleteNavigationNode("Test Node", string.Empty, NavigationType.QuickLaunch);

                clientContext.Load(web, w => w.Navigation.QuickLaunch);
                clientContext.ExecuteQueryRetry();

                if (web.Navigation.QuickLaunch.Any())
                {
                    var navNode = web.Navigation.QuickLaunch.FirstOrDefault(n => n.Title == "Test Node");
                    Assert.IsNull(navNode);
                }
            }
        }

        [TestMethod]
        public void DeleteSearchNavigationNodeTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                web.AddNavigationNode("Test Node", new Uri("https://www.microsoft.com"), string.Empty, NavigationType.SearchNav);

                web.DeleteNavigationNode("Test Node", string.Empty, NavigationType.SearchNav);

                NavigationNodeCollection searchNavigation = web.LoadSearchNavigation();

                if (searchNavigation.Any())
                {
                    var navNode = searchNavigation.FirstOrDefault(n => n.Title == "Test Node");
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
                clientContext.ExecuteQueryRetry();
                Assert.IsFalse(web.Navigation.QuickLaunch.Any());
            }
        }
        #endregion
    }
}

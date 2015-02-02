using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Tests;
namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class JavaScriptExtensionsTests
    {
        const string KEY = "TEST_KEY";

        [TestMethod()]
        public void AddJsLinkToWebTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddJsLink(KEY, "/jquery-2.1.1.min.js"); // Dummy link

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);
                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }

        [TestMethod()]
        public void AddJsLinkToSiteTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Site.AddJsLink(KEY, "/jquery-2.1.1.min.js"); // Dummy link

                var customActions = clientContext.Site.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);
                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }

        [TestMethod()]
        public void AddJsLinkIEnumerableToWebTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddJsLink(KEY, new List<string> {"/jquery-2.1.1.min.js", "/anotherjslink.js"}); // Dummy link

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);

                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }

        [TestMethod()]
        public void AddJsLinkIEnumerableToSiteTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Site.AddJsLink(KEY, new List<string> { "/jquery-2.1.1.min.js", "/anotherjslink.js" }); // Dummy link

                var customActions = clientContext.Site.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);

                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }



        [TestMethod()]
        public void DeleteJsLinkFromWebTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                clientContext.Web.AddJsLink(KEY, "/jquery-2.1.1.min.js"); // Dummy link

                // Test
                clientContext.Web.DeleteJsLink(KEY);

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);
                Assert.IsNull(existingAction, "Existing Action found");
            }
        }

        [TestMethod()]
        public void DeleteJsLinkFromSiteTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                clientContext.Site.AddJsLink(KEY, "/jquery-2.1.1.min.js"); // Dummy link

                // Test
                clientContext.Site.DeleteJsLink(KEY);

                var customActions = clientContext.Site.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);
                Assert.IsNull(existingAction, "Existing Action found");
            }
        }


        [TestMethod()]
        public void AddJsBlockToWebTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddJsBlock(KEY, "<script>alert('Testing')</script>");

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);

                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }

        [TestMethod()]
        public void AddJsBlockToSiteTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Site.AddJsBlock(KEY, "<script>alert('Testing')</script>");

                var customActions = clientContext.Site.GetCustomActions();
                var existingAction = customActions.FirstOrDefault(a => a.Name == KEY);

                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Site.DeleteCustomAction(existingAction.Id);
                }
            }
        }
    }
}

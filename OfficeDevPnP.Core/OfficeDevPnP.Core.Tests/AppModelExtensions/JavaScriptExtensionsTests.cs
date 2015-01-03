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
        public void AddJsLinkTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddJsLink(KEY, "/jquery-2.1.1.min.js"); // Dummy link

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.Where(a => a.Name == KEY).FirstOrDefault();
                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }

        [TestMethod()]
        public void AddJsLinkIEnumerableTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddJsLink(KEY, new List<string> { "/jquery-2.1.1.min.js", "/anotherjslink.js" }); // Dummy link

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.Where(a => a.Name == KEY).FirstOrDefault();

                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }

        [TestMethod()]
        public void DeleteJsLinkTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                clientContext.Web.AddJsLink(KEY, "/jquery-2.1.1.min.js"); // Dummy link

                // Test
                clientContext.Web.DeleteJsLink(KEY);

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.Where(a => a.Name == KEY).FirstOrDefault();
                Assert.IsNull(existingAction, "Existing Action found");
            }
        }

        [TestMethod()]
        public void AddJsBlockTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddJsBlock(KEY, "<script>alert('Testing')</script>");

                var customActions = clientContext.Web.GetCustomActions();
                var existingAction = customActions.Where(a => a.Name == KEY).FirstOrDefault();

                Assert.IsNotNull(existingAction, "Existing Action not found");

                // Teardown
                if (existingAction != null)
                {
                    clientContext.Web.DeleteCustomAction(existingAction.Id);
                }
            }
        }
    }
}

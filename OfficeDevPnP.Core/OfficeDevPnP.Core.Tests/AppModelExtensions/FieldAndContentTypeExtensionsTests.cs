using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;
using System.Configuration;
using OfficeDevPnP.Core.Tests;
namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class FieldAndContentTypeExtensionsTests
    {
        // **** IMPORTANT ****
        // In order to succesfully clean up after testing, create all artifacts that end up in the test site with a name starting with "Test_"
        // **** IMPORTANT ****


        #region [ CreateField ]
        [TestMethod()]
        public void CreateFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();
                var fieldChoice = clientContext.Web.CreateField<FieldChoice>(
                    fieldId,
                    fieldName,
                    FieldType.Choice.ToString(),
                    true,
                    fieldName,
                    "Test fields group");

                var field = clientContext.Web.Fields.GetByTitle(fieldName);
                clientContext.Load(field);
                clientContext.ExecuteQuery();

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("Choice", fieldChoice.TypeAsString, "Failed to create a FieldChoice object.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Field was able to be created twice without exception.")]
        public void CreateExistingFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var fieldName = "Test_ABC123";
                var fieldId = Guid.NewGuid();

                var fieldChoice1 = clientContext.Web.CreateField<FieldChoice>(
                    fieldId,
                    fieldName,
                    FieldType.Choice.ToString(),
                    true,
                    fieldName,
                    "Test fields group");
                var fieldChoice2 = clientContext.Web.CreateField<FieldChoice>(
                    fieldId,
                    fieldName,
                    FieldType.Choice.ToString(),
                    true,
                    fieldName,
                    "Test fields group");

                var field = clientContext.Web.Fields.GetByTitle(fieldName);
                clientContext.Load(field);
                clientContext.ExecuteQuery();
            }
        }
        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            // Remove all created Test Artifacts
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Fields
                var fields = clientContext.LoadQuery(clientContext.Web.Fields);
                clientContext.ExecuteQuery();
                var testFields = fields.Where(f => f.InternalName.StartsWith("Test_",StringComparison.OrdinalIgnoreCase));
                foreach(var field in testFields)
                {
                    field.DeleteObject();
                }
                clientContext.ExecuteQuery();

                // Lists
                var lists = clientContext.LoadQuery(clientContext.Web.Lists);
                clientContext.ExecuteQuery();
                var testLists = lists.Where(l => l.Title.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
                foreach(var list in testLists)
                {
                    list.DeleteObject();
                }
                clientContext.ExecuteQuery();
            }
        }
    }
}

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
namespace Microsoft.SharePoint.Client.Tests {
    [TestClass()]
    public class FieldAndContentTypeExtensionsTests {
        #region [ CreateField ]
        [TestMethod()]
        public void CreateFieldTest() {
            using (var clientContext = TestCommon.CreateClientContext()) {
                var fieldName = "Test_"+DateTime.Now.ToFileTime();
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
        public void CreateExistingFieldTest() {
            using (var clientContext = TestCommon.CreateClientContext()) {
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

	//FIXME: Tests does not revert target to a clean slate after running.
	//FIXME: Tests are tighthly coupled to eachother
	[TestMethod]
        public void DeleteExistingFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                clientContext.Load(web.Fields);
                clientContext.ExecuteQuery();
                var beforeNrOfFields = web.Fields.Count;

                web.RemoveFieldByInternalName("Test_ABC123");

                clientContext.Load(web.Fields);
                clientContext.ExecuteQuery();
                var afterNrOfFields = web.Fields.Count;
                Assert.AreEqual(beforeNrOfFields - 1, afterNrOfFields);
            }
        }

	[TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveFieldByInternalNameThrowsOnNoMatchTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                try
                {
                    web.RemoveFieldByInternalName("FieldThatDoesNotExistEver");
                }
                catch (ArgumentException ex)
                {
                    Assert.AreEqual(ex.Message, "Could not find field with internalName FieldThatDoesNotExistEver");
                    throw;
                }
            }
        }
        #endregion
    }
}

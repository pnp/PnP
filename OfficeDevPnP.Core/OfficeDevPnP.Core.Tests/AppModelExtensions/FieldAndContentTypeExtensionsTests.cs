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
        [TestCleanup]
        public void Cleanup()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var fields = clientContext.LoadQuery(clientContext.Web.Fields);
                clientContext.ExecuteQuery();
                var testFields = fields.Where(f => f.InternalName.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
                foreach (var field in testFields)
                {
                    field.DeleteObject();
                }
                clientContext.ExecuteQuery();

                var lists = clientContext.LoadQuery(clientContext.Web.Lists);
                clientContext.ExecuteQuery();
                var testLists = lists.Where(l => l.Title.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
                foreach (var list in testLists)
                {
                    list.DeleteObject();
                }
                clientContext.ExecuteQuery();

                var contentTypes = clientContext.LoadQuery(clientContext.Web.ContentTypes);
                clientContext.ExecuteQuery();
                var testContentTypes = contentTypes.Where(l => l.Name.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
                foreach (var ct in testContentTypes)
                {
                    ct.DeleteObject();
                }
                clientContext.ExecuteQuery();
            }
        }

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

        [TestMethod()]
        public void CanAddContentTypeToListByName()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var listName = "Test_" + DateTime.Now.ToFileTime();
                clientContext.Web.CreateList(ListTemplateType.GenericList, listName, enableContentTypes: true, enableVersioning: false);
                clientContext.ExecuteQuery();

                clientContext.Web.AddContentTypeToListByName(listName, "Issue", defaultContent: true);

                var list = clientContext.Web.GetListByTitle(listName);
                clientContext.Load(list.ContentTypes);
                clientContext.ExecuteQuery();
                var issueContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals("Issue"));

                Assert.AreEqual(1, issueContentTypeCount, "Issue content type was not added.");
            }
        }

        [TestMethod()]
        public void CanRemoveContentTypeFromListByName()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var listName = "Test_" + DateTime.Now.ToFileTime();
                clientContext.Web.CreateList(ListTemplateType.GenericList, listName, enableContentTypes: true,enableVersioning:false);
                clientContext.ExecuteQuery();

                clientContext.Web.AddContentTypeToListByName(listName,"Issue", defaultContent:true);
                clientContext.Web.AddContentTypeToListByName(listName, "Task", defaultContent: true);
                clientContext.Web.RemoveContentTypeFromListByName(listName, "Item");

                var list = clientContext.Web.GetListByTitle(listName);
                clientContext.Load(list.ContentTypes);
                clientContext.ExecuteQuery();
                var issueContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals("Issue"));
                var taskContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals("Task"));
                var itemContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals("Item"));
                
                Assert.AreEqual(1, issueContentTypeCount, "Issue content type was not added. Test is invalid.");
                Assert.AreEqual(1, taskContentTypeCount, "Task content type was not added. Test is invalid.");
                Assert.AreEqual(0, itemContentTypeCount, "Item content type was not removed.");
            }
        }

        [TestMethod()]
        public void CanRemoveContentTypeFromListById()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var listName = "Test_" + DateTime.Now.ToFileTime();
                clientContext.Web.CreateList(ListTemplateType.GenericList, listName, enableContentTypes: true, enableVersioning: false);
                clientContext.ExecuteQuery();

                clientContext.Web.AddContentTypeToListByName(listName, "Issue", defaultContent: true);
                clientContext.Web.AddContentTypeToListByName(listName, "Task", defaultContent: true);
                clientContext.Web.RemoveContentTypeFromListById(listName, "0x01");

                var list = clientContext.Web.GetListByTitle(listName);
                clientContext.Load(list.ContentTypes);
                clientContext.ExecuteQuery();
                var issueContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals("Issue"));
                var taskContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals("Task"));
                var itemContentTypeCount = list.ContentTypes.Count(x => x.Name.Equals("Item"));

                Assert.AreEqual(1, issueContentTypeCount, "Issue content type was not added. Test is invalid.");
                Assert.AreEqual(1, taskContentTypeCount, "Task content type was not added. Test is invalid.");
                Assert.AreEqual(0, itemContentTypeCount, "Item content type was not removed.");
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

	//FIXME: Tests does not revert target to a clean slate after running.
	//FIXME: Tests are tighthly coupled to eachother

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

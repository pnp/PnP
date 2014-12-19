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
using OfficeDevPnP.Core.Entities;
namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class FieldAndContentTypeExtensionsTests
    {
        const string DOC_LIB_TITLE = "Test_Library";
        const string TEST_CATEGORY = "Fields and Content Types";
        const string TEST_CT_PNP = "Test_CT_PNP";
        const string TEST_CT_PNP_ID = "0x01010080BA6ECAEDA6487EAD28FC3C21CA1900";

        // **** IMPORTANT ****
        // In order to succesfully clean up after testing, create all artifacts that end up in the test site with a name starting with "Test_"
        // **** IMPORTANT ****

        [TestCleanup]
        public void Cleanup()
        {
            using (var clientContext = TestCommon.CreateClientContext()) {
                var web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                EmptyRecycleBin(clientContext);

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
                foreach (var ctype in testContentTypes) {
                    ctype.DeleteObject();
                clientContext.ExecuteQuery();
                }

                EmptyRecycleBin(clientContext);
            }
        }

        #region Field tests
        [TestMethod()]
        [TestCategory(TEST_CATEGORY)]
        public void CreateFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();

                var fieldCI = new FieldCreationInformation(FieldType.Choice)
                {
                    Id = fieldId,
                    InternalName = fieldName,
                    DisplayName = fieldName,
                    AddToDefaultView = true,
                    Group = "Test fields group"
                };
                var fieldChoice = clientContext.Web.CreateField<FieldChoice>(fieldCI);

                var field = clientContext.Web.Fields.GetByTitle(fieldName);
                clientContext.Load(field);
                clientContext.ExecuteQuery();

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("Choice", fieldChoice.TypeAsString, "Failed to create a FieldChoice object.");
            }
        }

        [TestCategory(TEST_CATEGORY)]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Field was able to be created twice without exception.")]
        public void CreateExistingFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var fieldName = "Test_ABC123";
                var fieldId = Guid.NewGuid();

                FieldCreationInformation fieldCI = new FieldCreationInformation(FieldType.Choice)
                {
                    Id = fieldId,
                    InternalName = fieldName,
                    AddToDefaultView = true,
                    DisplayName = fieldName,
                    Group = "Test fields group"
                };
                var fieldChoice1 = clientContext.Web.CreateField<FieldChoice>(fieldCI);
                var fieldChoice2 = clientContext.Web.CreateField<FieldChoice>(fieldCI);

                var field = clientContext.Web.Fields.GetByTitle(fieldName);
                clientContext.Load(field);
                clientContext.ExecuteQuery();
            }
        }

        //FIXME: Tests does not revert target to a clean slate after running.
        //FIXME: Tests are tighthly coupled to eachother

        [TestCategory(TEST_CATEGORY)]
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

        [TestCategory(TEST_CATEGORY)]
        [TestMethod]
        public void CreateFieldFromXmlTest()
        {
            using(var clientContext = TestCommon.CreateClientContext())
            {
                var fieldId = Guid.NewGuid();
                var fieldXml = string.Format("<Field xmlns='http://schemas.microsoft.com/sharepoint/' ID='{0}' Name='Test_FieldFromXML' StaticName='Test_FieldFromXML' DisplayName='Test Field From XML' Group='Test_Group' Type='Text' Required='TRUE' DisplaceOnUpgrade='TRUE' />", fieldId.ToString("B").ToUpper());

                var field = clientContext.Web.CreateField(fieldXml);

                Assert.IsNotNull(field);
                Assert.IsInstanceOfType(field, typeof(Field));

            }
        }
        #endregion

        #region Contenttype tests
        [TestMethod]
        public void ContentTypeExistsByNameTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
                Assert.IsTrue(clientContext.Web.ContentTypeExistsByName(TEST_CT_PNP));
            }
        }

        [TestMethod]
        public void ContentTypeExistsByIdTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
                Assert.IsTrue(clientContext.Web.ContentTypeExistsById(TEST_CT_PNP_ID));
            }
        }

        [TestMethod]
        public void ContentTypeExistsByNameInSubWebTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);

                string subsiteurl = "Test_Pnp_" + Guid.NewGuid().ToString();
                var subweb = clientContext.Web.Webs.Add(new WebCreationInformation()
                {
                    Title = "Test Content type lookups",
                    Url = subsiteurl,
                });

                try
                {
                    clientContext.Load(subweb);
                    clientContext.ExecuteQuery();

                    using (var clientContextSub = new ClientContext(String.Format("{0}\\{1}", ConfigurationManager.AppSettings["SPODevSiteUrl"], subsiteurl)) { Credentials = clientContext.Credentials })
                    {
                        Assert.IsFalse(clientContextSub.Web.ContentTypeExistsByName(TEST_CT_PNP));
                        Assert.IsTrue(clientContextSub.Web.ContentTypeExistsByName(TEST_CT_PNP, true));
                    }
                }
                finally
                {
                    subweb.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
        }

        [TestMethod]
        public void ContentTypeExistsByIdInSubWebTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);

                string subsiteurl = "Test_Pnp_" + Guid.NewGuid().ToString();
                var subweb = clientContext.Web.Webs.Add(new WebCreationInformation()
                {
                    Title = "Test Content type lookups",
                    Url = subsiteurl,
                });

                try
                {
                    clientContext.Load(subweb);
                    clientContext.ExecuteQuery();

                    using (var clientContextSub = new ClientContext(String.Format("{0}\\{1}", ConfigurationManager.AppSettings["SPODevSiteUrl"], subsiteurl)) { Credentials = clientContext.Credentials })
                    {
                        Assert.IsFalse(clientContextSub.Web.ContentTypeExistsById(TEST_CT_PNP_ID));
                        Assert.IsTrue(clientContextSub.Web.ContentTypeExistsById(TEST_CT_PNP_ID, true));
                    }
                }
                finally
                {
                    subweb.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
        }

        [TestMethod]
        public void ContentTypeExistsByNameSearchInSiteHierarchyTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
                Assert.IsTrue(clientContext.Web.ContentTypeExistsByName(TEST_CT_PNP, true));
            }
        }

        [TestMethod]
        public void ContentTypeExistsByIdSearchInSiteHierarchyTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.CreateContentType(TEST_CT_PNP, TEST_CT_PNP_ID, TEST_CATEGORY);
                Assert.IsTrue(clientContext.Web.ContentTypeExistsById(TEST_CT_PNP_ID, true));
            }
        }
        #endregion

        [TestCategory(TEST_CATEGORY)]
        [TestMethod]
        public void SetDefaultContentTypeToListTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;

                var testList = web.CreateList(ListTemplateType.DocumentLibrary, "Test_SetDefaultContentTypeToListTestList", true, true, "", true);

                var parentCt = web.GetContentTypeById("0x0101");
                var ct = web.CreateContentType("Test_SetDefaultContentTypeToListCt", "Desc", "", "Test_Group", parentCt);
                clientContext.Load(ct);
                clientContext.Load(testList.RootFolder, f => f.ContentTypeOrder);
                clientContext.ExecuteQuery();

                var prevUniqueContentTypeOrder = testList.RootFolder.ContentTypeOrder;

                Assert.AreEqual(1, prevUniqueContentTypeOrder.Count());

                testList.AddContentTypeToList(ct);

                testList.SetDefaultContentTypeToList(ct);
                clientContext.Load(testList.RootFolder, f => f.ContentTypeOrder);
                clientContext.ExecuteQuery();

                Assert.AreEqual(2, testList.RootFolder.ContentTypeOrder.Count());
                Assert.IsTrue(testList.RootFolder.ContentTypeOrder.First().StringValue.StartsWith(ct.Id.StringValue, StringComparison.OrdinalIgnoreCase));

                testList.DeleteObject();
                ct.DeleteObject();
                clientContext.ExecuteQuery();
            }
        }

        [TestCategory(TEST_CATEGORY)]
        [TestMethod()]
        public void ReorderContentTypesTest() {
            using (var clientContext = TestCommon.CreateClientContext()) {
                var web = clientContext.Web;
                clientContext.Load(web, w=>w.ContentTypes);
                clientContext.ExecuteQuery();

                // create content types
                var documentCtype = web.ContentTypes.FirstOrDefault(ct=>ct.Name == "Document");
                var newCtypeInfo1 = new ContentTypeCreationInformation() {
                    Name = "Test_ContentType1",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo2 = new ContentTypeCreationInformation() {
                    Name = "Test_ContentType2",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };
                var newCtypeInfo3 = new ContentTypeCreationInformation() {
                    Name = "Test_ContentType3",
                    ParentContentType = documentCtype,
                    Group = "Test content types",
                    Description = "This is a test content type"
                };

                var newCtype1 = web.ContentTypes.Add(newCtypeInfo1);
                var newCtype2 = web.ContentTypes.Add(newCtypeInfo2);
                var newCtype3 = web.ContentTypes.Add(newCtypeInfo3);
                clientContext.Load(newCtype1);
                clientContext.Load(newCtype2);
                clientContext.Load(newCtype3);
                clientContext.ExecuteQuery();

                var newList = new ListCreationInformation() {
                    TemplateType = (int)ListTemplateType.DocumentLibrary,
                    Title = DOC_LIB_TITLE,
                    Url = "TestLibrary"
                };

                var doclib = clientContext.Web.Lists.Add(newList);
                doclib.ContentTypesEnabled = true;
                doclib.ContentTypes.AddExistingContentType(newCtype1);
                doclib.ContentTypes.AddExistingContentType(newCtype2);
                doclib.ContentTypes.AddExistingContentType(newCtype3);
                doclib.Update();
                clientContext.Load(doclib.ContentTypes);
                clientContext.ExecuteQuery();

                var expectedIds = new string[]{
                    newCtype3.Name,
                    newCtype1.Name,
                    newCtype2.Name,
                    documentCtype.Name
                };

                doclib.ReorderContentTypes(expectedIds);
                var reorderedCtypes = clientContext.LoadQuery(doclib.ContentTypes);
                clientContext.ExecuteQuery();

                var actualIds = reorderedCtypes.Except(
                                        // remove the folder content type
                                        reorderedCtypes.Where(ct => ct.Id.StringValue.StartsWith("0x012000"))
                                    ).Select(ct => ct.Name).ToArray();

                CollectionAssert.AreEqual(expectedIds, actualIds);
            }
        }

        void EmptyRecycleBin(ClientContext clientContext) {
            var recycleBin = clientContext.Web.RecycleBin;
            clientContext.Load(recycleBin);
            clientContext.ExecuteQuery();

            var items = recycleBin.ToArray();

            for (var i = 0; i < items.Length; i++)
                items[i].DeleteObject();

            clientContext.ExecuteQuery();
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Tests;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class TaxonomyExtensionsTests
    {
        private string _termGroupName; // For easy reference. Set in the Initialize method
        private string _termSetName; // For easy reference. Set in the Initialize method
        private string _termName; // For easy reference. Set in the Initialize method
        private Guid _termGroupId = new Guid("e879befa-2356-49fd-b43e-ba446be72d6c"); // Hardcoded for easier reference in tests
        private Guid _termSetId = new Guid("59ad0849-97b9-4755-a431-2bb9ebc8b66b"); // Hardcoded for easier reference in tests
        private Guid _termId = new Guid("51af0e21-ef8c-4e1f-b897-f677d0938f48");

        private Guid _listId; // For easy reference



        [TestInitialize]
        public void Initialize()
        {
            // Create some taxonomy groups and terms
            using (var clientContext = TestCommon.CreateClientContext())
            {
                _termGroupName = "Test_Group_" + DateTime.Now.ToFileTime();
                _termSetName = "Test_Termset_" + DateTime.Now.ToFileTime();
                _termName = "Test_Term_" + DateTime.Now.ToFileTime();
                // Termgroup
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var termGroup = termStore.CreateGroup(_termGroupName, _termGroupId);
                clientContext.Load(termGroup);
                clientContext.ExecuteQuery();

                // Termset
                var termSet = termGroup.CreateTermSet(_termSetName, _termSetId, 1033);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Term
                termSet.CreateTerm(_termName, 1033, _termId);
                clientContext.ExecuteQuery();

                // List
                ListCreationInformation listCI = new ListCreationInformation();
                listCI.TemplateType = (int)ListTemplateType.GenericList;
                listCI.Title = "Test_List_" + DateTime.Now.ToFileTime();
                var list = clientContext.Web.Lists.Add(listCI);
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                _listId = list.Id;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Clean up Taxonomy
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var termGroup = termStore.GetGroup(_termGroupId);
                var termSets = termGroup.TermSets;
                clientContext.Load(termSets);
                clientContext.ExecuteQuery();
                foreach (var termSet in termSets)
                {
                    termSet.DeleteObject();
                }
                termGroup.DeleteObject(); // Will delete underlying termset
                clientContext.ExecuteQuery();

                // Clean up fields
                var fields = clientContext.LoadQuery(clientContext.Web.Fields);
                clientContext.ExecuteQuery();
                var testFields = fields.Where(f => f.InternalName.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
                foreach (var field in testFields)
                {
                    field.DeleteObject();
                }
                clientContext.ExecuteQuery();

                // Clean up list
                var list = clientContext.Web.Lists.GetById(_listId);
                list.DeleteObject();
                clientContext.ExecuteQuery();
            }
        }

        [TestMethod()]
        public void CreateTaxonomyFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Get Test TermSet

                var web = clientContext.Web;
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();
                var field = web.CreateTaxonomyField(
                    fieldId,
                    fieldName,
                    fieldName,
                    "Test Fields Group",
                    termSet
                    );

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldType", field.TypeAsString, "Failed to create a TaxonomyField object.");
            }
        }

        [TestMethod()]
        public void CreateTaxonomyFieldMultiValueTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Get Test TermSet

                var web = clientContext.Web;
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();
                var field = web.CreateTaxonomyField(
                    fieldId,
                    fieldName,
                    fieldName,
                    "Test Fields Group",
                    termSet,
                    true
                    );

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldTypeMulti", field.TypeAsString, "Failed to create a TaxonomyField object.");
            }
        }

        [TestMethod()]
        public void SetTaxonomyFieldValueByTermPathTest()
        {
            var fieldName = "Test_" + DateTime.Now.ToFileTime();

            var fieldId = Guid.NewGuid();

            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve list
                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Create taxonomyfield first

                var field = list.CreateTaxonomyField(
                    fieldId,
                    fieldName,
                    fieldName,
                    "Test Fields Group",
                    termSet
                    );

                // Create Item
                ListItemCreationInformation itemCi = new ListItemCreationInformation();

                var item = list.AddItem(itemCi);
                item.Update();
                clientContext.Load(item);
                clientContext.ExecuteQuery();

                item.SetTaxonomyFieldValueByTermPath(_termGroupName + "|" + _termSetName + "|" + _termName, fieldId);

                clientContext.Load(item, i => i[fieldName]);
                clientContext.ExecuteQuery();

                var value = item[fieldName] as TaxonomyFieldValue;

                Assert.AreEqual(_termId.ToString(), value.TermGuid, "Term not set correctly");
            }
        }

        [TestMethod()]
        public void GetTaxonomySessionTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var session = site.GetTaxonomySession();
                Assert.IsInstanceOfType(session, typeof(TaxonomySession), "Did not return TaxonomySession object");
            }
        }

        [TestMethod()]
        public void GetDefaultKeywordsTermStoreTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var termStore = site.GetDefaultKeywordsTermStore();
                Assert.IsInstanceOfType(termStore, typeof(TermStore), "Did not return TermStore object");
            }
        }

        [TestMethod()]
        public void GetDefaultSiteCollectionTermStoreTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var termStore = site.GetDefaultSiteCollectionTermStore();
                Assert.IsInstanceOfType(termStore, typeof(TermStore), "Did not return TermStore object");
            }
        }

        [TestMethod()]
        public void GetTermSetsByNameTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var termSetCollection = site.GetTermSetsByName(_termSetName);
                Assert.IsInstanceOfType(termSetCollection, typeof(TermSetCollection), "Did not return TermSetCollection object");
                Assert.IsTrue(termSetCollection.AreItemsAvailable, "No terms available");
            }
        }

        [TestMethod()]
        public void GetTermGroupByNameTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var termGroup = site.GetTermGroupByName(_termGroupName);
                Assert.IsInstanceOfType(termGroup, typeof(TermGroup), "Did not return TermGroup object");
                Assert.AreEqual(_termGroupName, termGroup.Name, "Name does not match");
            }
        }

        [TestMethod()]
        public void GetTermByNameTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var term = site.GetTermByName(_termSetId, _termName);
                Assert.IsInstanceOfType(term, typeof(Term), "Did not return Term object");
                Assert.AreEqual(_termName, term.Name, "Name does not match");
            }
        }

        [TestMethod()]
        public void AddTermToTermsetTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var termName = "Test_Term_" + DateTime.Now.ToFileTime();
                var term = site.AddTermToTermset(_termSetId, termName);
                Assert.IsInstanceOfType(term, typeof(Term), "Did not return Term object");
                Assert.AreEqual(termName, term.Name, "Name does not match");
            }
        }

        [TestMethod()]
        public void AddTermToTermsetTest1()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var termName = "Test_Term_" + DateTime.Now.ToFileTime();
                var termId = Guid.NewGuid();
                var term = site.AddTermToTermset(_termSetId, termName, termId);
                Assert.IsInstanceOfType(term, typeof(Term), "Did not return Term object");
                Assert.AreEqual(termName, term.Name, "Name does not match");
                Assert.AreEqual(termId, term.Id, "Id does not match");

            }
        }

        [TestMethod()]
        public void ImportTermsTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;

                var termName1 = "Test_Term_1" + DateTime.Now.ToFileTime();
                var termName2 = "Test_Term_2" + DateTime.Now.ToFileTime();

                List<string> termLines = new List<string>();
                termLines.Add(_termGroupName + "|" + _termSetName + "|" + termName1);
                termLines.Add(_termGroupName + "|" + _termSetName + "|" + termName2);
                site.ImportTerms(termLines.ToArray(), 1033, "|");

                var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
                var termGroup = termStore.Groups.GetByName(_termGroupName);
                var termSet = termGroup.TermSets.GetByName(_termSetName);
                var term1 = termSet.Terms.GetByName(termName1);
                var term2 = termSet.Terms.GetByName(termName2);
                clientContext.Load(term1);
                clientContext.Load(term2);
                clientContext.ExecuteQuery();

                Assert.IsNotNull(term1);
                Assert.IsNotNull(term2);
            }
        }

        [TestMethod()]
        public void ExportTermSetTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var lines = site.ExportTermSet(_termSetId, false);
                Assert.IsTrue(lines.Any(), "No lines returned");
            }
        }

        [TestMethod()]
        public void ExportAllTermsTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var lines = site.ExportAllTerms(false);
                Assert.IsTrue(lines.Any(), "No lines returned");
            }
        }

        [TestMethod()]
        public void GetTaxonomyItemByPathTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var path = _termGroupName + "|" + _termSetName;
                var taxonomyItem = site.GetTaxonomyItemByPath(path);
                Assert.IsInstanceOfType(taxonomyItem, typeof(TaxonomyItem));
                Assert.AreEqual(_termSetName, taxonomyItem.Name, "Did not return correct termset");

                path = _termGroupName + "|" + _termSetName + "|" + _termName;
                taxonomyItem = site.GetTaxonomyItemByPath(path);

                Assert.IsInstanceOfType(taxonomyItem, typeof(TaxonomyItem));
                Assert.AreEqual(_termName, taxonomyItem.Name, "Did not return correct term");
            }

        }

        [TestMethod()]
        public void SetTaxonomyFieldValueTest()
        {
            var fieldName = "Test2_" + DateTime.Now.ToFileTime();

            var fieldId = Guid.NewGuid();

            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve list
                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Create taxonomyfield first

                var field = list.CreateTaxonomyField(
                    fieldId,
                    fieldName,
                    fieldName,
                    "Test Fields Group",
                    termSet
                    );

                // Create Item
                ListItemCreationInformation itemCi = new ListItemCreationInformation();

                var item = list.AddItem(itemCi);
                item.Update();
                clientContext.Load(item);
                clientContext.ExecuteQuery();

                item.SetTaxonomyFieldValue(fieldId, _termName, _termId);

                clientContext.Load(item, i => i[fieldName]);
                clientContext.ExecuteQuery();

                var value = item[fieldName] as TaxonomyFieldValue;

                Assert.AreEqual(_termId.ToString(), value.TermGuid, "Term not set correctly");
            }
        }

        [TestMethod()]
        public void CreateTaxonomyFieldTest1()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                var fieldId = Guid.NewGuid();
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var field = web.CreateTaxonomyField(
                        fieldId,
                        fieldName,
                        fieldName,
                        "Test Fields Group",
                        _termGroupName,
                        _termSetName);

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldType", field.TypeAsString, "Failed to create a TaxonomyField object.");
            }

        }

        [TestMethod()]
        public void CreateTaxonomyFieldTest2()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();
                var field = list.CreateTaxonomyField(
                    fieldId,
                    fieldName,
                    fieldName,
                    "Test Fields Group",
                    termSet
                    );

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldType", field.TypeAsString, "Failed to create a TaxonomyField object.");
            }
        }

        [TestMethod()]
        public void CreateTaxonomyFieldTest3()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {


                // Retrieve List
                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                // Create field
                var fieldId = Guid.NewGuid();
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var field = list.CreateTaxonomyField(
                        fieldId,
                        fieldName,
                        fieldName,
                        "Test Fields Group",
                        _termGroupName,
                        _termSetName);

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldType", field.TypeAsString, "Failed to create a TaxonomyField object.");
            }

        }

        [TestMethod()]
        public void WireUpTaxonomyFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Retrieve list
                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                // Create Field
                var fieldId = Guid.NewGuid();
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var field = list.CreateTaxonomyField(
                        fieldId,
                        fieldName,
                        fieldName,
                        "Test Fields Group",
                        _termGroupName,
                        _termSetName);

                list.WireUpTaxonomyField(field, termSet);

                field = list.Fields.GetById(fieldId);
                clientContext.Load(field);
                clientContext.ExecuteQuery();
                var taxField = clientContext.CastTo<TaxonomyField>(field);
                Assert.IsTrue(taxField.IsTermSetValid);
                Assert.AreEqual(_termSetId, taxField.TermSetId);
            }
        }

        [TestMethod()]
        public void WireUpTaxonomyFieldTest1()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Retrieve list
                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                // Create Field
                var fieldId = Guid.NewGuid();
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var field = list.CreateTaxonomyField(
                        fieldId,
                        fieldName,
                        fieldName,
                        "Test Fields Group",
                        _termGroupName,
                        _termSetName);

                list.WireUpTaxonomyField(field, _termGroupName, _termSetName);

                field = list.Fields.GetById(fieldId);
                clientContext.Load(field);
                clientContext.ExecuteQuery();
                var taxField = clientContext.CastTo<TaxonomyField>(field);
                Assert.IsTrue(taxField.IsTermSetValid);
                Assert.AreEqual(_termSetId, taxField.TermSetId);
            }
        }

        [TestMethod()]
        public void WireUpTaxonomyFieldTest2()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQuery();

                // Retrieve list
                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                // Create Field
                var fieldId = Guid.NewGuid();
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var field = list.CreateTaxonomyField(
                        fieldId,
                        fieldName,
                        fieldName,
                        "Test Fields Group",
                        _termGroupName,
                        _termSetName);

                list.WireUpTaxonomyField(fieldId, _termGroupName, _termSetName);

                field = list.Fields.GetById(fieldId);
                clientContext.Load(field);
                clientContext.ExecuteQuery();
                var taxField = clientContext.CastTo<TaxonomyField>(field);
                Assert.IsTrue(taxField.IsTermSetValid);
                Assert.AreEqual(_termSetId, taxField.TermSetId);
            }
        }

    }
}

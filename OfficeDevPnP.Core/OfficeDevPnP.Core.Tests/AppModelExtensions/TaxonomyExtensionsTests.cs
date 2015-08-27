using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Tests;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Linq;
using System.Collections.Generic;
using OfficeDevPnP.Core.Entities;

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

        private string SampleTermSetPath = "../../Resources/ImportTermSet.csv";
        private string SampleUpdateTermSetPath = "../../Resources/UpdateTermSet.csv";
        private string SampleGuidTermSetPath = "../../Resources/GuidTermSet.csv";
        private Guid UpdateTermSetId = new Guid("{35585956-83E4-4A44-8FC5-AC50942E3187}");
        private Guid GuidTermSetId = new Guid("{90FD4208-8281-40CC-872E-DD85F33B50AB}");

        #region Test initialize and cleanup
        [TestInitialize]
        public void Initialize()
        {
            if (!TestCommon.AppOnlyTesting())
            {
                Console.WriteLine("TaxonomyExtensionsTests.Initialise");
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
                    clientContext.ExecuteQueryRetry();

                    // Termset
                    var termSet = termGroup.CreateTermSet(_termSetName, _termSetId, 1033);
                    clientContext.Load(termSet);
                    clientContext.ExecuteQueryRetry();

                    // Term
                    termSet.CreateTerm(_termName, 1033, _termId);
                    clientContext.ExecuteQueryRetry();

                    // List
                    ListCreationInformation listCI = new ListCreationInformation();
                    listCI.TemplateType = (int)ListTemplateType.GenericList;
                    listCI.Title = "Test_List_" + DateTime.Now.ToFileTime();
                    var list = clientContext.Web.Lists.Add(listCI);
                    clientContext.Load(list);
                    clientContext.ExecuteQueryRetry();
                    _listId = list.Id;

                }
            }
            else
            {
                Assert.Inconclusive("Taxonomy tests are not supported when testing using app-only");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (!TestCommon.AppOnlyTesting())
            {
                Console.WriteLine("TaxonomyExtensionsTests.Cleanup");
                using (var clientContext = TestCommon.CreateClientContext())
                {
                    // Clean up Taxonomy
                    var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                    var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                    var termGroup = termStore.GetGroup(_termGroupId);
                    var termSets = termGroup.TermSets;
                    clientContext.Load(termSets);
                    clientContext.ExecuteQueryRetry();
                    foreach (var termSet in termSets)
                    {
                        termSet.DeleteObject();
                    }
                    termGroup.DeleteObject(); // Will delete underlying termset
                    clientContext.ExecuteQueryRetry();

                    // Clean up fields
                    var fields = clientContext.LoadQuery(clientContext.Web.Fields);
                    clientContext.ExecuteQueryRetry();
                    var testFields = fields.Where(f => f.InternalName.StartsWith("Test_", StringComparison.OrdinalIgnoreCase));
                    foreach (var field in testFields)
                    {
                        field.DeleteObject();
                    }
                    clientContext.ExecuteQueryRetry();

                    // Clean up list
                    var list = clientContext.Web.Lists.GetById(_listId);
                    list.DeleteObject();
                    clientContext.ExecuteQueryRetry();
                }
            }
        }
        #endregion

        #region Create taxonomy field tests
        [TestMethod()]
        public void CreateTaxonomyFieldTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQueryRetry();

                // Get Test TermSet

                var web = clientContext.Web;
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();

                TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
                {
                    Id = fieldId,
                    DisplayName = fieldName,
                    InternalName = fieldName,
                    Group = "Test Fields Group",
                    TaxonomyItem = termSet
                };
                var field = web.CreateTaxonomyField(fieldCI);

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
                clientContext.ExecuteQueryRetry();

                // Get Test TermSet

                var web = clientContext.Web;
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();
                TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
                {
                    Id = fieldId,
                    DisplayName = fieldName,
                    InternalName = fieldName,
                    Group = "Test Fields Group",
                    TaxonomyItem = termSet,
                    MultiValue = true
                };
                var field = web.CreateTaxonomyField(fieldCI);


                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldTypeMulti", field.TypeAsString, "Failed to create a TaxonomyField object.");
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
                clientContext.ExecuteQueryRetry();

                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQueryRetry();

                // Create taxonomyfield first
                TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
                {
                    Id = fieldId,
                    DisplayName = fieldName,
                    InternalName = fieldName,
                    Group = "Test Fields Group",
                    TaxonomyItem = termSet
                };
                var field = list.CreateTaxonomyField(fieldCI);

                // Create Item
                ListItemCreationInformation itemCi = new ListItemCreationInformation();

                var item = list.AddItem(itemCi);
                item.Update();
                clientContext.Load(item);
                clientContext.ExecuteQueryRetry();

                item.SetTaxonomyFieldValue(fieldId, _termName, _termId);

                clientContext.Load(item, i => i[fieldName]);
                clientContext.ExecuteQueryRetry();

                var value = item[fieldName] as TaxonomyFieldValue;

                Assert.AreEqual(_termId.ToString(), value.TermGuid, "Term not set correctly");
            }
        }

        [TestMethod()]
        public void CreateTaxonomyFieldLinkedToTermSetTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                clientContext.Load(termSet);
                clientContext.ExecuteQueryRetry();

                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQueryRetry();

                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                var fieldId = Guid.NewGuid();
                TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
                {
                    Id = fieldId,
                    DisplayName = fieldName,
                    InternalName = fieldName,
                    Group = "Test Fields Group",
                    TaxonomyItem = termSet
                };
                var field = list.CreateTaxonomyField(fieldCI);

                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldType", field.TypeAsString, "Failed to create a TaxonomyField object.");
            }
        }

        [TestMethod()]
        public void CreateTaxonomyFieldLinkedToTermTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Retrieve Termset and Term
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termSet = session.GetDefaultSiteCollectionTermStore().GetTermSet(_termSetId);
                var anchorTerm = termSet.GetTerm(_termId);
                clientContext.Load(termSet);
                clientContext.Load(anchorTerm);
                clientContext.ExecuteQueryRetry();

                // Retrieve List
                var list = clientContext.Web.Lists.GetById(_listId);
                clientContext.Load(list);
                clientContext.ExecuteQueryRetry();

                // Create field
                var fieldId = Guid.NewGuid();
                var fieldName = "Test_" + DateTime.Now.ToFileTime();
                TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
                {
                    Id = fieldId,
                    DisplayName = fieldName,
                    InternalName = fieldName,
                    Group = "Test Fields Group",
                    TaxonomyItem = anchorTerm
                };
                var field = list.CreateTaxonomyField(fieldCI);


                Assert.AreEqual(fieldId, field.Id, "Field IDs do not match.");
                Assert.AreEqual(fieldName, field.InternalName, "Field internal names do not match.");
                Assert.AreEqual("TaxonomyFieldType", field.TypeAsString, "Failed to create a TaxonomyField object.");
            }
        }
        #endregion

        #region Get taxonomy object tests
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
        public void GetTermGroupByIdTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                var termGroup = site.GetTermGroupById(_termGroupId);
                Assert.IsInstanceOfType(termGroup, typeof(TermGroup), "Did not return TermGroup object");
                Assert.AreEqual(_termGroupId, termGroup.Id, "Name does not match");
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
        #endregion

        #region Add term tests
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
        public void AddTermToTermsetWithTermIdTest()
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
        #endregion

        #region Import terms tests
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
                clientContext.ExecuteQueryRetry();

                Assert.IsNotNull(term1);
                Assert.IsNotNull(term2);
            }
        }

        [TestMethod()]
        public void ImportTermsToTermStoreTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;

                var termName1 = "Test_Term_1" + DateTime.Now.ToFileTime();
                var termName2 = "Test_Term_2" + DateTime.Now.ToFileTime();

                List<string> termLines = new List<string>();
                termLines.Add(_termGroupName + "|" + _termSetName + "|" + termName1);
                termLines.Add(_termGroupName + "|" + _termSetName + "|" + termName2);

                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = session.GetDefaultSiteCollectionTermStore();
                site.ImportTerms(termLines.ToArray(), 1033, termStore, "|");

                var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                var termGroup = termStore.Groups.GetByName(_termGroupName);
                var termSet = termGroup.TermSets.GetByName(_termSetName);
                var term1 = termSet.Terms.GetByName(termName1);
                var term2 = termSet.Terms.GetByName(termName2);
                clientContext.Load(term1);
                clientContext.Load(term2);
                clientContext.ExecuteQueryRetry();

                Assert.IsNotNull(term1);
                Assert.IsNotNull(term2);
            }
        }

        [TestMethod()]
        public void ImportTermSetSampleShouldCreateSetTest()
        {
            var importSetId = Guid.NewGuid();
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var termGroup = termStore.GetGroup(_termGroupId);

                // Act
                var termSet = termGroup.ImportTermSet(SampleTermSetPath, importSetId);
            }

            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var createdSet = termStore.GetTermSet(importSetId);
                var allTerms = createdSet.GetAllTerms();
                var rootCollection = createdSet.Terms;
                clientContext.Load(createdSet);
                clientContext.Load(allTerms);
                clientContext.Load(rootCollection, ts => ts.Include(t=> t.Name, t => t.Description, t => t.IsAvailableForTagging));
                clientContext.ExecuteQueryRetry();

                Assert.AreEqual("Political Geography", createdSet.Name);
                Assert.AreEqual("A sample term set, describing a simple political geography.", createdSet.Description);
                Assert.IsFalse(createdSet.IsOpenForTermCreation);
                Assert.AreEqual(12, allTerms.Count);

                Assert.AreEqual(1, rootCollection.Count);
                Assert.AreEqual("Continent", rootCollection[0].Name);
                Assert.AreEqual("One of the seven main land masses (Europe, Asia, Africa, North America, South America, Australia, and Antarctica)", rootCollection[0].Description);
                Assert.IsTrue(rootCollection[0].IsAvailableForTagging);
            }
        }

        [TestMethod()]
        public void ImportTermSetShouldUpdateSetTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                clientContext.Load(termStore, s => s.DefaultLanguage);
                clientContext.ExecuteQueryRetry();
                var lcid = termStore.DefaultLanguage;

                var termGroup = termStore.GetGroup(_termGroupId);
                var termSet = termGroup.CreateTermSet("Test Changes", UpdateTermSetId, lcid);
                termSet.Description = "Initial term set description";
                var retain1 = termSet.CreateTerm("Retain1", lcid, Guid.NewGuid());
                retain1.SetDescription("Test of deletes, adds and update", lcid);
                var update2 = retain1.CreateTerm("Update2", lcid, Guid.NewGuid());
                update2.SetDescription("Initial update2 description", lcid);
                var retain3 = update2.CreateTerm("Retain3", lcid, Guid.NewGuid());
                retain3.SetDescription("Test retaining same term", lcid);
                var delete2 = retain1.CreateTerm("Delete2", lcid, Guid.NewGuid());
                delete2.SetDescription("Term to delete", lcid);
                var delete3 = delete2.CreateTerm("Delete3", lcid, Guid.NewGuid());
                delete3.SetDescription("Child term to delete", lcid);
                clientContext.ExecuteQueryRetry();
            }

            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var termGroup = termStore.GetGroup(_termGroupId);

                // Act
                var termSet = termGroup.ImportTermSet(SampleUpdateTermSetPath, UpdateTermSetId, synchroniseDeletions:true, termSetIsOpen:true);
            }

            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var createdSet = termStore.GetTermSet(UpdateTermSetId);
                var allTerms = createdSet.GetAllTerms();
                var rootCollection = createdSet.Terms;
                clientContext.Load(createdSet);
                clientContext.Load(allTerms);
                clientContext.Load(rootCollection, ts => ts.Include(t => t.Name, t => t.Description, t => t.IsAvailableForTagging));
                clientContext.ExecuteQueryRetry();

                Assert.AreEqual("Updated term set description", createdSet.Description);
                Assert.IsTrue(createdSet.IsOpenForTermCreation);
                Assert.AreEqual(6, allTerms.Count);
                Assert.AreEqual(2, rootCollection.Count);

                var retain1Collection = rootCollection.First(t => t.Name == "Retain1").Terms;
                clientContext.Load(retain1Collection, ts => ts.Include(t => t.Name, t => t.Description, t => t.IsAvailableForTagging));
                clientContext.ExecuteQueryRetry();

                Assert.IsTrue(retain1Collection.Any(t => t.Name == "New2"));
                Assert.IsFalse(retain1Collection.Any(t => t.Name == "Delete2"));
                Assert.AreEqual("Changed description", retain1Collection.First(t => t.Name == "Update2").Description);
                Assert.IsFalse(retain1Collection.First(t => t.Name == "Update2").IsAvailableForTagging);
            }
        }

        [TestMethod()]
        public void ImportTermSetShouldUpdateByGuidTest()
        {
            var addedTermId = new Guid("{B564BD6F-21FF-4B60-9474-5E33F726DC6C}");
            var changedTermId = new Guid("{73DF85EE-313C-4485-A7B3-0FC3C17A7454}");

            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                clientContext.Load(termStore, s => s.DefaultLanguage);
                clientContext.ExecuteQueryRetry();
                var lcid = termStore.DefaultLanguage;

                var termGroup = termStore.GetGroup(_termGroupId);
                var termSet = termGroup.CreateTermSet("Test Guids", GuidTermSetId, lcid);
                termSet.Description = "Initial term set description";
                var retain1 = termSet.CreateTerm("Retain1", lcid, Guid.NewGuid());
                retain1.SetDescription("Retained term description", lcid);
                var toUpdate1 = termSet.CreateTerm("ToUpdate1", lcid, changedTermId);
                toUpdate1.SetDescription("Inital term description", lcid);
                clientContext.ExecuteQueryRetry();
            }

            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var termGroup = termStore.GetGroup(_termGroupId);

                // Act
                var termSet = termGroup.ImportTermSet(SampleGuidTermSetPath, Guid.Empty);
            }

            using (var clientContext = TestCommon.CreateClientContext())
            {
                var taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxSession.GetDefaultSiteCollectionTermStore();
                var createdSet = termStore.GetTermSet(GuidTermSetId);
                var rootCollection = createdSet.Terms;
                clientContext.Load(createdSet);
                clientContext.Load(rootCollection, ts => ts.Include(t => t.Name, t => t.Id));
                clientContext.ExecuteQueryRetry();

                Assert.AreEqual("Updated Guids", createdSet.Name);
                Assert.AreEqual("Updated Test Guid term set description", createdSet.Description);
                Assert.AreEqual(3, rootCollection.Count);

                Assert.AreEqual(addedTermId, rootCollection.First(t => t.Name == "Added1").Id);
                Assert.IsTrue(rootCollection.Any(t => t.Name == "Retain1"));
                Assert.IsFalse(rootCollection.Any(t => t.Name == "ToUpdate1"));
                Assert.AreEqual("Changed1", rootCollection.First(t => t.Id == changedTermId).Name);
            }
        }
        #endregion

        #region Export term tests
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
        public void ExportTermSetFromTermstoreTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var site = clientContext.Site;
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = session.GetDefaultSiteCollectionTermStore();

                var lines = site.ExportTermSet(_termSetId, false, termStore);
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
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Tests;
using OfficeDevPnP.Core.Entities;
namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class ListExtensionsTests
    {
        private string _termGroupName; // For easy reference. Set in the Initialize method
        private string _termSetName; // For easy reference. Set in the Initialize method
        private string _termName; // For easy reference. Set in the Initialize method
        private Guid _termGroupId = new Guid("e879befa-2356-49fd-b43e-ba446be72d6c"); // Hardcoded for easier reference in tests
        private Guid _termSetId = new Guid("59ad0849-97b9-4755-a431-2bb9ebc8b66b"); // Hardcoded for easier reference in tests
        private Guid _termId = new Guid("51af0e21-ef8c-4e1f-b897-f677d0938f48");

        private Guid _listId; // For easy reference

        [TestInitialize()]
        public void Initialize()
        {   
            /*** Make sure that the user defined in the App.config has permissions to Manage Terms ***/

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
                var list = clientContext.Web.CreateList(ListTemplateType.DocumentLibrary, "Test_list_" + DateTime.Now.ToFileTime(), false);

                var field = clientContext.Web.Fields.GetByInternalNameOrTitle("TaxKeyword"); // Enterprise Metadata

                list.Fields.Add(field);

                list.Update();
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
        public void CreateListTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var listName = "Test_list_" + DateTime.Now.ToFileTime();

                //Create List
                var web = clientContext.Web;

                web.CreateList(ListTemplateType.GenericList, listName, false);

                //Get List
                var list = web.GetListByTitle(listName);

                Assert.IsNotNull(list);
                Assert.AreEqual(listName, list.Title);
                    
                //Delete List
                list.DeleteObject();
                clientContext.ExecuteQuery();
            }
        }

        [TestMethod()]
        public void AddDefaultColumnValuesTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                List<DefaultColumnValue> defaultValues = new List<DefaultColumnValue>();

                var defaultColumnValue = new DefaultColumnValue();

                defaultColumnValue.FieldInternalName = "TaxKeyword"; // Enterprise metadata field, should be present on the list

                defaultColumnValue.FolderRelativePath = "/"; // Root Folder

                defaultColumnValue.TermPaths.Add(_termGroupName + "|" + _termSetName + "|" + _termName);

                defaultValues.Add(defaultColumnValue);

                var list = clientContext.Web.Lists.GetById(_listId);

                list.AddDefaultColumnValues(defaultValues);

            }
        }

        
    }
}

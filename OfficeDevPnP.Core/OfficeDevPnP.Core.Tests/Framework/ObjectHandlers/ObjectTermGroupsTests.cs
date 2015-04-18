using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;
using Term = OfficeDevPnP.Core.Framework.Provisioning.Model.Term;
using TermGroup = OfficeDevPnP.Core.Framework.Provisioning.Model.TermGroup;
using TermSet = OfficeDevPnP.Core.Framework.Provisioning.Model.TermSet;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectTermGroupsTests
    {

        private Guid _termSetGuid;
        private Guid _termGroupGuid;

        [TestInitialize]
        public void Initialize()
        {
            _termSetGuid = Guid.NewGuid();
            _termGroupGuid = Guid.NewGuid();
        }

        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                try
                {
                    TaxonomySession session = TaxonomySession.GetTaxonomySession(ctx);

                    var store = session.GetDefaultSiteCollectionTermStore();
                    var termSet = store.GetTermSet(_termSetGuid);
                    termSet.DeleteObject();

                    var termGroup = store.GetGroup(_termGroupGuid);
                    termGroup.DeleteObject();
                    store.CommitAll();
                    ctx.ExecuteQueryRetry();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();

            TermGroup termGroup = new TermGroup(_termGroupGuid, "TestProvisioningGroup", null);

            List<TermSet> termSets = new List<TermSet>();

            TermSet termSet = new TermSet(_termSetGuid, "TestProvisioningTermSet", null, null);

            List<Term> terms = new List<Term>();

            var term1 = new Term(Guid.NewGuid(), "TestProvisioningTerm 1", null, null, null, null, null);
            term1.Properties.Add("TestProp1", "Test Value 1");
            term1.LocalProperties.Add("TestLocalProp1", "Test Value 1");
            term1.Labels.Add(new TermLabel() { Language = 1033, Value = "Testing" });

            term1.Terms.Add(new Term(Guid.NewGuid(), "Sub Term 1", null, null, null, null, null));

            terms.Add(term1);

            terms.Add(new Term(Guid.NewGuid(), "TestProvisioningTerm 2", null, null, null, null, null));

            termSet.Terms.AddRange(terms);

            termSets.Add(termSet);

            termGroup.TermSets.AddRange(termSets);

            template.TermGroups.Add(termGroup);

            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser.Initialize(ctx.Web, template);

                new ObjectTermGroups().ProvisionObjects(ctx.Web, template);

                TaxonomySession session = TaxonomySession.GetTaxonomySession(ctx);

                var store = session.GetDefaultSiteCollectionTermStore();

                var group = store.GetGroup(_termGroupGuid);
                var set = store.GetTermSet(_termSetGuid);

                ctx.Load(group);
                ctx.Load(set, ts => ts.Terms);

                ctx.ExecuteQueryRetry();

                Assert.IsInstanceOfType(group, typeof(Microsoft.SharePoint.Client.Taxonomy.TermGroup));
                Assert.IsInstanceOfType(set, typeof (Microsoft.SharePoint.Client.Taxonomy.TermSet));
                Assert.IsTrue(set.Terms.Count == 2);

            }
        }
    }
}

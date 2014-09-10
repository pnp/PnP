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
    public class FeatureExtensionsTests
    {
        private ClientContext clientContext;
        private Guid publishingSiteFeatureId = new Guid("f6924d36-2fa8-4f0b-b16d-06b7250180fa");
        private Guid contentOrganizerWebFeatureId = new Guid("7ad5272a-2694-4349-953e-ea5ef290e97c");

        [TestInitialize()]
        public void Initialize()
        {
            clientContext = TestCommon.CreateClientContext();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            clientContext.Dispose();
        }
        
        [TestMethod()]
        public void ActivateFeatureTest()
        {
            // Test
            clientContext.Site.ActivateFeature(publishingSiteFeatureId);

            Assert.IsTrue(clientContext.Site.IsFeatureActive(publishingSiteFeatureId));

            // Teardown
            clientContext.Site.DeactivateFeature(publishingSiteFeatureId);
            
        }

        [TestMethod()]
        public void ActivateFeatureTest1()
        {
            // Test
            clientContext.Web.ActivateFeature(contentOrganizerWebFeatureId);

            Assert.IsTrue(clientContext.Web.IsFeatureActive(contentOrganizerWebFeatureId));

            // Teardown
            clientContext.Web.DeactivateFeature(contentOrganizerWebFeatureId);
        }

        [TestMethod()]
        public void DeactivateFeatureTest()
        {
            // Setup
            clientContext.Site.ActivateFeature(publishingSiteFeatureId);


            // Test
            clientContext.Site.DeactivateFeature(publishingSiteFeatureId);
            Assert.IsFalse(clientContext.Site.IsFeatureActive(publishingSiteFeatureId));

        }

        [TestMethod()]
        public void DeactivateFeatureTest1()
        {
            // Setup
            clientContext.Web.ActivateFeature(contentOrganizerWebFeatureId);

            // Test
            clientContext.Web.DeactivateFeature(contentOrganizerWebFeatureId);
            Assert.IsFalse(clientContext.Web.IsFeatureActive(contentOrganizerWebFeatureId));
        }

        [TestMethod()]
        public void IsFeatureActiveTest()
        {
            Assert.IsFalse(clientContext.Site.IsFeatureActive(publishingSiteFeatureId));
        }

        [TestMethod()]
        public void IsFeatureActiveTest1()
        {
            Assert.IsFalse(clientContext.Web.IsFeatureActive(contentOrganizerWebFeatureId));
        }
    }
}

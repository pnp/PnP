using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;

namespace OfficeDevPnP.Core.Tests.Framework.ExtensibilityCallOut
{
    [TestClass]
    public class ExtensibilityTests
    {
        private const string TEST_CATEGORY = "Framework Provisioning Extensibility";
        
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanProviderCallOut()
        {
            var _mockProvider = new Provider();
            _mockProvider.Assembly = "OfficeDevPnP.Core.Tests";
            _mockProvider.Type = "OfficeDevPnP.Core.Tests.Framework.ExtensibilityCallOut.ExtensibilityMockProvider";
            _mockProvider.Configuration = ExtensibilityTestConstants.PROVIDER_MOCK_DATA;

            var _mockctx = new ClientContext(ExtensibilityTestConstants.MOCK_URL);
            var _mockTemplate = new ProvisioningTemplate();
            _mockTemplate.ID = ExtensibilityTestConstants.PROVISIONINGTEMPLATE_ID;

            var _em = new ExtensibilityManager();
            _em.ExecuteCallout(_mockctx, _mockProvider, _mockTemplate);

        }
    }
}

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
        private const string TEST_CATEGORY = "Framework Provisioning Extensibility Providers";
        
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
            _mockTemplate.Id = ExtensibilityTestConstants.PROVISIONINGTEMPLATE_ID;

            var _em = new ExtensibilityManager();
            _em.ExecuteExtensibilityCallOut(_mockctx, _mockProvider, _mockTemplate);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [ExpectedException(typeof(ExtensiblityPipelineException))]
        public void ProviderCallOutThrowsException()
        {
            var _mockProvider = new Provider();
            _mockProvider.Assembly = "BLAHASSEMLBY";
            _mockProvider.Type = "BLAHTYPE";
            _mockProvider.Configuration = ExtensibilityTestConstants.PROVIDER_MOCK_DATA;

            var _mockctx = new ClientContext(ExtensibilityTestConstants.MOCK_URL);
            var _mockTemplate = new ProvisioningTemplate();
            _mockTemplate.Id = ExtensibilityTestConstants.PROVISIONINGTEMPLATE_ID;

            var _em = new ExtensibilityManager();
            _em.ExecuteExtensibilityCallOut(_mockctx, _mockProvider, _mockTemplate);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [ExpectedException(typeof(ArgumentException))]
        public void ProviderAssemblyMissingThrowsAgrumentException()
        {
            var _mockProvider = new Provider();
            _mockProvider.Assembly = "";
            _mockProvider.Type = "TYPE";
            _mockProvider.Configuration = ExtensibilityTestConstants.PROVIDER_MOCK_DATA;

            var _mockctx = new ClientContext(ExtensibilityTestConstants.MOCK_URL);
            var _mockTemplate = new ProvisioningTemplate();
            _mockTemplate.Id = ExtensibilityTestConstants.PROVISIONINGTEMPLATE_ID;

            var _em = new ExtensibilityManager();
            _em.ExecuteExtensibilityCallOut(_mockctx, _mockProvider, _mockTemplate);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [ExpectedException(typeof(ArgumentException))]
        public void ProviderTypeNameMissingThrowsAgrumentException()
        {
            var _mockProvider = new Provider();
            _mockProvider.Assembly = "BLAHASSEMBLY";
            _mockProvider.Type = "";
            _mockProvider.Configuration = ExtensibilityTestConstants.PROVIDER_MOCK_DATA;

            var _mockctx = new ClientContext(ExtensibilityTestConstants.MOCK_URL);
            var _mockTemplate = new ProvisioningTemplate();
            _mockTemplate.Id = ExtensibilityTestConstants.PROVISIONINGTEMPLATE_ID;

            var _em = new ExtensibilityManager();
            _em.ExecuteExtensibilityCallOut(_mockctx, _mockProvider, _mockTemplate);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProviderClientCtxIsNullThrowsAgrumentNullException()
        {
            var _mockProvider = new Provider();
            _mockProvider.Assembly = "BLAHASSEMBLY";
            _mockProvider.Type = "BLAH";
            _mockProvider.Configuration = ExtensibilityTestConstants.PROVIDER_MOCK_DATA;

            ClientContext _mockCtx = null;
            var _mockTemplate = new ProvisioningTemplate();
            _mockTemplate.Id = ExtensibilityTestConstants.PROVISIONINGTEMPLATE_ID;

            var _em = new ExtensibilityManager();
            _em.ExecuteExtensibilityCallOut(_mockCtx, _mockProvider, _mockTemplate);
        }
    }
}

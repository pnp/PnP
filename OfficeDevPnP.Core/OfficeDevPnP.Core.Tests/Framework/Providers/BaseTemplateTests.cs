using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace OfficeDevPnP.Core.Tests.Framework.Providers
{
    [TestClass]
    public class BaseTemplateTests
    {

        /// <summary>
        /// This is not a test, merely used to dump the needed template files
        /// </summary>
        [TestMethod]
        [Ignore]
        public void DumpBaseTemplate_STS0()
        {
            using (ClientContext ctx = TestCommon.CreateClientContext())
            {
                using(ClientContext cc = ctx.Clone("https://bertonline.sharepoint.com/sites/templateSTS0"))
                {
                    // Specify null as base template since we do want "everything" in this case
                    ProvisioningTemplateCreationInformation creationInfo = new ProvisioningTemplateCreationInformation(cc.Web);
                    creationInfo.BaseTemplate = null;

                    ProvisioningTemplate p = cc.Web.GetProvisioningTemplate(creationInfo);
                    p.ID = "STS0template";

                    // Cleanup before saving
                    p.Security.AdditionalAdministrators.Clear();


                    XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(".", "");
                    provider.SaveAs(p, "STS0Template.xml");                    
                }
            }
        }

        /// <summary>
        /// Get the base template for the current site
        /// </summary>
        [TestMethod]
        public void GetBaseTemplateForCurrentSiteTest()
        {
            using (ClientContext ctx = TestCommon.CreateClientContext())
            {
                ProvisioningTemplate t = ctx.Web.GetBaseTemplate();

                Assert.IsNotNull(t);
            }
        }

        /// <summary>
        /// Get the template for the current site
        /// </summary>
        [TestMethod]
        public void GetTemplateForCurrentSiteTest()
        {
            using (ClientContext cc = TestCommon.CreateClientContext())
            {
                using (ClientContext ctx = cc.Clone("https://bertonline.sharepoint.com/sites/temp2/s1"))
                {

                    ProvisioningTemplate p = ctx.Web.GetProvisioningTemplate();

                    Assert.IsNotNull(p);

                    // Save to file system
                    XMLFileSystemTemplateProvider xmlProv = new XMLFileSystemTemplateProvider("c:\\temp", "");
                    xmlProv.SaveAs(p, "test.xml");

                    // Verify that the saved XML is still valid
                    var p2 = xmlProv.GetTemplate("test.xml");
                }
            }
        }

        [TestMethod]
        public void ApplyRootSiteProvisioningTemplateToSubSiteTest()
        {
            using (ClientContext cc = TestCommon.CreateClientContext())
            {
                using (ClientContext ctx = cc.Clone("https://bertonline.sharepoint.com/sites/temp2"))
                {
                    ProvisioningTemplateCreationInformation creationInfo = new ProvisioningTemplateCreationInformation(ctx.Web);
                    creationInfo.FileConnector = new FileSystemConnector(@"c:\temp\template\garage", "");
                    creationInfo.PersistComposedLookFiles = true;

                    ProvisioningTemplate p = ctx.Web.GetProvisioningTemplate(creationInfo);
                    Assert.IsNotNull(p);

                    using (ClientContext cc2 = cc.Clone("https://bertonline.sharepoint.com/sites/temp2/s1"))
                    {
                        p.Connector = new FileSystemConnector("./resources", "");
                        cc2.Web.ApplyProvisioningTemplate(p);
                    }                    
                }
            }
        }

        [TestMethod]
        public void IsSubsiteTest()
        {
            using (ClientContext cc = TestCommon.CreateClientContext())
            {
                Assert.IsFalse(cc.Web.IsSubSite());

                using (ClientContext ctx = cc.Clone("https://bertonline.sharepoint.com/sites/temp2/s1"))
                {
                    Assert.IsTrue(ctx.Web.IsSubSite());
                }
            }
        }


    }
}

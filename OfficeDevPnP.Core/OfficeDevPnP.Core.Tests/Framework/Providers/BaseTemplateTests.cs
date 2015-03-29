using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
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
                    ProvisioningTemplate p = cc.Web.GetProvisioningTemplate(null);
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
                using (ClientContext ctx = cc.Clone("https://bertonline.sharepoint.com/sites/dev"))
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
        public void ParseFieldXmlTest()
        {
            string fieldXML = "<Field ID=\"{28081524-7C2F-4f08-9319-9C737B495BC1}\" Name=\"ParentName\" StaticName=\"ParentName\" Group=\"_Hidden\" ShowInNewForm=\"FALSE\" ShowInEditForm=\"FALSE\" ShowInFileDlg=\"FALSE\" Type=\"Text\" DisplayName=\"Report Parent Name\" AuthoringInfo=\"(the name of a snapshot's parent)\" Filterable=\"FALSE\" Sortable=\"TRUE\" SourceID=\"http://schemas.microsoft.com/sharepoint/v3\" />";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(fieldXML);
            var node = doc.DocumentElement.SelectSingleNode("/Field/@ID");

            if (node != null)
            {
                Guid newGuid;
                Assert.IsTrue(Guid.TryParse(node.Value, out newGuid));
            }
            
        }


    }
}

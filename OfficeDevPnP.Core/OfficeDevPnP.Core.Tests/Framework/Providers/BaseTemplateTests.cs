using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}

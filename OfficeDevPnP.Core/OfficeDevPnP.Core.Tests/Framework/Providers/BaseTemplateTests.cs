using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public void DumpBaseTemplates()
        {
            using (ClientContext ctx = TestCommon.CreateClientContext())
            {
                DumpTemplate(ctx, "STS0");
                DumpTemplate(ctx, "BLOG0");
                DumpTemplate(ctx, "BDR0");
                DumpTemplate(ctx, "DEV0");
                DumpTemplate(ctx, "OFFILE1");
#if !CLIENTSDKV15
                DumpTemplate(ctx, "EHS1");
                DumpTemplate(ctx, "BLANKINTERNETCONTAINER0", "", "BLANKINTERNET0");
#else
                DumpTemplate(ctx, "STS1");
                DumpTemplate(ctx, "BLANKINTERNET0");
#endif
                DumpTemplate(ctx, "BICENTERSITE0");
                DumpTemplate(ctx, "SRCHCEN0");
                DumpTemplate(ctx, "BLANKINTERNETCONTAINER0", "CMSPUBLISHING0");
                DumpTemplate(ctx, "ENTERWIKI0");
                DumpTemplate(ctx, "PROJECTSITE0");
                DumpTemplate(ctx, "COMMUNITY0");
                DumpTemplate(ctx, "COMMUNITYPORTAL0");
                DumpTemplate(ctx, "SRCHCENTERLITE0");
                DumpTemplate(ctx, "VISPRUS0");
            }
        }

        private void DumpTemplate(ClientContext ctx, string template, string subSiteTemplate = "", string saveAsTemplate = "")
        {
            Uri devSiteUrl = new Uri(ConfigurationManager.AppSettings["SPODevSiteUrl"]);
            string baseUrl = String.Format("{0}://{1}", devSiteUrl.Scheme, devSiteUrl.DnsSafeHost);

            string siteUrl = "";
            if (subSiteTemplate.Length > 0)
            {
                siteUrl = (String.Format("{1}/sites/template{0}/template{2}", template, baseUrl, subSiteTemplate));
            }
            else
            {
                siteUrl = (String.Format("{1}/sites/template{0}", template, baseUrl));
            }

            using (ClientContext cc = ctx.Clone(siteUrl))
            {
                // Specify null as base template since we do want "everything" in this case
                ProvisioningTemplateCreationInformation creationInfo = new ProvisioningTemplateCreationInformation(cc.Web);
                creationInfo.BaseTemplate = null;

                // Override the save name. Case is online site collection provisioned using blankinternetcontainer#0 which returns
                // blankinternet#0 as web template using CSOM/SSOM API
                if (saveAsTemplate.Length > 0)
                {
                    template = saveAsTemplate;
                }

                ProvisioningTemplate p = cc.Web.GetProvisioningTemplate(creationInfo);
                if (subSiteTemplate.Length > 0)
                {
                    p.Id = String.Format("{0}template", subSiteTemplate);
                }
                else
                {
                    p.Id = String.Format("{0}template", template);
                }

                // Cleanup before saving
                p.Security.AdditionalAdministrators.Clear();


                XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(".", "");
                if (subSiteTemplate.Length > 0)
                {
                    provider.SaveAs(p, String.Format("{0}Template.xml", subSiteTemplate));
                }
                else
                {
                    provider.SaveAs(p, String.Format("{0}Template.xml", template));
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

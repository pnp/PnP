using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Diagnostics;
using System.Xml.Linq;
using Provisioning.Extensibility.Providers.Helpers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;

namespace Provisioning.Extensibility.Providers
{
    public class PublishingPageProvisioningExtensibilityHandler : IProvisioningExtensibilityHandler
    {
        private readonly string logSource = "Provisioning.Extensibility.Providers.PublishingPageProvisioningExtensibilityHandler";
        private ClientContext clientContext;
        private Web web;
        private string configurationXml;

        #region IProvisioningExtensibilityHandler Implementation
        public void Provision(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation, TokenParser tokenParser, PnPMonitoredScope scope, string configurationData)
        {
            Log.Info(
                logSource,
                "ProcessRequest. Template: {0}. Config: {1}",
                template.Id,
                configurationData);

            clientContext = ctx;
            web = ctx.Web;
            configurationXml = configurationData;

            AddPublishingPages();
        }

        public ProvisioningTemplate Extract(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInformation, PnPMonitoredScope scope, string configurationData)
        {
            return template;
        }

        public IEnumerable<TokenDefinition> GetTokens(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            return new List<TokenDefinition>();
        }
        #endregion

        private void SetForceCheckOut(bool disable)
        {
            List pages = web.Lists.GetByTitle("Pages");

            pages.ForceCheckout = disable;
            pages.Update();

            clientContext.ExecuteQuery();
        }

        private void AddPublishingPages()
        {
            List<PublishingPage> pages = GetPublishingPagesListFromConfiguration();

            foreach (var page in pages)
            {
                try
                {
                    SetForceCheckOut(false);

                    PageHelper.AddPublishingPage(page, clientContext, web);

                    SetForceCheckOut(true);
                }
                catch (Exception ex)
                {
                    Log.Error(logSource, "Error adding publishing page: {0}. Exception: {1}", page.FileName, ex.ToString());
                }
            }
        }

        private List<PublishingPage> GetPublishingPagesListFromConfiguration()
        {
            List<PublishingPage> pages = new List<PublishingPage>();

            XNamespace ns = "http://schemas.somecompany.com/PublishingPageProvisioningExtensibilityHandlerConfiguration";
            XDocument doc = XDocument.Parse(configurationXml);

            foreach (var p in doc.Root.Descendants(ns + "Page"))
            {
                PublishingPage page = new PublishingPage
                {
                    Title = p.Attribute("Title").Value,
                    Layout = p.Attribute("Layout").Value,
                    Overwrite = bool.Parse(p.Attribute("Overwrite").Value),
                    FileName = p.Attribute("FileName").Value,
                    Publish = bool.Parse(p.Attribute("Publish").Value)
                };

                if (p.Attribute("WelcomePage") != null)
                {
                    page.WelcomePage = bool.Parse(p.Attribute("WelcomePage").Value);
                }

                var pageContentNode = p.Descendants(ns + "PublishingPageContent").FirstOrDefault();
                if (pageContentNode != null)
                {
                    page.PublishingPageContent = pageContentNode.Attribute("Value").Value;
                }

                foreach (var wp in p.Descendants(ns + "WebPart"))
                {
                    PublishingPageWebPart publishingPageWebPart = new PublishingPageWebPart();

                    if (wp.Attribute("DefaultViewDisplayName") != null)
                    {
                        publishingPageWebPart.DefaultViewDisplayName = wp.Attribute("DefaultViewDisplayName").Value;
                    }

                    publishingPageWebPart.Order = uint.Parse(wp.Attribute("Order").Value);
                    publishingPageWebPart.Title = wp.Attribute("Title").Value;
                    publishingPageWebPart.Zone = wp.Attribute("Zone").Value;

                    string webpartContensts = wp.Element(ns + "Contents").Value;
                    publishingPageWebPart.Contents = webpartContensts.Trim(new[] { '\n', ' ' });

                    page.WebParts.Add(publishingPageWebPart);
                }

                Dictionary<string, string> properties = new Dictionary<string, string>();
                foreach (var property in p.Descendants(ns + "Property"))
                {
                    properties.Add(
                        property.Attribute("Name").Value,
                        property.Attribute("Value").Value);
                }
                page.Properties = properties;

                pages.Add(page);
            }

            return pages;
        }
    }
}

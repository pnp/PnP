using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System.Xml;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectContentType : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var existingCts = web.AvailableContentTypes;
            web.Context.Load(existingCts);
            web.Context.ExecuteQueryRetry();

            foreach (var ct in template.ContentTypes)
            {
                // find the id of the content type
                XDocument document = XDocument.Parse(ct.SchemaXml);
                var contentTypeId = document.Root.Attribute("ID").Value;
                var existingCt = existingCts.FirstOrDefault(c => c.StringId == contentTypeId);
                if (existingCt == null)
                {
                    web.CreateContentTypeFromXMLString(ct.SchemaXml);
                }
            }            
        }

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            var cts = web.ContentTypes;
            web.Context.Load(cts);
            web.Context.ExecuteQueryRetry();

            foreach (var ct in cts)
            {
                if (!BuiltInContentTypeId.Contains(ct.StringId))
                {
                    template.ContentTypes.Add(new Model.ContentType() {SchemaXml = ct.SchemaXml});
                }
            }

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (baseTemplate != null)
            {
                template = CleanupEntities(template, baseTemplate);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            foreach (var ct in baseTemplate.ContentTypes)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ct.SchemaXml);
                var node = doc.DocumentElement.SelectSingleNode("/ContentType/@ID");

                if (node != null)
                {
                    int index = template.ContentTypes.FindIndex(f => f.SchemaXml.IndexOf(node.Value, StringComparison.InvariantCultureIgnoreCase) > -1);

                    if (index > -1)
                    {
                        template.ContentTypes.RemoveAt(index);
                    }
                }
            }

            return template;
        }
    }
}

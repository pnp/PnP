using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectContentType : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            // if this is a sub site then we're not provisioning content types. Technically this can be done but it's not a recommended practice
            if (web.IsSubSite())
            {
                return;
            }

            var existingCts = web.AvailableContentTypes;
            web.Context.Load(existingCts, cts => cts.Include(ct => ct.StringId));
            web.Context.ExecuteQueryRetry();

            var existingCtsIds = existingCts.Select(cts => cts.StringId.ToLower()).ToList();

            foreach (var ct in template.ContentTypes)
            {
                // find the id of the content type
                XDocument document = XDocument.Parse(ct.SchemaXml);
                var contentTypeId = document.Root.Attribute("ID").Value;
                if (!existingCtsIds.Contains(contentTypeId.ToLower()))
                {
                    web.CreateContentTypeFromXMLString(ct.SchemaXml);
                    existingCtsIds.Add(contentTypeId);
                }
            }
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            // if this is a sub site then we're not creating content type entities. 
            if (web.IsSubSite())
            {
                return template;
            }

            var cts = web.ContentTypes;
            web.Context.Load(cts);
            web.Context.ExecuteQueryRetry();

            foreach (var ct in cts)
            {
                if (!BuiltInContentTypeId.Contains(ct.StringId))
                {
                    template.ContentTypes.Add(new ContentType() { SchemaXml = ct.SchemaXml });
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
                XDocument xDoc = XDocument.Parse(ct.SchemaXml);
                var id = xDoc.Root.Attribute("ID") != null ? xDoc.Root.Attribute("ID").Value : null;
                if (id != null)
                {
                    int index = template.ContentTypes.FindIndex(f => f.SchemaXml.IndexOf(id, StringComparison.InvariantCultureIgnoreCase) > -1);

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

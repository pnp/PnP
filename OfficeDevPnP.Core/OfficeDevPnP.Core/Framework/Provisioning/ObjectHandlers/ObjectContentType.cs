using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

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

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
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

            return template;
        }
    }
}

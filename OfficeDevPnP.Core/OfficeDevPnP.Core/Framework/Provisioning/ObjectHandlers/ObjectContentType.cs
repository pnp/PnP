using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    class ObjectContentType : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            foreach (var ct in template.ContentTypes)
            {
                web.CreateContentTypeFromXMLString(ct.SchemaXml);
            }
            
        }

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var cts = web.ContentTypes;
            web.Context.Load(cts);
            web.Context.ExecuteQueryRetry();

            foreach (var ct in cts)
            {
                template.ContentTypes.Add(new Model.ContentType() {SchemaXml = ct.SchemaXml});
            }

            return template;
        }
    }
}

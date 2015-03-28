using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Web.Management;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using Field = OfficeDevPnP.Core.Framework.Provisioning.Model.Field;
using System.Xml;
using OfficeDevPnP.Core.Framework.ObjectHandlers;


namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectField : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var parser = new TokenParser(web);
            var existingFields = web.Fields;

            web.Context.Load(existingFields, fs => fs.Include(f => f.Id));
            var existingFieldIds = existingFields.Select(l => l.Id).ToList();

            web.Context.ExecuteQueryRetry();
            
            var fields = template.SiteFields;

            foreach (var field in fields)
            {
                XDocument document = XDocument.Parse(field.SchemaXml);
                var fieldId = document.Root.Attribute("ID").Value;


                if (!existingFieldIds.Contains(Guid.Parse(fieldId)))
                {
                    var fieldXml = parser.Parse(field.SchemaXml);
                    web.Fields.AddFieldAsXml(fieldXml, false, AddFieldOptions.DefaultValue);
                    web.Context.ExecuteQueryRetry();
                }
            }
        }


        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            var existingFields = web.Fields;
            web.Context.Load(existingFields, fs => fs.Include(f => f.Id, f=>  f.SchemaXml));
            web.Context.ExecuteQueryRetry();

            
            foreach (var field in existingFields)
            {
                if (!BuiltInFieldId.Contains(field.Id))
                {
                    template.SiteFields.Add(new Model.Field() {SchemaXml = field.SchemaXml});
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
            foreach (var field in baseTemplate.SiteFields)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(field.SchemaXml);
                var node = doc.DocumentElement.SelectSingleNode("/Field/@ID");

                if (node != null)
                {
                    int index = template.SiteFields.FindIndex(f => f.SchemaXml.IndexOf(node.Value, StringComparison.InvariantCultureIgnoreCase) > -1);

                    if (index > -1)
                    {
                        template.SiteFields.RemoveAt(index);
                    }
                }
            }

            return template;
        }
    }
}

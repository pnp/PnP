using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Field = OfficeDevPnP.Core.Framework.Provisioning.Model.Field;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectField : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            var parser = new TokenParser(web);
            var existingFields = web.Fields;

            web.Context.Load(existingFields, fs => fs.Include(f => f.Id));
            web.Context.ExecuteQueryRetry();
            var existingFieldIds = existingFields.Select(l => l.Id).ToList();

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


        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            var existingFields = web.Fields;
            web.Context.Load(existingFields, fs => fs.Include(f => f.Id, f => f.SchemaXml));
            web.Context.ExecuteQueryRetry();


            foreach (var field in existingFields)
            {
                if (!BuiltInFieldId.Contains(field.Id))
                {
                    template.SiteFields.Add(new Field() { SchemaXml = field.SchemaXml });
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

                XDocument xDoc = XDocument.Parse(field.SchemaXml);
                var id = xDoc.Root.Attribute("ID") != null ? xDoc.Root.Attribute("ID").Value : null;
                if (id != null)
                {
                    int index = template.SiteFields.FindIndex(f => f.SchemaXml.IndexOf(id, StringComparison.InvariantCultureIgnoreCase) > -1);

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

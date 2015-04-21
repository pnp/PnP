using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Services.Discovery;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;
using Field = OfficeDevPnP.Core.Framework.Provisioning.Model.Field;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectField : ObjectHandlerBase
    {

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, "Fields");

            // if this is a sub site then we're not provisioning fields. Technically this can be done but it's not a recommended practice
            if (web.IsSubSite())
            {
                return;
            }

            var existingFields = web.Fields;

            web.Context.Load(existingFields, fs => fs.Include(f => f.Id));
            web.Context.ExecuteQueryRetry();
            var existingFieldIds = existingFields.Select(l => l.Id).ToList();

            var fields = template.SiteFields;

            foreach (var field in fields)
            {
                XElement fieldElement = XElement.Parse(field.SchemaXml.ToParsedString());
                var fieldId = fieldElement.Attribute("ID").Value;


                if (!existingFieldIds.Contains(Guid.Parse(fieldId)))
                {
                    var listIdentifier = fieldElement.Attribute("List") != null ? fieldElement.Attribute("List").Value : null;

                    if (listIdentifier != null)
                    {
                        // Temporary remove list attribute from list
                        fieldElement.Attribute("List").Remove();
                    }

                    var fieldXml = fieldElement.ToString();

                    web.Fields.AddFieldAsXml(fieldXml, false, AddFieldOptions.DefaultValue);
                }
            }
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // if this is a sub site then we're not creating field entities.
            if (web.IsSubSite())
            {
                return template;
            }

            var existingFields = web.Fields;
            web.Context.Load(web, w => w.ServerRelativeUrl);
            web.Context.Load(existingFields, fs => fs.Include(f => f.Id, f => f.SchemaXml));
            web.Context.ExecuteQueryRetry();


            foreach (var field in existingFields)
            {
                if (!BuiltInFieldId.Contains(field.Id))
                {
                    var fieldXml = field.SchemaXml;
                    XElement element = XElement.Parse(fieldXml);

                    // Check if the field contains a reference to a list. If by Guid, rewrite the value of the attribute to use web relative paths
                    var listIdentifier = element.Attribute("List") != null ? element.Attribute("List").Value : null;
                    if (!string.IsNullOrEmpty(listIdentifier))
                    {
                        var listGuid = Guid.Empty;
                        if (Guid.TryParse(listIdentifier, out listGuid))
                        {
                            var list = web.Lists.GetById(listGuid);
                            web.Context.Load(list, l => l.RootFolder.ServerRelativeUrl);
                            web.Context.ExecuteQueryRetry();

                            var listUrl = list.RootFolder.ServerRelativeUrl.Substring(web.ServerRelativeUrl.Length).TrimStart('/');
                            element.Attribute("List").SetValue(listUrl);
                            fieldXml = element.ToString();
                        }
                    }
                  
                    // Check if we have version attribute. Remove if exists 
                    if (element.Attribute("Version") != null)
                    {
                        element.Attributes("Version").Remove();
                        fieldXml = element.ToString();
                    }
                    template.SiteFields.Add(new Field() { SchemaXml = fieldXml });
                }
            }
            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate);
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


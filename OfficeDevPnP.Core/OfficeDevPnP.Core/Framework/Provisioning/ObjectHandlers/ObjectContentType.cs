using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
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

            var skippedFieldIds = new List<Guid>();
            foreach (var field in template.SiteFields)
            {
                XElement fieldElement = XElement.Parse(field.SchemaXml);
                var id = Guid.Parse(fieldElement.Attribute("ID").Value);
                var listIdentifier = fieldElement.Attribute("List") != null ? fieldElement.Attribute("List").Value : null;
                if (listIdentifier != null)
                {
                    skippedFieldIds.Add(id);
                }
            }

            web.Context.Load(web.ContentTypes, ct => ct.Include(c => c.StringId));
            web.Context.ExecuteQueryRetry();

            foreach (var ct in template.ContentTypes)
            {
                var existingCT = web.ContentTypes.FirstOrDefault(c => c.StringId.Equals(ct.ID, StringComparison.OrdinalIgnoreCase));
                if (existingCT == null)
                {
                    CreateContentType(web, ct, skippedFieldIds);
                }
                else
                {
                    if (ct.Overwrite)
                    {
                        existingCT.DeleteObject();
                        web.Context.ExecuteQueryRetry();
                        CreateContentType(web, ct, skippedFieldIds);
                    }
                }
            }
        }

        private static void CreateContentType(Web web, ContentType ct, List<Guid> skippedFields)
        {
            var name = ct.Name.ToParsedString();
            var description = ct.Description.ToParsedString();
            var id = ct.ID.ToParsedString();
            var group = ct.Group.ToParsedString();

            var createdCT = web.CreateContentType(name, description, id, group);
            foreach (var fieldRef in ct.FieldRefs)
            {
                if (skippedFields.FindIndex(g => g == fieldRef.ID) == -1)
                {
                    var field = web.Fields.GetById(fieldRef.ID);
                    web.AddFieldToContentType(createdCT, field, fieldRef.Required, fieldRef.Hidden);
                }
            }

            createdCT.ReadOnly = ct.ReadOnly;
            createdCT.Hidden = ct.Hidden;
            createdCT.Sealed = ct.Sealed;
            if (!string.IsNullOrEmpty(ct.DocumentTemplate))
            {
                createdCT.DocumentTemplate = ct.DocumentTemplate;
            }

            web.Context.ExecuteQueryRetry();
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // if this is a sub site then we're not creating content type entities. 
            if (web.IsSubSite())
            {
                return template;
            }

            var cts = web.ContentTypes;
            web.Context.Load(cts, ctCollection => ctCollection.IncludeWithDefaultProperties(ct => ct.FieldLinks));
            web.Context.ExecuteQueryRetry();

            foreach (var ct in cts)
            {
                if (!BuiltInContentTypeId.Contains(ct.StringId))
                {
                    //   template.ContentTypes.Add(new ContentType() { SchemaXml = ct.SchemaXml });
                    template.ContentTypes.Add(new ContentType
                        (ct.StringId,
                        ct.Name,
                        ct.Description,
                        ct.Group,
                        ct.Sealed,
                        ct.Hidden,
                        ct.ReadOnly,
                        ct.DocumentTemplate,
                        false,
                            (from fieldLink in ct.FieldLinks
                             select new FieldRef()
                             {
                                 ID = fieldLink.Id,
                                 Hidden = fieldLink.Hidden,
                                 Required = fieldLink.Required,
                             })
                        ));
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
            foreach (var ct in baseTemplate.ContentTypes)
            {
                var index = template.ContentTypes.FindIndex(f => f.ID.Equals(ct.ID, StringComparison.OrdinalIgnoreCase));
                if (index > -1)
                {
                    template.ContentTypes.RemoveAt(index);
                }

            }

            return template;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectContentType : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Content Types"; }
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_ContentTypes);

            // if this is a sub site then we're not provisioning content types. Technically this can be done but it's not a recommended practice
            if (web.IsSubSite())
            {
                return;
            }


            web.Context.Load(web.ContentTypes, ct => ct.IncludeWithDefaultProperties(c => c.StringId, c => c.FieldLinks));
            web.Context.ExecuteQueryRetry();
            var existingCTs = web.ContentTypes.ToList();

            foreach (var ct in template.ContentTypes.OrderBy(ct => ct.Id)) // ordering to handle references to parent content types that can be in the same template
            {
                var existingCT = existingCTs.FirstOrDefault(c => c.StringId.Equals(ct.Id, StringComparison.OrdinalIgnoreCase));
                if (existingCT == null)
                {
                    var newCT = CreateContentType(web, ct);
                    if (newCT != null)
                    {
                        existingCTs.Add(newCT);
                    }

                }
                else
                {
                    if (ct.Overwrite)
                    {
                        existingCT.DeleteObject();
                        web.Context.ExecuteQueryRetry();
                        var newCT = CreateContentType(web, ct);
                        if (newCT != null)
                        {
                            existingCTs.Add(newCT);
                        }
                    }
                    else
                    {
                        UpdateContentType(web, existingCT, ct);
                    }
                }
            }

        }

        private static void UpdateContentType(Web web, Microsoft.SharePoint.Client.ContentType existingContentType, ContentType templateContentType)
        {
            var isDirty = false;
            if (existingContentType.Hidden != templateContentType.Hidden)
            {
                existingContentType.Hidden = templateContentType.Hidden;
                isDirty = true;
            }
            if (existingContentType.ReadOnly != templateContentType.ReadOnly)
            {
                existingContentType.ReadOnly = templateContentType.ReadOnly;
                isDirty = true;
            }
            if (existingContentType.Sealed != templateContentType.Sealed)
            {
                existingContentType.Sealed = templateContentType.Sealed;
                isDirty = true;
            }
            if (templateContentType.Description != null && existingContentType.Description != templateContentType.Description.ToParsedString())
            {
                existingContentType.Description = templateContentType.Description.ToParsedString();
                isDirty = true;
            }
            if (templateContentType.DocumentTemplate != null && existingContentType.DocumentTemplate != templateContentType.DocumentTemplate.ToParsedString())
            {
                existingContentType.DocumentTemplate = templateContentType.DocumentTemplate.ToParsedString();
                isDirty = true;
            }
            if (existingContentType.Name != templateContentType.Name.ToParsedString())
            {
                existingContentType.Name = templateContentType.Name.ToParsedString();
                isDirty = true;
            }
            if (templateContentType.Group != null && existingContentType.Group != templateContentType.Group.ToParsedString())
            {
                existingContentType.Group = templateContentType.Group.ToParsedString();
                isDirty = true;
            }
            if (isDirty)
            {
                existingContentType.Update(true);
                web.Context.ExecuteQueryRetry();
            }
            // Delta handling
            List<Guid> targetIds = existingContentType.FieldLinks.Select(c1 => c1.Id).ToList();
            List<Guid> sourceIds = templateContentType.FieldRefs.Select(c1 => c1.Id).ToList();

            var fieldsNotPresentInTarget = sourceIds.Except(targetIds).ToArray();
            
            if (fieldsNotPresentInTarget.Any())
            {
                foreach (var fieldId in fieldsNotPresentInTarget)
                {
                    var fieldRef = templateContentType.FieldRefs.Find(fr => fr.Id == fieldId);
                    var field = web.Fields.GetById(fieldId);
                    web.AddFieldToContentType(existingContentType, field, fieldRef.Required, fieldRef.Hidden);
                }
            }

            isDirty = false;
            foreach (var fieldId in targetIds.Intersect(sourceIds))
            {
                var fieldLink = existingContentType.FieldLinks.FirstOrDefault(fl => fl.Id == fieldId);
                var fieldRef = templateContentType.FieldRefs.Find(fr => fr.Id == fieldId);
                if (fieldRef != null)
                {
                 
                    if (fieldLink.Required != fieldRef.Required)
                    {
                        fieldLink.Required = fieldRef.Required;
                        isDirty = true;
                    }
                    if (fieldLink.Hidden != fieldRef.Hidden)
                    {
                        fieldLink.Hidden = fieldRef.Hidden;
                        isDirty = true;
                    }
                }
            }
            if (isDirty)
            {
                existingContentType.Update(true);
                web.Context.ExecuteQueryRetry();
            }
        }

        private static Microsoft.SharePoint.Client.ContentType CreateContentType(Web web, ContentType templateContentType)
        {
            var name = templateContentType.Name.ToParsedString();
            var description = templateContentType.Description.ToParsedString();
            var id = templateContentType.Id.ToParsedString();
            var group = templateContentType.Group.ToParsedString();

            var createdCT = web.CreateContentType(name, description, id, group);
            foreach (var fieldRef in templateContentType.FieldRefs)
            {
                var field = web.Fields.GetById(fieldRef.Id);
                web.AddFieldToContentType(createdCT, field, fieldRef.Required, fieldRef.Hidden);
            }

            createdCT.ReadOnly = templateContentType.ReadOnly;
            createdCT.Hidden = templateContentType.Hidden;
            createdCT.Sealed = templateContentType.Sealed;
            if (!string.IsNullOrEmpty(templateContentType.DocumentTemplate.ToParsedString()))
            {
                createdCT.DocumentTemplate = templateContentType.DocumentTemplate.ToParsedString();
            }

            web.Context.Load(createdCT);
            web.Context.ExecuteQueryRetry();

            return createdCT;
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // if this is a sub site then we're not creating content type entities. 
            if (web.IsSubSite())
            {
                return template;
            }

            template.ContentTypes.AddRange(GetEntities(web));

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate);
            }

            return template;
        }

        private IEnumerable<ContentType> GetEntities(Web web)
        {
            var cts = web.ContentTypes;
            web.Context.Load(cts, ctCollection => ctCollection.IncludeWithDefaultProperties(ct => ct.FieldLinks));
            web.Context.ExecuteQueryRetry();

            List<ContentType> ctsToReturn = new List<ContentType>();

            foreach (var ct in cts)
            {
                if (!BuiltInContentTypeId.Contains(ct.StringId))
                {
                    ctsToReturn.Add(new ContentType
                        (ct.StringId,
                        ct.Name,
                        ct.Description,
                        ct.Group,
                        ct.Sealed,
                        ct.Hidden,
                        ct.ReadOnly,
                        ct.DocumentTemplate,
                        false,
                            (from fieldLink in ct.FieldLinks.AsEnumerable<FieldLink>()
                             select new FieldRef(fieldLink.Name)
                             {
                                 Id = fieldLink.Id,
                                 Hidden = fieldLink.Hidden,
                                 Required = fieldLink.Required,
                             })
                        ));
                }
            }
            return ctsToReturn;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            foreach (var ct in baseTemplate.ContentTypes)
            {
                var index = template.ContentTypes.FindIndex(f => f.Id.Equals(ct.Id, StringComparison.OrdinalIgnoreCase));
                if (index > -1)
                {
                    template.ContentTypes.RemoveAt(index);
                }

            }

            return template;
        }

        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.ContentTypes.Any();
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                _willExtract = true;
            }
            return _willExtract.Value;
        }
    }
}

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

        private static void UpdateContentType(Web web, Microsoft.SharePoint.Client.ContentType existingCT, ContentType ct)
        {
            var isDirty = false;
            if (existingCT.Hidden != ct.Hidden)
            {
                existingCT.Hidden = ct.Hidden;
                isDirty = true;
            }
            if (existingCT.ReadOnly != ct.ReadOnly)
            {
                existingCT.ReadOnly = ct.ReadOnly;
                isDirty = true;
            }
            if (existingCT.Sealed != ct.Sealed)
            {
                existingCT.Sealed = ct.Sealed;
                isDirty = true;
            }
            if (ct.Description != null && existingCT.Description != ct.Description)
            {
                existingCT.Description = ct.Description;
                isDirty = true;
            }
            if (ct.DocumentTemplate != null && existingCT.DocumentTemplate != ct.DocumentTemplate)
            {
                existingCT.DocumentTemplate = ct.DocumentTemplate;
                isDirty = true;
            }
            if (existingCT.Name != ct.Name)
            {
                existingCT.Name = ct.Name;
                isDirty = true;
            }
            if (ct.Group != null && existingCT.Group != ct.Group)
            {
                existingCT.Group = ct.Group;
                isDirty = true;
            }
            if (isDirty)
            {
                existingCT.Update(true);
                web.Context.ExecuteQueryRetry();
            }
            // Delta handling
            List<Guid> targetIds = existingCT.FieldLinks.Select(c1 => c1.Id).ToList();
            List<Guid> sourceIds = ct.FieldRefs.Select(c1 => c1.Id).ToList();

            var fieldsNotPresentInTarget = sourceIds.Except(targetIds).ToArray();
            
            if (fieldsNotPresentInTarget.Any())
            {
                foreach (var fieldId in fieldsNotPresentInTarget)
                {
                    var fieldRef = ct.FieldRefs.Find(fr => fr.Id == fieldId);
                    var field = web.Fields.GetById(fieldId);
                    web.AddFieldToContentType(existingCT, field, fieldRef.Required, fieldRef.Hidden);
                }
            }

            isDirty = false;
            foreach (var fieldId in targetIds.Intersect(sourceIds))
            {
                var fieldLink = existingCT.FieldLinks.FirstOrDefault(fl => fl.Id == fieldId);
                var fieldRef = ct.FieldRefs.Find(fr => fr.Id == fieldId);
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
                existingCT.Update(true);
                web.Context.ExecuteQueryRetry();
            }
        }

        private static Microsoft.SharePoint.Client.ContentType CreateContentType(Web web, ContentType ct)
        {
            var name = ct.Name.ToParsedString();
            var description = ct.Description.ToParsedString();
            var id = ct.Id.ToParsedString();
            var group = ct.Group.ToParsedString();

            var createdCT = web.CreateContentType(name, description, id, group);
            foreach (var fieldRef in ct.FieldRefs)
            {
                var field = web.Fields.GetById(fieldRef.Id);
                web.AddFieldToContentType(createdCT, field, fieldRef.Required, fieldRef.Hidden);
            }

            createdCT.ReadOnly = ct.ReadOnly;
            createdCT.Hidden = ct.Hidden;
            createdCT.Sealed = ct.Sealed;
            if (!string.IsNullOrEmpty(ct.DocumentTemplate))
            {
                createdCT.DocumentTemplate = ct.DocumentTemplate;
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

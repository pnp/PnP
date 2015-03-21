using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    public static partial class ProvisioningTemplateExtensions
    {
        public static SharePointProvisioningTemplate ToXml(this Model.ProvisioningTemplate template)
        {
            SharePointProvisioningTemplate result = new SharePointProvisioningTemplate();
            return (result);
        }

        public static Model.ProvisioningTemplate ToProvisioningTemplate(this SharePointProvisioningTemplate template)
        {
            Model.ProvisioningTemplate result = new Model.ProvisioningTemplate();

            // Translate basic properties
            result.ID = template.ID;
            result.Version = Double.Parse(!String.IsNullOrEmpty(template.Version) ? template.Version : "0");
            result.SitePolicy = template.SitePolicy;

            // Translate PropertyBagEntries, if any
            if (template.PropertyBagEntries != null)
            {
                result.PropertyBagEntries.AddRange(
                    from bag in template.PropertyBagEntries
                    select new Model.PropertyBagEntry
                    {
                        Key = bag.Key,
                        Value = bag.Value,
                    });
            }

            // Translate Security configuration, if any
            if (template.Security != null)
            {
                if (template.Security.AdditionalAdministrators != null)
                {
                    result.Security.AdditionalAdministrators.AddRange(
                    from user in template.Security.AdditionalAdministrators
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
                if (template.Security.AdditionalOwners != null)
                {
                    result.Security.AdditionalOwners.AddRange(
                    from user in template.Security.AdditionalOwners
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
                if (template.Security.AdditionalMembers != null)
                {
                    result.Security.AdditionalMembers.AddRange(
                    from user in template.Security.AdditionalMembers
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
                if (template.Security.AdditionalVisitors != null)
                {
                    result.Security.AdditionalVisitors.AddRange(
                    from user in template.Security.AdditionalVisitors
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
            }

            // Translate Site Columns (Fields), if any
            if ((template.SiteFields != null) && (template.SiteFields.Any != null))
            {
                result.SiteFields.AddRange(
                    from field in template.SiteFields.Any
                    select new Model.Field { 
                        SchemaXml = field.OuterXml,
                    });
            }

            // Translate ContentTypes, if any
            if ((template.ContentTypes != null) && (template.ContentTypes.Any != null))
            {
                result.ContentTypes.AddRange(
                    from contentType in template.ContentTypes.Any
                    select new Model.ContentType
                    {
                        SchemaXml = contentType.OuterXml,
                    });
            }

            // Translate Lists Instances, if any
            if (template.Lists != null)
            {
                result.Lists.AddRange(
                    from list in template.Lists
                    select new Model.ListInstance(
                        (list.ContentTypeBindings != null ? 
                                (from contentTypeBinding in list.ContentTypeBindings
                                select new Model.ContentTypeBinding {
                                    ContentTypeID = contentTypeBinding.ContentTypeID,
                                    Default = contentTypeBinding.Default,
                                }) : null))
                    {
                        Description = list.Description,
                        DocumentTemplate = list.DocumentTemplate,
                        EnableVersioning = list.EnableVersioning,
                        OnQuickLaunch = list.OnQuickLaunch,
                        RemoveDefaultContentType = list.RemoveDefaultContentType,
                        TemplateType = list.TemplateType,
                        Title = list.Title,
                        Url = list.Url,
                    });
            }

            // Translate Features, if any
            if (template.Features != null)
            {
                if (result.Features.SiteFeatures != null)
                {
                    result.Features.SiteFeatures.AddRange(
                        from feature in template.Features.SiteFeatures
                        select new Model.Feature
                        {
                            ID = new Guid(feature.ID),
                            Deactivate = feature.Deactivate,
                        });
                }
                if (result.Features.WebFeatures != null)
                {
                    result.Features.WebFeatures.AddRange(
                        from feature in template.Features.WebFeatures
                        select new Model.Feature
                        {
                            ID = new Guid(feature.ID),
                            Deactivate = feature.Deactivate,
                        });
                }
            }

            // Translate CustomActions, if any
            if (template.CustomActions != null)
            {
                if (result.CustomActions.SiteCustomActions != null)
                {
                    result.CustomActions.SiteCustomActions.AddRange(
                        from customAction in template.CustomActions.SiteCustomActions
                        select new Model.CustomAction
                        {
                            Description = customAction.Description,
                            Enabled = customAction.Enabled,
                            Group = customAction.Group,
                            ImageUrl = customAction.ImageUrl,
                            Location = customAction.Location,
                            Name = customAction.Name,
                            RightsValue = customAction.RightsSpecified ? customAction.Rights : 0,
                            ScriptBlock = customAction.ScriptBlock,
                            ScriptSrc = customAction.ScriptSrc,
                            Sequence = customAction.SequenceSpecified ? customAction.Sequence : 100,
                            Title = customAction.Title,
                            Url = customAction.Url,
                        });
                }
                if (result.CustomActions.WebCustomActions != null)
                {
                    result.CustomActions.WebCustomActions.AddRange(
                        from customAction in template.CustomActions.WebCustomActions
                        select new Model.CustomAction
                        {
                            Description = customAction.Description,
                            Enabled = customAction.Enabled,
                            Group = customAction.Group,
                            ImageUrl = customAction.ImageUrl,
                            Location = customAction.Location,
                            Name = customAction.Name,
                            RightsValue = customAction.RightsSpecified ? customAction.Rights : 0,
                            ScriptBlock = customAction.ScriptBlock,
                            ScriptSrc = customAction.ScriptSrc,
                            Sequence = customAction.SequenceSpecified ? customAction.Sequence : 100,
                            Title = customAction.Title,
                            Url = customAction.Url,
                        });
                }
            }

            // Translate Files, if any
            if (template.Files != null)
            {
                result.Files.AddRange(
                    from file in template.Files
                    select new Model.File { 
                        Overwrite = file.Overwrite,
                        Src = file.Src,
                        TargetFolder = file.TargetFolder,
                    });
            }

            // Translate ComposedLook, if any
            if (template.ComposedLook != null)
            {
                result.ComposedLook.AlternateCSS = template.ComposedLook.AlternateCSS;
                result.ComposedLook.BackgroundFile = template.ComposedLook.BackgroundFile;
                result.ComposedLook.ColorFile = template.ComposedLook.ColorFile;
                result.ComposedLook.FontFile = template.ComposedLook.FontFile;
                result.ComposedLook.MasterPage = template.ComposedLook.MasterPage;
                result.ComposedLook.Name = template.ComposedLook.Name;
                result.ComposedLook.SiteLogo = template.ComposedLook.SiteLogo;
                result.ComposedLook.Version = template.ComposedLook.Version;
            }

            // Translate Providers, if any
            if (template.Providers != null)
            {
                result.Providers.AddRange(
                    from provider in template.Providers
                    select new Model.Provider { 
                        Assembly = provider.Assembly,
                        Configuration = provider.Configuration.OuterXml,
                        Enabled = provider.Enabled,
                        Type = provider.Type,
                    });
            }

            return (result);
        }
    }
}

using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Model = OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    public static partial class SharePointProvisioningTemplateExtensions
    {
        public static SharePointProvisioningTemplate ToXml(this Model.ProvisioningTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            SharePointProvisioningTemplate result = new SharePointProvisioningTemplate();

            // Translate basic properties
            result.ID = template.ID;
            result.Version = template.Version.ToString("###0.0", new System.Globalization.CultureInfo("en-US"));
            result.SitePolicy = template.SitePolicy;

            // Translate PropertyBagEntries, if any
            if (template.PropertyBagEntries != null)
            {
                result.PropertyBagEntries =
                    (from bag in template.PropertyBagEntries
                     select new PropertyBagEntry
                     {
                         Key = bag.Key,
                         Value = bag.Value,
                     }).ToArray();
            }

            // Translate Security configuration, if any
            if (template.Security != null)
            {
                result.Security = new SharePointProvisioningTemplateSecurity();

                if (template.Security.AdditionalAdministrators != null)
                {
                    result.Security.AdditionalAdministrators =
                        (from user in template.Security.AdditionalAdministrators
                         select new User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
                if (template.Security.AdditionalOwners != null)
                {
                    result.Security.AdditionalOwners =
                        (from user in template.Security.AdditionalOwners
                         select new User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
                if (template.Security.AdditionalMembers != null)
                {
                    result.Security.AdditionalMembers =
                        (from user in template.Security.AdditionalMembers
                         select new User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
                if (template.Security.AdditionalVisitors != null)
                {
                    result.Security.AdditionalVisitors =
                        (from user in template.Security.AdditionalVisitors
                         select new User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
            }

            // Translate Site Columns (Fields), if any
            if (template.SiteFields != null)
            {
                result.SiteFields = new SharePointProvisioningTemplateSiteFields
                {
                    Any =
                        (from field in template.SiteFields
                         select field.SchemaXml.ToXmlElement()).ToArray(),
                };
            }

            // Translate ContentTypes, if any
            if (template.ContentTypes != null)
            {
                result.ContentTypes = new SharePointProvisioningTemplateContentTypes
                {
                    Any =
                        (from contentType in template.ContentTypes
                         select contentType.SchemaXml.ToXmlElement()).ToArray(),
                };
            }

            // Translate Lists Instances, if any
            if (template.Lists != null)
            {
                result.Lists =
                    (from list in template.Lists
                     select new ListInstance
                     {
                         ContentTypesEnabled = list.ContentTypesEnabled,
                         Description = list.Description,
                         DocumentTemplate = list.DocumentTemplate,
                         EnableVersioning = list.EnableVersioning,
                         Hidden = list.Hidden,
                         MinorVersionLimit = list.MinorVersionLimit,
                         MaxVersionLimit = list.MaxVersionLimit,
                         OnQuickLaunch = list.OnQuickLaunch,
                         RemoveDefaultContentType = list.RemoveDefaultContentType,
                         TemplateType = list.TemplateType,
                         Title = list.Title,
                         Url = list.Url,
                         ContentTypeBindings = list.ContentTypeBindings.Count > 0 ?
                            (from contentTypeBinding in list.ContentTypeBindings
                             select new ContentTypeBinding
                             {
                                 ContentTypeID = contentTypeBinding.ContentTypeID,
                                 Default = contentTypeBinding.Default,
                             }).ToArray() : null,
                         Views = list.Views.Count > 0 ?
                         new ListInstanceViews
                         {
                             Any =
                                (from view in list.Views
                                 select view.SchemaXml.ToXmlElement()).ToArray(),
                         } : null,
                     }).ToArray();
            }

            // Translate Features, if any
            if (template.Features != null)
            {
                result.Features = new SharePointProvisioningTemplateFeatures();

                if (template.Features.SiteFeatures != null)
                {
                    result.Features.SiteFeatures =
                        (from feature in template.Features.SiteFeatures
                         select new Feature
                         {
                             ID = feature.ID.ToString(),
                             Deactivate = feature.Deactivate,
                         }).ToArray();
                }
                if (template.Features.WebFeatures != null)
                {
                    result.Features.WebFeatures =
                        (from feature in template.Features.WebFeatures
                         select new Feature
                         {
                             ID = feature.ID.ToString(),
                             Deactivate = feature.Deactivate,
                         }).ToArray();
                }
            }

            // Translate CustomActions, if any
            if (template.CustomActions != null)
            {
                result.CustomActions = new SharePointProvisioningTemplateCustomActions();

                if (template.CustomActions.SiteCustomActions != null)
                {
                    result.CustomActions.SiteCustomActions =
                        (from customAction in template.CustomActions.SiteCustomActions
                         select new CustomAction
                         {
                             Description = customAction.Description,
                             Enabled = customAction.Enabled,
                             Group = customAction.Group,
                             ImageUrl = customAction.ImageUrl,
                             Location = customAction.Location,
                             Name = customAction.Name,
                             Rights = customAction.RightsValue,
                             RightsSpecified = true,
                             ScriptBlock = customAction.ScriptBlock,
                             ScriptSrc = customAction.ScriptSrc,
                             Sequence = customAction.Sequence,
                             SequenceSpecified = true,
                             Title = customAction.Title,
                             Url = customAction.Url,
                         }).ToArray();
                }
                if (template.CustomActions.WebCustomActions != null)
                {
                    result.CustomActions.WebCustomActions =
                        (from customAction in template.CustomActions.WebCustomActions
                         select new CustomAction
                         {
                             Description = customAction.Description,
                             Enabled = customAction.Enabled,
                             Group = customAction.Group,
                             ImageUrl = customAction.ImageUrl,
                             Location = customAction.Location,
                             Name = customAction.Name,
                             Rights = customAction.RightsValue,
                             RightsSpecified = true,
                             ScriptBlock = customAction.ScriptBlock,
                             ScriptSrc = customAction.ScriptSrc,
                             Sequence = customAction.Sequence,
                             SequenceSpecified = true,
                             Title = customAction.Title,
                             Url = customAction.Url,
                         }).ToArray();
                }
            }

            // Translate Files, if any
            if (template.Files != null)
            {
                result.Files =
                    (from file in template.Files
                     select new File
                     {
                         Overwrite = file.Overwrite,
                         Src = file.Src,
                         Folder = file.Folder,
                     }).ToArray();
            }

            // Translate ComposedLook, if any
            if (template.ComposedLook != null)
            {
                result.ComposedLook = new ComposedLook
                {
                    AlternateCSS = template.ComposedLook.AlternateCSS,
                    BackgroundFile = template.ComposedLook.BackgroundFile,
                    ColorFile = template.ComposedLook.ColorFile,
                    FontFile = template.ComposedLook.FontFile,
                    MasterPage = template.ComposedLook.MasterPage,
                    Name = template.ComposedLook.Name,
                    SiteLogo = template.ComposedLook.SiteLogo,
                    Version = template.ComposedLook.Version,
                    VersionSpecified = true,
                };
            }

            // Translate Providers, if any
            if (template.Providers != null)
            {
                result.Providers =
                    (from provider in template.Providers
                     select new Provider
                     {
                         Assembly = provider.Assembly,
                         Configuration = provider.Configuration != null ? provider.Configuration.ToXmlNode() : null,
                         Enabled = provider.Enabled,
                         Type = provider.Type,
                     }).ToArray();
            }

            return (result);
        }

        public static Model.ProvisioningTemplate ToProvisioningTemplate(this SharePointProvisioningTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            // Check the provided template against the XML schema
            if (!template.IsValidSharePointProvisioningTemplate())
            {
                // TODO: Use resource file
                throw new ApplicationException("The provided template is not valid!");
            }

            Model.ProvisioningTemplate result = new Model.ProvisioningTemplate();

            // Translate basic properties
            result.ID = template.ID;
            result.Version = Double.Parse(!String.IsNullOrEmpty(template.Version) ? template.Version : "0", new System.Globalization.CultureInfo("en-US"));
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
                    select new Model.Field
                    {
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
                                 select new Model.ContentTypeBinding
                                 {
                                     ContentTypeID = contentTypeBinding.ContentTypeID,
                                     Default = contentTypeBinding.Default,
                                 }) : null),
                        (list.Views != null ?
                                (from view in list.Views.Any
                                 select new Model.View
                                 {
                                     SchemaXml = view.OuterXml,
                                 }) : null))
                    {
                        ContentTypesEnabled = list.ContentTypesEnabled,
                        Description = list.Description,
                        DocumentTemplate = list.DocumentTemplate,
                        EnableVersioning = list.EnableVersioning,
                        Hidden = list.Hidden,
                        MinorVersionLimit = list.MinorVersionLimit,
                        MaxVersionLimit = list.MaxVersionLimit,
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
                if (result.Features.SiteFeatures != null && template.Features.SiteFeatures != null)
                {
                    result.Features.SiteFeatures.AddRange(
                        from feature in template.Features.SiteFeatures
                        select new Model.Feature
                        {
                            ID = new Guid(feature.ID),
                            Deactivate = feature.Deactivate,
                        });
                }
                if (result.Features.WebFeatures != null && template.Features.WebFeatures != null)
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
                    select new Model.File
                    {
                        Overwrite = file.Overwrite,
                        Src = file.Src,
                        Folder = file.Folder,
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
                    select new Model.Provider
                    {
                        Assembly = provider.Assembly,
                        Configuration = provider.Configuration != null ? provider.Configuration.ToProviderConfiguration() : null,
                        Enabled = provider.Enabled,
                        Type = provider.Type,
                    });
            }

            return (result);
        }

        public static Boolean IsValidSharePointProvisioningTemplate(this XDocument xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            // Load the XSD embedded resource
            Stream stream = typeof(SharePointProvisioningTemplateExtensions)
                .Assembly
                .GetManifestResourceStream("OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml.ProvisioningSchema-2015-03.xsd");

            // Prepare the XML Schema Set
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(XMLConstants.PROVISIONING_SCHEMA_NAMESPACE,
                new XmlTextReader(stream));

            Boolean result = true;
            xml.Validate(schemas, (o, e) =>
            {
                result = false;
            });

            return (result);
        }

        public static Boolean IsValidSharePointProvisioningTemplate(this SharePointProvisioningTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            // Serialize the template into an XML string
            String xml = XMLSerializer.Serialize<SharePointProvisioningTemplate>(template);
            XDocument doc = XDocument.Parse(xml);

            return (doc.IsValidSharePointProvisioningTemplate());
        }

        #region Private extension methods for handling XML content

        /// <summary>
        /// Private extension method to convert an XElement into an XmlElement
        /// </summary>
        /// <param name="element">The XElement to convert</param>
        /// <returns>The converted XmlElement</returns>
        private static XmlElement ToXmlElement(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            using (XmlReader reader = element.CreateReader())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                return (doc.DocumentElement);
            }
        }

        /// <summary>
        /// Private extension method to convert an XmlElement into an XElement
        /// </summary>
        /// <param name="element">The XmlElement to convert</param>
        /// <returns>The converted XElement</returns>
        private static XElement ToXElement(this XmlElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            using (XmlReader reader = new XmlNodeReader(element))
            {
                XElement result = XElement.Load(reader);
                return (result);
            }
        }

        /// <summary>
        /// Private extension method to convert a String into an XElement
        /// </summary>
        /// <param name="element">The String to convert</param>
        /// <returns>The converted XElement</returns>
        private static XElement ToXElement(this String xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            XElement element = XElement.Parse(xml);
            return (element);
        }

        /// <summary>
        /// Private extension method to convert a String into an XmlElement
        /// </summary>
        /// <param name="element">The String to convert</param>
        /// <returns>The converted XmlElement</returns>
        private static XmlElement ToXmlElement(this String xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            XElement element = XElement.Parse(xml);
            return (element.ToXmlElement());
        }

        /// <summary>
        /// Private extension method to convert a String into an XmlNode
        /// </summary>
        /// <param name="element">The String to convert</param>
        /// <returns>The converted XmlNode</returns>
        private static XmlNode ToXmlNode(this String xml)
        {
            if (String.IsNullOrEmpty(xml))
            {
                throw new ArgumentException("xml");
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                return (doc.DocumentElement);
            }
            catch (XmlException)
            {
                XmlDocument doc = new XmlDocument();
                return (doc.CreateCDataSection(xml));
            }
        }

        private static String ToProviderConfiguration(this XmlNode xml)
        {
            switch (xml.NodeType)
            {
                case XmlNodeType.CDATA:
                    return (((XmlCDataSection)xml).InnerText);
                default:
                    return (xml.OuterXml);
            }
        }

        private static XmlElement FixupElementNamespace(this XmlElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            XElement xElement = XElement.Parse(element.OuterXml);
            XElement cleanedElement = new XElement(xElement.Name.LocalName,
                from a in xElement.Attributes()
                where a.IsNamespaceDeclaration == false
                select a);
            return (cleanedElement.ToXmlElement());
        }

        #endregion
    }
}

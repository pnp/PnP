using System;
using System.Web.Script.Serialization;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class SiteToTemplateConversion
    {
        /// <summary>
        /// Actual implementation of extracting configuration from existing site.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="baseTemplate"></param>
        /// <returns></returns>
        internal ProvisioningTemplate GetRemoteTemplate(Web web, ProvisioningTemplateCreationInformation creationInfo)
        {
            // Create empty object
            ProvisioningTemplate template = new ProvisioningTemplate();

            // Hookup connector, is handy when the generated template object is used to apply to another site
            template.Connector = creationInfo.FileConnector;

            // Get Security
            template = new ObjectSiteSecurity().CreateEntities(web, template, creationInfo);
            // Site Fields
            template = new ObjectField().CreateEntities(web, template, creationInfo);
            // Content Types
            template = new ObjectContentType().CreateEntities(web, template, creationInfo);
            // Get Lists 
            template = new ObjectListInstance().CreateEntities(web, template, creationInfo);
            // Get custom actions
            template = new ObjectCustomActions().CreateEntities(web, template, creationInfo);
            // Get features
            template = new ObjectFeatures().CreateEntities(web, template, creationInfo);
            // Get composite look
            template = new ObjectComposedLook().CreateEntities(web, template, creationInfo);
            // Get files
            template = new ObjectFiles().CreateEntities(web, template, creationInfo);
            // Get Property Bag Entries
            template = new ObjectPropertyBagEntry().CreateEntities(web, template, creationInfo);
            // In future we could just instantiate all objects which are inherited from object handler base dynamically 

            // Set default values for Template ID and Version
            template.ID = String.Format("TEMPLATE-{0:N}", Guid.NewGuid()).ToUpper();
            template.Version = 1;

            // Retrieve original Template ID and remove it from Property Bag Entries
            int provisioningTemplateIdIndex = template.PropertyBagEntries.FindIndex(f => f.Key.Equals("_PnP_ProvisioningTemplateId"));
            if (provisioningTemplateIdIndex > -1)
            {
                var templateId = template.PropertyBagEntries[provisioningTemplateIdIndex].Value;
                if (!String.IsNullOrEmpty(templateId))
                {
                    template.ID = templateId;
                }
                template.PropertyBagEntries.RemoveAt(provisioningTemplateIdIndex);
            }

            // Retrieve original Template Info and remove it from Property Bag Entries
            int provisioningTemplateInfoIndex = template.PropertyBagEntries.FindIndex(f => f.Key.Equals("_PnP_ProvisioningTemplateInfo"));
            if (provisioningTemplateInfoIndex > -1)
            {
                var jsonInfo = template.PropertyBagEntries[provisioningTemplateInfoIndex].Value;

                var s = new JavaScriptSerializer();
                ProvisioningTemplateInfo info = s.Deserialize<ProvisioningTemplateInfo>(jsonInfo);

                // Override any previously defined Template ID, Version, and SitePolicy
                // with the one stored in the Template Info, if any
                if (!String.IsNullOrEmpty(info.TemplateID))
                {
                    template.ID = info.TemplateID;
                }
                if (!String.IsNullOrEmpty(info.TemplateSitePolicy))
                {
                    template.SitePolicy = info.TemplateSitePolicy;
                }
                if (info.TemplateVersion > 0)
                {
                    template.Version = info.TemplateVersion;
                }

                template.PropertyBagEntries.RemoveAt(provisioningTemplateInfoIndex);
            }

            return template;
        }

        /// <summary>
        /// Actual implementation of the apply templates
        /// </summary>
        /// <param name="web"></param>
        /// <param name="template"></param>
        internal void ApplyRemoteTemplate(Web web, ProvisioningTemplate template)
        {
            // Site Security
            new ObjectSiteSecurity().ProvisionObjects(web, template);

            // Features
            new ObjectFeatures().ProvisionObjects(web, template);

            // Site Fields
            new ObjectField().ProvisionObjects(web, template);

            // Content Types
            new ObjectContentType().ProvisionObjects(web, template);

            // Lists
            new ObjectListInstance().ProvisionObjects(web, template);

            // Files
            new ObjectFiles().ProvisionObjects(web, template);

            // Custom actions
            new ObjectCustomActions().ProvisionObjects(web, template);

            // Composite look 
            new ObjectComposedLook().ProvisionObjects(web, template);

            // Property Bag Entries
            new ObjectPropertyBagEntry().ProvisionObjects(web, template);

            // Extensibility Provider CallOut the last thing we do.
            new ObjectExtensibilityProviders().ProvisionObjects(web, template);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateId", template.ID != null ? template.ID : "");
            web.AddIndexedPropertyBagKey("_PnP_ProvisioningTemplateId");

            ProvisioningTemplateInfo info = new ProvisioningTemplateInfo();
            info.TemplateID = template.ID != null ? template.ID : "";
            info.TemplateVersion = template.Version;
            info.TemplateSitePolicy = template.SitePolicy;
            info.Result = true;
            info.ProvisioningTime = DateTime.Now;

            var s = new JavaScriptSerializer();
            string jsonInfo = s.Serialize(info);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateInfo", jsonInfo);
        }
    }
}

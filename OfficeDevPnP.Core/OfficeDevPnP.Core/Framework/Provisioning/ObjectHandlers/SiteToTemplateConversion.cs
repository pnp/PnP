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
            // Get Property Bag Entires
            template = new ObjectPropertyBagEntry().CreateEntities(web, template, creationInfo);
            // In future we could just instantiate all objects which are inherited from object handler base dynamically 

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
            info.Result = true;
            info.ProvisioningTime = DateTime.Now;

            var s = new JavaScriptSerializer();
            string jsonInfo = s.Serialize(info);

            web.SetPropertyBagValue("_PnP_ProvisioningTemplateInfo",jsonInfo);
        }
    }
}

using System;
using System.Web.Script.Serialization;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectRetrieveTemplateInfo : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Retrieve Template Info"; }
        }

        public ObjectRetrieveTemplateInfo()
        {
            this.ReportProgress = false;
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
           
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // Set default values for Template ID and Version
            template.Id = String.Format("TEMPLATE-{0:N}", Guid.NewGuid()).ToUpper();
            template.Version = 1;

            // Retrieve original Template ID and remove it from Property Bag Entries
            int provisioningTemplateIdIndex = template.PropertyBagEntries.FindIndex(f => f.Key.Equals("_PnP_ProvisioningTemplateId"));
            if (provisioningTemplateIdIndex > -1)
            {
                var templateId = template.PropertyBagEntries[provisioningTemplateIdIndex].Value;
                if (!String.IsNullOrEmpty(templateId))
                {
                    template.Id = templateId;
                }
                template.PropertyBagEntries.RemoveAt(provisioningTemplateIdIndex);
            }

            // Retrieve original Template Info and remove it from Property Bag Entries
            int provisioningTemplateInfoIndex = template.PropertyBagEntries.FindIndex(f => f.Key.Equals("_PnP_ProvisioningTemplateInfo"));
            if (provisioningTemplateInfoIndex > -1)
            {
                var jsonInfo = template.PropertyBagEntries[provisioningTemplateInfoIndex].Value;
                jsonInfo = jsonInfo.Replace("TemplateID", "TemplateId"); // TemplateID changed into TemplateId, doing a replace for compatiblity of older template information.
                var s = new JavaScriptSerializer();
                ProvisioningTemplateInfo info = s.Deserialize<ProvisioningTemplateInfo>(jsonInfo);

                // Override any previously defined Template ID, Version, and SitePolicy
                // with the one stored in the Template Info, if any
                if (!String.IsNullOrEmpty(info.TemplateId))
                {
                    template.Id = info.TemplateId;
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
    }
}

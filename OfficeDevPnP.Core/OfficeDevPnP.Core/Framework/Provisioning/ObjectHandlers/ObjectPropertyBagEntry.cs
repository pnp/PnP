using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectPropertyBagEntry : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
           
            foreach (var propbagEntry in template.PropertyBagEntries)
            {
                if (!web.PropertyBagContainsKey(propbagEntry.Key))
                {
                    web.SetPropertyBagValue(propbagEntry.Key,propbagEntry.Value);

                }
            }
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            web.Context.Load(web, w => w.AllProperties);
            web.Context.ExecuteQueryRetry();

            var entries = new List<PropertyBagEntry>();

            foreach (var propbagEntry in web.AllProperties.FieldValues)
            {
                entries.Add(new PropertyBagEntry() {Key = propbagEntry.Key, Value = propbagEntry.Value.ToString()});
            }

            template.PropertyBagEntries.Clear();
            template.PropertyBagEntries.AddRange(entries);

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            foreach (var propertyBagEntry in baseTemplate.PropertyBagEntries)
            {
                int index = template.PropertyBagEntries.FindIndex(f => f.Key.Equals(propertyBagEntry.Key));

                if (index > -1)
                {
                    template.PropertyBagEntries.RemoveAt(index);
                }
            }

            // Scan for "system" properties that should be removed as well
            List<string> systemPropertyBagEntries = new List<string>(new string[] 
            { "dlc_ExpirationLastRun", "profileschemaversion", "dlc_PolicyUpdateLastRun", "_PnP_ProvisioningTemplateInfo", 
                "vti_indexedpropertykeys", "__InheritsThemedCssFolderUrl", "_PnP_ProvisioningTemplateId", "__InheritMasterUrl",
                "DesignPreviewLayoutUrl", "DesignPreviewThemedCssFolderUrl"
            });

            foreach(string property in systemPropertyBagEntries)
            {
                int index = template.PropertyBagEntries.FindIndex(f => f.Key.Equals(property));

                if (index > -1)
                {
                    template.PropertyBagEntries.RemoveAt(index);
                }
            }

            return template;
        }

    }
}

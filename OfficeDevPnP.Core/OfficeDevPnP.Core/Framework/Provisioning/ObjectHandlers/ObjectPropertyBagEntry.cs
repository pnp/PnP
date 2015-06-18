using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectPropertyBagEntry : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Property bag entries"; }
        }
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_PropertyBagEntries);

            foreach (var propbagEntry in template.PropertyBagEntries)
            {
                if (!web.PropertyBagContainsKey(propbagEntry.Key))
                {
                    web.SetPropertyBagValue(propbagEntry.Key, propbagEntry.Value.ToParsedString());
                    if (propbagEntry.Indexed)
                    {
                        web.AddIndexedPropertyBagKey(propbagEntry.Key);
                    }
                }
            }
        }

        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            web.Context.Load(web, w => w.AllProperties, w => w.ServerRelativeUrl);
            web.Context.ExecuteQueryRetry();

            var entries = new List<PropertyBagEntry>();

            var indexedProperties = web.GetIndexedPropertyBagKeys().ToList();
            foreach (var propbagEntry in web.AllProperties.FieldValues)
            {
                var indexed = indexedProperties.Contains(propbagEntry.Key);
                entries.Add(new PropertyBagEntry() { Key = propbagEntry.Key, Value = propbagEntry.Value.ToString(), Indexed = indexed });
            }

            template.PropertyBagEntries.Clear();
            template.PropertyBagEntries.AddRange(entries);

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo);
            }

            foreach (PropertyBagEntry propbagEntry in template.PropertyBagEntries)
            {
                propbagEntry.Value = Tokenize(propbagEntry.Value, web.ServerRelativeUrl);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            ProvisioningTemplate baseTemplate = creationInfo.BaseTemplate;

            foreach (var propertyBagEntry in baseTemplate.PropertyBagEntries)
            {
                int index = template.PropertyBagEntries.FindIndex(f => f.Key.Equals(propertyBagEntry.Key));

                if (index > -1)
                {
                    template.PropertyBagEntries.RemoveAt(index);
                }
            }

            // Scan for "system" properties that should be removed as well. Below list contains
            // prefixes of properties that will be dropped
            List<string> systemPropertyBagEntriesExclusions = new List<string>(new string[] 
            { 
                "_", 
                "vti_", 
                "dlc_", 
                "ecm_",
                "profileschemaversion", 
                "DesignPreview"
            });

            // Below property prefixes indicate properties that never can be dropped 
            List<string> systemPropertyBagEntriesInclusions = new List<string>(new string[]
            {
                "_PnP_"
            });
            systemPropertyBagEntriesInclusions.AddRange(creationInfo.PropertyBagPropertiesToPreserve);

            List<PropertyBagEntry> entriesToDelete = new List<PropertyBagEntry>();

            // Prepare the list of property bag entries that will be dropped
            foreach (string property in systemPropertyBagEntriesExclusions)
            {
                var results = from prop in template.PropertyBagEntries
                              where prop.Key.Contains(property)
                              select prop;
                entriesToDelete.AddRange(results);
            }

            // Remove the property bag entries that we want to forcifully keep
            foreach (string property in systemPropertyBagEntriesInclusions)
            {
                var results = from prop in entriesToDelete
                              where prop.Key.Contains(property)
                              select prop;
                // Drop the found elements from the delete list    
                entriesToDelete = new List<PropertyBagEntry>(SymmetricExcept<PropertyBagEntry>(results, entriesToDelete));
            }

            // Delete the resulting list of property bag entries
            foreach (var property in entriesToDelete)
            {
                template.PropertyBagEntries.Remove(property);
            }

            return template;
        }

        private IEnumerable<T> SymmetricExcept<T>(IEnumerable<T> seq1, IEnumerable<T> seq2)
        {
            HashSet<T> hashSet = new HashSet<T>(seq1);
            hashSet.SymmetricExceptWith(seq2);
            return hashSet.Select(x => x);
        }


        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.PropertyBagEntries.Any();;
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

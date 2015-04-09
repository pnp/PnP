using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    /// <summary>
    /// Helper class that abstracts from any specific version of XMLPnPSchemaFormatter
    /// </summary>
    public class XMLPnPSchemaFormatter : ITemplateFormatter
    {
        /// <summary>
        /// Static property to retrieve an instance of the latest XMLPnPSchemaFormatter
        /// </summary>
        public static ITemplateFormatter LatestFormatter
        {
            get
            {
                return (new XMLPnPSchemaV2Formatter());
            }
        }

        /// <summary>
        /// Static method to retrieve a specific XMLPnPSchemaFormatter instance
        /// </summary>
        /// <param name="namespaceUri"></param>
        /// <returns></returns>
        public static ITemplateFormatter GetSpecificFormatter(String namespaceUri)
        {
            switch (namespaceUri)
            {
                case (XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_03):
                    return (new XMLPnPSchemaV1Formatter());
                case (XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_04):
                    return (new XMLPnPSchemaV2Formatter());
                default:
                    throw new ArgumentException("Unsupporter namespace URI", "namespaceUri");
            }
        }

        public bool IsValid(System.IO.Stream template)
        {
            ITemplateFormatter formatter = this.GetSpecificFormatterInternal(template);
            return (formatter.IsValid(template));
        }

        public System.IO.Stream ToFormattedTemplate(Model.ProvisioningTemplate template)
        {
            ITemplateFormatter formatter = XMLPnPSchemaFormatter.LatestFormatter;
            return (formatter.ToFormattedTemplate(template));
        }

        public Model.ProvisioningTemplate ToProvisioningTemplate(System.IO.Stream template)
        {
            ITemplateFormatter formatter = this.GetSpecificFormatterInternal(template);
            return (formatter.ToProvisioningTemplate(template));
        }

        #region Helper Methods

        private ITemplateFormatter GetSpecificFormatterInternal(System.IO.Stream template)
        {
            return (null);
        }
        
        #endregion
    }
}


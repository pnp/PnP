using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    /// <summary>
    /// Helper class that abstracts from any specific version of XMLPnPSchemaFormatter
    /// </summary>
    public class XMLPnPSchemaFormatter : ITemplateFormatter
    {
        private TemplateProviderBase _provider;

        public void Initialize(TemplateProviderBase provider)
        {
            this._provider = provider;
        }

        #region Static methods and properties

        /// <summary>
        /// Static property to retrieve an instance of the latest XMLPnPSchemaFormatter
        /// </summary>
        public static ITemplateFormatter LatestFormatter
        {
            get
            {
                return (new XMLPnPSchemaV201505Formatter());
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
                    return (new XMLPnPSchemaV201503Formatter());
                case (XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05):
                    return (new XMLPnPSchemaV201505Formatter());
                default:
                    throw new ArgumentException("Unsupporter namespace URI", "namespaceUri");
            }
        }

        #endregion

        #region Abstract methods implementation

        public bool IsValid(System.IO.Stream template)
        {
            ITemplateFormatter formatter = this.GetSpecificFormatterInternal(ref template);
            formatter.Initialize(this._provider);
            return (formatter.IsValid(template));
        }

        public System.IO.Stream ToFormattedTemplate(Model.ProvisioningTemplate template)
        {
            ITemplateFormatter formatter = XMLPnPSchemaFormatter.LatestFormatter;
            formatter.Initialize(this._provider);
            return (formatter.ToFormattedTemplate(template));
        }

        public Model.ProvisioningTemplate ToProvisioningTemplate(System.IO.Stream template)
        {
            return (this.ToProvisioningTemplate(template, null));
        }

        public Model.ProvisioningTemplate ToProvisioningTemplate(System.IO.Stream template, String identifier)
        {
            ITemplateFormatter formatter = this.GetSpecificFormatterInternal(ref template);
            formatter.Initialize(this._provider);
            return (formatter.ToProvisioningTemplate(template, identifier));
        }

        #endregion

        #region Helper Methods

        private ITemplateFormatter GetSpecificFormatterInternal(ref System.IO.Stream template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            // Crate a copy of the source stream
            MemoryStream sourceStream = new MemoryStream();
            template.CopyTo(sourceStream);
            sourceStream.Position = 0;
            template = sourceStream;

            XDocument xml = XDocument.Load(template);
            template.Position = 0;

            String targetNamespaceURI = xml.Root.Name.NamespaceName;

            if (!String.IsNullOrEmpty(targetNamespaceURI))
            {
                return (XMLPnPSchemaFormatter.GetSpecificFormatter(targetNamespaceURI));
            }
            else
            {
                return (XMLPnPSchemaFormatter.LatestFormatter);
            }
        }
        
        #endregion
    }
}


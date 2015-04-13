using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    /// <summary>
    /// Provider for xml based configurations
    /// </summary>
    public abstract class XMLTemplateProvider : TemplateProviderBase
    {

        #region Constructor
        protected XMLTemplateProvider(FileConnectorBase connector)
            : base(connector)
        {
        }
        #endregion

        #region Base class overrides

        public override List<ProvisioningTemplate> GetTemplates()
        {
            var formatter = new XMLPnPSchemaFormatter();
            return (this.GetTemplates(formatter));
        }

        public override List<ProvisioningTemplate> GetTemplates(ITemplateFormatter formatter)
        {
            List<ProvisioningTemplate> result = new List<ProvisioningTemplate>();

            // Retrieve the list of available template files
            List<String> files = this.Connector.GetFiles();

            // For each file
            foreach (var file in files)
            {
                if (file.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Load it from a File Stream
                    Stream stream = this.Connector.GetFileStream(file);

                    // And convert it into a ProvisioningTemplate
                    ProvisioningTemplate provisioningTemplate = formatter.ToProvisioningTemplate(stream);

                    // Add the template to the result
                    result.Add(provisioningTemplate);
                }
            }

            return (result);
        }

        public override ProvisioningTemplate GetTemplate(string identifier)
        {
            var formatter = new XMLPnPSchemaFormatter();
            return (this.GetTemplate(identifier, formatter));
        }

        public override ProvisioningTemplate GetTemplate(string identifier, ITemplateFormatter formatter)
        {
            if (String.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("identifier");
            }

            // Get the XML document from a File Stream
            Stream stream = this.Connector.GetFileStream(identifier);

            // And convert it into a ProvisioningTemplate
            ProvisioningTemplate provisioningTemplate = formatter.ToProvisioningTemplate(stream);

            // Store the identifier of this template, is needed for latter save operation
            this.Identifier = identifier;

            return (provisioningTemplate);
        }

        public override void Save(ProvisioningTemplate template)
        {
            var formatter = new XMLPnPSchemaFormatter();
            this.Save(template, formatter);
        }

        public override void Save(ProvisioningTemplate template, ITemplateFormatter formatter)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            SaveToConnector(template, this.Identifier, formatter);
        }

        public override void SaveAs(ProvisioningTemplate template, string identifier)
        {
            var formatter = new XMLPnPSchemaFormatter();
            this.SaveAs(template, identifier, formatter);
        }

        public override void SaveAs(ProvisioningTemplate template, string identifier, ITemplateFormatter formatter)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (String.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("identifier");
            }

            SaveToConnector(template, identifier, formatter);
        }

        public override void Delete(string identifier)
        {
            if (String.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("identifier");
            }

            this.Connector.DeleteFile(identifier);
        }
        #endregion

        #region Helper methods
        
        private void SaveToConnector(ProvisioningTemplate template, string identifier, ITemplateFormatter formatter)
        {
            if (String.IsNullOrEmpty(template.ID))
            {
                template.ID = Path.GetFileNameWithoutExtension(identifier);
            }

            using (var stream = formatter.ToFormattedTemplate(template))
            {
                this.Connector.SaveFileStream(identifier, stream);
            }
        }

        #endregion
    }
}

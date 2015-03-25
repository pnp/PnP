using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
            List<ProvisioningTemplate> result = new List<ProvisioningTemplate>();

            // Retrieve the list of available template files
            List<String> files = this.Connector.GetFiles();

            // For each file
            foreach (var file in files)
            {
                if (file.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Load it from a File Stream
                    XDocument doc = XDocument.Load(new XmlTextReader(this.Connector.GetFileStream(file)));

                    // And convert it into a ProvisioningTemplate
                    ProvisioningTemplate provisioningTemplate = XMLSerializer.Deserialize<SharePointProvisioningTemplate>(doc).ToProvisioningTemplate();

                    // Add the template to the result
                    result.Add(provisioningTemplate);
                }
            }

            return (result);
        }

        public override ProvisioningTemplate GetTemplate(string identifier)
        {
            if (String.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("identifier");
            }

            // Get the XML document from a File Stream
            XDocument doc = XDocument.Load(this.Connector.GetFileStream(identifier));

            // And convert it into a ProvisioningTemplate
            ProvisioningTemplate provisioningTemplate = XMLSerializer.Deserialize<SharePointProvisioningTemplate>(doc).ToProvisioningTemplate();

            // Store the identifier of this template, is needed for latter save operation
            this.Identifier = identifier;

            return (provisioningTemplate);
        }

        public override void Save(ProvisioningTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            SaveToConnector(template, this.Identifier);
        }

        public override void SaveAs(ProvisioningTemplate template, string identifier)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (String.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("identifier");
            }

            SaveToConnector(template, identifier);
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
        private void SaveToConnector(ProvisioningTemplate template, string identifier)
        {
            SharePointProvisioningTemplate spProvisioningTemplate = template.ToXml();
            using (var stream = XMLSerializer.SerializeToStream<SharePointProvisioningTemplate>(spProvisioningTemplate))
            {
                this.Connector.SaveFileStream(identifier, stream);
            }
        }
        #endregion
    }
}

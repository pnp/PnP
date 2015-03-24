using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    /// <summary>
    /// Provider for xml based configurations
    /// </summary>
    public abstract class XMLTemplateProvider : TemplateProviderBase
    {
        protected XMLTemplateProvider(FileConnectorBase connector)
            : base(connector)
        {
        }

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
                    //String xml = this.Connector.GetFile(file);
                    //XDocument doc = XDocument.Parse(xml);
                    XDocument doc = XDocument.Load(new XmlTextReader(this.Connector.GetFileStream(file)));

                    // And convert it into a ProvisioningTemplate
                    ProvisioningTemplate provisioningTemplate = XMLSerializer.Deserialize<SharePointProvisioningTemplate>(doc).ToProvisioningTemplate();

                    // Add the template to the result
                    result.Add(provisioningTemplate);
                }
            }

            return (result);
        }

        public override ProvisioningTemplate GetTemplate(string identifyer)
        {
            if (String.IsNullOrEmpty(identifyer))
            {
                throw new ArgumentException("identifyer");
            }

            // Get the XML document from a File Stream
            XDocument doc = XDocument.Load(this.Connector.GetFileStream(identifyer));

            // And convert it into a ProvisioningTemplate
            ProvisioningTemplate provisioningTemplate = XMLSerializer.Deserialize<SharePointProvisioningTemplate>(doc).ToProvisioningTemplate();

            return (provisioningTemplate);
        }

        public override void Save(ProvisioningTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            //SharePointProvisioningTemplate spProvisioningTemplate = template.ToXml();
            //String xml = XMLSerializer.Serialize<SharePointProvisioningTemplate>(spProvisioningTemplate);

            // TODO: Wait for Save method implementation
            // this.Connector.Save(xml, name/identifyer?);

            throw new NotImplementedException();
        }

        public override void Delete(string identifyer)
        {
            if (String.IsNullOrEmpty(identifyer))
            {
                throw new ArgumentException("identifyer");
            }

            // TODO: Wait for Delete method implementation
            throw new NotImplementedException();
        }
    }
}

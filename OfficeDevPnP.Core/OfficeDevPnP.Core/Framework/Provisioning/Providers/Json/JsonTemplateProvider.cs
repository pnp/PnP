using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Json
{
    /// <summary>
    /// Provider for JSON based configurations
    /// </summary>
    public abstract class JsonTemplateProvider : TemplateProviderBase
    {

        #region Constructor
        protected JsonTemplateProvider() : base()
        {

        }

        protected JsonTemplateProvider(FileConnectorBase connector)
            : base(connector)
        {
        }
        #endregion

        #region Base class overrides

        public override List<ProvisioningTemplate> GetTemplates()
        {
            var formatter = new JsonPnPFormatter();
            formatter.Initialize(this);
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
                if (file.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
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

        public override ProvisioningTemplate GetTemplate(string uri)
        {
            var formatter = new JsonPnPFormatter();
            formatter.Initialize(this);
            return (this.GetTemplate(uri, null, formatter));
        }

        public override ProvisioningTemplate GetTemplate(string uri, string identifier)
        {
            var formatter = new JsonPnPFormatter();
            formatter.Initialize(this);
            return (this.GetTemplate(uri, identifier, formatter));
        }

        public override ProvisioningTemplate GetTemplate(string uri, ITemplateFormatter formatter)
        {
            return (this.GetTemplate(uri, null, formatter));
        }

        public override ProvisioningTemplate GetTemplate(string uri, string identifier, ITemplateFormatter formatter)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("uri");
            }

            // Get the XML document from a File Stream
            Stream stream = this.Connector.GetFileStream(uri);

            // And convert it into a ProvisioningTemplate
            ProvisioningTemplate provisioningTemplate = formatter.ToProvisioningTemplate(stream, identifier);

            // Store the identifier of this template, is needed for latter save operation
            this.Uri = uri;

            return (provisioningTemplate);
        }

        public override void Save(ProvisioningTemplate template)
        {
            var formatter = new JsonPnPFormatter();
            this.Save(template, formatter);
        }

        public override void Save(ProvisioningTemplate template, ITemplateFormatter formatter)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            SaveToConnector(template, this.Uri, formatter);
        }

        public override void SaveAs(ProvisioningTemplate template, string uri)
        {
            var formatter = new JsonPnPFormatter();
            this.SaveAs(template, uri, formatter);
        }

        public override void SaveAs(ProvisioningTemplate template, string uri, ITemplateFormatter formatter)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("uri");
            }

            SaveToConnector(template, uri, formatter);
        }

        public override void Delete(string uri)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("identifier");
            }

            this.Connector.DeleteFile(uri);
        }
       
        #endregion

        #region Helper methods
        
        private void SaveToConnector(ProvisioningTemplate template, string uri, ITemplateFormatter formatter)
        {
            if (String.IsNullOrEmpty(template.Id))
            {
                template.Id = Path.GetFileNameWithoutExtension(uri);
            }

            using (var stream = formatter.ToFormattedTemplate(template))
            {
                this.Connector.SaveFileStream(uri, stream);
            }
        }

        #endregion
    }
}

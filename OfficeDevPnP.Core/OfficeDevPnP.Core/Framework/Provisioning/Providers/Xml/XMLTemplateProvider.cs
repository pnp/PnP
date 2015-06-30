using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Linq;
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
        protected XMLTemplateProvider()
            : base()
        {

        }
        protected XMLTemplateProvider(FileConnectorBase connector)
            : base(connector)
        {
        }
        #endregion

        #region Base class overrides

        public override List<ProvisioningTemplate> GetTemplates()
        {
            var formatter = new XMLPnPSchemaFormatter();
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
                if (file.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Load it from a File Stream
                    Stream stream = this.Connector.GetFileStream(file);

                    ProvisioningTemplate provisioningTemplate;
                    try
                    {
                        // And convert it into a ProvisioningTemplate
                        provisioningTemplate = formatter.ToProvisioningTemplate(stream);
                    }
                    catch (ApplicationException)
                    {
                        Log.Warning(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_Providers_XML_InvalidFileFormat, file);
                        continue;
                    }

                    if (provisioningTemplate != null)
                    {
                        // Add the template to the result
                        result.Add(provisioningTemplate);
                    }
                }
            }

            return (result);
        }

        public override ProvisioningTemplate GetTemplate(string uri)
        {
            var formatter = new XMLPnPSchemaFormatter();
            formatter.Initialize(this);
            return (this.GetTemplate(uri, null, formatter));
        }

        public override ProvisioningTemplate GetTemplate(string uri, string identifier)
        {
            var formatter = new XMLPnPSchemaFormatter();
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

            //Resolve xml includes if any
            stream = ResolveXIncludes(stream);

            // And convert it into a ProvisioningTemplate
            ProvisioningTemplate provisioningTemplate = formatter.ToProvisioningTemplate(stream, identifier);

            // Store the identifier of this template, is needed for latter save operation
            this.Uri = uri;

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

            SaveToConnector(template, this.Uri, formatter);
        }

        public override void SaveAs(ProvisioningTemplate template, string uri)
        {
            var formatter = new XMLPnPSchemaFormatter();
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

        private Stream ResolveXIncludes(Stream stream)
        {
            var res = stream;
            XDocument xml = XDocument.Load(stream);

            //find XInclude elements by XName
            XName xiName = XName.Get("{http://www.w3.org/2001/XInclude}include");
            var includes = xml.Descendants(xiName).ToList();

            if (includes.Count > 0)
            {
                foreach (var xi in includes)
                {
                    //resolve xInclude and replace
                    var href = xi.Attribute("href").Value;
                    var incStream = this.Connector.GetFileStream(href);
                    var resolved = XElement.Load(incStream);
                    xi.ReplaceWith(resolved);
                }

                //save xml to a new stream
                res = new MemoryStream();
                xml.Save(res);
            }
            res.Seek(0, SeekOrigin.Begin);
            return res;
        }

        #endregion
    }
}

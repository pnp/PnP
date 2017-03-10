using NLog;
using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using System.Collections.Generic;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer
{
    [Serializable()]
    public class Template
    {
        #region Constants

        private const string PARAMETER_CONNECTION_STRING    = "ConnectionString";
        private const string ERROR_TEMPLATE_NOT_FOUND       = "Template '{0}' was not found";
        private const string EXTENSION_PNP_FILE             = ".pnp";

        #endregion


        #region Private Members

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Public Members

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlAttribute("ignore")]
        public bool Ignore { get; set; }

        #endregion


        #region Private Methods

        // ===========================================================================================================
        /// <summary>
        /// Returns whether the current template file exists based on the specified template provider
        /// </summary>
        /// <param name="templateProvider">The template provider that is mapped to the client's working directory</param>
        /// <returns>True if the template file exists, otherwise false</returns>
        // ===========================================================================================================
        private bool TemplateExists(XMLTemplateProvider templateProvider)
        {
            string workingDirectory = templateProvider.Connector.Parameters[PARAMETER_CONNECTION_STRING].ToString();
            string templatePath = System.IO.Path.Combine(workingDirectory, this.Path);
            return System.IO.File.Exists(templatePath);
        }


        // ===========================================================================================================
        /// <summary>
        /// Checks whether the current template is a packaged ".pnp" file or a regular ".xml" file
        /// </summary>
        /// <returns>Returns <b>true</b> if it's a packaged ".pnp" file, otherwise <b>false</b></returns>
        // ===========================================================================================================
        private bool IsOpenXml()
        {
            return this.Path.ToLower().Trim().EndsWith(EXTENSION_PNP_FILE);
        }


        // ===========================================================================================================
        /// <summary>
        /// Returns the provisioning template applying information 
        /// </summary>
        /// <returns>A <b>ProvisioningTemplateApplyingInformation</b> object</returns>
        // ===========================================================================================================
        private ProvisioningTemplateApplyingInformation GetTemplateApplyInfo()
        {
            ProvisioningTemplateApplyingInformation ptai = new ProvisioningTemplateApplyingInformation();
            ptai.ProgressDelegate = delegate (string message, int progress, int total)
            {
                logger.Info("Executing step {0:00}/{1:00} - {2}", progress, total, message);
            };

            return ptai;
        }


        // ===========================================================================================================
        /// <summary>
        /// Applies the current template as a regular XML template on the specified web
        /// </summary>
        /// <param name="web">The <b>Web</b> on which to apply the template</param>
        /// <param name="templateProvider">The <b>XMLTemplateProvider</b> that is mapped to the client's working directory</param>
        // ===========================================================================================================
        private void ApplyRegularXML(Web web, XMLTemplateProvider templateProvider)
        {
            logger.Info("Applying template '{0}' from file '{1}'", this.Name, this.Path);

            // --------------------------------------------------
            // Formats the template's execution rendering
            // --------------------------------------------------
            ProvisioningTemplateApplyingInformation ptai = GetTemplateApplyInfo();

            // --------------------------------------------------
            // Loads the template 
            // --------------------------------------------------
            ProvisioningTemplate template = templateProvider.GetTemplate(this.Path);
            template.Connector = templateProvider.Connector;

            // --------------------------------------------------
            // Applies the template 
            // --------------------------------------------------
            web.ApplyProvisioningTemplate(template, ptai);
        }


        // ===========================================================================================================
        /// <summary>
        /// Applies the current template as an open XML ".pnp" package on the specified web
        /// </summary>
        /// <param name="web">The <b>Web</b> on which to apply the template</param>
        /// <param name="templateProvider">The <b>XMLTemplateProvider</b> that is mapped to the client's working directory</param>
        // ===========================================================================================================
        private void ApplyOpenXML(Web web, XMLTemplateProvider templateProvider)
        {
            logger.Info("Applying open XML package '{0}' from file '{1}'", this.Name, this.Path);

            // --------------------------------------------------
            // Formats the template's execution rendering
            // --------------------------------------------------
            ProvisioningTemplateApplyingInformation ptai = GetTemplateApplyInfo();

            // --------------------------------------------------
            // Replaces the regular provider by an OpenXml one
            // --------------------------------------------------
            string workingDirectory = templateProvider.Connector.Parameters[PARAMETER_CONNECTION_STRING].ToString();
            FileSystemConnector fileSystemConnector = new FileSystemConnector(workingDirectory, "");
            OpenXMLConnector openXmlConnector = new OpenXMLConnector(this.Path, fileSystemConnector);
            XMLTemplateProvider openXmlTemplateProvider = new XMLOpenXMLTemplateProvider(openXmlConnector);

            // --------------------------------------------------
            // Loops through all templates within the .pnp package
            // --------------------------------------------------
            List<ProvisioningTemplate> templates = openXmlTemplateProvider.GetTemplates();

            foreach (ProvisioningTemplate template in templates)
            {
                logger.Info("Applying template '{0}' from file '{1}'", template.Id, this.Path);

                // --------------------------------------------------
                // Applies the template 
                // --------------------------------------------------
                template.Connector = openXmlTemplateProvider.Connector;
                web.ApplyProvisioningTemplate(template, ptai);
            }
        }

        #endregion


        #region Public Methods

        // ===========================================================================================================
        /// <summary>
        /// Applies the current template on the specified <b>Web</b> object
        /// </summary>
        /// <param name="web">The <b>Web</b> object on which the template needs to be applied</param>
        /// <param name="templateProvider">The <b>XMLTemplatePRovider</b> that is mapped to the client's working directory</param>
        // ===========================================================================================================
        public void Apply(Web web, XMLTemplateProvider templateProvider)
        {
            if(!this.Ignore)
            {
                if (TemplateExists(templateProvider))
                {
                    if (IsOpenXml())
                    {
                        // --------------------------------------------------
                        // Handles ".pnp" templates
                        // --------------------------------------------------
                        ApplyOpenXML(web, templateProvider);
                    }
                    else
                    {
                        // --------------------------------------------------
                        // Handles regular ".xml" templates
                        // --------------------------------------------------
                        ApplyRegularXML(web, templateProvider);
                    }
                }
                else
                {
                    throw new FileNotFoundException(string.Format(ERROR_TEMPLATE_NOT_FOUND, System.IO.Path.Combine(templateProvider.Connector.Parameters[PARAMETER_CONNECTION_STRING].ToString(), this.Path)));
                }
            }
            else
            {
                logger.Info("Ignoring template '{0}' from file '{1}'", this.Name, this.Path);
            }
        }

        #endregion
    }
}

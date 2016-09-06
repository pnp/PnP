using NLog;
using System;
using System.Globalization;
using System.Collections.Generic;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using PNP.Deployer.Common;



// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer.ExtensibilityProviders.SSOM
{
    public class SiteFieldsProvider : IProvisioningExtensibilityHandler
    {
        #region Constants

        private const string ERROR_GENERAL = "An error occured while executing the SiteFieldsProvider : {0}";

        #endregion


        #region Private Members

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        
        #region Interface Methods

        public void Provision(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation, TokenParser tokenParser, PnPMonitoredScope scope, string configurationData)
        {
            logger.Info("Entering the SiteFields provider");

            try
            {
                // ----------------------------------------------
                // Deserializes the configuration data
                // ----------------------------------------------
                SiteFieldsConfigurationData configData = XmlUtility.DeserializeXml<SiteFieldsConfigurationData>(configurationData);

                // ----------------------------------------------
                // Loads the site collection and root web
                // ----------------------------------------------
                using (SPSite site = new SPSite(ctx.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        // ----------------------------------------------
                        // Loops through the <Fields> nodes
                        // ----------------------------------------------
                        foreach (Field configField in configData.Fields)
                        {
                            SPField field = web.Fields.GetFieldByInternalName(configField.Name);

                            // ----------------------------------------------
                            // Configures the field's title resource
                            // ----------------------------------------------
                            ConfigureFieldTitleResource(field, configField);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(String.Format(ERROR_GENERAL, e.Message), e.InnerException);
            }
        }


        public ProvisioningTemplate Extract(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInformation, PnPMonitoredScope scope, string configurationData)
        {
            return template;
        }


        public IEnumerable<TokenDefinition> GetTokens(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            return new List<TokenDefinition>();
        }

        #endregion


        #region Private Methods

        // ===========================================================================================================
        /// <summary>
        /// Configures the TitleResource of the given <b>SPField</b> based on the specified <b>Field</b> config
        /// </summary>
        /// <param name="field">The <b>SPField</b> that needs to be configured</param>
        /// <param name="configField">The <b>Field</b> config</param>
        // ===========================================================================================================
        private void ConfigureFieldTitleResource(SPField field, Field configField)
        {
            logger.Info("Configuring title resources for field '{0}'", field.InternalName);

            foreach(TitleResource titleResource in configField.TitleResources)
            {
                field.TitleResource.SetValueForUICulture(new CultureInfo(titleResource.LCID), titleResource.Value);
            }
            field.Update();
        }

        #endregion
    }
}

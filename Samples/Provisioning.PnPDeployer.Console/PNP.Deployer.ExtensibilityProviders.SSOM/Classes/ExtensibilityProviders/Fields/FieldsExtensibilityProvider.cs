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
    public class FieldsExtensibilityProvider : IProvisioningExtensibilityHandler
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
                FieldsConfig configData = DeployerUtility.DeserializeProviderConfig<FieldsConfig>(configurationData);

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
                        foreach (FieldConfig fieldConfig in configData.Fields)
                        {
                            SPField field = web.Fields.GetFieldByInternalName(fieldConfig.Name);

                            // ----------------------------------------------
                            // Configures the field's title resource
                            // ----------------------------------------------
                            field.ConfigureFieldTitleResource(fieldConfig.TitleResources);
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
    }
}

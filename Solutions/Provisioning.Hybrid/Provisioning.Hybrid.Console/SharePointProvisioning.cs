using Contoso.Provisioning.Hybrid.Core.SiteTemplates;
using Microsoft.SharePoint.Client;
using OfficeAMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Contoso.Provisioning.Hybrid
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SharePointProvisioning" in both code and config file together.

    public class SharePointProvisioning : ISharePointProvisioning
    {
        public bool ProvisionSiteCollection(Contract.SharePointProvisioningData sharePointProvisioningData)
        {
            bool processed = false;
            try
            {
                SiteProvisioningBase siteToProvision = null;
                switch (sharePointProvisioningData.Template)
                {
                    case SiteProvisioningTypes.ContosoCollaboration:
                        siteToProvision = new ContosoCollaboration();
                        break;
                    case SiteProvisioningTypes.ContosoProject:
                        siteToProvision = new ContosoProject();
                        break;
                }

                siteToProvision.SharePointProvisioningData = sharePointProvisioningData;
                HookupAuthentication(siteToProvision);

                // Hookup class that will hold the on-prem overrides
                SiteProvisioningOnPremises spo = new SiteProvisioningOnPremises();
                siteToProvision.SiteProvisioningOnPremises = spo;

                // Provision the site collection
                processed = siteToProvision.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //log error
            }

            return processed;
        }

        private void HookupAuthentication(SiteProvisioningBase siteProvisioningInstance)
        {
            siteProvisioningInstance.Realm = GetConfiguration("Realm");
            siteProvisioningInstance.AppId = GetConfiguration("AppId");
            siteProvisioningInstance.AppSecret = EncryptionUtility.Decrypt(GetConfiguration("AppSecret"), GetConfiguration("General.EncryptionThumbPrint"));

            siteProvisioningInstance.InstantiateSiteDirectorySiteClientContext(GetConfiguration("General.SiteDirectoryUrl"));
        }

        private string GetConfiguration(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

using Contoso.Provisioning.Hybrid.Contract;
using Microsoft.WindowsAzure.ServiceRuntime;
using OfficeAMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Provisioning.Hybrid.Core.SiteTemplates
{
    public class ContosoProject: SiteProvisioningBase
    {
        public override bool Execute()
        {
            bool processed = false;

            string generalSiteDirectoryUrl = RoleEnvironment.GetConfigurationSettingValue("General.SiteDirectoryUrl");
            string generalSiteDirectoryListName = RoleEnvironment.GetConfigurationSettingValue("General.SiteDirectoryListName");
            string generalSiteCollectionUrl = RoleEnvironment.GetConfigurationSettingValue("General.SiteCollectionUrl");
            string generalMailSMTPServer = RoleEnvironment.GetConfigurationSettingValue("General.MailSMTPServer");
            string generalMailUser = RoleEnvironment.GetConfigurationSettingValue("General.MailUser");
            string generalMailUserPassword = RoleEnvironment.GetConfigurationSettingValue("General.MailUserPassword");
            string generalMailSiteAvailable = RoleEnvironment.GetConfigurationSettingValue("General.MailSiteAvailable");
            string generalEncryptionThumbPrint = RoleEnvironment.GetConfigurationSettingValue("General.EncryptionThumbPrint");
            //Decrypt mail password
            generalMailUserPassword = EncryptionUtility.Decrypt(generalMailUserPassword, generalEncryptionThumbPrint);
            //On-Prem settings
            string generalOnPremWebApplication = GetConfiguration("General.OnPremWebApplication");

            try
            {
                SiteDirectoryManager siteDirectoryManager = new SiteDirectoryManager();

                string tempSharePointUrl = this.SharePointProvisioningData.Url;
                string siteCollectionUrl = this.CreateOnPremises ? generalOnPremWebApplication : generalSiteCollectionUrl;

                // issue the final SharePoint url
                SharePointProvisioningData.Url = this.GetNextSiteCollectionUrl(generalSiteDirectoryUrl, generalSiteDirectoryListName, siteCollectionUrl);

                //update site directory status
                siteDirectoryManager.UpdateSiteDirectoryStatus(this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, generalSiteDirectoryUrl, generalSiteDirectoryListName, tempSharePointUrl, this.SharePointProvisioningData.Url, "Provisioning");

                //complete the site data
                this.SharePointProvisioningData.Template = "PROJECTSITE#0";
                this.SharePointProvisioningData.SiteOwner = this.SharePointProvisioningData.Owners[0];
                this.SharePointProvisioningData.Lcid = 1033;
                this.SharePointProvisioningData.TimeZoneId = 3;
                this.SharePointProvisioningData.StorageMaximumLevel = 100;
                this.SharePointProvisioningData.StorageWarningLevel = 80;

                //create the site collection
                this.AddSiteCollection(this.SharePointProvisioningData);

                // Update status
                siteDirectoryManager.UpdateSiteDirectoryStatus(this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, generalSiteDirectoryUrl, generalSiteDirectoryListName, this.SharePointProvisioningData.Url, "Available");

                // Send mail to owners
                List<String> mailTo = new List<string>();
                string ownerNames = "";
                string ownerAccounts = "";

                foreach (SharePointUser owner in this.SharePointProvisioningData.Owners)
                {
                    mailTo.Add(owner.Email);

                    if (ownerNames.Length > 0)
                    {
                        ownerNames = ownerNames + ", ";
                        ownerAccounts = ownerAccounts + ", ";
                    }
                    ownerNames = ownerNames + owner.Name;
                    ownerAccounts = ownerAccounts + owner.Login;
                }

                // send email to notify the use of successful provisioning
                string mailBody = String.Format(generalMailSiteAvailable, this.SharePointProvisioningData.Title, this.SharePointProvisioningData.Url, ownerNames, ownerAccounts);
                MailUtility.SendEmail(generalMailSMTPServer, generalMailUser, generalMailUserPassword, mailTo, null, "Your SharePoint site is ready to be used", mailBody);
            }
            catch (Exception ex)
            {
                new SiteDirectoryManager().UpdateSiteDirectoryStatus(this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, generalSiteDirectoryUrl, generalSiteDirectoryListName, this.SharePointProvisioningData.Url, "Error during provisioning", ex);
            }

            return processed;
        }
    }
}

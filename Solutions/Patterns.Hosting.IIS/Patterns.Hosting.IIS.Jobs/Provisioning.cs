using Contoso.Patterns.Provisioning;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Utilities;
using Patterns.Provisioning.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Patterns.Hosting.IIS.Jobs
{
    public class Provisioning
    {
        bool O365Modus = true;
        bool appOnlyModus = true;

        // Configuration setup
        string configurationFilePath = @"c:\tfsams\DEV\Solutions\Patterns.Hosting.IIS\Patterns.Hosting.IIS.Jobs\Configuration";
        string configurationFile = "configuration.xml";

        // Authentication setup
        string tenantAdminSite = "";
        string tenantAdminUser = "";
        string tenantAdminUserDomain = "";
        string tenantAdminUserPassword = "";
        string realm = "";
        string appId = "";
        string appSecret = "";

        SharePointUser siteOwner = null;
        string siteUrl = "";
        SharePointPlatform sharePointPlatform = SharePointPlatform.Office365;

        public void Provision(string password)
        {
            // Authentication setup
            if (O365Modus)
            {
                tenantAdminSite = "https://bertonline-admin.sharepoint.com";
                if (!appOnlyModus)
                {
                    tenantAdminUser = "bert.jansen@bertonline.onmicrosoft.com";
                    tenantAdminUserPassword = password;
                }
                else
                {
                    realm = ConfigurationManager.AppSettings["Realm"];
                    appId = ConfigurationManager.AppSettings["AppId"];
                    appSecret = EncryptionUtility.Decrypt(ConfigurationManager.AppSettings["AppSecret"], ConfigurationManager.AppSettings["EncryptionThumbPrint"]);
                }

                siteUrl = "https://bertonline.sharepoint.com/sites/jdp0001";
                //siteUrl = "https://bertonline.sharepoint.com/sites/20140057/demo";
                siteOwner = new SharePointUser()
                {
                    Name = "Bert Jansen",
                    Login = "bert.jansen@bertonline.onmicrosoft.com",
                    Email = "bert.jansen@bertonline.onmicrosoft.com"
                };
            }
            else
            {
                tenantAdminSite = "https://sp2013.set1.bertonline.info/sites/tenantadmin";
                if (!appOnlyModus)
                {
                    tenantAdminUser = "administrator";
                    tenantAdminUserDomain = "SET1";
                    tenantAdminUserPassword = password;
                }
                else
                {
                    realm = ConfigurationManager.AppSettings["Realm"];
                    appId = ConfigurationManager.AppSettings["AppId"];
                    appSecret = EncryptionUtility.Decrypt(ConfigurationManager.AppSettings["AppSecret"], ConfigurationManager.AppSettings["EncryptionThumbPrint"]);
                }

                sharePointPlatform = SharePointPlatform.OnPremises;

                siteUrl = "https://sp2013.set1.bertonline.info/sites/jdp001/sub1";
                siteOwner = new SharePointUser()
                {
                    Name = "Kevin Cook",
                    Login = "set1\\kevinc",
                    Email = "kevinc@set1.bertonline.info"
                };
            }

            // Site information

            SharePointUser[] siteAdministrators = new SharePointUser[1];

            siteAdministrators[0] = new SharePointUser()
            {
                Name = "administrator",
                Login = "set1\\administrator",
                Email = "administrator@set1.bertonline.info"
            }; 

            SiteRequestInformation siteToCreate = new SiteRequestInformation()
            {
                Url = siteUrl,
                Title = "Test site collection",
                Description = "Test site collection",
                Template = "ContosoTeam",
                SiteOwner = siteOwner,
                AdditionalAdministrators = siteAdministrators
            };


            // Instantiate 
            SiteProvisioningEngine siteProvisioningEngine = new SiteProvisioningEngine(Path.Combine(configurationFilePath, configurationFile), sharePointPlatform);
            
            // fill authentication related properties
            siteProvisioningEngine.TenantAdminSite = tenantAdminSite;
            siteProvisioningEngine.TenantAdminUser = tenantAdminUser;
            siteProvisioningEngine.TenantAdminUserPassword = tenantAdminUserPassword;
            siteProvisioningEngine.TenantAdminUserDomain = tenantAdminUserDomain;
            siteProvisioningEngine.Realm = realm;
            siteProvisioningEngine.AppId = appId;
            siteProvisioningEngine.AppSecret = appSecret;
            
            // Launch
            siteProvisioningEngine.Execute(siteToCreate);

        }
    }
}

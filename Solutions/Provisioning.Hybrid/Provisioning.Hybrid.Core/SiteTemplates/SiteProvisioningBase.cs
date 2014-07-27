using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core;
using Contoso.Provisioning.Hybrid.Contract;
using System.IO;
using System.Diagnostics;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace Contoso.Provisioning.Hybrid.Core.SiteTemplates
{
    public abstract class SiteProvisioningBase
    {
        ClientContext appOnlyClientContext = null;
        ClientContext createdSiteContext = null;
        ClientContext siteDirectorySiteContext = null;

        /// <summary>
        /// Returns the app only client context (the one with tenant level permissions)
        /// </summary>
        public ClientContext AppOnlyClientContext
        {
            get
            {
                return this.appOnlyClientContext;
            }
        }

        /// <summary>
        /// Returns the client context to manipulate the created site (collection)
        /// </summary>
        public ClientContext CreatedSiteContext
        {
            get
            {
                return this.createdSiteContext;
            }
        }

        /// <summary>
        /// Returns the client context to manipulate the site directory in the site directory site
        /// </summary>
        public ClientContext SiteDirectorySiteContext
        {
            get
            {
                return this.siteDirectorySiteContext;
            }
        }

        /// <summary>
        /// Class instance that will be used for on-premises specific provisioning code
        /// </summary>
        public ISiteProvisioningOnPremises SiteProvisioningOnPremises
        {
            get;
            set;
        }

        /// <summary>
        /// Information about the site to be provisioned
        /// </summary>
        public SharePointProvisioningData SharePointProvisioningData
        {
            get;
            set;
        }

        /// <summary>
        /// We're creating on-premises
        /// </summary>
        public bool CreateOnPremises
        {
            get
            {
                return SharePointProvisioningData.DataClassification.Equals("HBI", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Returns the root directory of the current deployment
        /// </summary>
        public string AppRootPath
        {
            get
            {
                string roleRoot = Environment.GetEnvironmentVariable("RoleRoot");
                if (null != roleRoot && roleRoot.Length > 0)
                {
                    // We're running on azure (real or emulated)
                    return roleRoot + @"\approot";
                }
                else
                {
                    Process process = Process.GetCurrentProcess();
                    string fullPath = Path.GetDirectoryName(process.MainModule.FileName);
                    return fullPath;
                }
            }
        }

        /// <summary>
        /// Realm to use for the access token's nameid and audience. In Office 365 use MSOL PowerShell (Get-MsolCompanyInformation).ObjectID to obtain Target/Tenant realm
        /// </summary>
        public string Realm
        {
            get;
            set;
        }

        /// <summary>
        /// The Application ID generated when you deploy an app (Visual Studio) or when you register an app via the appregnew.aspx page
        /// </summary>
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// The Application Secret generated when you deploy an app (Visual Studio) or when you register an app via the appregnew.aspx page
        /// </summary>
        public string AppSecret
        {
            get;
            set;
        }


        /// <summary>
        /// Triggers the provisioning
        /// </summary>
        /// <returns>True if OK, false otherwise</returns>
        public virtual bool Execute()
        {
            bool result = false;

            return result;
        }

        /// <summary>
        /// Instantiate an app only client context (the one with tenant level permissions)
        /// </summary>
        /// <param name="siteUrl">Url of the tenant admin site</param>
        public void InstantiateAppOnlyClientContext(string siteUrl)
        {
            this.appOnlyClientContext = new AuthenticationManager().GetAppOnlyAuthenticatedContext(siteUrl, this.Realm, this.AppId, this.AppSecret);
        }

        /// <summary>
        /// Instantiate an app only client context (the one with tenant level permissions)
        /// </summary>
        /// <param name="siteUrl">Url of the tenant admin site</param>
        public void InstantiateCreatedSiteClientContext(string siteUrl)
        {
            this.createdSiteContext = new AuthenticationManager().GetAppOnlyAuthenticatedContext(siteUrl, this.Realm, this.AppId, this.AppSecret);
        }

        /// <summary>
        /// Instantiate an app only client context (the one with tenant level permissions)
        /// </summary>
        /// <param name="siteUrl">Url of the tenant admin site</param>
        public void InstantiateSiteDirectorySiteClientContext(string siteUrl)
        {
            this.siteDirectorySiteContext = new AuthenticationManager().GetAppOnlyAuthenticatedContext(siteUrl, this.Realm, this.AppId, this.AppSecret);
        }

        public string GetNextSiteCollectionUrl(string siteDirectoryUrl, string siteDirectoryListName, string baseSiteUrl)
        {
            if (this.CreateOnPremises)
            {
                return this.SiteProvisioningOnPremises.GetNextSiteCollectionUrl(this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, siteDirectoryUrl, siteDirectoryListName, baseSiteUrl);
            }
            else
            {
                return new SiteDirectoryManager().GetNextSiteCollectionUrlTenant(this.AppOnlyClientContext, this.AppOnlyClientContext.Web, this.SiteDirectorySiteContext, this.SiteDirectorySiteContext.Web, siteDirectoryUrl, siteDirectoryListName, baseSiteUrl);
            }
        }

        /// <summary>
        /// Launches a site collection creation and waits for the creation to finish
        /// </summary>
        /// <param name="properties">Describes the site collection to be created</param>
        public void AddSiteCollection(SharePointProvisioningData properties)
        {
            if (this.CreateOnPremises)
            {
                this.SiteProvisioningOnPremises.CreateSiteCollectionOnPremises(this.SharePointProvisioningData);
                this.createdSiteContext = this.SiteProvisioningOnPremises.SpOnPremiseAuthentication(this.SharePointProvisioningData.Url);
            }
            else
            {
                SiteEntity newSite = new SiteEntity
                {
                    Description = properties.Description,
                    Title = properties.Title,
                    Url = properties.Url,
                    Template = properties.Template,
                    Lcid = properties.Lcid,
                    SiteOwnerLogin = properties.SiteOwner.Login,
                    StorageMaximumLevel = properties.StorageMaximumLevel,
                    StorageWarningLevel = properties.StorageWarningLevel,
                    TimeZoneId = properties.TimeZoneId,
                    UserCodeMaximumLevel = properties.UserCodeMaximumLevel,
                    UserCodeWarningLevel = properties.UserCodeWarningLevel,
                };

                this.AppOnlyClientContext.Web.AddSiteCollectionTenant(newSite);

                InstantiateCreatedSiteClientContext(newSite.Url);
            }
        }

        internal string GetConfiguration(string key)
        {
            string value = "";

            if (this.CreateOnPremises)
            {
                value = ConfigurationManager.AppSettings[key];
            }
            else
            {
                value = RoleEnvironment.GetConfigurationSettingValue(key);
            }

            return value;
        }

    }
}

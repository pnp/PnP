using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Enums;
using Patterns.Provisioning.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Patterns.Provisioning
{
    public class SiteProvisioningEngine
    {
        #region Private class variables
        private SiteProvisioningConfiguration siteProvisioningConfiguration = null;
        private Configurations config = null;
        private ClientContext provisioningEngineContext = null;
        private ClientContext createdSiteContext = null;
        private SharePointPlatform platform = SharePointPlatform.Office365;
        private string tenantAdminUser = "";
        private string tenantAdminUserPassword = "";
        private string tenantAdminUserDomain = "";
        private string tenantAdminSite = "";
        private string realm = "";
        private string appId = "";
        private string appSecret = "";
        #endregion

        #region public properties
        public ClientContext CreatedSiteContext
        {
            set
            {
                this.createdSiteContext = value;
            }
        }

        public string TenantAdminSite
        {
            set
            {
                this.tenantAdminSite = value;
            }
        }

        public string TenantAdminUser
        {
            set
            {
                this.tenantAdminUser = value;
            }
        }

        public string TenantAdminUserPassword
        {
            set
            {
                this.tenantAdminUserPassword = value;
            }
        }

        public string TenantAdminUserDomain
        {
            set
            {
                this.tenantAdminUserDomain = value;
            }
        }

        public string Realm
        {
            set
            {
                this.realm = value;
            }
        }

        public string AppId
        {
            set
            {
                this.appId = value;
            }
        }

        public string AppSecret
        {
            set
            {
                this.appSecret = value;
            }
        }
        #endregion

        #region private properties
        private bool AppOnly
        {
            get
            {
                return (this.realm.Length > 0);
            }
        }
        #endregion

        #region class constructor
        public SiteProvisioningEngine(string configurationFile, SharePointPlatform platform)
        {
            //Set the platform
            this.platform = platform;

            //Process the configuration file
            siteProvisioningConfiguration = new SiteProvisioningConfiguration(configurationFile);
            config = siteProvisioningConfiguration.Config;            
        }
        #endregion

        #region Public provisioning methods        
        /// <summary>
        /// The execute method will deal with the all the steps required to provision a site collection or site.
        /// Each step will be a separate method that can be overriden by an inheriting class to allow a behaviour
        /// that deviates from the default one which is driven by the XML file
        /// </summary>
        /// <returns>True if the provisioning succeeds, false otherwise</returns>
        public bool Execute(SiteRequestInformation requestedSite)
        {
            bool result = false;
            bool createSubSite = false;
            ConfigurationsTemplate siteTemplate = null;

            //Set the provisioning client context
            this.provisioningEngineContext = CreateUserScopedTenantContext();

            // set createSubSite for future use
            createSubSite = AreWeCreatingASubSite(requestedSite);

            // do we have a configuration definition for the provided template
            siteTemplate = this.siteProvisioningConfiguration.LoadTemplate(requestedSite.Template);
            if (siteTemplate == null)
            {
                throw new SiteProvisioningException(String.Format("Template {0} is not a known and enabled template. Site with url {1} cannot be created", requestedSite.Template, requestedSite.Url));
            }

            // Validate the retrieved template. Throws exceptions when there are issues
            // TODO: do we want to validate on managedpath?
            ValidateSiteTemplate(siteTemplate, createSubSite);

            // Verify that the requested site collection or sub site does not yet exist
            if (SiteExists(requestedSite, createSubSite))
            {
                throw new SiteProvisioningException(String.Format("Site with url {0} already exists as active site collection, recycled site collection, site collection being create or as sub site", requestedSite.Url));
            };

            // Transform the siterequest information into sitecreation information
            SiteEntity siteToCreate = PrepareSiteCreation(requestedSite, siteTemplate, createSubSite);
            
            // Create the requested site collection or sub site
            // TODO: deal with recycled sites in MT
            CreateSite(siteToCreate, createSubSite);

            // add the original owner and potential additional owners as site collection administrator if needed
            if (!createSubSite)
            {
                AddSiteCollectionAdministrators(requestedSite);
            }

            //****************************************************************************
            //* Start site processing code                                               *   
            //****************************************************************************

            // Enable and disable features
            EnableFeatures(siteTemplate, createSubSite);

            // Create additional lists
            CreateLists(siteTemplate);


            //****************************************************************************
            //* End of site processing code                                              *   
            //****************************************************************************

            // We made it :-)
            result = true;

            return result;
        }

        public virtual bool SiteExists(SiteRequestInformation requestedSite, bool createSubSite)
        {
            bool siteExists = false;

            if (!createSubSite)
            {
                // check if site collection exists
                if (platform == SharePointPlatform.Office365)
                {
                    siteExists = provisioningEngineContext.Web.SiteExistsInTenant(requestedSite.Url);
                }
                else
                {
                    siteExists = provisioningEngineContext.Web.SiteExists(requestedSite.Url);
                }
            }
            else
            {
                // check if site collection exists
                if (platform == SharePointPlatform.Office365)
                {
                    //check if sub site exists                    
                    siteExists = provisioningEngineContext.Web.SubSiteExistsInTenant(requestedSite.Url);
                }
                else
                {
                    //check if sub site exists
                    siteExists = provisioningEngineContext.Web.SubSiteExists(requestedSite.Url);
                }
            }

            return siteExists;
        }

        public virtual SiteEntity PrepareSiteCreation(SiteRequestInformation requestedSite, ConfigurationsTemplate siteTemplate, bool createSubSite)
        {
            SiteEntity siteToCreate = new SiteEntity();

            // map properties that can be mapped 
            if (!createSubSite)
            {
                siteToCreate.Url = requestedSite.Url;
                siteToCreate.Title = requestedSite.Title;
                siteToCreate.Description = requestedSite.Description;
                siteToCreate.SiteOwnerLogin = requestedSite.SiteOwner.Login;
                siteToCreate.Lcid = requestedSite.Lcid;

                // complement with properties coming from the configuration file
                siteToCreate.Template = siteTemplate.RootTemplate;
                siteToCreate.StorageMaximumLevel = siteTemplate.StorageMaximumLevel;
                siteToCreate.StorageWarningLevel = siteTemplate.StorageWarningLevel;
                siteToCreate.UserCodeMaximumLevel = siteTemplate.UserCodeMaximumLevel;
                siteToCreate.UserCodeWarningLevel = siteTemplate.UserCodeWarningLevel;
            }
            else
            {
                siteToCreate.Url = requestedSite.Url;
                siteToCreate.Title = requestedSite.Title;
                siteToCreate.Description = requestedSite.Description;
                siteToCreate.Lcid = requestedSite.Lcid;

                // complement with properties coming from the configuration file
                siteToCreate.Template = siteTemplate.RootTemplate;
            }

            // TODO: remove this once Frank has done his changes
            // property validations
            if (siteToCreate.Lcid == 0)
            {
                siteToCreate.Lcid = 1033;
            }
            if (siteToCreate.TimeZoneId == 0)
            {
                siteToCreate.TimeZoneId = 3;
            }

            return siteToCreate;
        }

        public virtual void CreateSite(SiteEntity siteToCreate, bool createSubSite)
        {
            try
            {
                if (!createSubSite)
                {
                    // if we're using a user context to perform the creation we need to ensure that the 
                    // user in question has permissions to the created site collection. We do this by initially 
                    // creating the site collection with the "current user" as owner. Once everything is created
                    // we need to remove this site collection admin again
                    string previousOwner = "";
                    if (this.provisioningEngineContext.Credentials != null)
                    {
                        previousOwner = siteToCreate.SiteOwnerLogin;
                        if (platform == SharePointPlatform.Office365)
                        {
                            SharePointOnlineCredentials cred = this.provisioningEngineContext.Credentials as SharePointOnlineCredentials;
                            siteToCreate.SiteOwnerLogin = cred.UserName;
                        }
                        else
                        {
                            System.Net.NetworkCredential cred = this.provisioningEngineContext.Credentials as System.Net.NetworkCredential;
                            siteToCreate.SiteOwnerLogin = String.Format("{0}\\{1}", cred.Domain, cred.UserName);
                        }
                    }

                    // create the site collection
                    this.provisioningEngineContext.Web.AddSiteCollectionTenant(siteToCreate);

                    // restore the original site owner value
                    if (previousOwner.Length > 0)
                    {
                        siteToCreate.SiteOwnerLogin = previousOwner;
                    }

                    // create a context object for the created site collection, will be used for future site operations
                    this.createdSiteContext = CreateUserScopedSiteContext(siteToCreate.Url);

                    // workaround: CSOM API does not create the three default groups in on-premises, so 
                    // manually create them
                    if (platform != SharePointPlatform.Office365)
                    {
                        Group owners = this.createdSiteContext.Web.AddGroup(String.Format("{0} Owners", siteToCreate.Title), 
                                                                            String.Format("Use this group to grant people full control permissions to the SharePoint site: {0}", siteToCreate.Title), true, false);
                        Group members = this.createdSiteContext.Web.AddGroup(String.Format("{0} Members", siteToCreate.Title),
                                                                            String.Format("Use this group to grant people contribute permissions to the SharePoint site: {0}", siteToCreate.Title), true, false);
                        Group visitors = this.createdSiteContext.Web.AddGroup(String.Format("{0} Visitors", siteToCreate.Title),
                                                                            String.Format("Use this group to grant people read permissions to the SharePoint site: {0}", siteToCreate.Title), true, false);
                        this.createdSiteContext.Web.AssociateDefaultGroups(owners, members, visitors);                        
                    }
                }
                else // sub site creation
                {
                    // Check if the provisioning context is for the parent site of the sub site to create, if not update it
                    string parentSiteUrl = GetParentSite(siteToCreate.Url);
                    
                    if (!this.provisioningEngineContext.Url.Equals(parentSiteUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.provisioningEngineContext = CreateUserScopedSiteContext(parentSiteUrl);
                    }
                    
                    // Capture the original fully qualified URL before changing it
                    string orginalSiteUrl = siteToCreate.Url;

                    // fix the URL to make it work for the sub site creation API call
                    siteToCreate.Url = GetSubSiteName(siteToCreate.Url);                

                    // create the sub site
                    this.provisioningEngineContext.Web.CreateSite(siteToCreate);

                    // restore the orginal site url
                    siteToCreate.Url = orginalSiteUrl;

                    // create a context object for the created sub site as this will be used for the further site manipulations
                    this.createdSiteContext = CreateUserScopedSiteContext(siteToCreate.Url);
                }

            }
            catch (Exception ex)
            {
                throw new SiteProvisioningException(String.Format("Site collection or sub site creation for url {0} failed",siteToCreate.Url), ex);
            }
        }

        public virtual void AddSiteCollectionAdministrators(SiteRequestInformation requestedSite)
        {
            List<UserEntity> admins = new List<UserEntity>();
            // Add the site owner
            admins.Add(new UserEntity() { LoginName = requestedSite.SiteOwner.Login });

            // Add the additional site owners
            foreach (SharePointUser admin in requestedSite.AdditionalAdministrators)
            {
                admins.Add(new UserEntity() { LoginName = admin.Login });
            }

            this.createdSiteContext.Web.AddAdministrators(admins, true);
        }

        public virtual void EnableFeatures(ConfigurationsTemplate siteTemplate, bool createSubSite)
        {
            // first run through the site collection scoped features, assuming we're creating a site collection
            if (!createSubSite)
            {
                foreach (ConfigurationsTemplateFeature feature in siteTemplate.Features)
                {
                    if (feature.Scope.Equals("Site", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (Convert.ToBoolean(feature.Activate) == true)
                        {
                            this.createdSiteContext.Site.ActivateFeature(new Guid(feature.ID));
                        }
                        else
                        {
                            this.createdSiteContext.Site.DeactivateFeature(new Guid(feature.ID));
                        }
                    }
                }
            }

            // process the web scoped features
            foreach (ConfigurationsTemplateFeature feature in siteTemplate.Features)
            {
                if (feature.Scope.Equals("Web", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (Convert.ToBoolean(feature.Activate) == true)
                    {
                        this.createdSiteContext.Web.ActivateFeature(new Guid(feature.ID));
                    }
                    else
                    {
                        this.createdSiteContext.Web.DeactivateFeature(new Guid(feature.ID));
                    }
                }
            }
        }

        public virtual void CreateLists(ConfigurationsTemplate siteTemplate)
        {

            foreach (ConfigurationsTemplateListInstance list in siteTemplate.Lists)
            {
                if (String.IsNullOrEmpty(list.TemplateFeatureId))
                {
                    this.createdSiteContext.Web.AddList(Convert.ToInt32(list.TemplateType), new Guid(list.TemplateFeatureId), list.Title, list.EnableVersioning, urlPath: list.Url);
                }
                else
                {
                    if (Enum.IsDefined(typeof(ListTemplateType), list.TemplateType))
                    {
                        ListTemplateType template = (ListTemplateType) Enum.Parse(typeof(ListTemplateType), list.TemplateType.ToString(), true);
                        this.createdSiteContext.Web.AddList(template, list.Title, list.EnableVersioning, urlPath: list.Url);
                    }
                }
            }


        }
        #endregion

        #region Private helper methods
        private ClientContext CreateUserScopedTenantContext()
        {
            return CreateUserScopedSiteContext(this.tenantAdminSite);
        }

        private ClientContext CreateUserScopedSiteContext(string siteUrl)
        {
            if (String.IsNullOrEmpty(siteUrl))
            {
                throw new SiteProvisioningException("You need to provide a site url for creating a ClientContext object");
            }

            if (AppOnly)
            {
                if (String.IsNullOrEmpty(this.appId) || String.IsNullOrEmpty(this.appSecret))
                {
                    throw new SiteProvisioningException("In apponly mode you need to specify a valid realm, appId and appSecret");
                }

                return new AuthenticationManager().GetAppOnlyAuthenticatedContext(siteUrl, this.realm, this.appId, this.appSecret);
            }
            else
            {
                if (platform == SharePointPlatform.Office365)
                {
                    if (String.IsNullOrEmpty(this.tenantAdminUser) || String.IsNullOrEmpty(this.tenantAdminUserPassword))
                    {
                        throw new SiteProvisioningException("In user mode for Office 365 you need to specify a valid tenantAdminUser and tenantAdminUserPassword");
                    }
                    return new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant(siteUrl, this.tenantAdminUser, this.tenantAdminUserPassword);
                }
                else
                {
                    if (String.IsNullOrEmpty(this.tenantAdminUser) || String.IsNullOrEmpty(this.tenantAdminUserPassword) || String.IsNullOrEmpty(this.tenantAdminUserDomain))
                    {
                        throw new SiteProvisioningException("In user mode for on-premises you need to specify a valid tenantAdminUser, tenantAdminUserPassword and tenantAdminUserDomain");
                    }
                    return new AuthenticationManager().GetNetworkCredentialAuthenticatedContext(siteUrl, this.tenantAdminUser, this.tenantAdminUserPassword, this.tenantAdminUserDomain);
                }
            }
        }

        private void ValidateSiteTemplate(ConfigurationsTemplate siteTemplate, bool createSubSite)
        {
            if (!createSubSite)
            {
                if (Convert.ToBoolean(siteTemplate.SubWebOnly) == true)
                {
                    throw new SiteProvisioningException(String.Format("The template {0} has been marked for sub site creation only. You cannot use it to create a root web (site collection)", siteTemplate.Name));
                }
            }
            else
            {
                if (Convert.ToBoolean(siteTemplate.RootWebOnly) == true)
                {
                    throw new SiteProvisioningException(String.Format("The template {0} has been marked for root web (site collection) creation only. You cannot use it to create a sub site", siteTemplate.Name));
                }
            }
        }

        private bool AreWeCreatingASubSite(SiteRequestInformation siteToCreate)
        {
            var url = new Uri(siteToCreate.Url);
            var urlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
            int idx = url.PathAndQuery.Substring(1).IndexOf("/") + 2;
            var urlPath = url.PathAndQuery.Substring(0, idx);
            var name = url.PathAndQuery.Substring(idx);
            var index = name.IndexOf('/');

            if (index == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string GetParentSite(string siteUrl)
        {
            var url = new Uri(siteUrl);
            var urlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
            int idx = url.PathAndQuery.Substring(1).IndexOf("/") + 2;
            var urlPath = url.PathAndQuery.Substring(0, idx);
            var name = url.PathAndQuery.Substring(idx);
            var index = name.IndexOf('/');

            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}{1}{2}", urlDomain, urlPath, name.Split("/".ToCharArray())[0]);
        }

        private string GetSubSiteName(string siteUrl)
        {
            var url = new Uri(siteUrl);
            var urlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
            int idx = url.PathAndQuery.Substring(1).IndexOf("/") + 2;
            var urlPath = url.PathAndQuery.Substring(0, idx);
            var name = url.PathAndQuery.Substring(idx);
            var index = name.IndexOf('/');

            return name.Substring(index + 1);
        }
        #endregion
    }
}

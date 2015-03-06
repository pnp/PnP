using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Application;
using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Utilities;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.InformationPolicy;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core
{
    /// <summary>
    /// Implementation class for Provisioning Office 365 Site Collections
    /// </summary>
    public class Office365ProvisioningService : AbstractProvisioningService
    {
        #region Instance Members
        const string LOGGING_SOURCE = "ProvisioningService";
        IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
        AppSettings _settings = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Office365ProvisioningService() : base()
        {
            IAppSettingsManager _appManager = _configFactory.GetAppSetingsManager();
            _settings = _appManager.GetAppSettings();
        }
        #endregion
               
        #region ISharePointService Members
        //public virtual void UsingContext(Action<ClientContext> action)
        //{
        //    UsingContext(action, Timeout.Infinite);
        //}

        //public virtual void UsingContext(Action<ClientContext> action, int csomTimeout)
        //{
        //    using (ClientContext _ctx = Authentication.GetAuthenticatedContext())
        //    {
        //        _ctx.RequestTimeout = csomTimeout;
        //        action(_ctx);
        //    }
        //}
        #endregion

        /// <summary>
        /// See Framework.Provisioning.Core.IProvisioningService
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public override Guid? ProvisionSite(SiteRequestInformation properties)
        {
            Guid? _guid = Guid.Empty;
            var _tf = _configFactory.GetTemplateFactory();
            var _tm = _tf.GetTemplateManager();
            var _template = _tm.GetTemplateByID(properties.Template);

            UsingContext(ctx =>
            {
                try
                {
                    Tenant _tenant = new Tenant(ctx);
                    var _newsite = new SiteCreationProperties();
                    _newsite.Title = properties.Title;
                    _newsite.Url = properties.Url;
                    _newsite.Owner = properties.SiteOwner.Email;
                    _newsite.Template = _template.RootTemplate;
                    _newsite.Lcid = properties.Lcid;
                    _newsite.TimeZoneId = properties.TimeZoneId;
                    _newsite.StorageMaximumLevel = _template.StorageMaximumLevel;
                    _newsite.StorageWarningLevel = _template.StorageWarningLevel;
                    _newsite.UserCodeMaximumLevel = _template.UserCodeMaximumLevel;
                    _newsite.UserCodeMaximumLevel = _template.UserCodeWarningLevel;

                    SpoOperation op = _tenant.CreateSite(_newsite);
                    ctx.Load(_tenant);
                    ctx.Load(op, i => i.IsComplete);
                    ctx.ExecuteQuery();

                    while (!op.IsComplete)
                    {
                        //wait 30seconds and try again
                        System.Threading.Thread.Sleep(30000);
                        op.RefreshLoad();
                        ctx.ExecuteQuery();
                    }

                    var _site = _tenant.GetSiteByUrl(properties.Url);
                    var _web = _site.RootWeb;
                    ctx.Load(_web);
                    this.SetPropertyBag(_web, Constants.PropertyBags.SITE_TEMPLATE_TYPE, properties.Template);
                }
                catch(Exception ex)
                {
                    Log.Fatal("Framework.Provisioning.Core.Office365ProvisioningService.ProvisionSite", "An Error occured occured while process the site request for {0}. The Error is {1}.", properties.Url, ex.Message);
                    throw;
                }
            });

            this.SetSiteDescription(properties);
            this.SetAdministrators(properties);
            if(properties.EnableExternalSharing)
            {
                this.SetExternalSharing(properties.Url);
            }
     
            if(!string.IsNullOrEmpty(properties.SitePolicy))
            {
                this.ApplySitePolicy(properties.Url, properties.SitePolicy);
            }
     
            _guid = this.GetSiteGuidByUrl(properties.Url);
            return _guid;
        }

        /// <summary>
        /// Sets External Sharing if requested
        /// </summary>
        /// <param name="properties"></param>
        public virtual void SetExternalSharing(string siteUrl)
        {
            UsingContext(ctx =>
            {
                bool canBeUpdated = false;

                var tenant = new Tenant(ctx);
                var siteProperties = tenant.GetSitePropertiesByUrl(siteUrl, false);
                ctx.Load(tenant);
                ctx.Load(siteProperties);
                ctx.ExecuteQuery();

                var globalSharingCapability = tenant.SharingCapability;
                var currentSharingCapability = siteProperties.SharingCapability;
                var targetSharingCapability = SharingCapabilities.Disabled;

                if (globalSharingCapability != SharingCapabilities.Disabled)
                {
                    targetSharingCapability = SharingCapabilities.ExternalUserSharingOnly;
                    canBeUpdated = true;
                }
                if (currentSharingCapability != targetSharingCapability && canBeUpdated)
                {
                    siteProperties.SharingCapability = targetSharingCapability;
                    siteProperties.Update();
                    ctx.ExecuteQuery();
                }
            });
        }
       
        /// <summary>
        /// Adds additional Administrators to the site collection. These users will also be associated in the Default Owners
        /// group of the site.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="siteUrl"></param>
        public override void AddAdditionalAdministrators(List<AdditionalAdministrator> users, Uri siteUrl)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(siteUrl.ToString());
                ctx.ExecuteQuery();

                var web = site.RootWeb;
                foreach (var user in users)
                {
                    try
                    {
                        var spuser = web.EnsureUser(user.Name);
                        ctx.Load(spuser);
                        ctx.ExecuteQuery();

                        tenant.SetSiteAdmin(siteUrl.ToString(), spuser.LoginName, true);
                        web.AssociatedOwnerGroup.Users.AddUser(spuser);
                        web.AssociatedOwnerGroup.Update();
                        ctx.ExecuteQuery();
                        Log.Debug("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalAdministrators", "Added {0} as Site Collection admin to {1}", user.Name, siteUrl);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalAdministrators", "Failed to add {0} as admin of {1}. Message {2}",
                            user.Name,
                            siteUrl,
                            ex.Message);
                    }
                }
            });
        }

        /// <summary>
        /// Adds additional users to the default members group
        /// </summary>
        /// <param name="users"></param>
        /// <param name="siteUrl"></param>
        public override void AddAdditionalOwners(List<Owner> users, Uri siteUrl)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(siteUrl.ToString());
                ctx.ExecuteQuery();

                var web = site.RootWeb;
                foreach (var user in users)
                {
                    try
                    {
                        var userToAdd = web.EnsureUser(user.Name);
                        web.AssociatedOwnerGroup.Users.AddUser(userToAdd);
                        web.AssociatedOwnerGroup.Update();
                        ctx.ExecuteQuery();
                        Log.Debug("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalOwners", "Added {0} as Owner to {1}", user.Name, siteUrl);
                    }
                    catch
                    {
                        Log.Error("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalOwners", "Failed to add {0} as Owner of {1}", user.Name, siteUrl);
                    }
                }
            });
        }

        /// <summary>
        /// Adds additional users to the default Members group in the site collection.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="siteUrl"></param>
        public override void AddAdditionalMembers(List<Member> users, Uri siteUrl)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(siteUrl.ToString());
                ctx.ExecuteQuery();
                var web = site.RootWeb;

                foreach (var user in users)
                {
                    try
                    {
                        var userToAdd = web.EnsureUser(user.Name);
                        web.AssociatedMemberGroup.Users.AddUser(userToAdd);
                        web.AssociatedMemberGroup.Update();
                        ctx.ExecuteQuery();
                        Log.Debug("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalMembers", "Added {0} as Member to {1}", user.Name, siteUrl);
                    }
                    catch
                    {
                        Log.Error("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalMembers", "Failed to add {0} as Owner of {1}", user.Name, siteUrl);
                    }
                }
            });
        }

        /// <summary>
        /// Adds additional users to the default Vistors group in the site collection.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="siteUrl"></param>
        public override void AddAdditionalVisitors(List<Vistor> users, Uri siteUrl)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(siteUrl.ToString());
                ctx.ExecuteQuery();

                var web = site.RootWeb;

                foreach (var user in users)
                {
                    try
                    {
                        var userToAdd = web.EnsureUser(user.Name);
                        web.AssociatedVisitorGroup.Users.AddUser(userToAdd);
                        web.AssociatedVisitorGroup.Update();
                        ctx.ExecuteQuery();
                        Log.Debug("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalVisitors", "Added {0} as a Visitor to {1}", user.Name, siteUrl);
                    }
                    catch
                    {
                        Log.Error("Framework.Provisioning.Core.Office365ProvisioningService.AddAdditionalVisitors", "Failed to add {0} as a Visitor of {1}", user.Name, siteUrl);
                    }
                }
            });
        }

        /// <summary>
        /// Activate site feature
        /// </summary>
        /// <param name="url"></param>
        /// <param name="featureID"></param>
        public override void ActivateSiteFeature(string url, Guid featureID)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);
                ctx.ExecuteQuery();

                if (!site.IsFeatureActive(featureID))
                {
                    site.ActivateFeature(featureID);
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.ActivateSiteFeature", "Activating Site Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.ActivateSiteFeature", "Feature ID {0} is already acvitated on site {1}", featureID, url);
                }
            });
        }

        /// <summary>
        /// Activate web feature
        /// </summary>
        /// <param name="url"></param>
        /// <param name="featureID"></param>
        public override void ActivateWebFeature(string url, Guid featureID)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);
                ctx.ExecuteQuery();
                var web = site.RootWeb;
      
                if (web.IsFeatureActive(featureID))
                {
                    web.ActivateFeature(featureID);
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.ActivateWebFeature", "Activating Web Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.ActivateWebFeature", "Feature ID {0} is already activated on site {1}", featureID, url);
                }
            });
        }

        /// <summary>
        /// Deactive Site Feature
        /// </summary>
        /// <param name="url"></param>
        /// <param name="featureID"></param>
        public override void DeactivateSiteFeature(string url, Guid featureID)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);
                ctx.ExecuteQuery();

                if (site.IsFeatureActive(featureID))
                {
                    site.DeactivateFeature(featureID);
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeactivateSiteFeature", "Dectivating Site Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeactivateSiteFeature", "Feature ID {0} is not activitated on site {1}", featureID, url);
                }
            });
        }

        /// <summary>
        /// Deactive web feature
        /// </summary>
        /// <param name="url"></param>
        /// <param name="featureID"></param>
        public override void DeactivateWebFeature(string url, Guid featureID)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);
                ctx.ExecuteQuery();

                var web = site.RootWeb;
                if (web.IsFeatureActive(featureID))
                {
                    web.DeactivateFeature(featureID);
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeactivateWebFeature", "Deactivating Web Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeactivateWebFeature", "Feature ID {0} is not activated on site {1}", featureID, url);
                }

            });
        }
        
        #region Branding
        /// <summary>
        /// Deploys BrandingPackage to the Site
        /// </summary>
        /// <param name="url">The Url of the Site</param>
        /// <param name="theme">The BrandingPackage to apply</param>
        public override void DeployTheme(string url, BrandingPackage theme)
        {
            UsingContext(ctx =>
            {
                try
                {
                    Tenant tenant = new Tenant(ctx);
                    var site = tenant.GetSiteByUrl(url);
                    var web = site.RootWeb;
                    ctx.Load(site);
                    ctx.Load(web);
                    ctx.ExecuteQuery();

                    if (!string.IsNullOrEmpty(theme.ColorFile)) {
                        web.UploadThemeFile(theme.ColorFile);
                        Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeployTheme", "Completed Uploading Color File {0}: for site {1}", theme.ColorFile, url);
                    }

                    if (!string.IsNullOrEmpty(theme.FontFile)) {
                        web.UploadThemeFile(theme.FontFile);
                        Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeployTheme", "Completed Uploading Font File {0}: for site {1}", theme.FontFile, url);
                    }

                    if (!string.IsNullOrEmpty(theme.BackgroundFile)) {
                        web.UploadThemeFile(theme.BackgroundFile);
                        Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeployTheme", "Completed Uploading BackgroundFile {0}: for site {1}", theme.BackgroundFile, url);
                    }
                       
                    web.CreateComposedLookByName(theme.Name,
                        theme.ColorFile,
                        theme.FontFile,
                        theme.BackgroundFile,
                        theme.MasterPage);

                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeployTheme", "Created Composed look {0} for site {1}", theme.Name, url);
                  
                    
                    web.SetComposedLookByUrl(theme.Name);
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeployTheme", "Seting theme {0} for site {1}", theme.Name, url);

                    this.ApplyCSS(web, theme);
                    this.ApplySiteLogo(web, theme);

                    this.SetPropertyBag(web, Constants.PropertyBags.BRANDING_THEME_NAME, theme.Name);
                    this.SetPropertyBag(web, Constants.PropertyBags.BRANDING_VERSION, theme.Version);

                }
                catch (ServerException ex)
                {
                    Log.Error("Framework.Provisioning.Core.Office365ProvisioningService.DeployTheme", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  ServerErrorTraceCorrelationId: {2} Exception: {3} Stack: {4} ",
                        url,
                        ctx.TraceCorrelationId,
                        ex.ServerErrorTraceCorrelationId,
                        ex,
                        ex.ServerStackTrace);
                }
                catch (Exception ex)
                {
                    Log.Error("Framework.Provisioning.Core.Office365ProvisioningService.DeployTheme", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  Exception: {2} Stack: {3} ",
                        url,
                        ctx.TraceCorrelationId,
                        ex,
                        ex.StackTrace);
                }
        });
        }

        #endregion

        #region Custom Actions
        /// <summary>
        /// Deploys CustomActions to the Site Collection
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customAction"></param>
        public override void DeployWebCustomAction(string url, CustomActionEntity customAction)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);
                var web = site.RootWeb;
                //PNP extension calls execute query 
                web.AddCustomAction(customAction);
            });
        }

        /// <summary>
        /// Deploys Site Custom Actions
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customAction"></param>
        public override void DeploySiteCustomAction(string url, CustomActionEntity customAction)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);
                //PNP extension calls execute query 
                site.AddCustomAction(customAction);
            });
        }
        #endregion

        #region Fields, Content Types, &  Libraries
        /// <summary>
        /// Deploys Fields to the Site 
        /// </summary>
        /// <param name="url">The Site Url</param>
        /// <param name="fieldXML">Represents a field XML element of the field</param>
        public override void DeployFields(string url, string fieldXML)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);

                var web = site.RootWeb;
                //PNP extension calls execute query 
                web.CreateFieldsFromXMLString(fieldXML);
            });
        }

        /// <summary>
        /// Deploys Content Types to a site
        /// </summary>
        /// <param name="url">Url of the site</param>
        /// <param name="contentTypeXML">Represents a content type xml element</param>
        public override void DeployContentType(string url, string contentTypeXML)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);

                var web = site.RootWeb;
                //PNP extension calls execute query 
                web.CreateContentTypeFromXMLString(contentTypeXML);
            });
        }

        /// <summary>
        /// Creates a List/Libary in the site
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="listToProvision">An object that represents the List to create</param>
        public override void DeployList(string url, ListInstance listToProvision)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);

                var web = site.RootWeb;

                //PNP extension calls execute query Check to see if the list exists
                if (!web.ListExists(listToProvision.Title))
                {
                    ListCreationInformation _lcInfo = new ListCreationInformation();
                    _lcInfo.Title = listToProvision.Title;
                    _lcInfo.Description = listToProvision.Description;
                    _lcInfo.Url = listToProvision.Url;
                    _lcInfo.TemplateType = listToProvision.TemplateType;
                    if (listToProvision.OnQuickLaunch)
                    {
                        _lcInfo.QuickLaunchOption = QuickLaunchOptions.On;
                    }
                    else
                    {
                        _lcInfo.QuickLaunchOption = QuickLaunchOptions.Off;
                    }

                    var _list = web.Lists.Add(_lcInfo);
                    ctx.ExecuteQuery();

                    if (listToProvision.EnableVersioning)
                    {
                        _list.UpdateListVersioning(true, false, true);
                    }
                 
                    var _ctBindings = listToProvision.GetContentTypeBindings;
                    if (_ctBindings.Count > 0)
                    {
                        _list.ContentTypesEnabled = true;
                        _list.Update();
                        ctx.ExecuteQuery();
                        ///check to remove the content types from the list
                        if (listToProvision.RemoveDefaultContentType)
                        {
                            ContentTypeCollection _cts = _list.ContentTypes;
                            ctx.Load(_cts);
                            ctx.ExecuteQueryRetry();
                            foreach (var contentType in _cts)
                            {
                                contentType.DeleteObject();
                            }
                            _list.Update();
                            ctx.ExecuteQueryRetry();
                        }

                        foreach (var ctBinding in listToProvision.GetContentTypeBindings)
                        {
                            _list.AddContentTypeToListById(ctBinding.ContentTypeID, ctBinding.Default);
                        }
                    }
                }
                else
                {
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.DeployList", "Unable to provision List {0} in Site {1}", listToProvision.Title, url);
                }
            });
        }
        #endregion

        #region Site Policy
        /// <summary>
        /// <seealso cref="Framework.Provisioning.Core.IProvisioningService"/> 
        /// </summary>
        public override void ApplySitePolicy(string url, string policyName)
        {
            UsingContext(ctx =>
            {
                try
                {
                    Log.Info("Framework.Provisioning.Core.Office365ProvisioningService.ApplySitePolicy", "Appling Site Policy {0} on Site {1}", policyName, url);
               
                    Tenant tenant = new Tenant(ctx);
                    var site = tenant.GetSiteByUrl(url);
                    var web = site.RootWeb;
                    web.ApplySitePolicy(policyName);
                }
                catch(Exception ex)
                {
                    Log.Error("Framework.Provisioning.Core.Office365ProvisioningService.ApplySitePolicy", "Unable to Apply Site Policy {0} on Site {1}. Exception {2}", policyName, url, ex);
                }
            });
        }
        #endregion


     
    }
}

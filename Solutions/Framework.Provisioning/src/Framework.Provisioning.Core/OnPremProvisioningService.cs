using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Application;
using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Utilities;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
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
    /// Site Provisioning Service Implementation for On-premises and Office 365 SPO-D Legacy
    /// </summary>
    public class OnPremProvisioningService : AbstractProvisioningService, ISharePointService
    {
        #region Instance Members
        const string LOGGING_SOURCE = "OnPremProvisioningService";
        IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
        AppSettings _settings = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public OnPremProvisioningService() : base()
        {
            IAppSettingsManager _appManager = _configFactory.GetAppSetingsManager();
            _settings = _appManager.GetAppSettings();
        }
        #endregion

        /// <summary>
        /// Provisions a site collection
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public override Guid? ProvisionSite(SiteRequestInformation properties)
        {
            Guid? _guid= Guid.Empty;
            var _tf = _configFactory.GetTemplateFactory();
            var _tm = _tf.GetTemplateManager();
            var _template = _tm.GetTemplateByID(properties.Template);
            Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.ProvisionSite", "Provisioning Site with url {0}", properties.Url);

            SiteCreationProperties _newsite;
            try
            {
                UsingContext(ctx =>
                {
                    Tenant _tenant = new Tenant(ctx);
                     _newsite = new SiteCreationProperties();
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
                    _tenant.CreateSite(_newsite);
                    ctx.ExecuteQuery();


                    Tenant tenant = new Tenant(ctx);
                    var site = tenant.GetSiteByUrl(properties.Url);
                  
                    using (var _cloneCtx = site.Context.Clone(properties.Url))
                    {
                        var _web = _cloneCtx.Site.RootWeb;
                        _cloneCtx.Load(_web);
                        this.SetPropertyBag(_web, Constants.PropertyBags.SITE_TEMPLATE_TYPE, properties.Template);
                    }
                    

                }, 1200000);
            }
            catch(Exception ex)
            {
                Log.Fatal("Framework.Provisioning.Core.OnPremProvisioningService.ProvisionSite", "An Error occured occured while process the site request for {0}. The Error is {1}. Inner Exception {2}", properties.Url, ex, ex.InnerException);
                throw;
            }
            Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.ProvisionSite", "Site Collection {0} created:", properties.Url);

           
            this.HandleDefaultGroups(properties);
            this.SetSiteDescription(properties);
            this.SetAdministrators(properties);
            _guid = this.GetSiteGuidByUrl(properties.Url);
            if (!string.IsNullOrEmpty(properties.SitePolicy))
            {
                this.ApplySitePolicy(properties.Url, properties.SitePolicy);
            }
            return _guid;
        }
       
        /// <summary>
        /// With on-premieses builds default groups are not created during the site provisoning 
        /// so we have to create them.
        /// </summary>
        /// <param name="properties"></param>
        public virtual void HandleDefaultGroups(SiteRequestInformation properties)
        {            
            //Shoud use a resource file
            string _ownerGroupFormat = "{0} Owners";
            string _memberGroupFormat = "{0} Members";
            string _visitorGroupFormat = "{0} Visitors";

            string _ownerGroupDisplayName =string.Format(_ownerGroupFormat, properties.Title);
            string _memberGroupDisplayName = string.Format(_memberGroupFormat, properties.Title);
            string _vistorGroupDisplayName = string.Format(_visitorGroupFormat, properties.Title);

            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(properties.Url);
                var web = site.RootWeb;

                ctx.Load(web.AssociatedOwnerGroup);
                ctx.Load(web.AssociatedMemberGroup);
                ctx.Load(web.AssociatedVisitorGroup);
                ctx.ExecuteQuery();

                Group _ownerGroup;
                Group _memberGroup;
                Group _visitorGroup;
                if (web.AssociatedOwnerGroup.ServerObjectIsNull == true)
                {
                    _ownerGroup = web.AddGroup(_ownerGroupDisplayName, "Use this group to grant people full control permissions to the SharePoint site", true, false);
                }
                else
                {
                    _ownerGroup = web.AssociatedOwnerGroup;
                }
                if (web.AssociatedMemberGroup.ServerObjectIsNull == true)
                {
                    _memberGroup = web.AddGroup(_memberGroupDisplayName, "Use this group to grant people full control permissions to the SharePoint site", false, false);
                }
                else
                {
                    _memberGroup = web.AssociatedMemberGroup;
                }
                if (web.AssociatedVisitorGroup.ServerObjectIsNull == true)
                {
                        _visitorGroup = web.AddGroup(_vistorGroupDisplayName, "Use this group to grant people full control permissions to the SharePoint site", false, false );
                }
                else
                {
                    _visitorGroup = web.AssociatedVisitorGroup;
                }

                web.AssociateDefaultGroups(_ownerGroup, _memberGroup, _visitorGroup);
                ctx.ExecuteQuery();
                Log.Debug("Framework.Provisioning.Core.OnPremProvisioningService.HandleDefaultGroups", "Default Groups for site {0} created:", properties.Url);

                using (var newSiteCtx = ctx.Clone(properties.Url))
                {
                    newSiteCtx.Web.AddPermissionLevelToGroup(_ownerGroupDisplayName, RoleType.Administrator);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_memberGroupDisplayName, RoleType.Editor);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_vistorGroupDisplayName, RoleType.Reader);
                    newSiteCtx.ExecuteQuery();
                    Log.Debug("Framework.Provisioning.Core.OnPremProvisioningService.HandleDefaultGroups", "Setting group Security Permissions for {0}, {1}, {2}.", _ownerGroupDisplayName, _memberGroupDisplayName, _vistorGroupDisplayName);
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

                ////Have to do this for SPO-D and on-prem
                using (var siteCtx = site.Context.Clone(siteUrl.ToString()))
                {
                    var web = siteCtx.Web;
                    foreach (var user in users)
                    {
                        try
                        {
                            var spuser = web.EnsureUser(user.Name);
                            siteCtx.Load(spuser);
                            siteCtx.ExecuteQuery();

                            tenant.SetSiteAdmin(siteUrl.ToString(), spuser.LoginName, true);
                            web.AssociatedOwnerGroup.Users.AddUser(spuser);
                            web.AssociatedOwnerGroup.Update();
                            siteCtx.ExecuteQuery();
                            ctx.ExecuteQuery();
                            Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalAdministrators", "Added {0} as Site Collection admin to {1}", user.Name, siteUrl);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalAdministrators", "Failed to add {0} as admin of {1}. Message {2}",
                                user.Name,
                                siteUrl,
                                ex.Message);
                        }
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

                using (var siteCtx = site.Context.Clone(siteUrl.ToString()))
                {
                    var web = siteCtx.Web;
                    foreach (var user in users)
                    {
                        try
                        {
                            var userToAdd = web.EnsureUser(user.Name);
                            web.AssociatedOwnerGroup.Users.AddUser(userToAdd);
                            web.AssociatedOwnerGroup.Update();
                            siteCtx.ExecuteQuery();
                            Log.Debug("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalOwners", "Added {0} as Owner to {1}", user.Name, siteUrl);
                        }
                        catch
                        {
                            Log.Error("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalOwners", "Failed to add {0} as Owner of {1}", user.Name, siteUrl);
                        }
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

                using (var siteCtx = site.Context.Clone(siteUrl.ToString()))
                {
                    var web = siteCtx.Web;
                    foreach (var user in users)
                    {
                        try
                        {
                            var userToAdd = web.EnsureUser(user.Name);
                            web.AssociatedMemberGroup.Users.AddUser(userToAdd);
                            web.AssociatedMemberGroup.Update();
                            siteCtx.ExecuteQuery();
                            Log.Debug("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalMembers", "Added {0} as Member to {1}", user.Name, siteUrl);
                        }
                        catch
                        {
                            Log.Error("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalMembers", "Failed to add {0} as Owner of {1}", user.Name, siteUrl);
                        }
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

                using (var siteCtx = site.Context.Clone(siteUrl.ToString()))
                {
                    var web = siteCtx.Web;
                    foreach (var user in users)
                    {
                        try
                        {
                            var userToAdd = web.EnsureUser(user.Name);
                            web.AssociatedVisitorGroup.Users.AddUser(userToAdd);
                            web.AssociatedVisitorGroup.Update();
                            siteCtx.ExecuteQuery();
                            Log.Debug("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalVisitors", "Added {0} as a Visitor to {1}", user.Name, siteUrl);
                        }
                        catch
                        {
                            Log.Error("Framework.Provisioning.Core.OnPremProvisioningService.AddAdditionalVisitors", "Failed to add {0} as a Visitor of {1}", user.Name, siteUrl);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Activate site features. 
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
                 
                 using(var siteCtx = site.Context.Clone(url))
                 {
                     var _newSite = siteCtx.Site;
                     if (!_newSite.IsFeatureActive(featureID))
                     {
                         _newSite.ActivateFeature(featureID);
                         Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.ActivateSiteFeature", "Activating Site Feature ID {0} on site {1}", featureID, url);
                     }
                     else
                     {
                         Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.ActivateSiteFeature", "Feature ID {0} is already acvitated on site {1}", featureID, url);
                     }
                 }
             });
        }
        /// <summary>
        /// Activate web features.
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

                using (var siteCtx = site.Context.Clone(url))
                {
                    var _newWeb = siteCtx.Site.RootWeb;
                   
                    if (!_newWeb.IsFeatureActive(featureID))
                    {
                        _newWeb.ActivateFeature(featureID);
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.ActivateWebFeature", "Activating Web Feature ID {0} on site {1}", featureID, url);
                    }
                    else
                    {
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.ActivateWebFeature", "Feature ID {0} is already activated on site {1}", featureID, url);
                    }
                }
            });
        }

        /// <summary>
        /// Deactivate SiteFeature
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

                using (var siteCtx = site.Context.Clone(url))
                {
                    var _newSite = siteCtx.Site;
                    if (_newSite.IsFeatureActive(featureID))
                    {
                        _newSite.DeactivateFeature(featureID);
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeactivateSiteFeature", "Deactivating Site Feature ID {0} on site {1}", featureID, url);
                    }
                    else
                    {
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeactivateSiteFeature", "Feature ID {0} is not activated on site {1}", featureID, url);
                    }
                }
            });
        }
        /// <summary>
        /// Deactivate Web Features
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

                using (var siteCtx = site.Context.Clone(url))
                {
                    var _newWeb = siteCtx.Site.RootWeb;

                    if (_newWeb.IsFeatureActive(featureID))
                    {
                        _newWeb.DeactivateFeature(featureID);
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeactivateWebFeature", "Deactivating Web Feature ID {0} on site {1}", featureID, url);
                    }
                    else
                    {
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeactivateWebFeature", "Feature ID {0} is not activated on site {1}", featureID, url);
                    }
                }
            });
        }
      
        #region Branding
        /// <summary>
        /// Deploys BrandingPackage to the Site using Composed Looks
        /// </summary>
        /// <param name="url">The Url of the Site</param>
        /// <param name="theme">The BrandingPackage to apply</param>
        public override void DeployTheme(string url, BrandingPackage theme)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);
                ctx.ExecuteQuery();
                
                using(var siteCtx = site.Context.Clone(url))
                {
                    try
                    {
                        var siteWeb = siteCtx.Web;
                        siteCtx.Load(siteWeb);
                        siteCtx.ExecuteQuery();

                        if(!string.IsNullOrEmpty(theme.ColorFile)) {
                            siteWeb.UploadThemeFile(theme.ColorFile);
                            Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Completed Uploading Color File {0}: for site {1}", theme.ColorFile, url);
                        }

                        if (!string.IsNullOrEmpty(theme.FontFile)) {
                            siteWeb.UploadThemeFile(theme.FontFile);
                            Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Completed Uploading Font File {0}: for site {1}", theme.FontFile, url);
                        }

                        if (!string.IsNullOrEmpty(theme.BackgroundFile)) { 
                            siteWeb.UploadThemeFile(theme.BackgroundFile);
                            Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Completed Uploading BackgroundFile {0}: for site {1}", theme.BackgroundFile, url);
                        }

                        siteWeb.CreateComposedLookByName(theme.Name,
                            theme.ColorFile,
                            theme.FontFile,
                            theme.BackgroundFile,
                            theme.MasterPage);


                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Created Composed look {0} for site {1}", theme.Name, url);
                        siteWeb.SetComposedLookByUrl(theme.Name);
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Seting theme {0} for site {1}", theme.Name, url);

                        this.ApplyCSS(siteWeb, theme);
                        this.ApplySiteLogo(siteWeb, theme);
                       
                        this.SetPropertyBag(siteWeb, Constants.PropertyBags.BRANDING_THEME_NAME, theme.Name);
                        this.SetPropertyBag(siteWeb, Constants.PropertyBags.BRANDING_VERSION, theme.Version);

                    }
                    catch(ServerException ex)
                    {
                        Log.Error("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  ServerErrorTraceCorrelationId: {2} Message: {3} Stack: {4} ",
                            url,
                            ctx.TraceCorrelationId,
                            ex.ServerErrorTraceCorrelationId,
                            ex.Message,
                            ex.ServerStackTrace);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  Message: {2} Stack: {3} ",
                           url,
                           ctx.TraceCorrelationId,
                           ex.Message,
                           ex.StackTrace);
                    }
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

                using (var cloneCtx = site.Context.Clone(url))
                {
                    var web = cloneCtx.Site.RootWeb;
                    //PNP extension calls execute query 
                    web.AddCustomAction(customAction);
                }
            });
        }
        /// <summary>
        /// Deploys Custom actions at the Site Collection Level
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customAction"></param>
        public override void DeploySiteCustomAction(string url, CustomActionEntity customAction)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(url);

                using (var cloneCtx = site.Context.Clone(url))
                {
                   //PNP extension calls execute query 
                    cloneCtx.Site.AddCustomAction(customAction);
                }
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
                using (var cloneCtx = site.Context.Clone(url))
                {
                    var web = cloneCtx.Site.RootWeb;
                    //PNP extension calls execute query 
                    web.CreateFieldsFromXMLString(fieldXML);
                }
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

                using (var cloneCtx = site.Context.Clone(url))
                {
                    var web = cloneCtx.Site.RootWeb;
                    //PNP extension calls execute query 
                    web.CreateContentTypeFromXMLString(contentTypeXML);
                }
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

                using (var cloneCtx = site.Context.Clone(url))
                {
                    var web = cloneCtx.Site.RootWeb;
                    //PNP extension calls execute query Check to see if the list exists
                    if(!web.ListExists(listToProvision.Title))
                    {
                        ListCreationInformation _lcInfo = new ListCreationInformation();
                        _lcInfo.Title = listToProvision.Title;
                        _lcInfo.Description = listToProvision.Description;
                        _lcInfo.Url = listToProvision.Url;
                        _lcInfo.TemplateType = listToProvision.TemplateType;
                        if(listToProvision.OnQuickLaunch) {
                            _lcInfo.QuickLaunchOption = QuickLaunchOptions.On;
                        }
                        else {
                            _lcInfo.QuickLaunchOption = QuickLaunchOptions.Off;
                        }

                        var _list = web.Lists.Add(_lcInfo);
                        cloneCtx.ExecuteQuery();

                        if(listToProvision.EnableVersioning) {
                            _list.UpdateListVersioning(true, false, true);
                        }

                        var _ctBindings = listToProvision.GetContentTypeBindings;
                        if(_ctBindings.Count > 0) 
                        {
                            _list.ContentTypesEnabled = true;
                            _list.Update();
                            cloneCtx.ExecuteQuery();
                            ///check to remove the content types from the list
                            if (listToProvision.RemoveDefaultContentType)
                            {
                                ContentTypeCollection _cts = _list.ContentTypes;
                                cloneCtx.Load(_cts);
                                cloneCtx.ExecuteQueryRetry();
                                foreach(var contentType in _cts)
                                {
                                    contentType.DeleteObject();
                                }
                                _list.Update();
                                cloneCtx.ExecuteQueryRetry();
                            }

                            foreach (var ctBinding in listToProvision.GetContentTypeBindings) {
                                _list.AddContentTypeToListById(ctBinding.ContentTypeID, ctBinding.Default);
                            }
                        }
                    }
                    else {
                        Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.DeployTheme", "Unable to provision List {0} on Site {1}", listToProvision.Title, url);
                    }
                }
            });
        }
        #endregion

        #region Site Policy
        /// <summary>
        /// Member to apply the Site Policy to a site collection 
        /// <see cref="https://technet.microsoft.com/en-us/library/jj219569.aspx"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="policyName"></param>
        public override void ApplySitePolicy(string url, string policyName)
        {
           UsingContext(ctx =>
           {
               try
               {
                   Log.Info("Framework.Provisioning.Core.OnPremProvisioningService.ApplySitePolicy", "Appling Site Policy {0} on Site {1}", policyName, url);
                   Tenant tenant = new Tenant(ctx);
                   var site = tenant.GetSiteByUrl(url);
                   ctx.ExecuteQuery();

                   using (var siteCtx = site.Context.Clone(url))
                   {
                       siteCtx.Web.ApplySitePolicy(policyName);
                   }
               }
               catch(Exception ex)
               {
                   Log.Error("Framework.Provisioning.Core.OnPremProvisioningService.ApplySitePolicy", "Unable to Apply Site Policy {0} on Site {1}. Exception {2}", policyName, url, ex);
         
               }
           });
        }
        #endregion

    }
}

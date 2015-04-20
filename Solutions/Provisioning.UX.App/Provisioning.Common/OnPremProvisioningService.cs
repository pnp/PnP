using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using Provisioning.Common.Configuration.Template;
using Provisioning.Common.Utilities;
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

namespace Provisioning.Common
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
            //Guid? _guid= Guid.Empty;
            //var _tf = _configFactory.GetTemplateFactory();
            //var _tm = _tf.GetTemplateManager();
            //var _template  _tm.GetTemplateByID(properties.Template);
            //Log.Info("Provisioning.Common.OnPremProvisioningService.ProvisionSite", "Provisioning Site with url {0}", properties.Url);

            //SiteCreationProperties _newsite;
            //try
            //{
            //    UsingContext(ctx =>
            //    {
            //        Tenant _tenant = new Tenant(ctx);
            //         _newsite = new SiteCreationProperties();
            //        _newsite.Title = properties.Title;
            //        _newsite.Url = properties.Url;
            //        _newsite.Owner = properties.SiteOwner.Email;
            //        _newsite.Template = _template.RootTemplate;
            //        _newsite.Lcid = properties.Lcid;
            //        _newsite.TimeZoneId = properties.TimeZoneId;
            //        _newsite.StorageMaximumLevel = _template.StorageMaximumLevel;
            //        _newsite.StorageWarningLevel = _template.StorageWarningLevel;
            //        _newsite.UserCodeMaximumLevel = _template.UserCodeMaximumLevel;
            //        _newsite.UserCodeMaximumLevel = _template.UserCodeWarningLevel;
            //        _tenant.CreateSite(_newsite);
            //        ctx.ExecuteQuery();


            //        Tenant tenant = new Tenant(ctx);
            //        var site = tenant.GetSiteByUrl(properties.Url);
                  
            //        using (var _cloneCtx = site.Context.Clone(properties.Url))
            //        {
            //            var _web = _cloneCtx.Site.RootWeb;
            //            _cloneCtx.Load(_web);
            //            this.SetPropertyBag(_web, Constants.PropertyBags.SITE_TEMPLATE_TYPE, properties.Template);
            //        }
                    

            //    }, 1200000);
            //}
            //catch(Exception ex)
            //{
            //    Log.Fatal("Provisioning.Common.OnPremProvisioningService.ProvisionSite", "An Error occured occured while process the site request for {0}. The Error is {1}. Inner Exception {2}", properties.Url, ex, ex.InnerException);
            //    throw;
            //}
            //Log.Info("Provisioning.Common.OnPremProvisioningService.ProvisionSite", "Site Collection {0} created:", properties.Url);

           
            //this.HandleDefaultGroups(properties);
            //this.SetSiteDescription(properties);
            //this.SetAdministrators(properties);
            //_guid = this.GetSiteGuidByUrl(properties.Url);
            //if (!string.IsNullOrEmpty(properties.SitePolicy))
            //{
            //    this.ApplySitePolicy(properties.Url, properties.SitePolicy);
            //}
            //return _guid;

            return null;
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
                Log.Debug("Provisioning.Common.OnPremProvisioningService.HandleDefaultGroups", "Default Groups for site {0} created:", properties.Url);

                using (var newSiteCtx = ctx.Clone(properties.Url))
                {
                    newSiteCtx.Web.AddPermissionLevelToGroup(_ownerGroupDisplayName, RoleType.Administrator);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_memberGroupDisplayName, RoleType.Editor);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_vistorGroupDisplayName, RoleType.Reader);
                    newSiteCtx.ExecuteQuery();
                    Log.Debug("Provisioning.Common.OnPremProvisioningService.HandleDefaultGroups", "Setting group Security Permissions for {0}, {1}, {2}.", _ownerGroupDisplayName, _memberGroupDisplayName, _vistorGroupDisplayName);
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
                         Log.Info("Provisioning.Common.OnPremProvisioningService.ActivateSiteFeature", "Activating Site Feature ID {0} on site {1}", featureID, url);
                     }
                     else
                     {
                         Log.Info("Provisioning.Common.OnPremProvisioningService.ActivateSiteFeature", "Feature ID {0} is already acvitated on site {1}", featureID, url);
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
                        Log.Info("Provisioning.Common.OnPremProvisioningService.ActivateWebFeature", "Activating Web Feature ID {0} on site {1}", featureID, url);
                    }
                    else
                    {
                        Log.Info("Provisioning.Common.OnPremProvisioningService.ActivateWebFeature", "Feature ID {0} is already activated on site {1}", featureID, url);
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
                        Log.Info("Provisioning.Common.OnPremProvisioningService.DeactivateSiteFeature", "Deactivating Site Feature ID {0} on site {1}", featureID, url);
                    }
                    else
                    {
                        Log.Info("Provisioning.Common.OnPremProvisioningService.DeactivateSiteFeature", "Feature ID {0} is not activated on site {1}", featureID, url);
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
                        Log.Info("Provisioning.Common.OnPremProvisioningService.DeactivateWebFeature", "Deactivating Web Feature ID {0} on site {1}", featureID, url);
                    }
                    else
                    {
                        Log.Info("Provisioning.Common.OnPremProvisioningService.DeactivateWebFeature", "Feature ID {0} is not activated on site {1}", featureID, url);
                    }
                }
            });
        }
      
    

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
                   Log.Info("Provisioning.Common.OnPremProvisioningService.ApplySitePolicy", "Appling Site Policy {0} on Site {1}", policyName, url);
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
                   Log.Error("Provisioning.Common.OnPremProvisioningService.ApplySitePolicy", "Unable to Apply Site Policy {0} on Site {1}. Exception {2}", policyName, url, ex);
         
               }
           });
        }
        #endregion

    }
}

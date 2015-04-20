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
        /// See Provisioning.Common.IProvisioningService
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public override Guid? ProvisionSite(SiteRequestInformation properties)
        {
            return null;
        //{
        //    Guid? _guid = Guid.Empty;
        //    var _tf = _configFactory.GetTemplateFactory();
        //    var _tm = _tf.GetTemplateManager();
        //    var _template = _tm.GetTemplateByID(properties.Template);

        //    UsingContext(ctx =>
        //    {
        //        try
        //        {
        //            Tenant _tenant = new Tenant(ctx);
        //            var _newsite = new SiteCreationProperties();
        //            _newsite.Title = properties.Title;
        //            _newsite.Url = properties.Url;
        //            _newsite.Owner = properties.SiteOwner.Email;
        //            _newsite.Template = _template.RootTemplate;
        //            _newsite.Lcid = properties.Lcid;
        //            _newsite.TimeZoneId = properties.TimeZoneId;
        //            _newsite.StorageMaximumLevel = _template.StorageMaximumLevel;
        //            _newsite.StorageWarningLevel = _template.StorageWarningLevel;
        //            _newsite.UserCodeMaximumLevel = _template.UserCodeMaximumLevel;
        //            _newsite.UserCodeMaximumLevel = _template.UserCodeWarningLevel;

        //            SpoOperation op = _tenant.CreateSite(_newsite);
        //            ctx.Load(_tenant);
        //            ctx.Load(op, i => i.IsComplete);
        //            ctx.ExecuteQuery();

        //            while (!op.IsComplete)
        //            {
        //                //wait 30seconds and try again
        //                System.Threading.Thread.Sleep(30000);
        //                op.RefreshLoad();
        //                ctx.ExecuteQuery();
        //            }

        //            var _site = _tenant.GetSiteByUrl(properties.Url);
        //            var _web = _site.RootWeb;
        //            ctx.Load(_web);
        //            this.SetPropertyBag(_web, Constants.PropertyBags.SITE_TEMPLATE_TYPE, properties.Template);
        //        }
        //        catch(Exception ex)
        //        {
        //            Log.Fatal("Provisioning.Common.Office365ProvisioningService.ProvisionSite", "An Error occured occured while process the site request for {0}. The Error is {1}.", properties.Url, ex.Message);
        //            throw;
        //        }
            //});

            //this.SetSiteDescription(properties);
            //this.SetAdministrators(properties);
            //this.SetExternalSharing(properties);

            //if(!string.IsNullOrEmpty(properties.SitePolicy))
            //{
            //    this.ApplySitePolicy(properties.Url, properties.SitePolicy);
            //}
     
            //_guid = this.GetSiteGuidByUrl(properties.Url);
            //return _guid;
        }

        /// <summary>
        /// Sets External Sharing if requested
        /// </summary>
        /// <param name="properties"></param>
        public virtual void SetExternalSharing(SiteRequestInformation properties)
        {
            UsingContext(ctx =>
            {
                bool canBeUpdated = false;

                var tenant = new Tenant(ctx);
                var siteProperties = tenant.GetSitePropertiesByUrl(properties.Url, false);
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
                    Log.Info("Provisioning.Common.Office365ProvisioningService.ActivateSiteFeature", "Activating Site Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Provisioning.Common.Office365ProvisioningService.ActivateSiteFeature", "Feature ID {0} is already acvitated on site {1}", featureID, url);
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
                    Log.Info("Provisioning.Common.Office365ProvisioningService.ActivateWebFeature", "Activating Web Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Provisioning.Common.Office365ProvisioningService.ActivateWebFeature", "Feature ID {0} is already activated on site {1}", featureID, url);
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
                    Log.Info("Provisioning.Common.Office365ProvisioningService.DeactivateSiteFeature", "Dectivating Site Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Provisioning.Common.Office365ProvisioningService.DeactivateSiteFeature", "Feature ID {0} is not activitated on site {1}", featureID, url);
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
                    Log.Info("Provisioning.Common.Office365ProvisioningService.DeactivateWebFeature", "Deactivating Web Feature ID {0} on site {1}", featureID, url);
                }
                else
                {
                    Log.Info("Provisioning.Common.Office365ProvisioningService.DeactivateWebFeature", "Feature ID {0} is not activated on site {1}", featureID, url);
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

     
        #endregion

        #region Site Policy
        /// <summary>
        /// See Provisioning.Common.IProvisioningService
        /// </summary>
        public override void ApplySitePolicy(string url, string policyName)
        {
            UsingContext(ctx =>
            {
                try
                {
                    Log.Info("Provisioning.Common.Office365ProvisioningService.ApplySitePolicy", "Appling Site Policy {0} on Site {1}", policyName, url);
               
                    Tenant tenant = new Tenant(ctx);
                    var site = tenant.GetSiteByUrl(url);
                    var web = site.RootWeb;
                    web.ApplySitePolicy(policyName);
                }
                catch(Exception ex)
                {
                    Log.Error("Provisioning.Common.Office365ProvisioningService.ApplySitePolicy", "Unable to Apply Site Policy {0} on Site {1}. Exception {2}", policyName, url, ex);
                }
            });
        }
        #endregion


     
    }
}

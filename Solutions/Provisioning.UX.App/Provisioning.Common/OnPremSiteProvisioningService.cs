using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
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
using Provisioning.Common.Data.Templates;
using System.Diagnostics;

namespace Provisioning.Common
{
    /// <summary>
    /// Site Provisioning Service Implementation for On-premises and Office 365 SPO-D
    /// </summary>
    public class OnPremSiteProvisioningService : AbstractSiteProvisioningService, ISharePointClientService
    {
        #region Private Instance Members
        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public OnPremSiteProvisioningService() : base()
        {
        }
        #endregion
       
        /// <summary>
        /// With on-premises builds default groups are not created during site provisioning 
        /// so we have to create them.
        /// </summary>
        /// <param name="properties"></param>
        public virtual void HandleDefaultGroups(SiteInformation properties)
        {

            Log.Info("Provisioning.Common.OnPremSiteProvisioningService.HandleDefaultGroups", "Creating Groups for site {0} created" , properties.Url);
            string _ownerGroupDisplayName =string.Format(PCResources.Site_Web_OwnerGroup_Title, properties.Title);
            string _memberGroupDisplayName = string.Format(PCResources.Site_Web_MemberGroup_Title, properties.Title);
            string _vistorGroupDisplayName = string.Format(PCResources.Site_Web_VisitorGroup_Title, properties.Title);


            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();

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
                if (web.AssociatedOwnerGroup.ServerObjectIsNull == true) {
                    _ownerGroup = web.AddGroup(_ownerGroupDisplayName, PCResources.Site_Web_OwnerGroup_Description, true, false);
                }
                else {
                    _ownerGroup = web.AssociatedOwnerGroup;
                }
                if (web.AssociatedMemberGroup.ServerObjectIsNull == true) {
                    _memberGroup = web.AddGroup(_memberGroupDisplayName, PCResources.Site_Web_MemberGroup_Description, false, false);
                }
                else {
                    _memberGroup = web.AssociatedMemberGroup;
                }
                if (web.AssociatedVisitorGroup.ServerObjectIsNull == true) {
                        _visitorGroup = web.AddGroup(_vistorGroupDisplayName, PCResources.Site_Web_VisitorGroup_Description, false, false );
                }
                else {
                    _visitorGroup = web.AssociatedVisitorGroup;
                }

                web.AssociateDefaultGroups(_ownerGroup, _memberGroup, _visitorGroup);
                ctx.ExecuteQuery();


                Log.Info("Provisioning.Common.OnPremSiteProvisioningService.HandleDefaultGroups", PCResources.Site_Web_DefaultGroups_Created, properties.Url);

                using (var newSiteCtx = ctx.Clone(properties.Url))
                {
                    newSiteCtx.Web.AddPermissionLevelToGroup(_ownerGroupDisplayName, RoleType.Administrator);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_memberGroupDisplayName, RoleType.Editor);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_vistorGroupDisplayName, RoleType.Reader);
                    newSiteCtx.ExecuteQuery();
                   Log.Info("Provisioning.Common.OnPremSiteProvisioningService.HandleDefaultGroups", PCResources.Site_Web_Groups_Security_Permissions_Set, 
                        _ownerGroupDisplayName, 
                        _memberGroupDisplayName, 
                        _vistorGroupDisplayName);
                }

                _timespan.Stop();
                Log.TraceApi("SharePoint", "OnPremSiteProvisioningService.HandleDefaultGroups", _timespan.Elapsed, "SiteUrl={0}", properties.Url);

            });

        }

        public override void CreateSiteCollection(SiteInformation siteRequest, Template template)
        {
           Log.Info("Provisioning.Common.OnPremSiteProvisioningService.CreateSiteCollection", PCResources.SiteCreation_Creation_Starting, siteRequest.Url);
            
            Web _web = null;
            try
            {
                UsingContext(ctx =>
                {
                    Stopwatch _timespan = Stopwatch.StartNew();

                    Tenant _tenant = new Tenant(ctx);
                    var _newsite = new SiteCreationProperties();
                    _newsite.Title = siteRequest.Title;
                    _newsite.Url = siteRequest.Url;
                    _newsite.Owner = siteRequest.SiteOwner.Name;
                    _newsite.Template = template.RootTemplate;
                    _newsite.Lcid = siteRequest.Lcid;
                    _newsite.TimeZoneId = siteRequest.TimeZoneId;
                    _newsite.StorageMaximumLevel = template.StorageMaximumLevel;
                    _newsite.StorageWarningLevel = template.StorageWarningLevel;
                    _newsite.UserCodeMaximumLevel = template.UserCodeMaximumLevel;
                    _newsite.UserCodeMaximumLevel = template.UserCodeWarningLevel;
                    _tenant.CreateSite(_newsite);
                    ctx.ExecuteQuery();

                    Tenant tenant = new Tenant(ctx);
                    var site = tenant.GetSiteByUrl(siteRequest.Url);

                    using (var _cloneCtx = site.Context.Clone(siteRequest.Url))
                    {
                         _web = _cloneCtx.Site.RootWeb;
                         _web.Description = siteRequest.Description;
                         _web.Update();
                        _cloneCtx.Load(_web);
                        _cloneCtx.ExecuteQuery();
                    }

                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "OnPremSiteProvisioningService.CreateSiteCollection", _timespan.Elapsed, "SiteUrl={0}", siteRequest.Url);
                }, 1200000);

            }
            catch(Exception ex)
            {
                Log.Error("Provisioning.Common.OnPremSiteProvisioningService.CreateSiteCollection",
                    PCResources.SiteCreation_Creation_Failure, 
                    siteRequest.Url, 
                    ex,
                    ex.InnerException);
                throw;
            }
           
            Log.Info("Provisioning.Common.OnPremSiteProvisioningService.CreateSiteCollection", PCResources.SiteCreation_Creation_Successful, siteRequest.Url);
            this.HandleDefaultGroups(siteRequest);
        }

        /// <summary>
        /// Returns if External Sharing is enabled. 
        /// This is not supported in on-premises builds
        /// </summary>
        /// <returns></returns>
        public override bool IsTenantExternalSharingEnabled(string tenantUrl)
        {
            Log.Warning("Provisioning.Common.OnPremSiteProvisioningService.IsTenantExternalSharingEnabled", PCResources.ExternalSharing_NotSupported, tenantUrl);
            return false;
        }

        /// <summary>
        /// Sets External Sharing
        /// This is not supported in on-premises builds.
        /// </summary>
        /// <param name="url"></param>
        public override void SetExternalSharing(SiteInformation siteInfo)
        {
            Log.Warning("Provisioning.Common.OnPremSiteProvisioningService.SetExternalSharing", PCResources.ExternalSharing_NotSupported, siteInfo.Url);
            return;
        }

        public override Web CreateSubSite(SiteInformation siteRequest, Template template)
        {
            Web newWeb;
            int pos = siteRequest.Url.LastIndexOf("/");
            string parentUrl = siteRequest.Url.Substring(0, pos);
            string subSiteUrl = siteRequest.Url.Substring(pos + 1, siteRequest.Url.Length);

            Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite", PCResources.SiteCreation_Creation_Starting, siteRequest.Url);
            Uri siteUri = new Uri(siteRequest.Url);
            Uri subSiteParent = new Uri(parentUrl);

            string realm = TokenHelper.GetRealmFromTargetUrl(subSiteParent);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, subSiteParent.Authority, realm).AccessToken;

            using (var ctx = TokenHelper.GetClientContextWithAccessToken(parentUrl, accessToken))
            {
                try
                {
                    Stopwatch _timespan = Stopwatch.StartNew();

                    try
                    {
                        // Get a reference to the parent Web
                        Web parentWeb = ctx.Web;

                        // Create the new sub site as a new child Web
                        WebCreationInformation webinfo = new WebCreationInformation();
                        webinfo.Description = siteRequest.Description;
                        webinfo.Language = (int)siteRequest.Lcid;
                        webinfo.Title = siteRequest.Title;
                        webinfo.Url = subSiteUrl;
                        webinfo.UseSamePermissionsAsParentSite = true;
                        webinfo.WebTemplate = template.RootTemplate;

                        newWeb = parentWeb.Webs.Add(webinfo);
                        ctx.ExecuteQueryRetry();

                    }
                    catch (ServerException ex)
                    {
                        var _message = string.Format("Error occured while provisioning site {0}, ServerErrorTraceCorrelationId: {1} Exception: {2}", siteRequest.Url, ex.ServerErrorTraceCorrelationId, ex);
                        Log.Error("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite", _message);
                        throw;
                    }

                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "Office365SiteProvisioningService.CreateSubSite", _timespan.Elapsed, "SiteUrl={0}", siteRequest.Url);
                }

                catch (Exception ex)
                {
                    Log.Error("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite",
                        PCResources.SiteCreation_Creation_Failure,
                        siteRequest.Url, ex.Message, ex);
                    throw;
                }
                Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite", PCResources.SiteCreation_Creation_Successful, siteRequest.Url);

            };

            return newWeb;
        }
    }
}

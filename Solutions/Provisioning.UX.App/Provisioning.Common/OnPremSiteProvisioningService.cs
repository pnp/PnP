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

namespace Provisioning.Common
{
    /// <summary>
    /// Site Provisioning Service Implementation for On-premises and Office 365 SPO-D
    /// </summary>
    public class OnPremSiteProvisioningService : AbstractSiteProvisioningService, ISharePointService
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public OnPremSiteProvisioningService() : base()
        {
        }
        #endregion
       
        /// <summary>
        /// With on-premieses builds default groups are not created during site provisioning 
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
                if (web.AssociatedOwnerGroup.ServerObjectIsNull == true) {
                    _ownerGroup = web.AddGroup(_ownerGroupDisplayName, "Use this group to grant people full control permissions to the SharePoint site", true, false);
                }
                else {
                    _ownerGroup = web.AssociatedOwnerGroup;
                }
                if (web.AssociatedMemberGroup.ServerObjectIsNull == true) {
                    _memberGroup = web.AddGroup(_memberGroupDisplayName, "Use this group to grant people full control permissions to the SharePoint site", false, false);
                }
                else {
                    _memberGroup = web.AssociatedMemberGroup;
                }
                if (web.AssociatedVisitorGroup.ServerObjectIsNull == true) {
                        _visitorGroup = web.AddGroup(_vistorGroupDisplayName, "Use this group to grant people full control permissions to the SharePoint site", false, false );
                }
                else {
                    _visitorGroup = web.AssociatedVisitorGroup;
                }

                web.AssociateDefaultGroups(_ownerGroup, _memberGroup, _visitorGroup);
                ctx.ExecuteQuery();
                Log.Info("Provisioning.Common.OnPremSiteProvisioningService.HandleDefaultGroups", "Default Groups for site {0} created:", properties.Url);

                using (var newSiteCtx = ctx.Clone(properties.Url))
                {
                    newSiteCtx.Web.AddPermissionLevelToGroup(_ownerGroupDisplayName, RoleType.Administrator);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_memberGroupDisplayName, RoleType.Editor);
                    newSiteCtx.Web.AddPermissionLevelToGroup(_vistorGroupDisplayName, RoleType.Reader);
                    newSiteCtx.ExecuteQuery();
                    Log.Info("Provisioning.Common.OnPremSiteProvisioningService.HandleDefaultGroups", "Setting group Security Permissions for {0}, {1}, {2}.", 
                        _ownerGroupDisplayName, 
                        _memberGroupDisplayName, 
                        _vistorGroupDisplayName);
                }
            });

        }

        public override void CreateSiteCollection(SiteRequestInformation siteRequest, Template template)
        {
            Log.Info("Provisioning.Common.OnPremSiteProvisioningService.CreateSiteCollection", "Creating Site Collection with url {0}", siteRequest.Url);
            
            Web _web = null;
            try
            {
                UsingContext(ctx =>
                {
                    Tenant _tenant = new Tenant(ctx);
                    var _newsite = new SiteCreationProperties();
                    _newsite.Title = siteRequest.Title;
                    _newsite.Url = siteRequest.Url;
                    _newsite.Owner = siteRequest.SiteOwner.Email;
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
                }, 1200000);
            }
            catch(Exception ex)
            {
                Log.Error("Provisioning.Common.OnPremSiteProvisioningService.CreateSiteCollection", 
                    "An Error occured occured while process the site request for {0}. The Error is {1}. Inner Exception {2}", 
                    siteRequest.Url, 
                    ex,
                    ex.InnerException);
                throw;
            }
            Log.Info("Provisioning.Common.OnPremSiteProvisioningService.CreateSiteCollection", "Site Collection {0} created:", siteRequest.Url);
            this.HandleDefaultGroups(siteRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsTenantExternalSharingEnabled(string tenantUrl)
        {
            return false;
        }
    }
}

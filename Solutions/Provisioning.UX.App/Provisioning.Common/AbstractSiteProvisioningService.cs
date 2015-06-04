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
    /// Abstract Site Provisioning Service
    /// </summary>
    public abstract class AbstractSiteProvisioningService : ISiteProvisioning, ISharePointClientService
    {
        #region Properties
        /// <summary>
        /// Gets or Sets the services Authentication.
        /// </summary>
        public IAuthentication Authentication
        {
            get;
            set;
        }
        #endregion

        #region ISiteProvisioning Members
        public abstract void CreateSiteCollection(SiteRequestInformation siteRequest, Template template);

        public virtual bool IsTenantExternalSharingEnabled(string tenantUrl)
        {
            var _returnResult = false;
            UsingContext(ctx =>
            {
                Tenant _tenant = new Tenant(ctx);
                ctx.Load(_tenant);
                try
                { 
                    //IF calling SP ONPREM THIS WILL FAIL
                    ctx.ExecuteQuery();
                    //check sharing capabilities
                    if(_tenant.SharingCapability == SharingCapabilities.Disabled)
                    {
                        _returnResult = false;
                    }
                    else
                    {
                        _returnResult = true;
                    }
                                }
                catch(Exception ex)
                {
                    Log.Warning("Provisioning.Common.AbstractSiteProvisioningService.IsTenantExternalSharingEnabled", 
                        PCResources.ExternalSharing_Enabled_Error_Message, 
                        tenantUrl, 
                        ex);
                }
            });

            return _returnResult;
        }

        public abstract void SetExternalSharing(SiteRequestInformation siteInfo);

        public virtual SitePolicyEntity GetAppliedSitePolicy()
        {
            SitePolicyEntity _appliedSitePolicy = null;
            UsingContext(ctx =>
            {
                var _web = ctx.Web;
                _appliedSitePolicy = _web.GetAppliedSitePolicy();

            });
            return _appliedSitePolicy;
        }

        public virtual void SetSitePolicy(string policyName)
        {
            UsingContext(ctx =>
            {
                var _web = ctx.Web;
                bool _policyApplied = _web.ApplySitePolicy(policyName);
            });
        }

        public virtual List<SitePolicyEntity> GetAvailablePolicies()
        {
            List<SitePolicyEntity> _results = new List<SitePolicyEntity>();
            UsingContext(ctx =>
            {
                var _web = ctx.Web;
                _results = _web.GetSitePolicies();
            });
            return _results;
        }
  
        public Web GetWebByUrl(string url)
        {
            Web _web = null;
            UsingContext(ctx =>
            {
                _web = ctx.Site.RootWeb;
                ctx.Load(_web);
                ctx.ExecuteQuery();
            });

            return _web;
        }
     
        /// <summary>
        /// Returns the Site Collection ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Guid? GetSiteGuidByUrl(string url)
        {
            Guid? _siteID = Guid.Empty;
            UsingContext(ctx =>
            {
                Tenant _tenant = new Tenant(ctx);
                _siteID = _tenant.GetSiteGuidByUrl(url);
            });

            return _siteID;
        }
        #endregion
     
        /// <summary>
        /// Checks to see if a site already exists.
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public bool SiteExists(string siteUrl)
        {
            bool _doesSiteExist = false;
            UsingContext(ctx =>
            {
                var tenant = new Tenant(ctx);
                _doesSiteExist = tenant.SiteExists(siteUrl);
            });
            return _doesSiteExist;
        }

        #region ISharePointService Members
        /// <summary>
        /// Delegate that is used to handle creation of ClientContext that is authenticated
        /// </summary>
        /// <param name="action"></param>
        public void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        /// <summary>
        /// Delegate that is used to handle creation of ClientContext that is authenticated
        /// </summary>
        /// <param name="action"></param>
        public void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }
        #endregion
      
    }
}

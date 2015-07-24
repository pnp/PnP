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
        public abstract void CreateSiteCollection(SiteInformation siteRequest, Template template);

        public virtual bool IsTenantExternalSharingEnabled(string tenantUrl)
        {
            Log.Info("AbstractSiteProvisioningService.IsTenantExternalSharingEnabled", "Entering IsTenantExternalSharingEnabled Url {0}", tenantUrl);
            var _returnResult = false;
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                Tenant _tenant = new Tenant(ctx);
                ctx.Load(_tenant);
                try
                { 
                    //IF CALLING SP ONPREM THIS WILL FAIL
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
                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "AbstractSiteProvisioningService.IsTenantExternalSharingEnabled", _timespan.Elapsed);

                }
                catch(Exception ex)
                {
                    Log.Error("Provisioning.Common.AbstractSiteProvisioningService.IsTenantExternalSharingEnabled", 
                        PCResources.ExternalSharing_Enabled_Error_Message, 
                        tenantUrl, 
                        ex);
                }
            });

            return _returnResult;
        }

        public abstract void SetExternalSharing(SiteInformation siteInfo);

        public virtual SitePolicyEntity GetAppliedSitePolicy()
        {
            Log.Info("AbstractSiteProvisioningService.GetAppliedSitePolicy", "Entering GetAppliedSitePolicy");
            SitePolicyEntity _appliedSitePolicy = null;
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                var _web = ctx.Web;
                _appliedSitePolicy = _web.GetAppliedSitePolicy();
               
                _timespan.Stop();
                Log.TraceApi("SharePoint", "AbstractSiteProvisioningService.IsTenantExternalSharingEnabled", _timespan.Elapsed);
            });
            return _appliedSitePolicy;
        }


        public virtual void SetSitePolicy(string policyName)
        {
            Log.Info("AbstractSiteProvisioningService.SetSitePolicy", "Entering SetSitePolicy Policy Name {0}", policyName);
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                var _web = ctx.Web;
                bool _policyApplied = _web.ApplySitePolicy(policyName);
                
                _timespan.Stop();
                Log.TraceApi("SharePoint", "AbstractSiteProvisioningService.SetSitePolicy", _timespan.Elapsed);
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
            Log.Info("AbstractSiteProvisioningService.GetWebByUrl", "Entering GetWebByUrl Url {0}", url);
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
            Log.Info("AbstractSiteProvisioningService.GetSiteGuidByUrl", "Entering GetSiteGuidByUrl Url {0}", url);
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

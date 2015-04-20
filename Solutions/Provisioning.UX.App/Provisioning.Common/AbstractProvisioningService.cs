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
    /// Abstract Site Provisioning Service
    /// </summary>
    public abstract class AbstractProvisioningService : IProvisioningService, ISharePointService
    {
        /// <summary>
        /// Gets or Sets the services Authentication.
        /// </summary>
        public IAuthentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a site collection.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public abstract Guid? ProvisionSite(SiteRequestInformation properties);
       
        /// <summary>
        /// Sets Administrators for the Site Collection
        /// </summary>
        /// <param name="properties"></param>
        public void SetAdministrators(SiteRequestInformation properties)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(properties.Url);
                var web = site.RootWeb;
                var spOwner = web.EnsureUser(properties.SiteOwner.LoginName);
                web.AssociatedOwnerGroup.Users.AddUser(spOwner);
                site.Owner = spOwner;
                ctx.ExecuteQuery();
                foreach (var admin in properties.AdditionalAdministrators)
                {
                    try
                    {
                        tenant.SetSiteAdmin(properties.Url, admin.LoginName, true);
                        var spAdmin = web.EnsureUser(admin.LoginName);
                        web.AssociatedOwnerGroup.Users.AddUser(spAdmin);
                        web.AssociatedOwnerGroup.Update();
                        ctx.ExecuteQuery();
                    }
                    catch
                    {
                        Log.Error("SetAdministrators", "Failed to set {0} as admin of {1}", admin.LoginName, properties.Url);
                    }
                }
            });
        }

        /// <summary>
        /// Sets the Description of the Site Collection
        /// </summary>
        /// <param name="properties"></param>
        public void SetSiteDescription(SiteRequestInformation properties)
        {
            UsingContext(ctx =>
            {
                Tenant tenant = new Tenant(ctx);
                var site = tenant.GetSiteByUrl(properties.Url);
                var web = site.RootWeb;
                web.Description = properties.Description;
                web.Update();
                ctx.ExecuteQuery();
                Log.Debug("Provisioning.Common.SetSiteDescription", "Setting Site Description {0}: for site {1}",
                    properties.Description,
                    properties.Url);
            });
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

        /// <summary>
        /// Member to apply the Site Policy to a site collection 
        /// <see cref="https://technet.microsoft.com/en-us/library/jj219569.aspx"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="policyName"></param>
        public abstract void ApplySitePolicy(string url, string policyName);
       
        /// <summary>
        /// Sets a Property bag for the site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetPropertyBag(Web web, string propertyName, string propertyValue)
        {
            try
            {
                web.SetPropertyBagValue(propertyName, propertyValue);
                web.AddIndexedPropertyBagKey(propertyName);
            }
            catch (Exception ex)
            {
                Log.Error("Provisioning.Common.SetSitePropertyBag", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  Message: {2} Stack: {3} ",
                           web.Url,
                           web.Context.TraceCorrelationId,
                           ex.Message,
                           ex.StackTrace);
            }
        }

        /// <summary>
        /// Sets a Property bag for the site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetPropertyBag(Web web, string propertyName, int propertyValue)
        {
            try
            {
                web.SetPropertyBagValue(propertyName, propertyValue);
                web.AddIndexedPropertyBagKey(propertyName);
            }
            catch (Exception ex)
            {
                Log.Error("Provisioning.Common.SetSitePropertyBag", "Exception occured during processing the request for Site {0}. TraceCorrelationId: {1}  Message: {2} Stack: {3} ",
                          web.Url,
                          web.Context.TraceCorrelationId,
                          ex.Message,
                          ex.StackTrace);
            }
        }

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
    }
}

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
    public abstract class AbstractSiteProvisioningService : ISiteProvisioning, ISharePointService
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
        public abstract Web CreateSiteCollection(SiteRequestInformation siteRequest, Template template);
      
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

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
    /// Implementation class for Provisioning Office 365 Site Collections
    /// </summary>
    public class Office365SiteProvisioningService : AbstractSiteProvisioningService
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Office365SiteProvisioningService() : base()
        {
        }
        #endregion
     
        public override Web CreateSiteCollection(SiteRequestInformation siteRequest, Template template)
        {
            Web _web = null;

            UsingContext(ctx =>
            {
                try
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

                    var _site = _tenant.GetSiteByUrl(siteRequest.Url);
                    _web = _site.RootWeb;
                    _web.Description = siteRequest.Description;
                    _web.Update();
                    ctx.Load(_web);
                    ctx.ExecuteQuery();
         //           this.SetPropertyBag(_web, Constants.PropertyBags.SITE_TEMPLATE_TYPE, siteRequest.Template);
                }
                catch (Exception ex)
                {
                    Log.Fatal("Provisioning.Common.Office365SiteProvisioningService.CreateSiteCollection", "An Error occured occured while provisioning the site {0}. The Error Message: {1}, Exception: {2}", siteRequest.Url, ex.Message, ex);
                    throw;
                }
            });
            return _web;
        }
    }
}

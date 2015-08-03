using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Governance.TimerJobs.Data;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Framework.TimerJobs;
using OfficeDevPnP.Core.Utilities;

namespace Governance.TimerJobs
{
    /// <summary>
    /// SynchronizationJob class is responsible of reading the current site collection status from SPO and update corresponding site information record to SQL Azure database
    /// </summary>
    public class SynchronizationJob : TenantManagementTimerJob
    {
        /// <summary>
        /// Initialize a new instance of SynchronizationJob object
        /// </summary>
        /// /// <param name="repository">The governance database repository to be synchronized with SPO tenant</param>        
        /// <param name="url">The tenant url, like https://contoso.microsoft.com</param>
        public SynchronizationJob(GovernanceDbRepository repository, string url)
            : base("SynchronizationJob", repository)
        {
            AddSite(string.Format("{0}/*", url.TrimEnd("/".ToCharArray())));
        }

        /// <summary>
        /// Timer Job worker method
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The timer job argument</param>
        protected override void TimerJobRunImpl(object sender, TimerJobRunEventArgs e)
        {
            // build site information from database or create a new one
            var siteInformation = GetSiteInformation(e.Url);

            // load the current site status from SPO
            Tenant tenant;
            Site site;
            SiteProperties properties;
            LoadSiteStatus(e, out tenant, out site, out properties);

            // update the site information object with updated site status
            siteInformation.Guid = site.Id;
            siteInformation.Title = site.RootWeb.Title;
            siteInformation.Description = site.RootWeb.Description;
            siteInformation.Lcid = (int)site.RootWeb.Language;
            siteInformation.CreatedDate = site.RootWeb.Created;
            siteInformation.StorageMaximumLevel = properties.StorageMaximumLevel;
            siteInformation.StorageWarningLevel = properties.StorageWarningLevel;
            siteInformation.UserCodeMaximumLevel = properties.UserCodeMaximumLevel;
            siteInformation.UserCodeWarningLevel = properties.UserCodeWarningLevel;
            siteInformation.TimeZoneId = properties.TimeZoneId;
            siteInformation.SharingStatus = IsExternalSharingEnabled(properties.SharingCapability, tenant.SharingCapability);
            // update site collection administrators
            var admins = site.RootWeb.GetAdministrators();
            siteInformation.Administrators = (from a in admins
                                              select new SiteUser()
                                                  {
                                                      LoginName = a.LoginName,
                                                      Email = a.Email,
                                                  }).ToList();
            // update external users
            if (siteInformation.SharingStatus != 0)
            {
                var externalUsers = site.RootWeb.GetExternalUsersForSiteTenant(new Uri(e.Url));
                siteInformation.ExternalUsers = (from u in externalUsers
                                                 select new ExternalSiteUser()
                                                 {
                                                     LoginName = u.AcceptedAs,
                                                     Email = u.InvitedAs,
                                                     ExternalUser_AcceptedAs = u.AcceptedAs,
                                                     ExternalUser_CreatedDate = u.WhenCreated,
                                                     ExternalUser_DisplayName = u.DisplayName,
                                                     ExternalUser_InvitedAs = u.InvitedAs,
                                                     ExternalUser_InvitedBy = u.InvitedBy,
                                                     ExternalUser_UniqueId = u.UniqueId,
                                                 } as SiteUser).ToList();
            }

            // write site information into database
            Log.Info(base.Name, TimerJobsResources.SynchJob_UpdateDbRecord, e.Url);
            DbRepository.UsingContext(dbContext =>
            {
                dbContext.SaveSite(siteInformation);
            });
        }

        /// <summary>
        /// Get the external sharing status of a site collection based on both of the site and tenant level setting.
        /// </summary>
        /// <param name="siteCapability">The external sharing capability of the site collection</param>
        /// <param name="tenantCapability">The external sharing capability setting of the current tenant</param>
        /// <returns></returns>
        private int IsExternalSharingEnabled(SharingCapabilities siteCapability, SharingCapabilities tenantCapability)
        {
            if (tenantCapability == SharingCapabilities.Disabled ||
                siteCapability == SharingCapabilities.Disabled)
                return 0;
            else if (siteCapability == SharingCapabilities.ExternalUserSharingOnly)
                return 1;
            else if (tenantCapability == SharingCapabilities.ExternalUserAndGuestSharing &&
                siteCapability == SharingCapabilities.ExternalUserAndGuestSharing)
                return 2;
            return 0;
        }

        /// <summary>
        /// Load the latest site collection status from SharePoint
        /// </summary>
        /// <param name="e">Timer job event arguments</param>
        /// <param name="tenant">The tenant object</param>
        /// <param name="site">The site object</param>
        /// <param name="properties">The site properties object</param>
        private void LoadSiteStatus(TimerJobRunEventArgs e, out Tenant tenant, out Site site, out SiteProperties properties)
        {
            var tenantClientContext = e.TenantClientContext;
            Log.Info(base.Name, TimerJobsResources.SynchJob_GetSiteStatus, e.Url);
            tenant = new Tenant(tenantClientContext);
            site = tenant.GetSiteByUrl(e.Url);
            properties = tenant.GetSitePropertiesByUrl(e.Url, includeDetail: false);
            tenantClientContext.Load(tenant,
                t => t.SharingCapability);
            tenantClientContext.Load(site,
                s => s.RootWeb.Title,
                s => s.RootWeb.Description,
                s => s.RootWeb.Language,
                s => s.RootWeb.Created,
                s => s.Id);
            tenantClientContext.Load(properties,
                s => s.StorageMaximumLevel,
                s => s.StorageWarningLevel,
                s => s.UserCodeMaximumLevel,
                s => s.UserCodeWarningLevel,
                s => s.TimeZoneId,
                s => s.SharingCapability);
            tenantClientContext.ExecuteQueryRetry();
        }

        /// <summary>
        /// Get an existing or new SiteInformation entity by a site collection URL
        /// </summary>
        /// <param name="url">site collection URL</param>
        /// <returns>The existing SiteInformation entity from DB or if the matching record is not existing, a new entity will be returned.</returns>
        private SiteInformation GetSiteInformation(string url)
        {
            Log.Info(base.Name, TimerJobsResources.SynchJob_GetDbRecord, url);
            DateTime time = DateTime.UtcNow;
            SiteInformation siteInformation = null;
            DbRepository.UsingContext(dbContext =>
            {
                siteInformation = dbContext.GetSite(url) ?? new SiteInformation()
                {
                    Url = url,
                    CreatedDate = time,
                    CreatedBy = base.Name,
                    ComplianceState = new ComplianceState(),
                };
                siteInformation.ModifiedDate = time;
                siteInformation.ModifiedBy = base.Name;
            });
            return siteInformation;
        }
    }
}

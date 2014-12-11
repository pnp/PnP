using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Linq;

namespace Microsoft.SharePoint.Client
{
    public static partial class TenantExtensions
    {
        [Obsolete("Use tenant.CreateSiteCollection() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Guid AddSiteCollection(this Tenant tenant, SiteEntity properties, bool removeFromRecycleBin = false, bool wait = true)
        {
            return tenant.CreateSiteCollection(properties, removeFromRecycleBin, wait);
        }

        [Obsolete("Use tenant.CreateSiteCollection() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Guid AddSiteCollection(this Tenant tenant, string siteFullUrl, string title, string siteOwnerLogin,
                                                string template, int storageMaximumLevel, int storageWarningLevel,
                                                int timeZoneId, int userCodeMaximumLevel, int userCodeWarningLevel,
                                                uint lcid, bool removeFromRecycleBin = false, bool wait = true)
        {
            SiteEntity siteCol = new SiteEntity()
            {
                Url = siteFullUrl,
                Title = title,
                SiteOwnerLogin = siteOwnerLogin,
                Template = template,
                StorageMaximumLevel = storageMaximumLevel,
                StorageWarningLevel = storageWarningLevel,
                TimeZoneId = timeZoneId,
                UserCodeMaximumLevel = userCodeMaximumLevel,
                UserCodeWarningLevel = userCodeWarningLevel,
                Lcid = lcid
            };
            return tenant.CreateSiteCollection(siteCol, removeFromRecycleBin, wait);
        }

        /// <summary>
        /// Checks if a site collection exists
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">URL to the site collection</param>
        /// <returns>True if existing, false if not</returns>
        [Obsolete("Use tenant.SiteExists() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool DoesSiteExist(this Tenant tenant, string siteFullUrl)
        {
            try
            {
                return tenant.CheckIfSiteExists(siteFullUrl, SITE_STATUS_ACTIVE) ||
                       tenant.CheckIfSiteExists(siteFullUrl, SITE_STATUS_CREATING) ||
                       tenant.CheckIfSiteExists(siteFullUrl, SITE_STATUS_RECYCLED);
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.SharePoint.Client.ServerException && (ex.Message.IndexOf("Unable to access site") != -1 || ex.Message.IndexOf("Cannot get site") != -1))
                {
                    return true;
                }
                else
                {
                    LoggingUtility.Internal.TraceError((int)EventId.UnknownExceptionAccessingSite, ex, CoreResources.TenantExtensions_UnknownExceptionAccessingSite);
                }

                return false;
            }
        }




    }
}

using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.Search.Query;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with site (both site collection and web site) creation, status, retrieval and settings
    /// </summary>
    public static partial class WebExtensions
    {
#if !CLIENTSDKV15
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails")]
        [Obsolete("Use Tenant.CreateSiteCollection()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Guid AddSiteCollectionTenant(this Web web, SiteEntity properties, bool removeFromRecycleBin = false, bool wait = true)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.CreateSiteCollection(properties, removeFromRecycleBin, wait);
        }

        [Obsolete("Use Tenant.CheckIfSiteExists()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool CheckIfSiteExistsInTenant(this Web web, string siteUrl, string status)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.CheckIfSiteExists(siteUrl, status);
        }

        [Obsolete("Use Tenant.CreateSiteCollection()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Guid CreateSiteCollectionTenant(this Web web, string url, string title, string siteOwnerLogin,
                                                        string template, int storageMaximumLevel, int storageWarningLevel,
                                                        int timeZoneId, int userCodeMaximumLevel, int userCodeWarningLevel,
                                                        uint lcid, bool removeFromRecycleBin = false, bool wait = true)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.AddSiteCollection(url, title, siteOwnerLogin, template, storageMaximumLevel, storageWarningLevel, timeZoneId, userCodeMaximumLevel, userCodeWarningLevel, lcid, removeFromRecycleBin, wait);
        }

        [Obsolete("Use Tenant.DeleteSiteCollection()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool DeleteSiteCollectionTenant(this Web web, string siteUrl, bool useRecycleBin)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.DeleteSiteCollection(siteUrl, useRecycleBin);
        }

        [Obsolete("Use Tenant.DeleteSiteCollection()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool DeleteSiteCollectionFromRecycleBinTenant(this Web web, string siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.DeleteSiteCollectionFromRecycleBin(siteUrl);
        }

        [Obsolete("Use Tenant.DoesSiteExist()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool DoesSiteExistInTenant(this Web web, string siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.DoesSiteExist(siteUrl);
        }

        [Obsolete("Use Tenant.GetSiteGuidByUrl()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Guid GetSiteGuidByUrlTenant(this Web web, string siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.GetSiteGuidByUrl(siteUrl);
        }

        [Obsolete("Use Tenant.GetSiteGuidByUrl()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Guid GetSiteGuidByUrlTenant(this Web web, Uri siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.GetSiteGuidByUrl(siteUrl);
        }

        [Obsolete("Use Tenant.GetWebTemplates()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SPOTenantWebTemplateCollection GetWebTemplatesTenant(this Web web, uint lcid, int compatibilityLevel)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.GetWebTemplates(lcid, compatibilityLevel);
        }

        [Obsolete("Use Tenant.IsSiteActive()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool IsSiteActiveTenant(this Web web, string siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.IsSiteActive(siteUrl);
        }

        [Obsolete("Use Tenant.SiteExists()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SiteExistsInTenant(this Web web, string siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.SiteExists(siteUrl);
        }

        [Obsolete("Use Tenant.SubSiteExists()")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SubSiteExistsInTenant(this Web web, string siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            return tenant.SubSiteExists(siteUrl);
        }
#endif

        [Obsolete("Should use CreateWeb(), to avoid confusion betweeen Site (collection) and Web (site)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddSite(this Web web, SiteEntity parent, SiteEntity subsite, bool inheritPermissions, bool inheritNavigation)
        {
            CreateWeb(web, subsite.Title, subsite.Url, subsite.Description, subsite.Template, (int)subsite.Lcid, inheritPermissions, inheritNavigation);
        }

        [Obsolete("Should use CreateWeb(), to avoid confusion betweeen Site (collection) and Web (site)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddSite(this Web web, string title, string url, string description, string template, uint language, bool inheritPermissions, bool inheritNavigation)
        {
            CreateWeb(web, title, url, description, template, (int)language, inheritPermissions, inheritNavigation);
        }

        [Obsolete("Should use CreateWeb(), to avoid confusion betweeen Site (collection) and Web (site)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Web CreateSite(this Web web, SiteEntity subsite, bool inheritPermissions = true, bool inheritNavigation = true)
        {
            // Call actual implementation
            return CreateWeb(web, subsite.Title, subsite.Url, subsite.Description, subsite.Template, (int)subsite.Lcid, inheritPermissions, inheritNavigation);
        }

        [Obsolete("Should use CreateWeb(), to avoid confusion betweeen Site (collection) and Web (site)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Web CreateSite(this Web web, string title, string url, string description, string template, int language, bool inheritPermissions = true, bool inheritNavigation = true)
        {
            return CreateWeb(web, title, url, description, template, language, inheritPermissions, inheritNavigation);
        }

        [Obsolete("Should use Context.WebExistsFullUrl(), to avoid confusion betweeen Site (collection) and Web (site)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SiteExists(this Web web, string siteUrl)
        {
            return WebExistsFullUrl(web.Context, siteUrl);
        }

        [Obsolete("Should use Context.WebExists(), to avoid confusion betweeen Site (collection) and Web (site)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SubSiteExists(this Web web, string siteUrl)
        {
            return WebExistsFullUrl(web.Context, siteUrl);
        }

        [Obsolete("Should use WebExists(), to avoid confusion betweeen Site (collection) and Web (site)")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SubSiteExistsWithUrl(this Web web, string url)
        {
            return WebExists(web, url);
        }

        /// <summary>
        /// Queues a web for a _full_ crawl the next incremental crawl
        /// </summary>
        /// <param name="web">Site to be processed</param>
        [Obsolete("Use ReIndexWeb()")]
        public static void ReIndexSite(this Web web)
        {
            ReIndexWeb(web);
        }
        /// <summary>
        /// Registers a remote event receiver
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="name">The name of the event receiver (needs to be unique among the event receivers registered on this list)</param>
        /// <param name="url">The URL of the remote WCF service that handles the event</param>
        /// <param name="eventReceiverType"></param>
        /// <param name="synchronization"></param>
        /// <param name="force">If True any event already registered with the same name will be removed first.</param>
        /// <returns>Returns an EventReceiverDefinition if succeeded. Returns null if failed.</returns>
        [Obsolete("Use Web.AddRemoteEventReceiver()")]
        public static EventReceiverDefinition RegisterRemoteEventReceiver(this Web web, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, bool force)
        {
            return web.AddRemoteEventReceiver(name, url, eventReceiverType, synchronization, force);
        }


    }
}

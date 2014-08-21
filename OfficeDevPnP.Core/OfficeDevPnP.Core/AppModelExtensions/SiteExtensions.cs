using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with site (collection) creation, status, retrieval and settings
    /// </summary>
    public static class SiteExtensions
    {
        const string MSG_CONTEXT_CLOSED = "ClientContext gets closed after action is completed. Calling ExecuteQuery again returns an error. Verify that you have an open ClientContext object.";
        const string SITE_STATUS_ACTIVE = "Active";
        const string SITE_STATUS_CREATING = "Creating";
        const string SITE_STATUS_RECYCLED = "Recycled";
        const string INDEXED_PROPERTY_KEY = "vti_indexedpropertykeys";

        #region Check for site status in SharePoint Online
        /// <summary>
        /// Gets the ID of site collection with specified URL
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">A URL that specifies a site collection to get ID.</param>
        /// <returns>The Guid of a site collection</returns>
        public static Guid GetSiteGuidByUrlTenant(this Web web, string siteUrl)
        {
            if (!string.IsNullOrEmpty(siteUrl))
                throw new ArgumentNullException("siteUrl");

            return web.GetSiteGuidByUrlTenant(new Uri(siteUrl));
        }

        /// <summary>
        /// Gets the ID of site collection with specified URL
        /// </summary>
        /// <param name="web">Tenant admin web</param>
        /// <param name="siteUrl">A URL that specifies a site collection to get ID.</param>
        /// <returns>The Guid of a site collection</returns>
        public static Guid GetSiteGuidByUrlTenant(this Web web, Uri siteUrl)
        {
            Guid siteGuid = Guid.Empty;

            Site site = null;
            Tenant tenant = new Tenant(web.Context);
            site = tenant.GetSiteByUrl(siteUrl.OriginalString);
            web.Context.Load(site);
            web.Context.ExecuteQuery();
            siteGuid = site.Id;

            return siteGuid;
        }

        /// <summary>
        /// Returns if a site collection is in a particular status. If the url contains a sub site then returns true is the sub site exists, false if not. 
        /// Status is irrelevant for sub sites
        /// </summary>
        /// <param name="web">Tenant admin web</param>
        /// <param name="siteUrl">Url to the site collection</param>
        /// <param name="status">Status to check (Active, Creating, Recycled)</param>
        /// <returns>True if in status, false if not in status</returns>
        public static bool CheckIfSiteExistsInTenant(this Web web, string siteUrl, string status)
        {
            bool ret = false;
            //Get the site name
            var url = new Uri(siteUrl);
            var UrlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
            int idx = url.PathAndQuery.Substring(1).IndexOf("/") + 2;
            var UrlPath = url.PathAndQuery.Substring(0, idx);
            var Name = url.PathAndQuery.Substring(idx);
            var index = Name.IndexOf('/');
            Tenant tenant = new Tenant(web.Context);

            //Judge whether this site collection is existing or not
            if (index == -1)
            {
                var properties = tenant.GetSitePropertiesByUrl(siteUrl, false);
                web.Context.Load(properties);
                web.Context.ExecuteQuery();
                ret = properties.Status.Equals(status, StringComparison.OrdinalIgnoreCase);
            }
            //Judge whether this sub web site is existing or not
            else
            {
                var site = tenant.GetSiteByUrl(string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}{1}{2}", UrlDomain, UrlPath, Name.Split("/".ToCharArray())[0]));
                var subweb = site.OpenWeb(Name.Substring(index + 1));
                web.Context.Load(subweb, w => w.Title);
                web.Context.ExecuteQuery();
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Checks if a site collection exists
        /// </summary>
        /// <param name="web">Tenant admin web</param>
        /// <param name="siteUrl">URL to the site collection</param>
        /// <returns>True if existing, false if not</returns>
        public static bool DoesSiteExistInTenant(this Web web, string siteUrl)
        {
            try
            {
                return web.CheckIfSiteExistsInTenant(siteUrl, SITE_STATUS_ACTIVE) ||
                       web.CheckIfSiteExistsInTenant(siteUrl, SITE_STATUS_CREATING) ||
                       web.CheckIfSiteExistsInTenant(siteUrl, SITE_STATUS_RECYCLED);
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.SharePoint.Client.ServerException && (ex.Message.IndexOf("Unable to access site") != -1 || ex.Message.IndexOf("Cannot get site") != -1))
                {
                    return true;
                }
                else
                    LoggingUtility.LogError("Could not determine if site exists in tenant.", ex, EventCategory.Site);

                return false;
            }
        }

        /// <summary>
        /// Checks if a sub site exists
        /// </summary>
        /// <param name="web">Tenant admin web</param>
        /// <param name="siteUrl">URL to the sub site</param>
        /// <returns>True if existing, false if not</returns>
        public static bool SubSiteExistsInTenant(this Web web, string siteUrl)
        {
            try
            {
                return web.CheckIfSiteExistsInTenant(siteUrl, "");
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.SharePoint.Client.ServerException && (ex.Message.IndexOf("Unable to access site") != -1 || ex.Message.IndexOf("Cannot get site") != -1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if a site collection exists
        /// </summary>
        /// <param name="web">Site object opened with credentials that are reused for the site collection check</param>
        /// <param name="siteUrl">Fully qualified URL to the sub site</param>
        /// <returns>true if exists, false otherwise</returns>
        public static bool SiteExists(this Web web, string siteUrl)
        {
            return web.SubSiteExists(siteUrl);
        }


        /// <summary>
        /// Checks if a subsite exists
        /// </summary>
        /// <param name="web">Site object opened with credentials that are reused for the sub site check</param>
        /// <param name="siteUrl">Fully qualified URL to the sub site</param>
        /// <returns>true if exists, false otherwise</returns>
        public static bool SubSiteExists(this Web web, string siteUrl)
        {
            try
            {
                bool ret = false;

                ClientContext test = new ClientContext(siteUrl);
                test.Credentials = web.Context.Credentials;
                test.Load(test.Web, w => w.Title);
                test.ExecuteQuery();

                ret = true;

                return ret;
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.SharePoint.Client.ServerException &&
                    (ex.Message.IndexOf("Unable to access site") != -1 ||
                     ex.Message.IndexOf("Cannot get site") != -1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks if a site collection exists, relies on tenant admin API
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">URL to the site collection</param>
        /// <returns>True if existing, false if not</returns>
        public static bool SiteExistsInTenant(this Web web, string siteUrl)
        {
            try
            {
                //Get the site name
                Tenant tenant = new Tenant(web.Context);
                var properties = tenant.GetSitePropertiesByUrl(siteUrl, false);
                web.Context.Load(properties);
                web.Context.ExecuteQuery();

                // Will cause an exception if site URL is not there. Not optimal, but the way it works.
                return true;
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.SharePoint.Client.ServerException && (ex.Message.IndexOf("Unable to access site") != -1 || ex.Message.IndexOf("Cannot get site") != -1))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Checks if a site collection is Active
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">URL to the site collection</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsSiteActiveTenant(this Web web, string siteUrl)
        {
            try
            {
                return web.CheckIfSiteExistsInTenant(siteUrl, "Active");
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cannot get site"))
                {
                    return false;
                }
                LoggingUtility.LogError("Error finding if site is active tenant.", ex, EventCategory.Site);
                throw;
            }
        }

        /// <summary>
        /// Gets the collection of the URLs of all Web sites that are contained within the site collection, 
        /// including the top-level site and its subsites.
        /// </summary>
        /// <param name="site">Site collection to retrieve the URLs for.</param>
        /// <returns>An enumeration containing the full URLs as strings.</returns>
        /// <remarks>
        /// <para>
        /// This is analagous to the <code>SPSite.AllWebs</code> property and can be used to get a collection
        /// of all web site URLs to loop through, e.g. for branding.
        /// </para>
        /// </remarks>
        public static IEnumerable<string> GetAllWebUrls(this Site site)
        {
            var siteContext = site.Context;
            siteContext.Load(site, s => s.Url);
            siteContext.ExecuteQuery();
            var queue = new Queue<string>();
            queue.Enqueue(site.Url);
            while (queue.Count > 0)
            {
                var currentUrl = queue.Dequeue();
                using (var webContext = new ClientContext(currentUrl))
                {
                    webContext.Credentials = siteContext.Credentials;
                    webContext.Load(webContext.Web, web => web.Webs);
                    webContext.ExecuteQuery();
                    foreach (var subWeb in webContext.Web.Webs)
                    {
                        queue.Enqueue(subWeb.Url);
                    }
                }
                yield return currentUrl;
            }
        }

        #endregion

        #region site (collection) creation and deletion
        /// <summary>
        /// Launches a site collection creation and waits for the creation to finish
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="properties">Describes the site collection to be created</param>
        /// <param name="removeSiteFromRecycleBin">It true and site is present in recycle bin, it will be removed first from the recycle bin</param>
        /// <param name="wait">If true, processing will halt until the site collection has been created</param>
        /// <returns>Guid of the created site collection</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails")]
        public static Guid AddSiteCollectionTenant(this Web web, SiteEntity properties, bool removeFromRecycleBin = false, bool wait = true)
        {
            if (removeFromRecycleBin)
            {
                if (CheckIfSiteExistsInTenant(web, properties.Url, SITE_STATUS_RECYCLED))
                {
                    web.DeleteSiteCollectionFromRecycleBinTenant(properties.Url);
                }
            }

            Tenant tenant = new Tenant(web.Context);
            SiteCreationProperties newsite = new SiteCreationProperties();
            newsite.Url = properties.Url;
            newsite.Owner = properties.SiteOwnerLogin;
            newsite.Template = properties.Template;
            newsite.Title = properties.Title;
            newsite.StorageMaximumLevel = properties.StorageMaximumLevel;
            newsite.StorageWarningLevel = properties.StorageWarningLevel;
            newsite.TimeZoneId = properties.TimeZoneId;
            newsite.UserCodeMaximumLevel = properties.UserCodeMaximumLevel;
            newsite.UserCodeWarningLevel = properties.UserCodeWarningLevel;
            newsite.Lcid = properties.Lcid;

            try
            {
                SpoOperation op = tenant.CreateSite(newsite);
                web.Context.Load(tenant);
                web.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
                web.Context.ExecuteQuery();

                if (wait)
                {
                    //check if site creation operation is complete
                    while (!op.IsComplete)
                    {
                        System.Threading.Thread.Sleep(op.PollingInterval);
                        op.RefreshLoad();
                        if (!op.IsComplete)
                        {
                            try
                            {
                                web.Context.ExecuteQuery();
                            }
                            catch (WebException webEx)
                            {
                                // Context connection gets closed after action completed.
                                // Calling ExecuteQuery again returns an error which can be ignored
                                LoggingUtility.LogWarning(MSG_CONTEXT_CLOSED, webEx, EventCategory.Site);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Eat the siteSubscription exception to make the same code work for MT as on-prem April 2014 CU+
                if (ex.Message.IndexOf("Parameter name: siteSubscription") == -1)
                {
                    throw ex;
                }
            }

            // Get site guid and return
            var siteGuid = web.GetSiteGuidByUrlTenant(new Uri(properties.Url));

            return siteGuid;
        }

        /// <summary>
        /// Launches a site collection creation and waits for the creation to finish 
        /// </summary>
        /// <param name="web">Context to admin site</param>
        /// <param name="url">The SPO url</param>
        /// <param name="title">The site title</param>
        /// <param name="siteOwnerLogin">Owner account</param>
        /// <param name="template">Site template being used</param>
        /// <param name="storageMaximumLevel">Site quota in MB</param>
        /// <param name="storageWarningLevel">Site quota warning level in MB</param>
        /// <param name="timeZoneId">TimeZoneID for the site. "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris" = 3 </param>
        /// <param name="userCodeMaximumLevel">The user code quota in points</param>
        /// <param name="userCodeWarningLevel">The user code quota warning level in points</param>
        /// <param name="lcid">The site locale. See http://technet.microsoft.com/en-us/library/ff463597.aspx for a complete list of Lcid's</param>
        /// <returns></returns>
        public static Guid CreateSiteCollectionTenant(this Web web, string url, string title, string siteOwnerLogin,
                                                        string template, int storageMaximumLevel, int storageWarningLevel,
                                                        int timeZoneId, int userCodeMaximumLevel, int userCodeWarningLevel,
                                                        uint lcid, bool removeFromRecycleBin = false, bool wait = true)
        {
            SiteEntity siteCol = new SiteEntity()
            {
                Url = url,
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

            return AddSiteCollectionTenant(web, siteCol, removeFromRecycleBin, wait);
        }

        /// <summary>
        /// Adds a sub site to an existing site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="parent">Information about the parent site</param>
        /// <param name="subsite">Information describing the sub site to be added</param>
        /// <param name="inheritPermissions">Does the sub site inherit the permissions of the parent site</param>
        /// <param name="inheritNavigation">Does the sub site inherit the navigation of the parent site</param>
        public static void AddSite(this Web web, SiteEntity parent, SiteEntity subsite, bool inheritPermissions, bool inheritNavigation)
        {
            // Call actual implementation
            CreateSite(web, subsite, inheritPermissions, inheritNavigation);
        }

        /// <summary>
        /// Adds a sub site to an existing site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="title">Title for the site</param>
        /// <param name="description">Description for the new site</param>
        /// <param name="template">Template for the site, like STS#0</param>
        /// <param name="language">Language code for the site, like 1033</param>
        /// <param name="inheritPermissions">Should the new site inherit permissions</param>
        /// <param name="inheritNavigation">Should the new site inherent navigation</param>
        public static void AddSite(this Web web, string title, string url, string description, string template, uint language, bool inheritPermissions, bool inheritNavigation)
        {
            // Call centralized route to call internal creation logic
            CreateSite(web, title, url, description, template, (int)language, inheritPermissions, inheritNavigation);
        }

        /// <summary>
        /// Deletes a site collection
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">Url of the site collection to delete</param>
        /// <param name="useRecycleBin">Leave the deleted site collection in the site collection recycle bin</param>
        /// <returns>True if deleted</returns>
        public static bool DeleteSiteCollectionTenant(this Web web, string siteUrl, bool useRecycleBin)
        {
            bool ret = false;
            Tenant tenant = new Tenant(web.Context);
            SpoOperation op = tenant.RemoveSite(siteUrl);
            web.Context.Load(tenant);
            web.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
            web.Context.ExecuteQuery();

            //check if site creation operation is complete
            while (!op.IsComplete)
            {
                System.Threading.Thread.Sleep(op.PollingInterval);
                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        web.Context.ExecuteQuery();
                    }
                    catch (WebException webEx)
                    {
                        // Context connection gets closed after action completed.
                        // Calling ExecuteQuery again returns an error which can be ignored
                        LoggingUtility.LogWarning(MSG_CONTEXT_CLOSED, webEx, EventCategory.Site);
                    }
                }
            }

            if (useRecycleBin)
            {
                return true;
            }

            // To delete Site collection completely, (may take a longer time)
            op = tenant.RemoveDeletedSite(siteUrl);
            web.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
            web.Context.ExecuteQuery();

            while (!op.IsComplete)
            {
                System.Threading.Thread.Sleep(op.PollingInterval);
                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        web.Context.ExecuteQuery();
                    }
                    catch (WebException webEx)
                    {
                        // Context connection gets closed after action completed.
                        // Calling ExecuteQuery again returns an error which can be ignored
                        LoggingUtility.LogWarning(MSG_CONTEXT_CLOSED, webEx, EventCategory.Site);
                    }
                }
            }

            ret = true;
            return ret;
        }

        /// <summary>
        /// Deletes a site collection from the site collection recycle bin
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">URL of the site collection to delete</param>
        /// <returns>True if deleted</returns>
        public static bool DeleteSiteCollectionFromRecycleBinTenant(this Web web, string siteUrl)
        {
            bool ret = false;
            Tenant tenant = new Tenant(web.Context);
            SpoOperation op = tenant.RemoveDeletedSite(siteUrl);
            web.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
            web.Context.ExecuteQuery();

            while (!op.IsComplete)
            {
                System.Threading.Thread.Sleep(op.PollingInterval);
                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        web.Context.ExecuteQuery();
                    }
                    catch (WebException webEx)
                    {
                        // Context connection gets closed after action completed.
                        // Calling ExecuteQuery again returns an error which can be ignored
                        LoggingUtility.LogWarning(MSG_CONTEXT_CLOSED, webEx, EventCategory.Site);
                    }
                }
            }

            ret = true;
            return ret;
        }

        /// <summary>
        /// Returns available webtemplates/site definitions
        /// </summary>
        /// <param name="web">Site to be processed - needs to be tenant site admin site</param>
        /// <param name="lcid"></param>
        /// <param name="compatibilityLevel">14 for SharePoint 2010, 15 for SharePoint 2013/SharePoint Online</param>
        /// <returns></returns>
        public static SPOTenantWebTemplateCollection GetWebTemplatesTenant(this Web web, uint lcid, int compatibilityLevel)
        {
            Tenant tenant = new Tenant(web.Context);

            var templates = tenant.GetSPOTenantWebTemplates(lcid, compatibilityLevel);

            web.Context.Load(templates);

            web.Context.ExecuteQuery();

            return templates;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="subsite"></param>
        /// <param name="inheritPermissions"></param>
        /// <param name="inheritNavigation"></param>
        public static Web CreateSite(this Web web, SiteEntity subsite, bool inheritPermissions = true, bool inheritNavigation = true)
        {
            // Call actual implementation
            return CreateSite(web, subsite.Title, subsite.Url, subsite.Description, subsite.Template, (int)subsite.Lcid, inheritPermissions, inheritNavigation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="template"></param>
        /// <param name="language"></param>
        /// <param name="inheritPermissions"></param>
        /// <param name="inheritNavigation"></param>
        public static Web CreateSite(this Web web, string title, string url, string description, string template, int language, bool inheritPermissions = true, bool inheritNavigation = true)
        {
            WebCreationInformation wci = new WebCreationInformation();
            wci.Url = url;
            wci.Title = title;
            wci.Description = description;
            wci.UseSamePermissionsAsParentSite = inheritPermissions;
            wci.WebTemplate = template;
            wci.Language = language;

            Web w = web.Webs.Add(wci);
            w.Navigation.UseShared = inheritNavigation;
            w.Update();

            web.Context.ExecuteQuery();

            return w;
        }

        public static bool SubSiteExistsWithUrl(this Web web, string url)
        {
            Utility.EnsureWeb(web.Context, web, "ServerRelativeUrl");

            string siteUrl = string.Format("{0}/{1}", web.ServerRelativeUrl, url).ToLowerInvariant();
            WebCollection subSites = web.Webs;
            IEnumerable<Web> results = web.Context.LoadQuery<Web>(subSites.Where(item => item.ServerRelativeUrl == siteUrl));
            web.Context.ExecuteQuery();
            Web existingWeb = results.FirstOrDefault();
            if (existingWeb != null)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Apps

        /// <summary>
        /// Returns all app instances
        /// </summary>
        /// <param name="web">The site to process</param>
        /// <returns></returns>
        public static ClientObjectList<AppInstance> GetAppInstances(this Web web)
        {
            ClientObjectList<AppInstance> instances = Microsoft.SharePoint.Client.AppCatalog.GetAppInstances(web.Context, web);
            web.Context.Load(instances);
            web.Context.ExecuteQuery();

            return instances;
        }

        #endregion

        #region Site retrieval via search
        /// <summary>
        /// Returns all my site site collections
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns>All my site site collections</returns>
        public static List<SiteEntity> MySiteSearch(this Web web)
        {
            string keywordQuery = String.Format("contentclass:\"STS_Site\" AND site:{0}", web.Context.Url);
            return web.SiteSearch(keywordQuery);
        }

        /// <summary>
        /// Returns all site collections that are indexed. In MT the search center, mysite host and contenttype hub are defined as non indexable by default and thus 
        /// are not returned
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns>All site collections</returns>
        public static List<SiteEntity> SiteSearch(this Web web)
        {
            return web.SiteSearch(string.Empty);
        }

        /// <summary>
        /// Returns the site collections that comply with the passed keyword query
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="keywordQueryValue">Keyword query</param>
        /// <returns>All found site collections</returns>
        public static List<SiteEntity> SiteSearch(this Web web, string keywordQueryValue)
        {
            try
            {
                LoggingUtility.Internal.TraceVerbose("Site search '{0}'", keywordQueryValue);

                List<SiteEntity> sites = new List<SiteEntity>();

                KeywordQuery keywordQuery = new KeywordQuery(web.Context);
                keywordQuery.TrimDuplicates = false;

                if (keywordQueryValue.Length == 0)
                {
                    keywordQueryValue = "contentclass:\"STS_Site\"";
                }

                int startRow = 0;
                int totalRows = 0;

                totalRows = web.ProcessQuery(keywordQueryValue, sites, keywordQuery, startRow);

                if (totalRows > 0)
                {
                    while (totalRows >= sites.Count)
                    {
                        startRow += 500;
                        totalRows = web.ProcessQuery(keywordQueryValue, sites, keywordQuery, startRow);
                    }
                }

                return sites;
            }
            catch (Exception ex)
            {
                LoggingUtility.Internal.TraceError((int)EventId.SiteSearchUnhandledException, ex, "Site search error.");
                // rethrow does lose one line of stack trace, but we want to log the error at the component boundary
                throw;
            }
        }

        /// <summary>
        /// Returns all site collection that start with the provided URL
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">Base URL for which sites can be returned</param>
        /// <returns>All found site collections</returns>
        public static List<SiteEntity> SiteSearchScopedByUrl(this Web web, string siteUrl)
        {
            string keywordQuery = String.Format("contentclass:\"STS_Site\" AND site:{0}", siteUrl);
            return web.SiteSearch(keywordQuery);
        }

        /// <summary>
        /// Returns all site collection that match with the provided title
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">Base URL for which sites can be returned</param>
        /// <returns>All found site collections</returns>
        public static List<SiteEntity> SiteSearchScopedByTitle(this Web web, string siteTitle)
        {
            string keywordQuery = String.Format("contentclass:\"STS_Site\" AND Title:{0}", siteTitle);
            return web.SiteSearch(keywordQuery);
        }

        // private methods
        /// <summary>
        /// Runs a query
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="keywordQueryValue">keyword query </param>
        /// <param name="sites">sites variable that hold the resulting sites</param>
        /// <param name="keywordQuery">KeywordQuery object</param>
        /// <param name="startRow">Start row of the resultset to be returned</param>
        /// <returns>Total number of rows for the query</returns>
        private static int ProcessQuery(this Web web, string keywordQueryValue, List<SiteEntity> sites, KeywordQuery keywordQuery, int startRow)
        {
            int totalRows = 0;

            keywordQuery.QueryText = keywordQueryValue;
            keywordQuery.RowLimit = 500;
            keywordQuery.StartRow = startRow;
            keywordQuery.SelectProperties.Add("Title");
            keywordQuery.SelectProperties.Add("SPSiteUrl");
            keywordQuery.SelectProperties.Add("Description");
            keywordQuery.SelectProperties.Add("WebTemplate");
            keywordQuery.SortList.Add("SPSiteUrl", SortDirection.Ascending);
            SearchExecutor searchExec = new SearchExecutor(web.Context);
            ClientResult<ResultTableCollection> results = searchExec.ExecuteQuery(keywordQuery);
            web.Context.ExecuteQuery();

            if (results != null)
            {
                if (results.Value[0].RowCount > 0)
                {
                    totalRows = results.Value[0].TotalRows;

                    foreach (var row in results.Value[0].ResultRows)
                    {
                        sites.Add(new SiteEntity
                        {
                            Title = row["Title"] != null ? row["Title"].ToString() : "",
                            Url = row["SPSiteUrl"] != null ? row["SPSiteUrl"].ToString() : "",
                            Description = row["Description"] != null ? row["Description"].ToString() : "",
                            Template = row["WebTemplate"] != null ? row["WebTemplate"].ToString() : "",
                        });
                    }
                }
            }

            return totalRows;
        }
        #endregion

        #region Site Property Bag Modifiers

        /// <summary>
        /// Sets a key/value pair in the web property bag
        /// </summary>
        /// <param name="web">Web that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">Integer value for the property bag entry</param>
        public static void SetPropertyBagValue(this Web web, string key, int value)
        {
            SetPropertyBagValueInternal(web, key, value);
        }


        /// <summary>
        /// Sets a key/value pair in the web property bag
        /// </summary>
        /// <param name="web">Web that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">String value for the property bag entry</param>
        public static void SetPropertyBagValue(this Web web, string key, string value)
        {
            SetPropertyBagValueInternal(web, key, value);
        }


        /// <summary>
        /// Sets a key/value pair in the web property bag
        /// </summary>
        /// <param name="web">Web that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">Value for the property bag entry</param>
        private static void SetPropertyBagValueInternal(Web web, string key, object value)
        {
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQuery();

            props[key] = value;

            web.Update();
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Removes a property bag value from the property bag
        /// </summary>
        /// <param name="web">The site to process</param>
        /// <param name="key">The key to remove</param>
        public static void RemovePropertyBagValue(this Web web, string key)
        {
            RemovePropertyBagValueInternal(web, key, true);
        }

        private static void RemovePropertyBagValueInternal(Web web, string key, bool checkIndexed)
        {
            // In order to remove a property from the property bag, remove it both from the AllProperties collection by setting it to null
            // -and- by removing it from the FieldValues collection. Bug in CSOM?
            web.AllProperties[key] = null;
            web.AllProperties.FieldValues.Remove(key);

            web.Update();

            web.Context.ExecuteQuery();
            if(checkIndexed)
            RemoveIndexedPropertyBagKey(web, key); // Will only remove it if it exists as an indexed property
        }
        /// <summary>
        /// Get int typed property bag value. If does not contain, returns default value.
        /// </summary>
        /// <param name="web">Web to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <returns>Value of the property bag entry as integer</returns>
        public static int? GetPropertyBagValueInt(this Web web, string key, int defaultValue)
        {
            object value = GetPropertyBagValueInternal(web, key);
            if (value != null)
            {
                return (int)value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get string typed property bag value. If does not contain, returns given default value.
        /// </summary>
        /// <param name="web">Web to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <returns>Value of the property bag entry as string</returns>
        public static string GetPropertyBagValueString(this Web web, string key, string defaultValue)
        {
            object value = GetPropertyBagValueInternal(web, key);
            if (value != null)
            {
                return (string)value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Type independent implementation of the property getter.
        /// </summary>
        /// <param name="web">Web to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <returns>Value of the property bag entry</returns>
        private static object GetPropertyBagValueInternal(Web web, string key)
        {
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQuery();
            if (props.FieldValues.ContainsKey(key))
            {
                return props.FieldValues[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the given property bag entry exists
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="key">Key of the property bag entry to check</param>
        /// <returns>True if the entry exists, false otherwise</returns>
        public static bool PropertyBagContainsKey(this Web web, string key)
        {
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQuery();
            if (props.FieldValues.ContainsKey(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Used to convert the list of property keys is required format for listing keys to be index
        /// </summary>
        /// <param name="keys">list of keys to set to be searchable</param>
        /// <returns>string formatted list of keys in proper format</returns>
        private static string GetEncodedValueForSearchIndexProperty(IEnumerable<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string current in keys)
            {
                stringBuilder.Append(Convert.ToBase64String(Encoding.Unicode.GetBytes(current)));
                stringBuilder.Append('|');
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns all keys in the property bag that have been marked for indexing
        /// </summary>
        /// <param name="web">The site to process</param>
        /// <returns></returns>
        public static IEnumerable<string> GetIndexedPropertyBagKeys(this Web web)
        {
            List<string> keys = new List<string>();

            if (web.PropertyBagContainsKey(INDEXED_PROPERTY_KEY))
            {
                foreach (string key in web.GetPropertyBagValueString(INDEXED_PROPERTY_KEY, "").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    byte[] bytes = Convert.FromBase64String(key);
                    keys.Add(Encoding.Unicode.GetString(bytes));
                }
            }

            return keys;
        }

        /// <summary>
        /// Marks a property bag key for indexing
        /// </summary>
        /// <param name="web"></param>
        /// <param name="key"></param>
        /// <returns>Returns True if succeeded</returns>
        public static bool AddIndexedPropertyBagKey(this Web web, string key)
        {
            bool result = false;
            var keys = GetIndexedPropertyBagKeys(web).ToList();
            if (!keys.Contains(key))
            {
                keys.Add(key);
                web.SetPropertyBagValue(INDEXED_PROPERTY_KEY, GetEncodedValueForSearchIndexProperty(keys));
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Unmarks a property bag key for indexing
        /// </summary>
        /// <param name="web">The site to process</param>
        /// <param name="key">The key to unmark for indexed. Case-sensitive</param>
        /// <returns>Returns True if succeeded</returns>
        public static bool RemoveIndexedPropertyBagKey(this Web web, string key)
        {
            bool result = false;
            var keys = GetIndexedPropertyBagKeys(web).ToList();
            if (key.Contains(key))
            {
                keys.Remove(key);
                if (keys.Any())
                {
                    web.SetPropertyBagValue(INDEXED_PROPERTY_KEY, GetEncodedValueForSearchIndexProperty(keys));
                }
                else
                {
                    RemovePropertyBagValueInternal(web, INDEXED_PROPERTY_KEY, false);
                }
                result = true;
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Queues a web for a _full_ crawl the next incremental crawl
        /// </summary>
        /// <param name="web">Site to be processed</param>
        public static void ReIndexSite(this Web web)
        {
            int searchversion = 0;
            if (web.PropertyBagContainsKey("vti_searchversion"))
            {
                searchversion = (int)web.GetPropertyBagValueInt("vti_searchversion", 0);
            }
            web.SetPropertyBagValue("vti_searchversion", searchversion + 1);
        }

        #region Localization
        /// <summary>
        /// Can be used to set translations for different cultures. 
        /// </summary>
        /// <example>
        ///     web.SetLocalizationForSiteLabels("fi-fi", "Name of the site in Finnish", "Description in Finnish");
        /// </example>
        /// <seealso cref="http://blogs.msdn.com/b/vesku/archive/2014/03/20/office365-multilingual-content-types-site-columns-and-site-other-elements.aspx"/>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="cultureName">Culture name like en-us or fi-fi</param>
        /// <param name="titleResource">Localized Title string</param>
        /// <param name="descriptionResource">Localized Description string</param>
        public static void SetLocalizationLabels(this Web web, string cultureName, string titleResource, string descriptionResource)
        {
            // Ensure web
            Utility.EnsureWeb(web.Context, web, "TitleResource");
            // Set translations for the culture
            web.TitleResource.SetValueForUICulture(cultureName, titleResource);
            web.DescriptionResource.SetValueForUICulture(cultureName, descriptionResource);
            web.Update();
            web.Context.ExecuteQuery();
        }
        #endregion

    }
}

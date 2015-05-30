using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.Search.Query;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with site (both site collection and web site) creation, status, retrieval and settings
    /// </summary>
    public static partial class WebExtensions
    {
        const string MSG_CONTEXT_CLOSED = "ClientContext gets closed after action is completed. Calling ExecuteQuery again returns an error. Verify that you have an open ClientContext object.";
        const string SITE_STATUS_ACTIVE = "Active";
        const string SITE_STATUS_CREATING = "Creating";
        const string SITE_STATUS_RECYCLED = "Recycled";
        const string INDEXED_PROPERTY_KEY = "vti_indexedpropertykeys";

        #region Web (site) query, creation and deletion

        /// <summary>
        /// Adds a new child Web (site) to a parent Web.
        /// </summary>
        /// <param name="parentWeb">The parent Web (site) to create under</param>
        /// <param name="subsite">Details of the Web (site) to add. Only Title, Url (as the leaf URL), Description, Template and Language are used.</param>
        /// <param name="inheritPermissions">Specifies whether the new site will inherit permissions from its parent site.</param>
        /// <param name="inheritNavigation">Specifies whether the site inherits navigation.</param>
        /// <returns></returns>
        public static Web CreateWeb(this Web parentWeb, SiteEntity subsite, bool inheritPermissions = true, bool inheritNavigation = true)
        {
            return CreateWeb(parentWeb, subsite.Title, subsite.Url, subsite.Description, subsite.Template, (int)subsite.Lcid, inheritPermissions, inheritNavigation);
        }

        /// <summary>
        /// Adds a new child Web (site) to a parent Web.
        /// </summary>
        /// <param name="parentWeb">The parent Web (site) to create under</param>
        /// <param name="title">The title of the new site. </param>
        /// <param name="leafUrl">A string that represents the URL leaf name.</param>
        /// <param name="description">The description of the new site. </param>
        /// <param name="template">The name of the site template to be used for creating the new site. </param>
        /// <param name="language">The locale ID that specifies the language of the new site. </param>
        /// <param name="inheritPermissions">Specifies whether the new site will inherit permissions from its parent site.</param>
        /// <param name="inheritNavigation">Specifies whether the site inherits navigation.</param>
        public static Web CreateWeb(this Web parentWeb, string title, string leafUrl, string description, string template, int language, bool inheritPermissions = true, bool inheritNavigation = true)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (leafUrl.Contains('/') || leafUrl.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single web URL and cannot contain path characters.", "leafUrl");
            }

            Log.Info(Constants.LOGGING_SOURCE, CoreResources.WebExtensions_CreateWeb, leafUrl, template);
            WebCreationInformation creationInfo = new WebCreationInformation()
            {
                Url = leafUrl,
                Title = title,
                Description = description,
                UseSamePermissionsAsParentSite = inheritPermissions,
                WebTemplate = template,
                Language = language
            };

            Web newWeb = parentWeb.Webs.Add(creationInfo);
            newWeb.Navigation.UseShared = inheritNavigation;
            newWeb.Update();

            parentWeb.Context.ExecuteQueryRetry();

            return newWeb;
        }

        /// <summary>
        /// Deletes the child website with the specified leaf URL, from a parent Web, if it exists. 
        /// </summary>
        /// <param name="parentWeb">The parent Web (site) to delete from</param>
        /// <param name="leafUrl">A string that represents the URL leaf name.</param>
        /// <returns>true if the web was deleted; otherwise false if nothing was done</returns>
        public static bool DeleteWeb(this Web parentWeb, string leafUrl)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (leafUrl.Contains('/') || leafUrl.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single web URL and cannot contain path characters.", "leafUrl");
            }

            var deleted = false;
            Utility.EnsureWeb(parentWeb.Context, parentWeb, "ServerRelativeUrl");
            var serverRelativeUrl = UrlUtility.Combine(parentWeb.ServerRelativeUrl, leafUrl);
            var webs = parentWeb.Webs;
            // NOTE: Predicate does not take into account a required case-insensitive comparison
            //var results = parentWeb.Context.LoadQuery<Web>(webs.Where(item => item.ServerRelativeUrl == serverRelativeUrl));
            parentWeb.Context.Load(webs, wc => wc.Include(w => w.ServerRelativeUrl));
            parentWeb.Context.ExecuteQueryRetry();
            var existingWeb = webs.FirstOrDefault(item => string.Equals(item.ServerRelativeUrl, serverRelativeUrl, StringComparison.OrdinalIgnoreCase));
            if (existingWeb != null)
            {
                Log.Info(Constants.LOGGING_SOURCE, CoreResources.WebExtensions_DeleteWeb, serverRelativeUrl);
                existingWeb.DeleteObject();
                parentWeb.Context.ExecuteQueryRetry();
                deleted = true;
            }
            else
            {
                Log.Debug(Constants.LOGGING_SOURCE, "Delete requested but web '{0}' not found, nothing to do.", serverRelativeUrl);
            }
            return deleted;
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
            siteContext.ExecuteQueryRetry();
            var queue = new Queue<string>();
            queue.Enqueue(site.Url);
            while (queue.Count > 0)
            {
                var currentUrl = queue.Dequeue();
                using (var webContext = siteContext.Clone(currentUrl))
                {
                    webContext.Load(webContext.Web, web => web.Webs);
                    webContext.ExecuteQueryRetry();
                    foreach (var subWeb in webContext.Web.Webs)
                    {
                        queue.Enqueue(subWeb.Url);
                    }
                }
                yield return currentUrl;
            }
        }

        /// <summary>
        /// Returns the child Web site with the specified leaf URL.
        /// </summary>
        /// <param name="parentWeb">The Web site to check under</param>
        /// <param name="leafUrl">A string that represents the URL leaf name.</param>
        /// <returns>The requested Web, if it exists, otherwise null.</returns>
        /// <remarks>
        /// <para>
        /// The ServerRelativeUrl property of the retrieved Web is instantiated.
        /// </para>
        /// </remarks>
        public static Web GetWeb(this Web parentWeb, string leafUrl)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (leafUrl.Contains('/') || leafUrl.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single web URL and cannot contain path characters.", "leafUrl");
            }

            Utility.EnsureWeb(parentWeb.Context, parentWeb, "ServerRelativeUrl");
            var serverRelativeUrl = UrlUtility.Combine(parentWeb.ServerRelativeUrl, leafUrl);
            var webs = parentWeb.Webs;
            // NOTE: Predicate does not take into account a required case-insensitive comparison
            //var results = parentWeb.Context.LoadQuery<Web>(webs.Where(item => item.ServerRelativeUrl == serverRelativeUrl));
            parentWeb.Context.Load(webs, wc => wc.Include(w => w.ServerRelativeUrl));
            parentWeb.Context.ExecuteQueryRetry();
            var childWeb = webs.FirstOrDefault(item => string.Equals(item.ServerRelativeUrl, serverRelativeUrl, StringComparison.OrdinalIgnoreCase));
            return childWeb;
        }

        /// <summary>
        /// Determines if a child Web site with the specified leaf URL exists. 
        /// </summary>
        /// <param name="parentWeb">The Web site to check under</param>
        /// <param name="leafUrl">A string that represents the URL leaf name.</param>
        /// <returns>true if the Web (site) exists; otherwise false</returns>
        public static bool WebExists(this Web parentWeb, string leafUrl)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (leafUrl.Contains('/') || leafUrl.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single web URL and cannot contain path characters.", "leafUrl");
            }

            Utility.EnsureWeb(parentWeb.Context, parentWeb, "ServerRelativeUrl");
            var serverRelativeUrl = UrlUtility.Combine(parentWeb.ServerRelativeUrl, leafUrl);
            var webs = parentWeb.Webs;
            // NOTE: Predicate does not take into account a required case-insensitive comparison
            //var results = parentWeb.Context.LoadQuery<Web>(webs.Where(item => item.ServerRelativeUrl == serverRelativeUrl));
            parentWeb.Context.Load(webs, wc => wc.Include(w => w.ServerRelativeUrl));
            parentWeb.Context.ExecuteQueryRetry();
            var exists = webs.Any(item => string.Equals(item.ServerRelativeUrl, serverRelativeUrl, StringComparison.OrdinalIgnoreCase));
            return exists;
        }

        /// <summary>
        /// Determines if a Web (site) exists at the specified full URL, either accessible or that returns an access error.
        /// </summary>
        /// <param name="context">Existing context, used to provide credentials.</param>
        /// <param name="webFullUrl">Full URL of the site to check.</param>
        /// <returns>true if the Web (site) exists; otherwise false</returns>
        public static bool WebExistsFullUrl(this ClientRuntimeContext context, string webFullUrl)
        {
            bool exists = false;
            try
            {
                using (ClientContext testContext = context.Clone(webFullUrl))
                {
                    testContext.Load(testContext.Web, w => w.Title);
                    testContext.ExecuteQueryRetry();
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                if (IsUnableToAccessSiteException(ex) || IsCannotGetSiteException(ex))
                {
                    // Site exists, but you don't have access .. not sure if this is really valid
                    // (I guess if checking if URL is already taken, e.g. want to create a new site
                    // then this makes sense).
                    exists = true;
                }
            }
            return exists;
        }

        /// <summary>
        /// Checks if the current web is a sub site or not
        /// </summary>
        /// <param name="web">Web to check</param>
        /// <returns>True is sub site, false otherwise</returns>
        public static bool IsSubSite(this Web web)
        {
            bool executeQueryNeeded = false;
            Site site = (web.Context as ClientContext).Site;

            if (!web.IsObjectPropertyInstantiated("Url"))
            {
                web.Context.Load(web);
                executeQueryNeeded = true;
            }

            if (!site.IsObjectPropertyInstantiated("Url"))
            {
                web.Context.Load(site);
                executeQueryNeeded = true;
            }

            if (executeQueryNeeded)
            {
                web.Context.ExecuteQueryRetry();
            }

            if (web.Url.Equals(site.Url, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsPublishingWeb(this Web web)
        {
            var featureActivated = GetPropertyBagValueInternal(web, "__PublishingFeatureActivated");

            return featureActivated != null && bool.Parse(featureActivated.ToString());
        }


        private static bool IsCannotGetSiteException(Exception ex)
        {
            if (ex is ServerException)
            {
                if (((ServerException)ex).ServerErrorCode == -1 && ((ServerException)ex).ServerErrorTypeName.Equals("Microsoft.Online.SharePoint.Common.SpoNoSiteException", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool IsUnableToAccessSiteException(Exception ex)
        {
            if (ex is ServerException)
            {
                if (((ServerException)ex).ServerErrorCode == -2147024809 && ((ServerException)ex).ServerErrorTypeName.Equals("System.ArgumentException", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Apps and sandbox solutions

        /// <summary>
        /// Returns all app instances
        /// </summary>
        /// <param name="web">The site to process</param>
        /// <returns></returns>
        public static ClientObjectList<AppInstance> GetAppInstances(this Web web)
        {
            var instances = AppCatalog.GetAppInstances(web.Context, web);
            web.Context.Load(instances);
            web.Context.ExecuteQueryRetry();

            return instances;
        }

        /// <summary>
        /// Removes the app instance with the specified title.
        /// </summary>
        /// <param name="web">Web to remove the app instance from</param>
        /// <param name="appTitle">Title of the app instance to remove</param>
        /// <returns>true if the the app instance was removed; false if it does not exist</returns>
        public static bool RemoveAppInstanceByTitle(this Web web, string appTitle)
        {
            // Removes the association between the App and the Web
            bool removed = false;
            var instances = AppCatalog.GetAppInstances(web.Context, web);
            web.Context.Load(instances);
            web.Context.ExecuteQueryRetry();
            foreach (var app in instances)
            {
                if (string.Equals(app.Title, appTitle, StringComparison.OrdinalIgnoreCase))
                {
                    removed = true;
                    Log.Info(Constants.LOGGING_SOURCE, CoreResources.WebExtensions_RemoveAppInstance, appTitle, app.Id);
                    app.Uninstall();
                    web.Context.ExecuteQueryRetry();
                }
            }
            if (!removed)
            {
                Log.Debug(Constants.LOGGING_SOURCE, "Requested to remove app '{0}', but no instances found; nothing to remove.", appTitle);
            }
            return removed;
        }

        /// <summary>
        /// Uploads and installs a sandbox solution package (.WSP) file, replacing existing solution if necessary.
        /// </summary>
        /// <param name="site">Site collection to install to</param>
        /// <param name="packageGuid">ID of the solution, from the solution manifest (required for the remove step)</param>
        /// <param name="sourceFilePath">Path to the sandbox solution package (.WSP) file</param>
        /// <param name="majorVersion">Optional major version of the solution, defaults to 1</param>
        /// <param name="minorVersion">Optional minor version of the solution, defaults to 0</param>
        public static void InstallSolution(this Site site, Guid packageGuid, string sourceFilePath, int majorVersion = 1, int minorVersion = 0)
        {
            string fileName = Path.GetFileName(sourceFilePath);
            Log.Info(Constants.LOGGING_SOURCE, CoreResources.WebExtensions_InstallSolution, fileName, site.Context.Url);

            var rootWeb = site.RootWeb;
            var sourceFileName = Path.GetFileName(sourceFilePath);

            var rootFolder = rootWeb.RootFolder;
            rootWeb.Context.Load(rootFolder, f => f.ServerRelativeUrl);
            rootWeb.Context.ExecuteQueryRetry();

            rootFolder.UploadFile(sourceFileName, sourceFilePath, true);

            var packageInfo = new DesignPackageInfo()
            {
                PackageName = fileName,
                PackageGuid = packageGuid,
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
            };

            Log.Debug(Constants.LOGGING_SOURCE, "Uninstalling package '{0}'", packageInfo.PackageName);
            DesignPackage.UnInstall(site.Context, site, packageInfo);
            site.Context.ExecuteQueryRetry();


            var packageServerRelativeUrl = UrlUtility.Combine(rootWeb.RootFolder.ServerRelativeUrl, fileName);
            Log.Debug(Constants.LOGGING_SOURCE, "Installing package '{0}'", packageInfo.PackageName);

            // NOTE: The lines below (in OfficeDev PnP) wipe/clear all items in the composed looks aka design catalog (_catalogs/design, list template 124).
            // The solution package should be loaded into the solutions catalog (_catalogs/solutions, list template 121).

            DesignPackage.Install(site.Context, site, packageInfo, packageServerRelativeUrl);
            site.Context.ExecuteQueryRetry();

            // Remove package from rootfolder
            var uploadedSolutionFile = rootFolder.Files.GetByUrl(fileName);
            uploadedSolutionFile.DeleteObject();
            site.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Uninstalls a sandbox solution package (.WSP) file
        /// </summary>
        /// <param name="site">Site collection to install to</param>
        /// <param name="packageGuid">ID of the solution, from the solution manifest</param>
        /// <param name="fileName">filename of the WSP file to uninstall</param>
        /// <param name="majorVersion">Optional major version of the solution, defaults to 1</param>
        /// <param name="minorVersion">Optional minor version of the solution, defaults to 0</param>
        public static void UninstallSolution(this Site site, Guid packageGuid, string fileName, int majorVersion = 1, int minorVersion = 0)
        {
            Log.Info(Constants.LOGGING_SOURCE, CoreResources.WebExtensions_UninstallSolution, packageGuid);

            var rootWeb = site.RootWeb;
            var solutionGallery = rootWeb.GetCatalog((int)ListTemplateType.SolutionCatalog);

            var camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(
              @"<View>  
                        <Query> 
                           <Where><Eq><FieldRef Name='SolutionId' /><Value Type='Guid'>{0}</Value></Eq></Where> 
                        </Query> 
                         <ViewFields><FieldRef Name='ID' /><FieldRef Name='FileLeafRef' /></ViewFields> 
                  </View>", packageGuid);

            var solutions = solutionGallery.GetItems(camlQuery);
            site.Context.Load(solutions);
            site.Context.ExecuteQueryRetry();

            if (solutions.AreItemsAvailable)
            {
                var packageItem = solutions.FirstOrDefault();
                var packageInfo = new DesignPackageInfo()
                {
                    PackageGuid = packageGuid,
                    PackageName = fileName,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion
                };

                DesignPackage.UnInstall(site.Context, site, packageInfo);
                site.Context.ExecuteQueryRetry();
            }
        }

        #endregion

        #region Site retrieval via search
        /// <summary>
        /// Returns all my site site collections
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns>All my site site collections</returns>
        [SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods",
            Justification = "Search Query code")]
        public static List<SiteEntity> MySiteSearch(this Web web)
        {
            const string keywordQuery = "contentclass:\"STS_Site\" AND WebTemplate:SPSPERS";
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
        /// <param name="trimDublicates">Indicates if dublicates should be trimmed or not</param>
        /// <returns>All found site collections</returns>
        public static List<SiteEntity> SiteSearch(this Web web, string keywordQueryValue, bool trimDublicates = true)
        {
            try
            {
                Log.Debug(Constants.LOGGING_SOURCE, "Site search '{0}'", keywordQueryValue);

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
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.WebExtensions_SiteSearchUnhandledException, ex.Message);
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
        /// <param name="siteTitle">Title of the site to search for</param>
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
            web.Context.ExecuteQueryRetry();

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

        #region Web (site) Property Bag Modifiers

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

            // Get the value, if the web properties are already loaded
            if (props.FieldValues.Count > 0)
            {
                props[key] = value;
            }
            else
            {
                // Load the web properties
                web.Context.Load(props);
                web.Context.ExecuteQueryRetry();

                props[key] = value;
            }

            web.Update();
            web.Context.ExecuteQueryRetry();
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

        /// <summary>
        /// Removes a property bag value
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="key">They key to remove</param>
        /// <param name="checkIndexed"></param>
        private static void RemovePropertyBagValueInternal(Web web, string key, bool checkIndexed)
        {
            // In order to remove a property from the property bag, remove it both from the AllProperties collection by setting it to null
            // -and- by removing it from the FieldValues collection. Bug in CSOM?
            web.AllProperties[key] = null;
            web.AllProperties.FieldValues.Remove(key);

            web.Update();

            web.Context.ExecuteQueryRetry();
            if (checkIndexed)
                RemoveIndexedPropertyBagKey(web, key); // Will only remove it if it exists as an indexed property
        }

        /// <summary>
        /// Get int typed property bag value. If does not contain, returns default value.
        /// </summary>
        /// <param name="web">Web to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <param name="defaultValue"></param>
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
        /// <param name="defaultValue"></param>
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
                return defaultValue;
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
            web.Context.ExecuteQueryRetry();
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
            web.Context.ExecuteQueryRetry();
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
        /// <param name="web">The web to process</param>
        /// <param name="key">The key to mark for indexing</param>
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

        #region Search

        /// <summary>
        /// Queues a web for a full crawl the next incremental crawl
        /// </summary>
        /// <param name="web">Site to be processed</param>
        public static void ReIndexWeb(this Web web)
        {
            int searchversion = 0;
            if (web.PropertyBagContainsKey("vti_searchversion"))
            {
                searchversion = (int)web.GetPropertyBagValueInt("vti_searchversion", 0);
            }
            web.SetPropertyBagValue("vti_searchversion", searchversion + 1);
        }
        #endregion

        #region Events


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
        public static EventReceiverDefinition AddRemoteEventReceiver(this Web web, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, bool force)
        {
            return web.AddRemoteEventReceiver(name, url, eventReceiverType, synchronization, 1000, force);
        }

        /// <summary>
        /// Registers a remote event receiver
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="name">The name of the event receiver (needs to be unique among the event receivers registered on this list)</param>
        /// <param name="url">The URL of the remote WCF service that handles the event</param>
        /// <param name="eventReceiverType"></param>
        /// <param name="synchronization"></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="force">If True any event already registered with the same name will be removed first.</param>
        /// <returns>Returns an EventReceiverDefinition if succeeded. Returns null if failed.</returns>
        public static EventReceiverDefinition AddRemoteEventReceiver(this Web web, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, int sequenceNumber, bool force)
        {
            var query = from receiver
                   in web.EventReceivers
                        where receiver.ReceiverName == name
                        select receiver;
            web.Context.LoadQuery(query);
            web.Context.ExecuteQueryRetry();

            var receiverExists = query.Any();
            if (receiverExists && force)
            {
                var receiver = query.FirstOrDefault();
                receiver.DeleteObject();
                web.Context.ExecuteQueryRetry();
                receiverExists = false;
            }
            EventReceiverDefinition def = null;

            if (!receiverExists)
            {
                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.EventType = eventReceiverType;
                receiver.ReceiverUrl = url;
                receiver.ReceiverName = name;
                receiver.SequenceNumber = sequenceNumber;
                receiver.Synchronization = synchronization;
                def = web.EventReceivers.Add(receiver);
                web.Context.Load(def);
                web.Context.ExecuteQueryRetry();
            }
            return def;
        }

        /// <summary>
        /// Returns an event receiver definition
        /// </summary>
        /// <param name="web">Web to process</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EventReceiverDefinition GetEventReceiverById(this Web web, Guid id)
        {
            IEnumerable<EventReceiverDefinition> receivers = null;
            var query = from receiver
                        in web.EventReceivers
                        where receiver.ReceiverId == id
                        select receiver;

            receivers = web.Context.LoadQuery(query);
            web.Context.ExecuteQueryRetry();
            if (receivers.Any())
            {
                return receivers.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an event receiver definition
        /// </summary>
        /// <param name="web"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EventReceiverDefinition GetEventReceiverByName(this Web web, string name)
        {
            IEnumerable<EventReceiverDefinition> receivers = null;
            var query = from receiver
                        in web.EventReceivers
                        where receiver.ReceiverName == name
                        select receiver;

            receivers = web.Context.LoadQuery(query);
            web.Context.ExecuteQueryRetry();
            if (receivers.Any())
            {
                return receivers.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Localization
#if !CLIENTSDKV15
        /// <summary>
        /// Can be used to set translations for different cultures. 
        /// </summary>
        /// <example>
        ///     web.SetLocalizationForSiteLabels("fi-fi", "Name of the site in Finnish", "Description in Finnish");
        /// </example>
        /// <see href="http://blogs.msdn.com/b/vesku/archive/2014/03/20/office365-multilingual-content-types-site-columns-and-site-other-elements.aspx"/>
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
            web.Context.ExecuteQueryRetry();
        }
#endif
        #endregion

        #region TemplateHandling

        /// <summary>
        /// Can be used to apply custom remote provisioning template on top of existing site. 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="template">ProvisioningTemplate with the settings to be applied</param>
        /// <param name="applyingInformation">Specified additional settings and or properties</param>
        public static void ApplyProvisioningTemplate(this Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation = null)
        {
            // Call actual handler
            new SiteToTemplateConversion().ApplyRemoteTemplate(web, template, applyingInformation);
        }

        /// <summary>
        /// Can be used to extract custom provisioning template from existing site. The extracted template
        /// will be compared with the default base template.
        /// </summary>
        /// <param name="web">Web to get template from</param>
        /// <returns>ProvisioningTemplate object with generated values from existing site</returns>
        public static ProvisioningTemplate GetProvisioningTemplate(this Web web)
        {
            ProvisioningTemplateCreationInformation creationInfo = new ProvisioningTemplateCreationInformation(web);
            // Load the base template which will be used for the comparison work
            creationInfo.BaseTemplate = web.GetBaseTemplate();

            return new SiteToTemplateConversion().GetRemoteTemplate(web, creationInfo);
        }

        /// <summary>
        /// Can be used to extract custom provisioning template from existing site. The extracted template
        /// will be compared with the default base template.
        /// </summary>
        /// <param name="web">Web to get template from</param>
        /// <param name="connector">Connector that will be used to persist the files retrieved from the template "get"</param>
        /// <param name="creationInfo">Specifies additional settings and/or properties</param>
        /// <returns>ProvisioningTemplate object with generated values from existing site</returns>
        public static ProvisioningTemplate GetProvisioningTemplate(this Web web, ProvisioningTemplateCreationInformation creationInfo)
        {
            return new SiteToTemplateConversion().GetRemoteTemplate(web, creationInfo);
        }
        #endregion

        #region Output Cache
        /// <summary>
        /// Sets output cache on publishing web. The settings can be maintained from UI by visiting url /_layouts/15/sitecachesettings.aspx
        /// </summary>
        /// <param name="web">SharePoint web</param>
        /// <param name="enableOutputCache">Specify true to enable output cache. False otherwise.</param>
        /// <param name="anonymousCacheProfileId">Applies for anonymous users access for a site in Site Collection. Id of the profile specified in "Cache Profiles" list.</param>
        /// <param name="authenticatedCacheProfileId">Applies for authenticated users access for a site in the Site Collection. Id of the profile specified in "Cache Profiles" list.</param>
        /// <param name="debugCacheInformation">Specify true to enable the display of additional cache information on pages in this site collection. False otherwise.</param>
        public static void SetPageOutputCache(this Web web, bool enableOutputCache, int anonymousCacheProfileId, int authenticatedCacheProfileId, bool debugCacheInformation)
        {
            const string cacheProfileUrl = "Cache Profiles/{0}_.000";

            string publishingWebValue = web.GetPropertyBagValueString("__PublishingFeatureActivated", string.Empty);
            if (string.IsNullOrEmpty(publishingWebValue))
            {
                throw new Exception("Page output cache can be set only on publishing sites.");
            }

            web.SetPropertyBagValue("EnableCache", enableOutputCache.ToString());
            web.SetPropertyBagValue("AnonymousPageCacheProfileUrl", string.Format(cacheProfileUrl, anonymousCacheProfileId));
            web.SetPropertyBagValue("AuthenticatedPageCacheProfileUrl", string.Format(cacheProfileUrl, authenticatedCacheProfileId));
            web.SetPropertyBagValue("EnableDebuggingOutput", debugCacheInformation.ToString());
        }
        #endregion

    }
}

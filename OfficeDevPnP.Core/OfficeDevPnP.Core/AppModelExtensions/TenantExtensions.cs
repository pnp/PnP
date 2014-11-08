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
    public static class TenantExtensions
    {
        const string SITE_STATUS_ACTIVE = "Active";
        const string SITE_STATUS_CREATING = "Creating";
        const string SITE_STATUS_RECYCLED = "Recycled";

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
        /// Adds a SiteEntity by launching site collection creation and waits for the creation to finish
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="properties">Describes the site collection to be created</param>
        /// <param name="removeSiteFromRecycleBin">It true and site is present in recycle bin, it will be removed first from the recycle bin</param>
        /// <param name="wait">If true, processing will halt until the site collection has been created</param>
        /// <returns>Guid of the created site collection and Guid.Empty is the wait parameter is specified as false</returns>
        public static Guid CreateSiteCollection(this Tenant tenant, SiteEntity properties, bool removeFromRecycleBin = false, bool wait = true)
        {
            if (removeFromRecycleBin)
            {
                if (tenant.CheckIfSiteExists(properties.Url, SITE_STATUS_RECYCLED))
                {
                    tenant.DeleteSiteCollectionFromRecycleBin(properties.Url);
                }
            }

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
                tenant.Context.Load(tenant);
                tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
                tenant.Context.ExecuteQuery();

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
                                tenant.Context.ExecuteQuery();
                            }
                            catch (WebException webEx)
                            {
                                // Context connection gets closed after action completed.
                                // Calling ExecuteQuery again returns an error which can be ignored
                                LoggingUtility.Internal.TraceWarning((int)EventId.ClosedContextWarning, webEx, CoreResources.TenantExtensions_ClosedContextWarning);
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
                    throw;
                }
            }

            // Get site guid and return. If we create the site asynchronously, return an empty guid as we cannot retrieve the site by URL yet.
            Guid siteGuid = Guid.Empty;
            if (wait)
            {
                siteGuid = tenant.GetSiteGuidByUrl(new Uri(properties.Url));
            }
            return siteGuid;
        }

        /// <summary>
        /// Launches a site collection creation and waits for the creation to finish 
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">The SPO url</param>
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
        public static Guid CreateSiteCollection(this Tenant tenant, string siteFullUrl, string title, string siteOwnerLogin,
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
        /// Returns if a site collection is in a particular status. If the url contains a sub site then returns true is the sub site exists, false if not. 
        /// Status is irrelevant for sub sites
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">Url to the site collection</param>
        /// <param name="status">Status to check (Active, Creating, Recycled)</param>
        /// <returns>True if in status, false if not in status</returns>
        public static bool CheckIfSiteExists(this Tenant tenant, string siteFullUrl, string status)
        {
            bool ret = false;
            //Get the site name
            var url = new Uri(siteFullUrl);
            var UrlDomain = string.Format("{0}://{1}", url.Scheme, url.Host);
            int idx = url.PathAndQuery.Substring(1).IndexOf("/") + 2;
            var UrlPath = url.PathAndQuery.Substring(0, idx);
            var Name = url.PathAndQuery.Substring(idx);
            var index = Name.IndexOf('/');

            //Judge whether this site collection is existing or not
            if (index == -1)
            {
                var properties = tenant.GetSitePropertiesByUrl(siteFullUrl, false);
                tenant.Context.Load(properties);
                tenant.Context.ExecuteQuery();
                ret = properties.Status.Equals(status, StringComparison.OrdinalIgnoreCase);
            }
            //Judge whether this sub web site is existing or not
            else
            {
                var site = tenant.GetSiteByUrl(string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}{1}{2}", UrlDomain, UrlPath, Name.Split("/".ToCharArray())[0]));
                var subweb = site.OpenWeb(Name.Substring(index + 1));
                tenant.Context.Load(subweb, w => w.Title);
                tenant.Context.ExecuteQuery();
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Deletes a site collection
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">Url of the site collection to delete</param>
        /// <param name="useRecycleBin">Leave the deleted site collection in the site collection recycle bin</param>
        /// <returns>True if deleted</returns>
        public static bool DeleteSiteCollection(this Tenant tenant, string siteFullUrl, bool useRecycleBin)
        {
            bool ret = false;
            SpoOperation op = tenant.RemoveSite(siteFullUrl);
            tenant.Context.Load(tenant);
            tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
            tenant.Context.ExecuteQuery();

            //check if site creation operation is complete
            while (!op.IsComplete)
            {
                System.Threading.Thread.Sleep(op.PollingInterval);
                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        tenant.Context.ExecuteQuery();
                    }
                    catch (WebException webEx)
                    {
                        // Context connection gets closed after action completed.
                        // Calling ExecuteQuery again returns an error which can be ignored
                        LoggingUtility.Internal.TraceWarning((int)EventId.ClosedContextWarning, webEx, CoreResources.TenantExtensions_ClosedContextWarning);
                    }
                }
            }

            if (useRecycleBin)
            {
                return true;
            }

            // To delete Site collection completely, (may take a longer time)
            op = tenant.RemoveDeletedSite(siteFullUrl);
            tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
            tenant.Context.ExecuteQuery();

            while (!op.IsComplete)
            {
                System.Threading.Thread.Sleep(op.PollingInterval);
                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        tenant.Context.ExecuteQuery();
                    }
                    catch (WebException webEx)
                    {
                        // Context connection gets closed after action completed.
                        // Calling ExecuteQuery again returns an error which can be ignored
                        LoggingUtility.Internal.TraceWarning((int)EventId.ClosedContextWarning, webEx, CoreResources.TenantExtensions_ClosedContextWarning);
                    }
                }
            }

            ret = true;
            return ret;
        }

        /// <summary>
        /// Deletes a site collection from the site collection recycle bin
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">URL of the site collection to delete</param>
        /// <returns>True if deleted</returns>
        public static bool DeleteSiteCollectionFromRecycleBin(this Tenant tenant, string siteFullUrl)
        {
            bool ret = false;
            SpoOperation op = tenant.RemoveDeletedSite(siteFullUrl);
            tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
            tenant.Context.ExecuteQuery();

            while (!op.IsComplete)
            {
                System.Threading.Thread.Sleep(op.PollingInterval);
                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        tenant.Context.ExecuteQuery();
                    }
                    catch (WebException webEx)
                    {
                        // Context connection gets closed after action completed.
                        // Calling ExecuteQuery again returns an error which can be ignored
                        LoggingUtility.Internal.TraceWarning((int)EventId.ClosedContextWarning, webEx, CoreResources.TenantExtensions_ClosedContextWarning);
                    }
                }
            }

            ret = true;
            return ret;
        }

        /// <summary>
        /// Checks if a site collection exists
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">URL to the site collection</param>
        /// <returns>True if existing, false if not</returns>
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

        /// <summary>
        /// Gets the ID of site collection with specified URL
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">A URL that specifies a site collection to get ID.</param>
        /// <returns>The Guid of a site collection</returns>
        public static Guid GetSiteGuidByUrl(this Tenant tenant, string siteFullUrl)
        {
            if (!string.IsNullOrEmpty(siteFullUrl))
                throw new ArgumentNullException("siteFullUrl");

            return tenant.GetSiteGuidByUrl(new Uri(siteFullUrl));
        }

        /// <summary>
        /// Gets the ID of site collection with specified URL
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">A URL that specifies a site collection to get ID.</param>
        /// <returns>The Guid of a site collection or an Guid.Empty if the Site does not exist</returns>
        public static Guid GetSiteGuidByUrl(this Tenant tenant, Uri siteFullUrl)
        {
            Guid siteGuid = Guid.Empty;

            Site site = null;
            site = tenant.GetSiteByUrl(siteFullUrl.OriginalString);
            tenant.Context.Load(site);
            tenant.Context.ExecuteQuery();
            siteGuid = site.Id;

            return siteGuid;
        }

        /// <summary>
        /// Returns available webtemplates/site definitions
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="lcid"></param>
        /// <param name="compatibilityLevel">14 for SharePoint 2010, 15 for SharePoint 2013/SharePoint Online</param>
        /// <returns></returns>
        public static SPOTenantWebTemplateCollection GetWebTemplates(this Tenant tenant, uint lcid, int compatibilityLevel)
        {

            var templates = tenant.GetSPOTenantWebTemplates(lcid, compatibilityLevel);

            tenant.Context.Load(templates);

            tenant.Context.ExecuteQuery();

            return templates;
        }

        /// <summary>
        /// Checks if a site collection is Active
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">URL to the site collection</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsSiteActive(this Tenant tenant, string siteFullUrl)
        {
            try
            {
                return tenant.CheckIfSiteExists(siteFullUrl, "Active");
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("Cannot get site"))
                {
                    return false;
                }
                LoggingUtility.Internal.TraceError((int)EventId.UnknownExceptionAccessingSite, ex, CoreResources.TenantExtensions_UnknownExceptionAccessingSite);
                throw;
            }
        }

        /// <summary>
        /// Sets tenant site Properties
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl"></param>
        /// <param name="title"></param>
        /// <param name="allowSelfServiceUpgrade"></param>
        /// <param name="sharingCapability"></param>
        /// <param name="storageMaximumLevel"></param>
        /// <param name="storageWarningLevel"></param>
        /// <param name="userCodeMaximumLevel"></param>
        /// <param name="userCodeWarningLevel"></param>
        public static void SetSiteProperties(this Tenant tenant, string siteFullUrl,
            string title = null,
            Nullable<bool> allowSelfServiceUpgrade = null,
            Nullable<SharingCapabilities> sharingCapability = null,
            Nullable<long> storageMaximumLevel = null,
            Nullable<long> storageWarningLevel = null,
            Nullable<double> userCodeMaximumLevel = null,
            Nullable<double> userCodeWarningLevel = null
            )
        {
            var siteProps = tenant.GetSitePropertiesByUrl(siteFullUrl, true);
            tenant.Context.Load(siteProps);
            tenant.Context.ExecuteQuery();
            if (siteProps != null)
            {
                if (allowSelfServiceUpgrade != null)
                    siteProps.AllowSelfServiceUpgrade = allowSelfServiceUpgrade.Value;
                if (sharingCapability != null)
                    siteProps.SharingCapability = sharingCapability.Value;
                if (storageMaximumLevel != null)
                    siteProps.StorageMaximumLevel = storageMaximumLevel.Value;
                if (storageWarningLevel != null)
                    siteProps.StorageWarningLevel = storageMaximumLevel.Value;
                if (userCodeMaximumLevel != null)
                    siteProps.UserCodeMaximumLevel = userCodeMaximumLevel.Value;
                if (userCodeWarningLevel != null)
                    siteProps.UserCodeWarningLevel = userCodeWarningLevel.Value;
                if (title != null)
                    siteProps.Title = title;

                siteProps.Update();
                tenant.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Checks if a site collection exists, relies on tenant admin API
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">URL to the site collection</param>
        /// <returns>True if existing, false if not</returns>
        public static bool SiteExists(this Tenant tenant, string siteFullUrl)
        {
            try
            {
                //Get the site name
                var properties = tenant.GetSitePropertiesByUrl(siteFullUrl, false);
                tenant.Context.Load(properties);
                tenant.Context.ExecuteQuery();

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
        /// Checks if a sub site exists
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">URL to the sub site</param>
        /// <returns>True if existing, false if not</returns>
        public static bool SubSiteExists(this Tenant tenant, string siteFullUrl)
        {
            try
            {
                return tenant.CheckIfSiteExists(siteFullUrl, "");
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
        /// Adds additional administrators to a site collection using the Tenant administration csom. See AddAdministrators for a method
        /// that does not have a dependency on the Tenant administration csom.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="adminLogins">Array of logins for the additional admins</param>
        /// <param name="siteUrl">Url of the site to operate on</param>
        public static void AddAdministrators(this Tenant tenant, String[] adminLogins, Uri siteUrl)
        {
            if (adminLogins == null)
                throw new ArgumentNullException("adminLogins");

            if (siteUrl == null)
                throw new ArgumentNullException("siteUrl");

            foreach (var admin in adminLogins)
            {
                var siteUrlString = siteUrl.ToString();
                tenant.SetSiteAdmin(siteUrlString, admin, true);
                tenant.Context.ExecuteQuery();

                using (var clientContext = new ClientContext(siteUrl))
                {
                    var spAdmin = clientContext.Web.EnsureUser(admin);
                    clientContext.Web.AssociatedOwnerGroup.Users.AddUser(spAdmin);
                    clientContext.Web.AssociatedOwnerGroup.Update();
                    clientContext.ExecuteQuery();
                }
            }
        }

        /// <summary>
        /// Add a site collection administrator to a site collection
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <param name="adminLogins">Array of admins loginnames to add</param>
        /// <param name="siteUrl">Url of the site to operate on</param>
        /// <param name="addToOwnersGroup">Optionally the added admins can also be added to the Site owners group</param>
        public static void AddAdministrators(this Tenant tenant, IEnumerable<UserEntity> adminLogins, Uri siteUrl, bool addToOwnersGroup = false)
        {
            if (adminLogins == null)
                throw new ArgumentNullException("adminLogins");

            if (siteUrl == null)
                throw new ArgumentNullException("siteUrl");

            foreach (UserEntity admin in adminLogins)
            {
                var siteUrlString = siteUrl.ToString();
                tenant.SetSiteAdmin(siteUrlString, admin.LoginName, true);
                tenant.Context.ExecuteQuery();
                if (addToOwnersGroup)
                {
                    // Create a separate context to the web
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = tenant.Context.Credentials;
                        var spAdmin = clientContext.Web.EnsureUser(admin.LoginName);
                        clientContext.Web.AssociatedOwnerGroup.Users.AddUser(spAdmin);
                        clientContext.Web.AssociatedOwnerGroup.Update();
                        clientContext.ExecuteQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Returns all site collections in the current Tenant
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public static IList<SiteEntity> GetSiteCollections(this Tenant tenant)
        {
            var sites = new List<SiteEntity>();

            var props = tenant.GetSiteProperties(0, true);
            tenant.Context.Load(props);
            tenant.Context.ExecuteQuery();

            foreach(var prop in props)
            {
                var siteEntity = new SiteEntity();
                siteEntity.Lcid = prop.Lcid;
                siteEntity.SiteOwnerLogin = prop.Owner;
                siteEntity.StorageMaximumLevel = prop.StorageMaximumLevel;
                siteEntity.StorageWarningLevel = prop.StorageWarningLevel;
                siteEntity.Template = prop.Template;
                siteEntity.TimeZoneId = prop.TimeZoneId;
                siteEntity.Title = prop.Title;
                siteEntity.Url = prop.Url;
                siteEntity.UserCodeMaximumLevel = prop.UserCodeMaximumLevel;
                siteEntity.UserCodeWarningLevel = prop.UserCodeWarningLevel;
                sites.Add(siteEntity);
            }
            return sites;
        }

        public static IList<SiteEntity> GetOneDriveSiteCollections(this Tenant tenant)
        {
            var creds = (Microsoft.SharePoint.Client.SharePointOnlineCredentials)tenant.Context.Credentials;

            var sites = new List<SiteEntity>();

            OfficeDevPnP.Core.UPAWebService.UserProfileService svc = new OfficeDevPnP.Core.UPAWebService.UserProfileService();
            
            svc.Url = tenant.Context.Url + "/_vti_bin/UserProfileService.asmx";
            svc.UseDefaultCredentials = false;
            svc.Credentials = tenant.Context.Credentials;

            var authCookie = creds.GetAuthenticationCookie(new Uri(tenant.Context.Url));
            var cookieContainer = new CookieContainer();
            
            cookieContainer.SetCookies(new Uri(tenant.Context.Url), authCookie);
            svc.CookieContainer = cookieContainer;


            var userProfileResult = svc.GetUserProfileByIndex(-1);

            var profileCount = svc.GetUserProfileCount();

            while(int.Parse(userProfileResult.NextValue) != -1)
            {
                var personalSpaceProperty = userProfileResult.UserProfile.Where(p => p.Name == "PersonalSpace").FirstOrDefault();

                if (personalSpaceProperty != null)
                {
                    if (personalSpaceProperty.Values.Any())
                    {
                        var usernameProperty = userProfileResult.UserProfile.Where(p => p.Name == "UserName").FirstOrDefault();
                        var nameProperty = userProfileResult.UserProfile.Where(p => p.Name == "PreferredName").FirstOrDefault();
                        var url = personalSpaceProperty.Values[0].Value as string;
                        var name = nameProperty.Values[0].Value as string;
                        SiteEntity siteEntity = new SiteEntity();
                        siteEntity.Url = url;
                        siteEntity.Title = name;
                        siteEntity.SiteOwnerLogin = usernameProperty.Values[0].Value as string;
                        sites.Add(siteEntity);
                    }
                }
                
                userProfileResult = svc.GetUserProfileByIndex(int.Parse(userProfileResult.NextValue));
            }

            return sites;
        }
    }
}

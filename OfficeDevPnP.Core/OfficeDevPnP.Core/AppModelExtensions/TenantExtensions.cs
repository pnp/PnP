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

#if !CLIENTSDKV15
        #region Site collection creation
        /// <summary>
        /// Adds a SiteEntity by launching site collection creation and waits for the creation to finish
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="properties">Describes the site collection to be created</param>
        /// <param name="removeFromRecycleBin">It true and site is present in recycle bin, it will be removed first from the recycle bin</param>
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
                tenant.Context.ExecuteQueryRetry();

                if (wait)
                {
                    WaitForIsComplete(tenant, op);
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
        #endregion

        #region Site status checks
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
            var siteDomainUrl = url.GetLeftPart(UriPartial.Scheme | UriPartial.Authority);
            int siteNameIndex = url.AbsolutePath.IndexOf('/', 1) + 1;
            var managedPath = url.AbsolutePath.Substring(0, siteNameIndex);
            var siteRelativePath = url.AbsolutePath.Substring(siteNameIndex);
            var isSiteCollection = siteRelativePath.IndexOf('/') == -1;

            //Judge whether this site collection is existing or not
            if (isSiteCollection)
            {
                try
                {
                    var properties = tenant.GetSitePropertiesByUrl(siteFullUrl, false);
                    tenant.Context.Load(properties);
                    tenant.Context.ExecuteQueryRetry();
                    ret = properties.Status.Equals(status, StringComparison.OrdinalIgnoreCase);
                }
                catch(ServerException ex)
                {
                    if (ex.Message.IndexOf("Unable to access site") > -1)
                    {
                        try
                        {
                            //Let's retry to see if this site collection was recycled
                            var deletedProperties = tenant.GetDeletedSitePropertiesByUrl(siteFullUrl);
                            tenant.Context.Load(deletedProperties);
                            tenant.Context.ExecuteQueryRetry();
                            ret = deletedProperties.Status.Equals(status, StringComparison.OrdinalIgnoreCase);
                        }
                        catch
                        {
                            // eat exception
                        }
                    }
                }
            }
            //Judge whether this sub web site is existing or not
            else
            {
                var subsiteUrl = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                            "{0}{1}{2}", siteDomainUrl, managedPath, siteRelativePath.Split('/')[0]);
                var subsiteRelativeUrl = siteRelativePath.Substring(siteRelativePath.IndexOf('/') + 1);
                var site = tenant.GetSiteByUrl(subsiteUrl);
                var subweb = site.OpenWeb(subsiteRelativeUrl);
                tenant.Context.Load(subweb, w => w.Title);
                tenant.Context.ExecuteQueryRetry();
                ret = true;
            }
            return ret;
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
        /// Checks if a site collection exists, relies on tenant admin API. Sites that are recycled also return as existing sites
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
                tenant.Context.ExecuteQueryRetry();

                // Will cause an exception if site URL is not there. Not optimal, but the way it works.
                return true;
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.SharePoint.Client.ServerException && (ex.Message.IndexOf("Unable to access site") != -1 || ex.Message.IndexOf("Cannot get site") != -1))
                {
                    if (ex.Message.IndexOf("Unable to access site") != -1)
                    {
                        //Let's retry to see if this site collection was recycled
                        try
                        {
                            var deletedProperties = tenant.GetDeletedSitePropertiesByUrl(siteFullUrl);
                            tenant.Context.Load(deletedProperties);
                            tenant.Context.ExecuteQueryRetry();
                            return deletedProperties.Status.Equals("Recycled", StringComparison.OrdinalIgnoreCase);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
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
                return tenant.CheckIfSiteExists(siteFullUrl, "Active");
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
        #endregion

        #region Site collection deletion
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

            try
            {
                SpoOperation op = tenant.RemoveSite(siteFullUrl);
                tenant.Context.Load(tenant);
                tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
                tenant.Context.ExecuteQueryRetry();

                //check if site creation operation is complete
                WaitForIsComplete(tenant, op);
            }
            catch(ServerException ex)
            {
                if (!useRecycleBin && ex.Message.IndexOf("Cannot remove site") > -1 && ex.Message.IndexOf("because the site is not available") > -1)
                {
                    //eat exception as the site might be in the recycle bin and we allowed deletion from recycle bin 
                }
                else
                {
                    throw;
                }
            }

            if (useRecycleBin)
            {
                return true;
            }

            // To delete Site collection completely, (may take a longer time)
            SpoOperation op2 = tenant.RemoveDeletedSite(siteFullUrl);
            tenant.Context.Load(op2, i => i.IsComplete, i => i.PollingInterval);
            tenant.Context.ExecuteQueryRetry();

            WaitForIsComplete(tenant, op2);
            ret = true;

            return ret;
        }

        /// <summary>
        /// Deletes a site collection from the site collection recycle bin
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">URL of the site collection to delete</param>
        /// <param name="wait">If true, processing will halt until the site collection has been deleted from the recycle bin</param>
        /// <returns>True if deleted</returns>
        public static bool DeleteSiteCollectionFromRecycleBin(this Tenant tenant, string siteFullUrl, bool wait = true)
        {
            bool ret = false;
            SpoOperation op = tenant.RemoveDeletedSite(siteFullUrl);
            tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
            tenant.Context.ExecuteQueryRetry();

            if (wait)
            {
                WaitForIsComplete(tenant, op);
            }

            ret = true;
            return ret;
        }
        #endregion

        #region Site collection properties
        /// <summary>
        /// Gets the ID of site collection with specified URL
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">A URL that specifies a site collection to get ID.</param>
        /// <returns>The Guid of a site collection</returns>
        public static Guid GetSiteGuidByUrl(this Tenant tenant, string siteFullUrl)
        {
            if (string.IsNullOrEmpty(siteFullUrl))
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
            tenant.Context.ExecuteQueryRetry();
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

            tenant.Context.ExecuteQueryRetry();

            return templates;
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
            tenant.Context.ExecuteQueryRetry();
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
                tenant.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Sets a site to Unlock access or NoAccess. This operation may occur immediately, but the site lock may take a short while before it goes into effect.
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site (i.e. https://[tenant]-admin.sharepoint.com)</param>
        /// <param name="siteFullUrl">The target site to change the lock state.</param>
        /// <param name="lockState">The target state the site should be changed to.</param>
        /// <param name="wait">If true, processing will halt until the site collection lock state has been implemented</param>      
        public static void SetSiteLockState(this Tenant tenant, string siteFullUrl, SiteLockState lockState, bool wait = false)
        {
            var siteProps = tenant.GetSitePropertiesByUrl(siteFullUrl, true);
            tenant.Context.Load(siteProps);
            tenant.Context.ExecuteQueryRetry();

            LoggingUtility.Internal.TraceInformation(0, CoreResources.TenantExtensions_SetLockState, siteProps.LockState, lockState);

            if (siteProps.LockState != lockState.ToString())
            {
                siteProps.LockState = lockState.ToString();
                SpoOperation op = siteProps.Update();
                tenant.Context.Load(op, i => i.IsComplete, i => i.PollingInterval);
                tenant.Context.ExecuteQueryRetry();

                if (wait)
                {
                    WaitForIsComplete(tenant, op);
                }

            }
        }
        #endregion

        #region Site collection administrators
        /// <summary>
        /// Add a site collection administrator to a site collection
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
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
                tenant.Context.ExecuteQueryRetry();
                if (addToOwnersGroup)
                {
                    // Create a separate context to the web
                    using (var clientContext = tenant.Context.Clone(siteUrl))
                    {
                        var spAdmin = clientContext.Web.EnsureUser(admin.LoginName);
                        clientContext.Web.AssociatedOwnerGroup.Users.AddUser(spAdmin);
                        clientContext.Web.AssociatedOwnerGroup.Update();
                        clientContext.ExecuteQueryRetry();
                    }
                }
            }
        }
        #endregion

        #region Site enumeration
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
            tenant.Context.ExecuteQueryRetry();

            foreach (var prop in props)
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
                siteEntity.CurrentResourceUsage = prop.CurrentResourceUsage;
                siteEntity.LastContentModifiedDate = prop.LastContentModifiedDate;
                siteEntity.StorageUsage = prop.StorageUsage;
                siteEntity.WebsCount = prop.WebsCount;
                sites.Add(siteEntity);
            }
            return sites;
        }

        /// <summary>
        /// Get OneDrive site collections by iterating through all user profiles.
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns>List of <see cref="SiteEntity"/> objects containing site collection info.</returns>
        public static IList<SiteEntity> GetOneDriveSiteCollections(this Tenant tenant)
        {
            var sites = new List<SiteEntity>();
            var svcClient = GetUserProfileServiceClient(tenant);

            // get all user profiles
            var userProfileResult = svcClient.GetUserProfileByIndex(-1);
            var profileCount = svcClient.GetUserProfileCount();

            while (int.Parse(userProfileResult.NextValue) != -1)
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

                userProfileResult = svcClient.GetUserProfileByIndex(int.Parse(userProfileResult.NextValue));
            }

            return sites;
        }

        /// <summary>
        /// Gets the UserProfileService proxy to enable calls to the UPA web service.
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns>UserProfileService web service client</returns>
        public static OfficeDevPnP.Core.UPAWebService.UserProfileService GetUserProfileServiceClient(this Tenant tenant)
        {
            var client = new OfficeDevPnP.Core.UPAWebService.UserProfileService();

            client.Url = tenant.Context.Url + "/_vti_bin/UserProfileService.asmx";
            client.UseDefaultCredentials = false;
            client.Credentials = tenant.Context.Credentials;

            if (tenant.Context.Credentials is SharePointOnlineCredentials)
            {
                var creds = (SharePointOnlineCredentials)tenant.Context.Credentials;
                var authCookie = creds.GetAuthenticationCookie(new Uri(tenant.Context.Url));
                var cookieContainer = new CookieContainer();

                cookieContainer.SetCookies(new Uri(tenant.Context.Url), authCookie);
                client.CookieContainer = cookieContainer;
            }
            return client;
        }
        #endregion

        #region Private helper methods
        private static void WaitForIsComplete(Tenant tenant, SpoOperation op)
        {
            while (!op.IsComplete)
            {
                System.Threading.Thread.Sleep(op.PollingInterval);
                op.RefreshLoad();
                if (!op.IsComplete)
                {
                    try
                    {
                        tenant.Context.ExecuteQueryRetry();
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
        #endregion
#else
        /// <summary>
        /// Adds a SiteEntity by launching site collection creation and waits for the creation to finish
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="properties">Describes the site collection to be created</param>
        public static void CreateSiteCollection(this Tenant tenant, SiteEntity properties)
        {
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
                tenant.CreateSite(newsite);
                tenant.Context.ExecuteQueryRetry();
            }
            catch (Exception ex)
            {
                // Eat the siteSubscription exception to make the same code work for MT as on-prem April 2014 CU+
                if (ex.Message.IndexOf("Parameter name: siteSubscription") == -1)
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// Deletes a site collection
        /// </summary>
        /// <param name="tenant">A tenant object pointing to the context of a Tenant Administration site</param>
        /// <param name="siteFullUrl">Url of the site collection to delete</param>
        public static void DeleteSiteCollection(this Tenant tenant, string siteFullUrl)
        {
            tenant.RemoveSite(siteFullUrl);
            tenant.Context.ExecuteQueryRetry();
        }

#endif
    }
}

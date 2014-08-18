using OfficeDevPnP.SPOnline.Core.Utils;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Core
{
    public static class SPOSite
    {
        public static void EnableAppSideLoading(ClientContext clientContext)
        {
            ClientResult<bool> appsideloading = Microsoft.SharePoint.Client.AppCatalog.IsAppSideloadingEnabled(clientContext);
            clientContext.ExecuteQuery();
            if (appsideloading.Value == false)
            {
                Site site = clientContext.Site;
                SPOFeatures.ActivateFeature(new Guid(Properties.Resources.AppSideLoadingFeatureGuid), false, SPOFeatures.FeatureScope.Site, clientContext);
            }
        }

        public static void DisableAppSideLoading(ClientContext clientContext)
        {
            ClientResult<bool> appsideloading = Microsoft.SharePoint.Client.AppCatalog.IsAppSideloadingEnabled(clientContext);
            clientContext.ExecuteQuery();
            if (appsideloading.Value == true)
            {
                Site site = clientContext.Site;
                SPOFeatures.DeactivateFeature(new Guid(Properties.Resources.AppSideLoadingFeatureGuid), false, SPOFeatures.FeatureScope.Site, clientContext);
            }
        }

        public static void DeleteTenantSite(string url, Tenant tenant, bool wait, bool skipTrash)
        {
            SpoOperation removedSite = tenant.RemoveSite(url);

            tenant.Context.Load(removedSite);
            tenant.Context.ExecuteQuery();

            if (wait || skipTrash)
            {
                Poll(removedSite);
                if (skipTrash)
                {
                    DeleteTenantSiteFromRecycleBin(url, tenant, wait);
                }
            }
        }

        public static void DeleteTenantSiteFromRecycleBin(string url, Tenant tenant, bool wait)
        {
            var deletedsites = GetDeletedSites(tenant);
            var deletedSite = deletedsites.Where(x => x.Url == url).FirstOrDefault();
            if (deletedSite != null)
            {
                SpoOperation removedSite = tenant.RemoveDeletedSite(url);
                tenant.Context.Load(removedSite);
                tenant.Context.ExecuteQuery();
                if (wait)
                {
                    Poll(removedSite);
                }
            }
        }

        //private static SiteManager.SiteManagerClient GetSiteManagerClient(string serviceUrl, NetworkCredential credentials)
        //{
        //    BasicHttpBinding binding = new BasicHttpBinding();
        //    if (serviceUrl.ToLower().Contains("https://"))
        //    {
        //        binding.Security.Mode = BasicHttpSecurityMode.Transport;
        //    }
        //    else
        //    {
        //        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
        //    }
        //    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

        //    EndpointAddress endPoint = new EndpointAddress(serviceUrl);
        //    //Set time outs
        //    binding.ReceiveTimeout = TimeSpan.FromMinutes(15);
        //    binding.CloseTimeout = TimeSpan.FromMinutes(15);
        //    binding.OpenTimeout = TimeSpan.FromMinutes(15);
        //    binding.SendTimeout = TimeSpan.FromMinutes(15);

        //    //Create proxy instance
        //    SiteManager.SiteManagerClient managerClient = new SiteManager.SiteManagerClient(binding, endPoint);
        //    if (credentials != null)
        //    {
        //        managerClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

        //        managerClient.ClientCredentials.Windows.ClientCredential = credentials;
        //    }
        //    return managerClient;
        //}

        //public static void NewSite(string serverUrl, string serviceUrl, NetworkCredential credentials, string title, string ownerLogin, string secondaryContactLogin, string description, string template, string url, UInt16 lcid)
        //{
        //    SiteManager.SiteManagerClient siteManagerClient = GetSiteManagerClient(serverUrl.TrimEnd('/') + serviceUrl, credentials);

        //    SiteManager.SiteData data = new SiteManager.SiteData();
        //    data.Title = title;
        //    data.OwnerLogin = ownerLogin;
        //    data.SecondaryContactLogin = secondaryContactLogin;
        //    data.Description = description;
        //    data.WebTemplate = template;
        //    data.Url = url;
        //    data.LcId = lcid.ToString();
        //    siteManagerClient.CreateSiteCollection(data);
        //}

        public static void NewTenantSite(string title, string url, string template, string owner, uint lcid, int timeZoneId, double userCodeMaximumLevel, double userCodeWarningLevel, long storageMaximumLevel, long storageWarningLevel, Tenant tenant, bool wait, bool removedDeletedSiteFirst)
        {
            if (tenant != null)
            {
                if (removedDeletedSiteFirst)
                {
                    var deletedsites = SPOSite.GetDeletedSites(tenant);
                    var deletedSite = deletedsites.Where(x => x.Url == url).FirstOrDefault();
                    if (deletedSite != null)
                    {
                        tenant.RemoveDeletedSite(url);
                        tenant.Context.ExecuteQuery();
                    }

                }
                SiteCreationProperties newSite = new SiteCreationProperties();
                newSite.CompatibilityLevel = 15;
                newSite.Lcid = lcid;
                newSite.Owner = owner;
                newSite.StorageMaximumLevel = storageMaximumLevel;
                newSite.StorageWarningLevel = storageWarningLevel;
                newSite.Template = template;
                newSite.TimeZoneId = (int)timeZoneId;
                newSite.Title = title;
                newSite.Url = url;
                newSite.UserCodeMaximumLevel = userCodeMaximumLevel;
                newSite.UserCodeWarningLevel = userCodeWarningLevel;
                SpoOperation site = tenant.CreateSite(newSite);
                tenant.Context.Load(site);
                tenant.Context.ExecuteQuery();

                if (wait)
                {
                    Poll(site);
                }
            }
        }

        public static void Poll(SpoOperation spoOperation)
        {
            while (!spoOperation.IsComplete)
            {
                if (spoOperation.HasTimedout)
                    throw new TimeoutException(Properties.Resources.OperationTimedOut);
                Thread.Sleep(spoOperation.PollingInterval);
                spoOperation.Context.Load(spoOperation);
                spoOperation.Context.ExecuteQuery();
            }
        }

        public static SPOTenant GetTenant(string administrationUrl, ICredentials credentials)
        {
            Tenant tenant = null;
            using (ClientContext c = new ClientContext(administrationUrl))
            {
                c.Credentials = credentials;
                tenant = new Tenant(c);
                c.Load(tenant);
                c.ExecuteQuery();

            }

            return new SPOTenant() { Tenant = tenant };
        }

        public static Site GetSite(ClientContext clientContext)
        {
            Site site = clientContext.Site;
            clientContext.Load(site);

            clientContext.ExecuteQuery();

            return site;
        }

        public static object GetTenantSiteProperties(Tenant tenant, bool detailed)
        {
            var list = tenant.GetSiteProperties(0, detailed);
            list.Context.Load(list);
            list.Context.ExecuteQuery();
            return list;
        }

        public static object GetTenantSitePropertiesByUrl(Tenant tenant, string url, bool detailed)
        {
            var site = tenant.GetSitePropertiesByUrl(url, detailed);
            site.Context.Load(site);
            site.Context.ExecuteQuery();
            return site;

        }

        public static SPODeletedSitePropertiesEnumerable GetDeletedSites(Tenant tenant)
        {
            var sites = tenant.GetDeletedSiteProperties(0);
            tenant.Context.Load(sites);
            tenant.Context.ExecuteQuery();
            return sites;
        }

        public class SPOTenant : IDisposable
        {
            public Tenant Tenant { get; set; }

            public void Dispose()
            {

            }
        }

    }

}

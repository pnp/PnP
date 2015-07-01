using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Provisioning.Cloud.Management.Utils;
using model = Provisioning.Cloud.Management.Models;

namespace Provisioning.Cloud.Management.Repositories
{
    public interface ISharePointRepository
    {
        Task<IEnumerable<model.Site>> GetSitesAsync();

        Task<model.Site> CreateSiteAsync(model.Site siteProperties);

        Task<bool> DeleteSiteAsync(string uri);

        Task<IEnumerable<model.LanguageVM>> GetAvailableLanguagesAsync();

        Task<IEnumerable<model.WebTemplateVM>> GetWebTemplatesAsync(uint lcid);
    }

    public class SharePointRepository : ISharePointRepository
    {
        private readonly TokenProvider _tokenProvider;

        public SharePointRepository()
        {
            _tokenProvider = new TokenProvider();
        }

        public async Task<IEnumerable<model.Site>> GetSitesAsync()
        {
            // Get the access token
            String accessToken = await _tokenProvider.GetSharePointAdminAccessToken();

            // Keep track of results
            List<model.Site> resultSiteProperties = new List<model.Site>();

            // Get the clientcontext
            using (ClientContext ctx = GetClientContext(SettingsHelper.SharePointAdminResourceUri, accessToken))
            {
                // Create tenant
                Tenant tenant = new Tenant(ctx);

                // Get the site properties
                SPOSitePropertiesEnumerable allSiteProperties = tenant.GetSiteProperties(0, true);

                // Load
                ctx.Load(allSiteProperties);
                await ctx.ExecuteQueryAsync();

                // Loop the properties
                foreach (SiteProperties siteProp in allSiteProperties)
                {
                    resultSiteProperties.Add(new model.Site(siteProp));
                }
            }

            // Return the result
            return resultSiteProperties;
        }

        public async Task<model.Site> CreateSiteAsync(model.Site siteProperties)
        {
            // Get the access token
            String accessToken = await _tokenProvider.GetSharePointAdminAccessToken();

            // Keep track of the result
            model.Site resultSiteProperties = null;

            // Get the clientcontext
            using (ClientContext ctx = GetClientContext(SettingsHelper.SharePointAdminResourceUri, accessToken))
            {
                // Create tenant
                Tenant tenant = new Tenant(ctx);

                // Create properties
                SiteCreationProperties siteCreationProperties =
                    new SiteCreationProperties()
                    {
                        Url = siteProperties.Uri
                        ,
                        Title = siteProperties.Title
                        ,
                        Lcid = siteProperties.Language
                        ,
                        Template = siteProperties.Template
                        ,
                        Owner = siteProperties.Owner
                        ,
                        StorageMaximumLevel = siteProperties.StorageMaximumLevel
                        ,
                        UserCodeMaximumLevel = siteProperties.UserCodeMaximumLevel
                    };

                // Create the sitecollection
                SpoOperation operation = tenant.CreateSite(siteCreationProperties);

                // Execute query
                await ctx.ExecuteQueryAsync();

                // Reload the site
                SiteProperties actualSiteProperties = tenant.GetSitePropertiesByUrl(siteProperties.Uri, true);
                ctx.Load(actualSiteProperties);
                await ctx.ExecuteQueryAsync();

                // Set result
                resultSiteProperties = new model.Site(actualSiteProperties);
            }

            return resultSiteProperties;
        }

        public async Task<bool> DeleteSiteAsync(string uri)
        {
             // Get the access token
            String accessToken = await _tokenProvider.GetSharePointAdminAccessToken();

            try
            {
                // Get the clientcontext
                using (ClientContext ctx = GetClientContext(SettingsHelper.SharePointAdminResourceUri, accessToken))
                {
                    // Create tenant
                    Tenant tenant = new Tenant(ctx);

                    // Perform delete
                    SpoOperation spoOperation = tenant.RemoveSite(uri);

                    // Load and execute
                    ctx.Load(spoOperation);
                    await ctx.ExecuteQueryAsync();
                }

                return true;
            }
            catch (ServerException)
            {
                // Do not propagate
                return false;
            }
        }

        public async Task<IEnumerable<model.LanguageVM>> GetAvailableLanguagesAsync()
        {
            // Get the access token
            String accessToken = await _tokenProvider.GetSharePointAdminAccessToken();

            // Keep track of results
            List<model.LanguageVM> result = new List<model.LanguageVM>();

            // Get the clientcontext
            using (ClientContext ctx = GetClientContext(SettingsHelper.SharePointAdminResourceUri, accessToken))
            {
                // Load
                ctx.Load(ctx.Web, w => w.SupportedUILanguageIds);

                // Execute query
                await ctx.ExecuteQueryAsync();

                // Loop the languages
                if (ctx.Web.SupportedUILanguageIds != null)
                {
                    result.AddRange(ctx.Web.SupportedUILanguageIds.Select(lcid => new model.LanguageVM(lcid)));
                }
            }

            // Sort the result
            result.Sort((l1, l2) => l1.DisplayName.CompareTo(l2.DisplayName));

            // Return
            return result;
        }

        public async Task<IEnumerable<model.WebTemplateVM>> GetWebTemplatesAsync(uint lcid)
        {
             // Get the access token
            String accessToken = await _tokenProvider.GetSharePointAdminAccessToken();

            // Keep track of results
            List<model.WebTemplateVM> result = new List<model.WebTemplateVM>();

            // Get the clientcontext
            using (ClientContext ctx = GetClientContext(SettingsHelper.SharePointAdminResourceUri, accessToken))
            {
                // Get the webtemplates
                WebTemplateCollection webTemplates = ctx.Web.GetAvailableWebTemplates(lcid, false);

                // Load
                ctx.Load(webTemplates, templates => templates.Where(t => t.IsHidden == false));

                // Execute
                await ctx.ExecuteQueryAsync();

                // Add
                result.AddRange(webTemplates.ToList().Select(webTemplate => new model.WebTemplateVM(webTemplate)));
            }

            // Sort the result
            result.Sort((t1, t2) => t1.Title.CompareTo(t2.Title));

            // Return context
            return result;
        }

        // Helper method: should be placed in other class
        private ClientContext GetClientContext(String webFullUrl, String accessToken)
        {
            // Create the client context
            ClientContext ctx = new ClientContext(webFullUrl);

            // Set the Bearer
            ctx.ExecutingWebRequest += (sender, e) => { e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + accessToken; };

            // Return context
            return ctx;
        }
    }
}
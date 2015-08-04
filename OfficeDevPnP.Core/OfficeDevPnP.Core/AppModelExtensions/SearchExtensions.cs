using Microsoft.SharePoint.Client.Search.Administration;
using Microsoft.SharePoint.Client.Search.Portability;
using System;
using System.Text;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class for Search extension methods
    /// </summary>
    public static partial class SearchExtensions
    {
        /// <summary>
        /// Exports the search settings to file.
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="exportFilePath">Path where to export the search settings</param>
        /// <param name="searchSettingsExportLevel">Search settings export level
        /// Reference: http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.search.administration.searchobjectlevel(v=office.15).aspx
        /// </param>
        public static void ExportSearchSettings(this ClientContext context, string exportFilePath, SearchObjectLevel searchSettingsExportLevel)
        {
            if (string.IsNullOrEmpty(exportFilePath))
            {
                throw new ArgumentNullException("exportFilePath");
            }

            var searchConfig = GetSearchConfigurationImplementation(context, searchSettingsExportLevel);

            if (searchConfig != null)
            {
                System.IO.File.WriteAllText(exportFilePath, searchConfig, Encoding.ASCII);
            }
            else
            {
                throw new Exception("No search settings to export.");
            }
        }

        /// <summary>
        /// Returns the current search configuration as as string
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static string GetSearchConfiguration(this Web web)
        {
            return GetSearchConfigurationImplementation(web.Context, SearchObjectLevel.SPWeb);
        }

        /// <summary>
        /// Returns the current search configuration as as string
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static string GetSearchConfiguration(this Site site)
        {
            return GetSearchConfigurationImplementation(site.Context, SearchObjectLevel.SPSite);
        }

        /// <summary>
        /// Returns the current search configuration for the specified object level
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchSettingsObjectLevel"></param>
        /// <returns></returns>
        private static string GetSearchConfigurationImplementation(ClientRuntimeContext context, SearchObjectLevel searchSettingsObjectLevel)
        {
            SearchConfigurationPortability sconfig = new SearchConfigurationPortability(context);
            SearchObjectOwner owner = new SearchObjectOwner(context, searchSettingsObjectLevel);

            ClientResult<string> configresults = sconfig.ExportSearchConfiguration(owner);
            context.ExecuteQueryRetry();

            return configresults.Value;
        }

        /// <summary>
        /// Imports search settings from file.
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="searchSchemaImportFilePath">Search schema xml file path</param>
        /// <param name="searchSettingsImportLevel">Search settings import level
        /// Reference: http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.search.administration.searchobjectlevel(v=office.15).aspx
        /// </param>
        public static void ImportSearchSettings(this ClientContext context, string searchSchemaImportFilePath, SearchObjectLevel searchSettingsImportLevel)
        {
            if (string.IsNullOrEmpty(searchSchemaImportFilePath))
            {
                throw new ArgumentNullException("searchSchemaImportFilePath");
            }

            SetSearchConfigurationImplementation(context, searchSettingsImportLevel, System.IO.File.ReadAllText(searchSchemaImportFilePath));

        }

        /// <summary>
        /// Sets the search configuration
        /// </summary>
        /// <param name="web"></param>
        /// <param name="searchConfiguration"></param>
        public static void SetSearchConfiguration(this Web web, string searchConfiguration)
        {
            SetSearchConfigurationImplementation(web.Context, SearchObjectLevel.SPWeb, searchConfiguration);
        }

        /// <summary>
        /// Sets the search configuration
        /// </summary>
        /// <param name="site"></param>
        /// <param name="searchConfiguration"></param>
        public static void SetSearchConfiguration(this Site site, string searchConfiguration)
        {
            SetSearchConfigurationImplementation(site.Context, SearchObjectLevel.SPWeb, searchConfiguration);
        }


        /// <summary>
        /// Sets the search configuration at the specified object level
        /// </summary>
        /// <param name="context"></param>
        /// <param name="searchObjectLevel"></param>
        /// <param name="searchConfiguration"></param>
        private static void SetSearchConfigurationImplementation(ClientRuntimeContext context, SearchObjectLevel searchObjectLevel, string searchConfiguration)
        {
#if CLIENTSDKV15
            if (searchObjectLevel == SearchObjectLevel.Ssa)
            {
                // Reference: https://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.search.portability.searchconfigurationportability_members.aspx
                throw new Exception("You cannot import customized search configuration settings to a Search service application (SSA).");
            }
#endif
            SearchConfigurationPortability searchConfig = new SearchConfigurationPortability(context);
            SearchObjectOwner owner = new SearchObjectOwner(context, searchObjectLevel);

            // Import search configuration
            searchConfig.ImportSearchConfiguration(owner, searchConfiguration);
            context.Load(searchConfig);
            context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Sets the search center url on site collection (Site Settings -> Site collection administration --> Search Settings)
        /// </summary>
        /// <param name="web">SharePoint site - root web</param>
        /// <param name="searchCenterUrl">Search center url</param>
        public static void SetSiteCollectionSearchCenterUrl(this Web web, string searchCenterUrl)
        {
            if (searchCenterUrl == null)
            {
                throw new ArgumentNullException("searchCenterUrl");
            }

            // Currently there is no direct API available to set the search center url on web.
            // Set search setting at web level   

            // if another value was set then respect that
            if (String.IsNullOrEmpty(web.GetPropertyBagValueString("SRCH_SB_SET_SITE", string.Empty)))
            {
                web.SetPropertyBagValue("SRCH_SB_SET_SITE", "{'Inherit':false,'ResultsPageAddress':null,'ShowNavigation':true}");
            }

            if (!string.IsNullOrEmpty(searchCenterUrl))
            {
                // Set search center url
                web.SetPropertyBagValue("SRCH_ENH_FTR_URL_SITE", searchCenterUrl);
            }
            else
            {
                // When search center url is blank remove the property (like the SharePoint UI does)
                web.RemovePropertyBagValue("SRCH_ENH_FTR_URL_SITE");
            }
        }

        /// <summary>
        /// Get the search center url for the site collection (Site Settings -> Site collection administration --> Search Settings)
        /// </summary>
        /// <param name="web">SharePoint site - root web</param>
        /// <returns>Search center url for web</returns>
        public static string GetSiteCollectionSearchCenterUrl(this Web web)
        {
            // Currently there is no direct API available to get the search center url on web.
            // Get search center url
            return web.GetPropertyBagValueString("SRCH_ENH_FTR_URL_SITE", string.Empty);
        }


    }
}

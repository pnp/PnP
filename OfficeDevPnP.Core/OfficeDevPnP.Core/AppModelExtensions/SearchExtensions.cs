using Microsoft.SharePoint.Client.Search.Administration;
using Microsoft.SharePoint.Client.Search.Portability;
using System;
using System.Text;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class for Search utility methods
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
            SearchConfigurationPortability sconfig = new SearchConfigurationPortability(context);
            SearchObjectOwner owner = new SearchObjectOwner(context, searchSettingsExportLevel);

            ClientResult<string> configresults = sconfig.ExportSearchConfiguration(owner);
            context.ExecuteQuery();

            if (configresults.Value != null)
            {
                string results = configresults.Value;
                System.IO.File.WriteAllText(exportFilePath, results, Encoding.ASCII);
            }
            else
            {
                throw new Exception("No search settings to export.");
            }
        }

#if !CLIENTSDKV15

        /// <summary>
        /// Imports search settings from file.
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="searchSchemaXMLPath">Search schema xml file path</param>
        /// <param name="searchSettingsImportLevel">Search settings import level
        /// Reference: http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.search.administration.searchobjectlevel(v=office.15).aspx
        /// </param>
        public static void ImportSearchSettings(this ClientContext context, string searchSchemaXMLPath, SearchObjectLevel searchSettingsImportLevel)
        {
            SearchConfigurationPortability searchConfig = new SearchConfigurationPortability(context);
            SearchObjectOwner owner = new SearchObjectOwner(context, searchSettingsImportLevel);

            // Import search configuration
            searchConfig.ImportSearchConfiguration(owner, System.IO.File.ReadAllText(searchSchemaXMLPath));
            context.Load(searchConfig);
            context.ExecuteQueryRetry();
        }

#else

        /// <summary>
        /// Imports search settings from file.
        /// </summary>
        /// <param name="context">Context for SharePoint objects and operations</param>
        /// <param name="searchSchemaXMLPath">Search schema xml file path</param>
        /// <param name="searchSettingsImportLevel">Search settings import level
        /// Reference: http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.search.administration.searchobjectlevel(v=office.15).aspx
        /// </param>
        public static void ImportSearchSettings(this ClientContext context, string searchSchemaXMLPath, SearchObjectLevel searchSettingsImportLevel)
        {
            if (searchSettingsImportLevel == SearchObjectLevel.Ssa)
            {
                // Reference: https://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.search.portability.searchconfigurationportability_members.aspx
                throw new Exception("You cannot import customized search configuration settings to a Search service application (SSA).");
            }

            SearchConfigurationPortability searchConfig = new SearchConfigurationPortability(context);
            SearchObjectOwner owner = new SearchObjectOwner(context, searchSettingsImportLevel);

            // Import search configuration
            searchConfig.ImportSearchConfiguration(owner, System.IO.File.ReadAllText(searchSchemaXMLPath));
            context.Load(searchConfig);
            context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Sets the search center url on web
        /// </summary>
        /// <param name="web">SharePoint site - root web</param>
        /// <param name="searchCenterUrl">Search center url</param>
        public static void SetSearchCenterUrl(this Web web, string searchCenterUrl)
        {
            // Currently there is no direct API available to set the search center url on web.
            // Set search setting at web level          
            web.SetPropertyBagValue("SRCH_SB_SET_SITE", "{'Inherit':false,'ResultsPageAddress':null,'ShowNavigation':true}");

            // Set search center url
            web.SetPropertyBagValue("SRCH_ENH_FTR_URL_SITE", searchCenterUrl);
        }

        /// <summary>
        /// Get the search center url for web
        /// </summary>
        /// <param name="web">SharePoint site - root web</param>
        /// <returns>Search center url for web</returns>
        public static string GetSearchCenterUrl(this Web web)
        {
            // Currently there is no direct API available to get the search center url on web.
            // Get search center url
            return web.GetPropertyBagValueString("SRCH_ENH_FTR_URL_SITE", string.Empty);
        }

#endif
    }
}

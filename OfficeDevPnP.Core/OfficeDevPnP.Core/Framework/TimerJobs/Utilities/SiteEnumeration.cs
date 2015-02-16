using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.TimerJobs.Utilities
{
    /// <summary>
    /// Singleton class that's responsible for resolving wildcard site Url's into a list af site Url's
    /// </summary>
    internal class SiteEnumeration
    {
        private List<string> sites;

        #region Singleton implementation
        // Singleton variables
        private static volatile SiteEnumeration instance;
        private static object syncRoot = new Object();

        // Singleton private constructor
        private SiteEnumeration() { }

        /// <summary>
        /// Singleton instance to access this class
        /// </summary>
        public static SiteEnumeration Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SiteEnumeration();
                    }
                }

                return instance;
            }
        }
        #endregion  
     
        #region Site resolving
        /// <summary>
        /// Builds up a list of site collections that match the passed site wildcard. This method can be used against Office 365
        /// </summary>
        /// <param name="tenant">Tenant object to use for resolving the regular sites</param>
        /// <param name="siteWildCard">The widcard site Url (e.g. https://tenant.sharepoint.com/sites/*) </param>
        /// <param name="resolvedSites">List of site collections matching the passed wildcard site Url</param>
        internal void ResolveSite(Tenant tenant, string siteWildCard, List<string> resolvedSites)
        {
            //strip the wildcard
            string searchString = siteWildCard.Substring(0, siteWildCard.IndexOf("*"));

            // If we did not yet load all sites then do it...this one is only hit the first time
            if (this.sites == null)
            {
                // Loads all regular Office 365 site collections via the tenant admin API and uses search to resolve the Onedrive site collections
                FillSitesViaTenantAPIAndSearch(tenant);
            }

            //iterate the found site collections and add the sites that match to the site wildcard
            MatchSites(resolvedSites, searchString);
        }

        /// <summary>
        /// Builds up a list of site collections that match the passed site wildcard. This method can be used against on-premises
        /// </summary>
        /// <param name="context">ClientContext object of an arbitrary site collection accessible by the defined enumeration username and password</param>
        /// <param name="siteWildCard">The widcard site Url (e.g. https://tenant.sharepoint.com/sites/*) </param>
        /// <param name="resolvedSites">List of site collections matching the passed wildcard site Url</param>
        internal void ResolveSite(ClientContext context, string site, List<string> resolvedSites)
        {
            //strip the wildcard
            string searchString = site.Substring(0, site.IndexOf("*"));

            // If we did not yet load all sites then do it...this one is only hit the first time
            if (this.sites == null)
            {
                // Load all site collections via search
                FillSitesViaSearch(context);
            }

            MatchSites(resolvedSites, searchString);
        }

        private void MatchSites(List<string> resolvedSites, string searchString)
        {
            foreach (string availableSite in this.sites)
            {
                if (availableSite.Contains(searchString))
                {
                    if (!resolvedSites.Contains(availableSite))
                    {
                        resolvedSites.Add(availableSite);
                    }
                }
            }
        }

        /// <summary>
        /// Fill site list via tenant API for "regular" site collections. Search API is used for OneDrive for Business site collections
        /// </summary>
        /// <param name="tenant">Tenant object to operate against</param>
        private void FillSitesViaTenantAPIAndSearch(Tenant tenant)
        {
            // Use tenant API to get the regular sites
            var props = tenant.GetSiteProperties(0, false);
            tenant.Context.Load(props);
            tenant.Context.ExecuteQueryRetry();

            if (props.Count == 0)
            {
                return;
            }
            else
            {
                if (this.sites == null)
                {
                    this.sites = new List<string>(props.Count);
                }
            }

            foreach (var prop in props)
            {        
                this.sites.Add(prop.Url.ToLower());
            }

            // Use search api to get the OneDrive sites
            this.sites.AddRange(SiteSearch(tenant.Context, "contentclass:\"STS_Site\" AND WebTemplate:SPSPERS"));

        }

        /// <summary>
        /// Fill site list via the Search API. Applies to all type of sites. Typically used in on-premises environments
        /// </summary>
        /// <param name="context">ClientContext object of an arbitrary site collection accessible by the defined enumeration username and password</param>
        private void FillSitesViaSearch(ClientContext context)
        {
            if (this.sites == null)
            {
                this.sites = new List<string>();
            }

            // Use search api to get the OneDrive and regular sites
            this.sites.AddRange(SiteSearch(context, ""));
        }

        /// <summary>
        /// Get all sites that match the passed query. Batching is done in batches of 500 as this is compliant for both Office 365 as SharePoint on-premises
        /// </summary>
        /// <param name="cc">ClientContext object of an arbitrary site collection accessible by the defined enumeration username and password</param>
        /// <param name="keywordQueryValue">Query string</param>
        /// <returns>List of found site collections</returns>
        private static List<String> SiteSearch(ClientRuntimeContext cc, string keywordQueryValue)
        {
            List<String> sites = new List<String>();

            KeywordQuery keywordQuery = new KeywordQuery(cc);
            // Important to avoid trimming "similar" site collections
            keywordQuery.TrimDuplicates = false;

            if (keywordQueryValue.Length == 0)
            {
                keywordQueryValue = "contentclass:\"STS_Site\"";
            }

            int startRow = 0;
            int totalRows = 0;

            totalRows = ProcessQuery(cc, keywordQueryValue, sites, keywordQuery, startRow);

            if (totalRows > 0)
            {
                while (totalRows >= sites.Count)
                {
                    startRow += 500;
                    totalRows = ProcessQuery(cc, keywordQueryValue, sites, keywordQuery, startRow);
                }
            }

            return sites;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cc">ClientContext object of an arbitrary site collection accessible by the defined enumeration username and password</param>
        /// <param name="keywordQueryValue">Query to execute</param>
        /// <param name="sites">List of found site collections</param>
        /// <param name="keywordQuery">KeywordQuery instance that will perform the actual queries</param>
        /// <param name="startRow">Row as of which we want to see the results</param>
        /// <returns>Total result rows of the query</returns>
        private static int ProcessQuery(ClientRuntimeContext cc, string keywordQueryValue, List<string> sites, KeywordQuery keywordQuery, int startRow)
        {
            int totalRows = 0;

            keywordQuery.QueryText = keywordQueryValue;
            keywordQuery.RowLimit = 500;
            keywordQuery.StartRow = startRow;
            keywordQuery.SelectProperties.Add("SPSiteUrl");
            keywordQuery.SortList.Add("SPSiteUrl", SortDirection.Ascending);
            SearchExecutor searchExec = new SearchExecutor(cc);
            ClientResult<ResultTableCollection> results = searchExec.ExecuteQuery(keywordQuery);
            cc.ExecuteQueryRetry();

            if (results != null)
            {
                if (results.Value[0].RowCount > 0)
                {
                    totalRows = results.Value[0].TotalRows;

                    foreach (var row in results.Value[0].ResultRows)
                    {
                        if (row["SPSiteUrl"] != null)
                        {
                            sites.Add(row["SPSiteUrl"].ToString());
                        }
                    }
                }
            }

            return totalRows;
        }
        #endregion
    }
}

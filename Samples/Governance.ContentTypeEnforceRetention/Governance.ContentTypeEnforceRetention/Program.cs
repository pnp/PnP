using Core.Throttling;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;

namespace Governance.ContentTypeEnforceRetention
{
    public static class Program
    {
        /// <summary>
        ///     To register the app:
        ///     1) Go to appregnew.aspx to create the client ID and client secret
        ///     2) Copy the client ID and client secret to app.config
        ///     3) Go to appinv.aspx to lookup by client ID and add permission XML below
        /// </summary>
        /// <param name="args"></param>
        /*
        <AppPermissionRequests AllowAppOnlyPolicy="true">
           <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="Write" />
        </AppPermissionRequests>
       */

        private static void Main(string[] args)
        {
            //Read site collections to apply policy to from app.config file
            var configSites = (NameValueCollection)ConfigurationManager.GetSection("Sites");
            var configContentTypeRetentionPolicyPeriods =
                (NameValueCollection)ConfigurationManager.GetSection("ContentTypeRetentionPolicyPeriod");
            //Iterate through site collections
            foreach (var key in configSites.Keys)
            {
                var siteUrls = configSites.GetValues(key as string);
                if (siteUrls != null)
                {
                    //Build ClientContext
                    var sitetUri = new Uri(siteUrls[0]);
                    var siteRealm = TokenHelper.GetRealmFromTargetUrl(sitetUri);
                    var tenantAccessToken =
                        TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, sitetUri.Authority, siteRealm)
                            .AccessToken;
                    using (
                        var clientContext = TokenHelper.GetClientContextWithAccessToken(sitetUri.ToString(),
                            tenantAccessToken))
                    {
                        //Get all webs for site collection including root web
                        var webs = GetAllWebs(clientContext);

                        foreach (var web in webs)
                        {
                            //Get all document libraries. Lists are excluded.
                            var documentLibraries = GetAllDocumentLibraries(clientContext, web);

                            //Iterate through all document libraries
                            foreach (var documentLibrary in documentLibraries)
                            {
                                //Iterate through configured content type retention policies in app.config
                                foreach (var contentTypeName in configContentTypeRetentionPolicyPeriods.Keys)
                                {
                                    var retentionPeriods =
                                        configContentTypeRetentionPolicyPeriods.GetValues(contentTypeName as string);
                                    if (retentionPeriods != null)
                                    {
                                        var retentionPeriod = int.Parse(retentionPeriods[0]);
                                        ApplyRetentionPolicy(clientContext, documentLibrary, contentTypeName,
                                            retentionPeriod);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        ///     Apply retention policy to old files
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="documentLibrary"></param>
        /// <param name="contentTypeName"></param>
        /// <param name="retentionPeriod"></param>
        private static void ApplyRetentionPolicy(ClientContext clientContext, List documentLibrary,
            object contentTypeName, int retentionPeriod)
        {
            //Calculate validation date. Any document modified before that date is considered old
            var validationDate = DateTime.Now.AddDays(-retentionPeriod);
            var camlDate = validationDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

            //Get old documents in the library that are matching requested content type
            if (documentLibrary.ItemCount > 0)
            {
                var camlQuery = new CamlQuery();
                camlQuery.ViewXml = String.Format(
                    @"<View>
                        <Query>
                            <Where><And>
                                <Eq><FieldRef Name='ContentType' /><Value Type='Computed'>{0}</Value></Eq>
                                <Lt><FieldRef Name='Modified' /><Value Type='DateTime'>{1}</Value></Lt>
                            </And></Where>
                        </Query>
                    </View>", contentTypeName, camlDate);

                var listItems = documentLibrary.GetItems(camlQuery);
                clientContext.Load(listItems,
                    items => items.Include(
                        item => item.Id,
                        item => item.DisplayName,
                        item => item.ContentType));
                //clientContext.ExecuteQuery(); // Commented out in favor of next line and execution throttling
                clientContext.ExecuteQueryWithExponentialRetry(5, 30000); //5 retries, with a base delay of 10 secs.

                foreach (var listItem in listItems)
                {
                    Console.WriteLine("Document {0} has been last modified earlier than {1}.Retention policy will be applied.", listItem.DisplayName, validationDate);
                    ApplyRetentionAction(clientContext, listItem);
                }
            }
        }

        /// <summary>
        ///     Return all document libraries from specified web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="listItem"></param>
        private static void ApplyRetentionAction(ClientContext clientContext, ListItem listItem)
        {
            ////We can delete old items.
            //listItem.DeleteObject();

            //We can start a perhaps workflow against them, move to archive folder etc.
        }

        /// <summary>
        ///     Return all document libraries from specified web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        private static IEnumerable<List> GetAllDocumentLibraries(ClientContext clientContext, Web web)
        {
            //Retrieve all lists from specified web
            var lists = web.Lists;
            clientContext.Load(lists);
            //clientContext.ExecuteQuery(); // Commented out in favor of next line and execution throttling
            clientContext.ExecuteQueryWithExponentialRetry(5, 30000); //5 retries, with a base delay of 10 secs.

            //Filter out only document libraries and append to return list collection
            var libraries = new List<List>();
            foreach (var list in lists)
            {
                if (list.BaseType.ToString() == "DocumentLibrary")
                {
                    libraries.Add(list);
                }
            }
            return libraries;
        }

        /// <summary>
        ///     Return all webs from site context
        /// </summary>
        /// <param name="clientContext"></param>
        private static List<Web> GetAllWebs(ClientContext clientContext)
        {
            //Retrieve all subwebs. This excludes root web.
            var subWebs = clientContext.Site.RootWeb.Webs;
            //Retrieve root web.
            var rootWeb = clientContext.Site.RootWeb;
            clientContext.Load(subWebs);
            clientContext.Load(rootWeb);
            //clientContext.ExecuteQuery(); // Commented out in favor of next line and execution throttling
            clientContext.ExecuteQueryWithExponentialRetry(5, 30000); //5 retries, with a base delay of 10 secs.

            //Add sub webs + root web to list. Insert root web in the beginning of list just for clarity.
            var allWebs = subWebs.ToList();
            allWebs.Insert(0, rootWeb);

            return allWebs;
        }
    }
}
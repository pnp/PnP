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
        static NameValueCollection configSites;
        static NameValueCollection configContentTypeRetentionPolicyPeriods;
        /// <summary>
        ///     To register the app:
        ///     1) Go to appregnew.aspx to create the client ID and client secret
        ///     2) Copy the client ID and client secret to app.config
        ///     3) Go to appinv.aspx to lookup by client ID and add permission XML below
        ///     /*
        ///      <AppPermissionRequests AllowAppOnlyPolicy="true">
        ///        <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="Write" />
        ///      </AppPermissionRequests>
        ///     */
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            //Read site collections to apply policy to from app.config file
            configSites = (NameValueCollection)ConfigurationManager.GetSection("Sites");
            configContentTypeRetentionPolicyPeriods =
                (NameValueCollection)ConfigurationManager.GetSection("ContentTypeRetentionPolicyPeriod");

            //Iterate through site collections
            foreach (var key in configSites.Keys)
            {
                var siteUrls = configSites.GetValues(key as string);
                if (siteUrls != null)
                {
                    //Build ClientContext with AppOnly Token
                    var sitetUri = new Uri(siteUrls[0]);
                    var siteRealm = TokenHelper.GetRealmFromTargetUrl(sitetUri);
                    var tenantAccessToken = TokenHelper.GetAppOnlyAccessToken
                        (TokenHelper.SharePointPrincipal, sitetUri.Authority, siteRealm).AccessToken;

                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken
                            (sitetUri.ToString(), tenantAccessToken))
                    {
                        //Retrieve root web.
                        var rootWeb = clientContext.Site.RootWeb;
                        clientContext.Load(rootWeb);
                        clientContext.ExecuteQueryWithExponentialRetry(5, 30000);

                        ProcessWebRecursively(clientContext, rootWeb);
                    }
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// This method will recurse through a tree of Web objects
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        private static void ProcessWebRecursively(ClientContext clientContext, Web web)
        {
            ProcessLibrariesInWeb(clientContext, web);

            var subWebs = web.Webs;
            clientContext.Load(subWebs);
            clientContext.ExecuteQueryWithExponentialRetry(5, 30000);

            foreach (var subWeb in subWebs)
            {
                ProcessWebRecursively(clientContext, subWeb);
            }
        }

        /// <summary>
        /// This method goes through all libraries in a given web and iterates for each defined content type
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        private static void ProcessLibrariesInWeb(ClientContext clientContext, Web web)
        {
            Console.WriteLine("Scanning web " + web.Url);

            //Get all document libraries. Lists are excluded.
            var documentLibraries = GetAllDocumentLibrariesInWeb(clientContext, web);

            //Iterate through all document libraries
            foreach (var documentLibrary in documentLibraries)
            {
                Console.WriteLine("Scanning library " + documentLibrary.Title);

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

        /// <summary>
        /// This method executes a CAML query to get all old documents by content type
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="documentLibrary"></param>
        /// <param name="contentTypeName"></param>
        /// <param name="retentionPeriod"></param>
        private static void ApplyRetentionPolicy(ClientContext clientContext, List documentLibrary,
            object contentTypeId, int retentionPeriodDays)
        {
            //Calculate validation date. Any document modified before that date is considered old
            var validationDate = DateTime.Now.AddDays(-retentionPeriodDays);
            var camlDate = validationDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

            //Get old documents in the library that are matching requested content type
            if (documentLibrary.ItemCount > 0)
            {
                var camlQuery = new CamlQuery();

                //This CAML query uses Content Type ID with BeginsWith.
                //You can replace with ContentType for CT Display Name, for example
                //<Eq><FieldRef Name='ContentType' /><Value Type='Computed'>{0}</Value></Eq>
                camlQuery.ViewXml = String.Format(
                    @"<View>
                        <Query>
                            <Where><And>
                                <BeginsWith><FieldRef Name='ContentTypeId'/><Value Type='ContentTypeId'>{0}</Value></BeginsWith>
                                <Lt><FieldRef Name='Modified' /><Value Type='DateTime'>{1}</Value></Lt>
                            </And></Where>
                        </Query>
                    </View>", contentTypeId, camlDate);

                var listItems = documentLibrary.GetItems(camlQuery);
                clientContext.Load(listItems,
                    items => items.Include(
                        item => item.Id,
                        item => item.DisplayName,
                        item => item.ContentType));

                //clientContext.ExecuteQuery(); // Commented out in favor of next line and execution throttling
                clientContext.ExecuteQueryWithExponentialRetry(10, 30000); //10 retries, with a base delay of 30 secs.

                foreach (var listItem in listItems)
                {
                    Console.WriteLine("Document '{0}' has been modified earlier than {1}. Retention policy will be applied.", listItem.DisplayName, validationDate);
                    ApplyRetentionActions(clientContext, listItem);
                }

                //perform Retention Actions
                clientContext.ExecuteQueryWithExponentialRetry(10, 30000);
            }
        }

        /// <summary>
        ///     Return all document libraries from specified web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="listItem"></param>
        private static void ApplyRetentionActions(ClientContext clientContext, ListItem listItem)
        {
            Console.WriteLine("Applying retention actions to " + listItem.DisplayName);
            //We can delete old items:
            //listItem.DeleteObject();

            //We can start a workflow against them, move to archive folder etc.
        }

        /// <summary>
        ///     Return all document libraries from specified web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="web"></param>
        private static IEnumerable<List> GetAllDocumentLibrariesInWeb(ClientContext clientContext, Web web)
        {
            //Retrieve all lists from specified web
            var lists = web.Lists;
            clientContext.Load(lists);
            //clientContext.ExecuteQuery(); // Commented out in favor of next line and execution throttling
            clientContext.ExecuteQueryWithExponentialRetry(5, 30000); //5 retries, with a base delay of 30 secs.

            //Filter out only document libraries and append to return list collection
            var libraries = new List<List>();
            foreach (var list in lists)
            {
                if (list.BaseType.ToString() == "DocumentLibrary")
                {
                    libraries.Add(list);
                }
            }
            Console.WriteLine("The number of libraries found: " + libraries.Count);
            return libraries;
        }
    }
}
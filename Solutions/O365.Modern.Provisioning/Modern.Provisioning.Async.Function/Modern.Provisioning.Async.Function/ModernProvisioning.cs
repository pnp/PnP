using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Sites;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace Modern.Provisioning.Async.Function
{
    public static class ModernProvisioning
    {
        private static string adminPassword;

        [FunctionName("ModernProvisioning")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            if (adminPassword == null)
            {
                // This is the part where I grab the secret.
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                log.Info("Getting the secret.");
                var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                log.Info("KeyVaultSecret: " + Environment.GetEnvironmentVariable("KeyVaultSecret"));
                adminPassword = (await kvClient.GetSecretAsync(Environment.GetEnvironmentVariable("KeyVaultSecret"))).Value;
            }

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;
            string title = data?.title;

            string siteUrl = string.Empty;
            string adminUser = Environment.GetEnvironmentVariable("spAdminUser");
            log.Info("adminUser: " + adminUser);
            string spSite = Environment.GetEnvironmentVariable("spSite");
            log.Info("spSite: " + adminUser);
            System.Security.SecureString secureString = new System.Security.SecureString();
            foreach (char ch in adminPassword)
            {
                secureString.AppendChar(ch);
            }
            string sitesRequest = Environment.GetEnvironmentVariable("listName");
            log.Info("listName: " + sitesRequest);
            Dictionary<string, string> siteInfo = new Dictionary<string, string>();
            OfficeDevPnP.Core.AuthenticationManager authManager = new OfficeDevPnP.Core.AuthenticationManager();
            string camlQuery =
                "<View>" +
                    "<Query>" +
                        "<Where>" +
                            "<Eq>" +
                                "<FieldRef Name='Status' />" +
                                "<Value Type='Choice'>Approved</Value>" +
                            "</Eq>" +
                        "</Where>" +
                    "</Query>" +
                    "<RowLimit>1</RowLimit>" +
                "</View>";
            CamlQuery cq = new CamlQuery();
            cq.ViewXml = camlQuery;
            using (var context = authManager.GetSharePointOnlineAuthenticatedContextTenant(spSite, adminUser, secureString))
            {
                List list = context.Web.Lists.GetByTitle(sitesRequest);
                ListItemCollection lic = list.GetItems(cq);
                context.Load(lic);
                context.ExecuteQuery();
                foreach (ListItem item in lic)
                {
                    siteInfo.Add("Id", item["ID"].ToString());
                    siteInfo.Add("title", item["Title"].ToString());
                    siteInfo.Add("owner", item["Owner"].ToString());
                    siteInfo.Add("description", item["Description"] == null ? "" : item["Description"].ToString());
                    siteInfo.Add("type", item["SiteType"].ToString());
                    siteInfo.Add("alias", item["Alias"].ToString());
                    log.Info("Processing: " + item["Title"].ToString());
                    var siteType = siteInfo["type"];
                    switch (siteType.ToLower())
                    {
                        case "communicationsite":
                            var ctx = context.CreateSiteAsync(new CommunicationSiteCollectionCreationInformation
                            {
                                Title = siteInfo["title"].ToString(),
                                Owner = siteInfo["owner"].ToString(),
                                Lcid = 1033,
                                Description = siteInfo["description"].ToString(),
                                Url = spSite + "/sites/" + siteInfo["alias"].ToString(),
                            }).GetAwaiter().GetResult();
                            // Add OWner
                            User user = ctx.Web.EnsureUser(siteInfo["owner"].ToString());
                            ctx.Web.Context.Load(user);
                            ctx.Web.Context.ExecuteQueryRetry();
                            ctx.Web.AssociatedOwnerGroup.Users.AddUser(user);
                            ctx.Web.AssociatedOwnerGroup.Update();
                            ctx.Web.Context.ExecuteQueryRetry();
                            break;
                        case "teamsite":
                            var ctxTeamsite = context.CreateSiteAsync(new TeamSiteCollectionCreationInformation
                            {
                                DisplayName = siteInfo["title"].ToString(),
                                Description = siteInfo["description"].ToString(),
                                Alias = siteInfo["alias"].ToString(),
                                IsPublic = false,
                            }).GetAwaiter().GetResult();
                            siteUrl = ctxTeamsite.Url;
                            // Add OWner
                            User userTeamSite = ctxTeamsite.Web.EnsureUser(siteInfo["owner"].ToString());
                            ctxTeamsite.Web.Context.Load(userTeamSite);
                            ctxTeamsite.Web.Context.ExecuteQueryRetry();
                            ctxTeamsite.Web.AssociatedOwnerGroup.Users.AddUser(userTeamSite);
                            ctxTeamsite.Web.AssociatedOwnerGroup.Update();
                            ctxTeamsite.Web.Context.ExecuteQueryRetry();
                            break;
                        case "teams":
                            string token = Graph.getToken();
                            log.Info("Access Token: " + token);
                            string userId = string.Empty;
                            string groupId = string.Empty;
                            if (string.IsNullOrEmpty(token) == false)
                            {
                                userId = Graph.getUser(token, siteInfo["owner"].ToString());
                                log.Info("userId: " + userId);
                            }
                            if (string.IsNullOrEmpty(userId) == false)
                            {
                                string dataPost = 
                                    "{ 'displayName': '" + siteInfo["title"].ToString() + "', 'groupTypes': ['Unified'], 'mailEnabled': true, 'mailNickname': '" + siteInfo["alias"].ToString().Replace("\r\n", "").Replace(" ","") + "', 'securityEnabled': false, 'owners@odata.bind': ['https://graph.microsoft.com/v1.0/users/" + userId + "'], 'visibility': 'Private' }";
                                groupId = Graph.createUnifiedGroup(token, dataPost);
                                log.Info("userId: " + groupId);
                                //Graph.addOwnerToUnifiedGroup(token, groupId, userId);
                                //removeOwnerToUnifiedGroup(token, groupId, userId);
                            }
                            siteUrl = siteInfo["title"].ToString();
                            log.Info("Teams ready: " + siteUrl);
                            break;
                    }
                    // When the site or Teams has been created the status of the list item will change in ready
                    if (siteUrl != string.Empty)
                    {
                        item["Status"] = "Ready";
                        item.Update();

                        context.ExecuteQuery();
                    }
                }
            }

            return siteUrl == null
                ? req.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong!")
                : req.CreateResponse(HttpStatusCode.OK, siteUrl);

        }
    }
}

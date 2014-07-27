using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.PermissionListingWeb
{
    public partial class Default : System.Web.UI.Page
    {
        string outPutText = string.Empty;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // define initial script, needed to render the chrome control
            string script = @"
            function chromeLoaded() {
                $('body').show();
            }

            //function callback to render chrome after SP.UI.Controls.js loads
            function renderSPChrome() {
                //Set the chrome options for launching Help, Account, and Contact pages
                var options = {
                    'appTitle': document.title,
                    'onCssLoaded': 'chromeLoaded()'
                };

                //Load the Chrome Control in the divSPChrome element of the page
                var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
                chromeNavigation.setVisible(true);
            }";

            //register script in page
            Page.ClientScript.RegisterClientScriptBlock(typeof(Default), "BasePageScript", script, true);
        }

        protected void btnListPermissions_Click(object sender, EventArgs e)
        {
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                GetPermissions(clientContext);
            }
        }

        private void GetPermissions(ClientContext clientContext)
        {
            List<SiteData> allSites = GetSites(clientContext);

            outPutText = string.Empty;
            foreach (var site in allSites)
            {
                CallAnotherSite(site.Url);
            }
            lblStatus.Text = outPutText;
        }

        private void CallAnotherSite(string siteUrl)
        {
            var siteUri = new Uri(siteUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            string token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), token))
            {
                var web = ctx.Web;
                ctx.Load(web);

                outPutText += ctx.Web.Url + "<br/>";
                ProcessRoleAssignments(web, ctx);

                WebCollection webs = web.Webs;
                ctx.Load<WebCollection>(webs);
                ctx.ExecuteQuery();

                foreach (var subWeb in webs)
                {
                    outPutText += subWeb.Url + "<br/>";
                    ProcessRoleAssignments(subWeb, ctx);
                }
            }
        }


        private void ProcessRoleAssignments(SecurableObject securableObject, ClientContext clientContext)
        {
            clientContext.Load(securableObject, x => x.HasUniqueRoleAssignments);
            clientContext.ExecuteQuery();

            if (!securableObject.HasUniqueRoleAssignments)
            {
                outPutText += "Same perms as parent" + "<br/>";
            }
            else
            {
                RoleAssignmentCollection roleAssignments = securableObject.RoleAssignments;

                clientContext.Load<RoleAssignmentCollection>(roleAssignments);
                clientContext.ExecuteQuery();

                foreach (RoleAssignment roleAssignment in roleAssignments)
                {
                    Principal member = roleAssignment.Member;
                    RoleDefinitionBindingCollection roleDef = roleAssignment.RoleDefinitionBindings;

                    clientContext.Load(member);
                    clientContext.Load<RoleDefinitionBindingCollection>(roleDef);
                    clientContext.ExecuteQuery();

                    foreach (var binding in roleDef)
                    {
                        outPutText += string.Format("[{0}]{1}: {2}<br/>", member.PrincipalType, member.LoginName, binding.Name);
                    }
                }
            }
        }


        private List<SiteData> GetSites(ClientContext clientContext)
        {
            List<SiteData> sites = new List<SiteData>();

            KeywordQuery keywordQuery = new KeywordQuery(clientContext);
            string keywordQueryValue = "contentclass:\"STS_Site\"";

            ProcessQuery(keywordQueryValue, sites, clientContext, keywordQuery, 0);

            return sites;
        }

        private int ProcessQuery(string keywordQueryValue, List<SiteData> sites, ClientContext ctx, KeywordQuery keywordQuery, int startRow)
        {
            int totalRows = 0;

            keywordQuery.QueryText = keywordQueryValue;
            keywordQuery.RowLimit = 500;
            keywordQuery.StartRow = startRow;
            keywordQuery.SelectProperties.Add("Title");
            keywordQuery.SelectProperties.Add("SPSiteUrl");
            keywordQuery.SelectProperties.Add("Description");
            keywordQuery.SelectProperties.Add("WebTemplate");
            keywordQuery.SortList.Add("SPSiteUrl", Microsoft.SharePoint.Client.Search.Query.SortDirection.Ascending);
            SearchExecutor searchExec = new SearchExecutor(ctx);
            ClientResult<ResultTableCollection> results = searchExec.ExecuteQuery(keywordQuery);
            ctx.ExecuteQuery();

            if (results != null)
            {
                if (results.Value[0].RowCount > 0)
                {
                    totalRows = results.Value[0].TotalRows;

                    foreach (var row in results.Value[0].ResultRows)
                    {
                        sites.Add(new SiteData
                        {
                            Title = row["Title"] != null ? row["Title"].ToString() : "",
                            Url = row["SPSiteUrl"] != null ? row["SPSiteUrl"].ToString() : "",
                            Description = row["Description"] != null ? row["Description"].ToString() : "",
                        });
                    }
                }
            }

            return totalRows;
        }
    }
}
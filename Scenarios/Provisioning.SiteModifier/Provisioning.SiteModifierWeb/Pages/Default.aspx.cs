using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.SiteModifierWeb
{
    public partial class Default : System.Web.UI.Page
    {
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                AddJsLink(ctx);
                lblStatus.Text = string.Format("Modify Site link has been added to the Site Actions meny of <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }

        private void AddJsLink(Microsoft.SharePoint.Client.ClientContext ctx)
        {

            Web web = ctx.Web;
            ctx.Load(web, w => w.UserCustomActions);
            ctx.ExecuteQuery();

            ctx.Load(web, w => w.UserCustomActions, w => w.Url, w => w.AppInstanceId);
            ctx.ExecuteQuery();

            UserCustomAction userCustomAction = web.UserCustomActions.Add();
            userCustomAction.Location = "Microsoft.SharePoint.StandardMenu";
            userCustomAction.Group = "SiteActions";
            BasePermissions perms = new BasePermissions();
            perms.Set(PermissionKind.ManageWeb);
            userCustomAction.Rights = perms;
            userCustomAction.Sequence = 100;
            userCustomAction.Title = "Modify Site";

            string realm = TokenHelper.GetRealmFromTargetUrl(new Uri(ctx.Url));
            string issuerId = WebConfigurationManager.AppSettings.Get("ClientId");

            var modifyPageUrl = string.Format("https://{0}/Pages/Modify.aspx?{{StandardTokens}}", Request.Url.Authority);
            string url = "javascript:LaunchApp('{0}', 'i:0i.t|ms.sp.ext|{1}@{2}','{3}',{{width:300,height:200,title:'Modify Site'}});";
            url = string.Format(url, Guid.NewGuid().ToString(), issuerId, realm, modifyPageUrl);

            userCustomAction.Url = url;
            userCustomAction.Update();
            ctx.ExecuteQuery();

            // Remove the entry from the 'Recents' node
            NavigationNodeCollection nodes = web.Navigation.QuickLaunch;
            ctx.Load(nodes, n => n.IncludeWithDefaultProperties(c => c.Children));
            ctx.ExecuteQuery();
            var recent = nodes.Where(x => x.Title == "Recent").FirstOrDefault();
            if (recent != null)
            {
                var appLink = recent.Children.Where(x => x.Title == "Site Modifier").FirstOrDefault();
                if (appLink != null) appLink.DeleteObject();
                ctx.ExecuteQuery();
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                DeleteJsLink(ctx);
                lblStatus.Text = string.Format("Modify Site link has been removed from the Site Actions menu of <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
            }
        }

        private void DeleteJsLink(ClientContext ctx)
        {
            Web web = ctx.Web;
            ctx.Load(web, w => w.UserCustomActions);
            ctx.ExecuteQuery();

            foreach (var action in web.UserCustomActions)
            {
                if (action.Title == "Modify Site")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                    break;
                }
            }
        }
    }
}
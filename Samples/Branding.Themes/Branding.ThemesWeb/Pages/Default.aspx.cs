using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Branding.ThemesWeb
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

        protected void btnScenario1_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;
                    string selectedTheme = drpThemes.SelectedValue;
                    web.SetComposedLookByUrl(selectedTheme);
                    lblStatus1.Text = string.Format("Theme '{0}' has been applied to the <a href='{1}'>host web</a>.", selectedTheme, spContext.SPHostUrl.ToString());
                }
            }
        }

        protected void btnScenario2_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    clientContext.Site.RootWeb.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/SPC/SPCTheme.spcolor")));
                    clientContext.Site.RootWeb.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/SPC/SPCbg.jpg")));

                    Web web = clientContext.Web;
                    clientContext.Load(clientContext.Site, w => w.RootWeb.ServerRelativeUrl); // loading RootWeb.ServerRelativeUrl property;
                    clientContext.ExecuteQuery();
                    // Let's first upload the contoso theme to host web, if it does not exist there
                    web.CreateComposedLookByUrl("SPC",
                                    clientContext.Site.RootWeb.ServerRelativeUrl + "/_catalogs/theme/15/SPCTheme.spcolor",
                                    null,
                                    clientContext.Site.RootWeb.ServerRelativeUrl + "/_catalogs/theme/15/SPCbg.jpg",
                                    string.Empty);

                    // Setting the Contoos theme to host web
                    web.SetComposedLookByUrl("SPC");
                    lblStatus2.Text = string.Format("Custom theme called 'SPC' has been uploaded and applied to the <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString());
                }
            }

        }
    }
}
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Provisioning.SubSiteCreationAppWeb
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

            lblBasePath.Text = Request["SPHostUrl"] + "/";
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Team", "STS#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Super Team", "STS#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Über Team", "STS#0"));
            listSites.SelectedIndex = 0;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                Web newWeb = CreateSubSite(ctx, ctx.Web, txtUrl.Text, listSites.SelectedValue, txtTitle.Text, txtDescription.Text);
                // Redirect to just created site
                Response.Redirect(newWeb.Url);

            }
        }

        public Web CreateSubSite(Microsoft.SharePoint.Client.ClientContext ctx, Web hostWeb, string txtUrl,
                                 string template, string title, string description)
        {
            // Create web creation configuration
            WebCreationInformation information = new WebCreationInformation();
            information.WebTemplate = template;
            information.Description = description;
            information.Title = title;
            information.Url = txtUrl;
            // Currently all English, could be extended to be configurable based on language pack usage
            information.Language = 1033;

            Microsoft.SharePoint.Client.Web newWeb = null;
            newWeb = hostWeb.Webs.Add(information);
            ctx.ExecuteQuery();

            ctx.Load(newWeb);
            ctx.ExecuteQuery();

            // Add sub site link override
            new LabHelper().AddJsLink(ctx, newWeb, this.Request);

            // Let's first upload the custom theme to host web
            new LabHelper().DeployThemeToWeb(hostWeb, "MyCustomTheme",
                            HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/custom.spcolor")),
                            string.Empty,
                            HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/custombg.jpg")),
                            string.Empty);

            // Setting the Custom theme to host web
            new LabHelper().SetThemeBasedOnName(ctx, newWeb, hostWeb, "MyCustomTheme");

            // Set logo to the site

            // Get the path to the file which we are about to deploy
            new LabHelper().UploadAndSetLogoToSite(ctx.Web, System.Web.Hosting.HostingEnvironment.MapPath(
                                                            string.Format("~/{0}", "resources/pnp.png")));

            // All done, let's return the newly created site
            return newWeb;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }


    }
}
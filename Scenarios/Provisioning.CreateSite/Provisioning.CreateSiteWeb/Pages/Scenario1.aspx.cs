using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace Provisioning.CreateSiteWeb.Pages
{
    public partial class Scenario1 : System.Web.UI.Page
    {
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

            if (!Page.IsPostBack)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    Web web = ctx.Web;
                    ctx.Load(web);
                    ctx.ExecuteQuery();
                    // Get templates to choose from
                    WebTemplateCollection templates = ctx.Web.GetAvailableWebTemplates(ctx.Web.Language, false);
                    ctx.Load(templates);
                    ctx.ExecuteQuery();

                    drpContentTypes.Items.Clear();
                    foreach (var item in templates)
                    {
                        // Provide options for the template
                        drpContentTypes.Items.Add(new System.Web.UI.WebControls.ListItem(item.Title, item.Name));
                        drpContentTypes.SelectedValue = "STS#0";
                    }
                }
            }
        }

        protected void btnCheckUrl_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                if (ctx.Web.WebExists(txtUrl.Text))
                {
                    lblStatus1.Text = "URL has been already taken for sub site.";
                }
                else
                {
                    lblStatus1.Text = "URL is available for sub site.";
                }
            }

        }

        protected void btnCreateSite_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                if (!ctx.Web.WebExists(txtUrl.Text))
                {
                    // Create the sub site
                    Web newWeb = ctx.Web.CreateWeb(txtName.Text, txtUrl.Text, "Description", drpContentTypes.SelectedValue, 1033);

                    // Let's add two document libraries to the site 
                    newWeb.CreateDocumentLibrary("Specifications");
                    newWeb.CreateDocumentLibrary("Presentations");

                    // Let's also apply theme to the site to demonstrate how easy this is
                    newWeb.SetComposedLookByUrl("Characters");

                    string newUrl = ctx.Web.Url + "/" + txtUrl.Text;
                    lblStatus1.Text = string.Format("New sub site created. Check the site from <a href='{0}'>here</a>", newUrl);
                }
                else
                {
                    lblStatus1.Text = "URL has been already taken for sub site. Creation cancelled.";
                }
            }
        }
    }
}
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.XMLBasedFieldsAndContentTypesWeb
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
                // Start XML importing from the xml file located in this provider hosted app
                clientContext.Web.CreateFieldsFromXMLFile(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SiteColumns.xml")));
                lblStatus.Text = string.Format("Site columns created to the host web. Check for <a href='#'>site columns</a> with group name of 'Contoso Columns'.",
                                                spContext.SPHostUrl.ToString() + "/_layouts/15/mngfield.aspx");
            }
        }

        protected void btnScenario2_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                // Start XML importing from the xml file located in this provider hosted app
                clientContext.Web.CreateContentTypeFromXMLFile(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/contenttypes.xml")));
                lblStatus2.Text = string.Format("Content types created to the host web. Check for <a href='#'>content types</a> with group name of 'Contoso'.",
                                                spContext.SPHostUrl.ToString() + "/_layouts/15/mngctype.aspx");
            }
        }

        protected void btnScenario3_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext.Web.ListExists(txtDocLib.Text))
                {
                    lblStatus3.Text = "Library with that name already exists";
                }

                // Create document library
                clientContext.Web.CreateDocumentLibrary(txtDocLib.Text);
                List list = clientContext.Web.GetListByTitle(txtDocLib.Text);

                // Add content type to the document library - This assumes that scenario 2 has been executed
                list.AddContentTypeToListByName("ContosoWorkspaceDocument");

                // Create views for document library
                list.CreateViewsFromXMLFile(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/ListViews.xml")));

                lblStatus3.Text = string.Format("Document library called <a href='{0}'>{1}</a> was created with few custom views.",
                                                spContext.SPHostUrl.ToString() + "/" + txtDocLib.Text, txtDocLib.Text);
            }
        }
    }
}
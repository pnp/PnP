using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.Services.Authenticate.SharePointWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private ClientContext cc;

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

            if (Page.IsPostBack)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
                cc = spContext.CreateUserClientContextForSPHost();
            }

            //register the web API service in this SharePoint app
            Page.RegisterWebAPIService("api/demo/register");
            //register an other web API service (hosted in a different domain)
            //Page.RegisterWebAPIService("api/demo/register", new Uri("https://localhost:44350/"));
            Page.RegisterWebAPIService("api/demo/register", new Uri("https://bjansencorswebapi.azurewebsites.net/"));
        }

        protected void btnCreateTestData_Click(object sender, EventArgs e)
        {
            List demoList = null;
            if (!cc.Web.ListExists("WebAPIDemo"))
            {
                demoList = cc.Web.CreateList(ListTemplateType.GenericList, "WebAPIDemo", false);
                AddDemoItem(demoList, "Item 1");
                AddDemoItem(demoList, "Item 2");
                AddDemoItem(demoList, "Item 3");
                AddDemoItem(demoList, "Item 4");
                AddDemoItem(demoList, "Item 5");
                cc.ExecuteQuery();
            }
        }

        private void AddDemoItem(List demoList, string title)
        {
            Microsoft.SharePoint.Client.ListItem item = demoList.AddItem(new ListItemCreationInformation());
            item["Title"] = title;
            item.Update();
        }


        protected void btnCleanupTestData_Click(object sender, EventArgs e)
        {
            if (cc.Web.ListExists("WebAPIDemo"))
            {
                List demoList = cc.Web.GetListByTitle("WebAPIDemo");
                demoList.DeleteObject();
                cc.ExecuteQuery();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.SharePoint.Client;

namespace ECM.AutoTaggingWeb
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

            if (!Page.IsPostBack)
            {

            }
        }

        protected void btnScenario1_Click(object sender, EventArgs e)
        {

            var _libraryToCreate = this.GetLibraryToCreate();

            Uri sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);

            string _realm = TokenHelper.GetRealmFromTargetUrl(sharepointUrl);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, sharepointUrl.Authority, _realm).AccessToken;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken))
            {

               // EventReceiverDefinitionCreationInformation _recUpdating = ReceiverHelper.CreateEventReciever("InformationManagementItemUpdating", EventReceiverType.ItemUpdating);
                EventReceiverDefinitionCreationInformation _recAdding = ReceiverHelper.CreateEventReciever("InformationManagementItemItemAdding", EventReceiverType.ItemAdding);
                List _list = clientContext.Web.Lists.GetByTitle(_libraryToCreate.Title);
                clientContext.Load(_list, er => er.EventReceivers);
                clientContext.ExecuteQuery();


              //  ReceiverHelper.AddEventReceiver(clientContext, _list, _recUpdating);
                ReceiverHelper.AddEventReceiver(clientContext, _list, _recAdding);
            }
        }
            
        protected void btnScenario2_Click(object sender, EventArgs e)
        {

            Uri sharepointUrl = new Uri(Request.QueryString["SPHostUrl"]);

            string _realm = TokenHelper.GetRealmFromTargetUrl(sharepointUrl);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, sharepointUrl.Authority, _realm).AccessToken;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken))
            {
     
                List _list = clientContext.Web.Lists.GetByTitle("TEST");

            //    ReceiverHelper.RemoveEventReceiver(clientContext, _list, "InformationManagementItemUpdating");
                ReceiverHelper.RemoveEventReceiver(clientContext, _list, "InformationManagementItemItemAdding");
            }
        }

        private Library GetLibraryToCreate()
        {
            Library _libraryToCreate = new Library()
            {
                Title = "AutoTaggingSample",
                Description = "This is a demo",
                VerisioningEnabled = false
            };
            return _libraryToCreate;
        }
    }
}
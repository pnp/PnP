using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.EventReceiverBasedModificationsWeb
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

            //if (Page.IsPostBack)
            //{
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            cc = spContext.CreateUserClientContextForSPHost();
            cc.Load(cc.Web.EventReceivers);
            cc.ExecuteQuery();

            StringBuilder sb = new StringBuilder();

            foreach (EventReceiverDefinition rer in cc.Web.EventReceivers)
            {
                sb.Append(string.Format("Type:<B>{0}</B>, Url:<B>{1}</B>, Class:{3}, Assembly:{2} <BR/>", rer.EventType, rer.ReceiverUrl, rer.ReceiverAssembly, rer.ReceiverClass));
            }

            lblEventReceivers.Text = sb.ToString();

            //}
        }
    }
}
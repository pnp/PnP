using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.Settings.LocaleAndLanguageWeb
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
            // Just routing of the app token cross post backs if http is used with ACS in on-prem.
            // Quick fix for http handling.
            if (!string.IsNullOrEmpty(this.Request.Form["SPAppToken"]))
            {
                SPAppToken.Value = this.Request.Form["SPAppToken"];
            }

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

            if (!IsPostBack)
            {
                ddlLocales.Items.Add(new System.Web.UI.WebControls.ListItem("English - US", "1033"));
                ddlLocales.Items.Add(new System.Web.UI.WebControls.ListItem("English - UK", "2057"));
                ddlLocales.Items.Add(new System.Web.UI.WebControls.ListItem("Finnish", "1035"));
                ddlLocales.Items.Add(new System.Web.UI.WebControls.ListItem("Swedish", "1053"));

                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    UpdateLanguageList(clientContext);
                }
            }
        }

        private void UpdateLanguageList(ClientContext clientContext)
        {
            clientContext.Load(clientContext.Web, w => w.SupportedUILanguageIds);
            clientContext.ExecuteQuery();

            lblCurrentlySupportedLanguages.Text = "";
            foreach (var item in clientContext.Web.SupportedUILanguageIds)
            {
                lblCurrentlySupportedLanguages.Text = lblCurrentlySupportedLanguages.Text + " | " + item;
            }
        }

        protected void btnScenario_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();

                // Set regional settings to host web and execute the query
                web.RegionalSettings.LocaleId = uint.Parse(ddlLocales.SelectedValue);
                web.RegionalSettings.Update();
                clientContext.ExecuteQuery();

                lblStatus.Text = string.Format("Regional setting updated based on selection to <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString() + "/_layouts/15/regionalsetng.aspx");
            }
        }

        protected void btnScenario2Add_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Web.AddSupportedUILanguage(1035);
                clientContext.Web.Update();
                clientContext.ExecuteQuery();

                UpdateLanguageList(clientContext);
                lblStatus2.Text = string.Format("Language settings updated to <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString() + "/_layouts/15/muisetng.aspx");
               
            }
        }

        protected void btnScenario2Remove_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Web.RemoveSupportedUILanguage(1035);
                clientContext.Web.Update();
                clientContext.ExecuteQuery();

                lblStatus2.Text = string.Format("Language settings updated to <a href='{0}'>host web</a>.", spContext.SPHostUrl.ToString() + "/_layouts/15/muisetng.aspx");
 
            }
        }
    }
}
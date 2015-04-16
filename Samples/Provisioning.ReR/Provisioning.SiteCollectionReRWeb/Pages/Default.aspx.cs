using Contoso.Provisioning.SiteCollectionCreationWeb.Models;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Provisioning.SiteCollectionCreationWeb
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


            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Team", "STS#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Super Team", "STS#0"));
            listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Über Team", "STS#0"));
            listSites.SelectedIndex = 0;

            lblBasePath.Text = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/sites/";
        }


        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            string newWebUrl = string.Empty;
            using (ClientContext ctx = spContext.CreateUserClientContextForSPHost())
            {

                SharePointUser currentUser;
                ctx.Load(ctx.Web.CurrentUser);
                ctx.ExecuteQuery();

                var user = ctx.Web.CurrentUser;
                currentUser = new SharePointUser()
                {
                    Email = user.Email,
                    Login = user.LoginName,
                    Name = user.Title
                };


                var siteRequestInfo = new SiteRequestInformation()
                {
                    Title = this.txtTitle.Text,
                    Description = this.txtDescription.Text,
                    EnumStatus = SiteRequestStatus.New,
                    Template = "STS#0",
                    SiteOwner = currentUser,
                  
               
                };

                siteRequestInfo.Url = string.Format(@"{0}{1}", this.lblBasePath.Text, siteRequestInfo.Title);
                LabHelper _helper = new LabHelper();
                _helper.AddRequest(siteRequestInfo, ctx);
            }


            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }
    }
}
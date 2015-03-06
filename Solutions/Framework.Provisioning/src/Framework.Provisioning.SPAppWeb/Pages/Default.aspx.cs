using Framework.Provisioning.Core;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Data;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Framework.Provisioning.SPAppWeb
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

            if(!IsPostBack)
            {
                this.SetControls();
            }
        }

        protected void SetControls()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            var _siteTemplates = _tm.GetAvailableTemplates();
            foreach(var t in _siteTemplates)
            {
                listSites.Items.Add(new System.Web.UI.WebControls.ListItem(t.Title, t.Name));
            }
            listSites.SelectedIndex = 0;
            var path = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/" + "sites/";
            this.lblBasePath.Text = path;
        }
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            Microsoft.SharePoint.Client.User currUser;
            using (ClientContext ctx = spContext.CreateUserClientContextForSPHost())
            {
                currUser = ctx.Web.CurrentUser;
                ctx.Load(currUser);
                ctx.ExecuteQuery();
            }
            
            var _owner = new SharePointUser()
            {
                Email = currUser.Email
            };

            ISiteRequestFactory _requestFactory = SiteRequestFactory.GetInstance();
            var _manager = _requestFactory.GetSiteRequestManager();

            var _siteRequest = new SiteRequestInformation();
            _siteRequest.Template = listSites.SelectedItem.Value;
            _siteRequest.SiteOwner = _owner;
            _siteRequest.Description = this.txtDescription.Text;
            _siteRequest.Title = this.txtTitle.Text;
            _siteRequest.Url = this.lblBasePath.Text + this.txtUrl.Text;
            try
            {
                _manager.CreateNewSiteRequest(_siteRequest);
                Response.Redirect(Page.Request["SPHostUrl"]);
            }
            catch(Exception _ex)
            {
                lblErrorMessage.Text = _ex.Message;
            }
          
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }
    }
}
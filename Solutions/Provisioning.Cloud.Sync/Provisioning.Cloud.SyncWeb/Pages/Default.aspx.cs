using Contoso.Provisioning.Cloud.SyncWeb.ApplicationLogic;
using Contoso.Provisioning.Cloud.SyncWeb.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Contoso.Provisioning.Cloud.SyncWeb 
{
    public partial class Default : System.Web.UI.Page
    {
        private IEnumerable<XElement> templates;

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
            string validationScript = @"
                $(document).ready(function () {
                    $('#btnCreate').click(function () {
                        var valid = true;
                        for (i = 0; i < validationChecks.length; i++) {
                            var v = validationChecks[i]();
                            if (!v)
                                valid = false;
                        }
                        return valid;
                    });

                    var isDialog = decodeURIComponent(getQueryStringParameter('IsDlg'));
                    if (isDialog == '1') {
                        MakeSSCDialogPageVisible();
                        UpdateSSCDialogPageSize();

                        $('#btnCancel').click(function () {
                            closeDialog();
                            return false;
                        });
                    }
                });";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Default), "ValidationScript", validationScript, true);

            //load templates each time based on dialog
            if (Page.Request["IsDlg"].Contains("1"))
            {
                templates = this.Configuration.Root.Descendants("Template").Where(i => !i.Attribute("SubWebOnly").Value.Equals("true", StringComparison.CurrentCultureIgnoreCase));
                lblBasePath.Text = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/";
            }
            else
            {
                templates = this.Configuration.Root.Descendants("Template").Where(i => !i.Attribute("RootWebOnly").Value.Equals("true", StringComparison.CurrentCultureIgnoreCase));
                lblBasePath.Text = Request["SPHostUrl"] + "/";
                btnCancel.Click += btnCancel_Click;
            }

            if (!this.IsPostBack)
            {
                //get url path to host site (will serve as base path for subsites)
                Uri url = SharePointContext.GetSPHostUrl(HttpContext.Current.Request);

                // Show the available templates from configuration
                foreach (XElement element in templates)
                {
                    listSites.Items.Add(new ListItem(element.Attribute("Title").Value, element.Attribute("Name").Value));
                }
            }

            //load modules every time since they are dynamic
            LoadModules();

            // Verify that configuration list exists in the root site of the tenant / web application
            new DeployManager().EnsureConfigurationListInTenant(Page.Request["SPHostUrl"]);
        }

        public XDocument Configuration
        {
            get
            {
                if (Cache["Config"] == null)
                {
                    string fileUrl = Server.MapPath("~/Configuration/Configuration.xml");
                    XDocument doc = XDocument.Load(fileUrl);
                    Cache["Config"] = doc;
                    return doc;
                }
                else
                {
                    return (XDocument)Cache["Config"];
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            var clientContext = spContext.CreateUserClientContextForSPHost();

            //check if we should create site collection or subsite
            Microsoft.SharePoint.Client.Web newWeb = null;
            if (Page.Request["IsDlg"].Contains("1"))
            {
                newWeb = new DeployManager().CreateSiteCollection(
                    Page.Request["SPHostUrl"], txtUrl.Text, listSites.SelectedValue, txtTitle.Text,
                    txtDescription.Text, clientContext, this, this.Configuration);

                //update the client context
                var newWebUri = new Uri(newWeb.Url);
                var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, newWebUri.Authority, TokenHelper.GetRealmFromTargetUrl(newWebUri)).AccessToken;
                clientContext = TokenHelper.GetClientContextWithAccessToken(newWeb.Url, token);
                newWeb = clientContext.Web;
                clientContext.Load(newWeb);
                clientContext.ExecuteQuery();
            }
            else
            {
                newWeb = new DeployManager().CreateSubSite(
                    txtUrl.Text, listSites.SelectedValue, txtTitle.Text,
                    txtDescription.Text, clientContext, this, this.Configuration);
            }

            //Call Provision on each provisioning module
            foreach (Control ctrl in pnlModules.Controls)
            {
                if (ctrl is BaseProvisioningModule)
                    ((BaseProvisioningModule)ctrl).Provision(clientContext, newWeb);
            }

            //dispose the clientContext
            clientContext.Dispose();

            if (Page.Request["IsDlg"].Contains("1"))
            {
                //redirect to new site
                ScriptManager.RegisterClientScriptBlock(this, typeof(Default), "RedirectToSite", "navigateParent('" + newWeb.Url + "');", true);
            }
            else
            {
                // Redirect to just created site
                Response.Redirect(newWeb.Url);
            }
        }

        private void LoadModules()
        {
            if (listSites.SelectedIndex != -1)
            {
                //change the options
                var templateElement = templates.ElementAt(listSites.SelectedIndex);
                var modules = templateElement.Descendants("Module");
                foreach (var module in modules)
                {
                    BaseProvisioningModule ctrl = (BaseProvisioningModule)this.LoadControl(module.Attribute("CtrlSrc").Value);
                    pnlModules.Controls.Add(ctrl);
                }

                //update the path
                if (Page.Request["IsDlg"].Contains("1"))
                {
                    //get host web as base path
                    lblBasePath.Text = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/" + templateElement.Attribute("ManagedPath").Value + "/";
                }

            }
            else
                pnlModules.Controls.Clear();
        }

        protected void listSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            //load the modules
            //loadModules();
        }
    }
}
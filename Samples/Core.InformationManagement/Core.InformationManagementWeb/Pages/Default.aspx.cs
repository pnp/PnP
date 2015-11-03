using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.InformationManagementWeb
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
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            cc = spContext.CreateUserClientContextForSPHost();

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
                    //Scenario 1 

                    //Get site expiration and closure dates
                    if (cc.Web.HasSitePolicyApplied())
                    {
                        lblSiteExpiration.Text = String.Format("The expiration date for the site is {0}", cc.Web.GetSiteExpirationDate());
                        lblSiteClosure.Text = String.Format("The closure date for the site is {0}", cc.Web.GetSiteCloseDate());
                    }
                    else
                    {
                        lblSiteExpiration.Text = String.Format("The expiration date for the site is {0}", "not defined");
                        lblSiteClosure.Text = String.Format("The closure date for the site is {0}", "not defined");
                    }

                    //List the defined policies
                    List<SitePolicyEntity> policies = cc.Web.GetSitePolicies();
                    string policiesString = "";
                    foreach (var policy in policies)
                    {
                        policiesString += String.Format("{0} ({1}) <BR />", policy.Name, policy.Description);
                    }
                    lblSitePolicies.Text = policiesString;

                    //Show the assigned policy
                    SitePolicyEntity appliedPolicy = cc.Web.GetAppliedSitePolicy();
                    if (appliedPolicy != null)
                    {
                        lblAppliedPolicy.Text = String.Format("{0} ({1})", appliedPolicy.Name, appliedPolicy.Description);
                    }
                    else
                    {
                        lblAppliedPolicy.Text = "No policy has been applied";
                    }

                    //Scenario 2

                    //Fill the policies combo
                    foreach (var policy in policies)
                    {
                        if (appliedPolicy == null || !policy.Name.Equals(appliedPolicy.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            drlPolicies.Items.Add(policy.Name);
                        }
                    }
                    btnApplyPolicy.Enabled = drlPolicies.Items.Count > 0;
                }
        }

        protected void btnApplyPolicy_Click(object sender, EventArgs e)
        {
            if (drlPolicies.SelectedItem != null)
            {
                cc.Web.ApplySitePolicy(drlPolicies.SelectedItem.Text);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
        }
    }
}
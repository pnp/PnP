using Microsoft.SharePoint.Client;
using OfficeAMS.Core.Entities;
using OfficeAMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.YammerWeb
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
                lblBasePath.Text = Request["SPHostUrl"] + "/";
                listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Team", "STS#0"));
                listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Super Team", "STS#0"));
                listSites.Items.Add(new System.Web.UI.WebControls.ListItem("Über Team", "STS#0"));
                listSites.SelectedIndex = 0;

            }

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {

                string groupName = string.Empty;
                if (YammerFeedType.SelectedValue == "Group" && YammerGroupAssociationType.SelectedValue == "Existing")
                {
                    groupName = YammerExistingGroups.SelectedValue;
                }
                else
                {
                    groupName = txtYammerGroup.Text;
                }
                CreateSubSite(ctx.Web, txtUrl.Text, listSites.SelectedValue, txtTitle.Text, txtDescription.Text, YammerFeedType.SelectedValue, groupName);

                // Redirect to just created site
                Response.Redirect(Request["SPHostUrl"] + "/" + txtUrl.Text);

            }
        }

        /// <summary>
        /// Actual sub site creation and modification logic. Calls Core component methods to make things work
        /// </summary>
        /// <param name="hostWeb"></param>
        /// <param name="url"></param>
        /// <param name="template"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="feedType"></param>
        /// <param name="yammerGroupName"></param>
        /// <returns></returns>
        public void CreateSubSite(Web hostWeb, string url, string template,
                                    string title, string description, string feedType, string yammerGroupName)
        {
            // Create new sub site
            Web newWeb = hostWeb.CreateSite(title, url, description, template, 1033);

            // Set theme for the site
            newWeb.SetThemeToSubWeb(hostWeb, "Orange");

            //Remove the out of the box "NewsFeed" web part
            newWeb.DeleteWebPart("SitePages", "Site feed", "home.aspx");

            // Let's first get the details on the Yammer network using the access token
            WebPartEntity wpYammer;
            YammerUser user = YammerUtility.GetYammerUser(ConfigurationManager.AppSettings["YammerAccessToken"]);

            // Notice that in general we do not recommend of matching Yammer group for each site to avoid "group pollution" in Yammer
            if (feedType == "Group")
            {
                // Get Yammer Group - Creates if does not exist. Let's create these as public by default.
                YammerGroup group =
                    YammerUtility.CreateYammerGroup(yammerGroupName, false, ConfigurationManager.AppSettings["YammerAccessToken"]);
                // Get Yammer web part
                wpYammer = YammerUtility.GetYammerGroupDiscussionPart(user.network_name, group.id, false, false);
            }
            else
            {
                // Get OpenGrap object for using that as the discussion feed
                wpYammer = YammerUtility.GetYammerOpenGraphDiscussionPart(user.network_name, Request["SPHostUrl"] + "/" + txtUrl.Text,
                                                                            false, false, "SharePoint Site Feed - " + title);
            }
            // Add Yammer web part to the page
            newWeb.AddWebPartToWikiPage("SitePages", wpYammer, "home.aspx", 2, 1, false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        protected void YammerFeedType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (YammerFeedType.SelectedValue == "Group")
            {
                YammerGroupAssociationType.Enabled = true;
                // Get existing Yammer groups from the network to associate to them
                List<YammerGroup> groups = YammerUtility.GetYammerGroups(ConfigurationManager.AppSettings["YammerAccessToken"]);
                foreach (var item in groups)
                {
                    // Add items to the list.
                    YammerExistingGroups.Items.Add(new System.Web.UI.WebControls.ListItem(item.full_name, item.full_name));
                }
                YammerGroupAssociationType.Enabled = true;
                txtYammerGroup.Enabled = true;
                YammerExistingGroups.Enabled = true;
            }
            else
            {
                YammerGroupAssociationType.Enabled = false;
                txtYammerGroup.Enabled = false;
                YammerExistingGroups.Enabled = false;
            }
        }

        protected void YammerGroupAssociationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (YammerGroupAssociationType.SelectedValue == "Existing")
            {
                YammerExistingGroups.Enabled = true;
                txtYammerGroup.Enabled = false;
            }
            else
            {
                YammerExistingGroups.Enabled = false;
                txtYammerGroup.Enabled = true;
            }
        }
    }
}
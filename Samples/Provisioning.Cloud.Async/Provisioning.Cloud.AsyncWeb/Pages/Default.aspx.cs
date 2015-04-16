using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.Cloud.AsyncWeb
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


            listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Team", "STS#0"));
            listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Community", "COMMUNITY#0"));
            listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Blog", "BLOG#0"));
            listTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("Wiki", "WIKI#0"));
            listTemplates.SelectedIndex = 0;

            lblBasePath.Text = Request["SPHostUrl"].Substring(0, 8 + Request["SPHostUrl"].Substring(8).IndexOf("/")) + "/";
        }


        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (ClientContext ctx = spContext.CreateUserClientContextForSPHost())
            {
                // Make sure that request list does exist in the host location
                EnsureRequestList(ctx.Web);

                //get the current user to set as owner
                var currUser = ctx.Web.CurrentUser;
                ctx.Load(currUser);
                ctx.ExecuteQuery();

                // Add request to the process queue
                AddRequestToQueue(ctx.Web, currUser.Email);

                // Change active view
                processViews.ActiveViewIndex = 1;

                // Show that the information has been recorded.
                lblTitle.Text = txtTitle.Text;
                lblUrl.Text = ResolveFutureUrl();
                lblEmailToNotify.Text = txtEmailToNotify.Text;
                lblSiteColAdmin.Text = currUser.Email;
            }
        }

        private string ResolveFutureUrl()
        {
            var tenantStr = Page.Request["SPHostUrl"].ToLower().Replace("-my", "").Substring(8);
            tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));
            var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", txtUrl.Text);
            return webUrl;
        }

        private void AddRequestToQueue(Web web, string currentUserEmail)
        {
            string listName = ConfigurationManager.AppSettings["SiteCollectionRequests_List"];
            List list = GetListByTitle(web, listName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            Microsoft.SharePoint.Client.ListItem listItem = list.AddItem(itemCreateInfo);
            listItem["Title"] = txtTitle.Text;
            listItem["SiteUrl"] = txtUrl.Text;
            listItem["Template"] = listTemplates.SelectedValue;
            listItem["Description"] = txtDescription.Text;
            listItem["RequestorEmail"] = currentUserEmail;
            listItem["NotifyEmail"] = txtEmailToNotify.Text;
            listItem["Status"] = "Requested";
            listItem["StatusMessage"] = "Request stored successfully for async process";
            listItem.Update();

            web.Context.ExecuteQuery();
        }

        private void EnsureRequestList(Web web)
        {
            string listName = ConfigurationManager.AppSettings["SiteCollectionRequests_List"];
            if (!ListExists(web, listName))
            {
                // Let's create the request list
                CreateList(web, ListTemplateType.GenericList, listName, false);
                List list = GetListByTitle(web, listName);
                web.Context.Load(list.Fields);

                FieldCollection collField = list.Fields;
                collField.AddFieldAsXml("<Field Type='Text' DisplayName='SiteUrl' Name='SiteUrl' />", true, AddFieldOptions.AddToDefaultContentType);
                collField.AddFieldAsXml("<Field Type='Text' DisplayName='Template' Name='Template' />", true, AddFieldOptions.AddToDefaultContentType);
                collField.AddFieldAsXml("<Field Type='Text' DisplayName='Description' Name='Description' />", true, AddFieldOptions.AddToDefaultContentType);
                collField.AddFieldAsXml("<Field Type='Text' DisplayName='RequestorEmail' Name='RequestorEmail' />", true, AddFieldOptions.AddToDefaultContentType);
                collField.AddFieldAsXml("<Field Type='Text' DisplayName='NotifyEmail' Name='NotifyEmail' />", true, AddFieldOptions.AddToDefaultContentType);
                collField.AddFieldAsXml("<Field Type='Text' DisplayName='Status' Name='Status' />", true, AddFieldOptions.AddToDefaultContentType);
                collField.AddFieldAsXml("<Field Type='Text' DisplayName='StatusMessage' Name='StatusMessage' />", true, AddFieldOptions.AddToDefaultContentType);
                web.Context.Load(collField);
                web.Context.ExecuteQuery();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Page.Request["SPHostUrl"]);
        }

        /// <summary>
        /// Get list by using Title
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list to return</param>
        /// <returns>Loaded list instance mathing to title or null</returns>
        private List GetListByTitle(Web web, string listTitle)
        {
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
            web.Context.ExecuteQuery();
            return results.FirstOrDefault();
        }

        private bool ListExists(Web web, string listTitle)
        {
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                return true;
            }

            return false;
        }

        private void CreateList(Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            // Call actual implementation
            CreateListInternal(web, listType, listName, enableVersioning, updateAndExecuteQuery, urlPath);
        }

        private void CreateListInternal(Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            ListCollection listCol = web.Lists;
            ListCreationInformation lci = new ListCreationInformation();
            lci.Title = listName;
            lci.TemplateType = (int)listType;

            if (!string.IsNullOrEmpty(urlPath))
                lci.Url = urlPath;

            List newList = listCol.Add(lci);

            if (enableVersioning)
            {
                newList.EnableVersioning = true;
                newList.EnableMinorVersions = true;
            }

            if (updateAndExecuteQuery)
            {
                newList.Update();
                web.Context.Load(listCol);
                web.Context.ExecuteQuery();
            }

        }
    }
}
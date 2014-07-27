using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.GroupManagementWeb
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
        }

        protected void btnLoadGroups_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Groups defined in this site collection are:<BR/>");
            IEnumerable<Group> groups = cc.LoadQuery(cc.Web.SiteGroups.Include(grp => grp.Title,
                                                                               grp => grp.Users.Include(
                                                                                   usr => usr.LoginName,
                                                                                   usr => usr.Title)
                                                                              ));
            cc.ExecuteQuery();
            foreach(Group group in groups)
            {
                sb.Append(String.Format("<BR/>Group <B>\"{0}\"</B> has following members:<BR/>", group.Title));

                foreach(Microsoft.SharePoint.Client.User user in group.Users)
                {
                    sb.Append(String.Format("Name: {0}, loginname: {1}<BR/>", user.Title, user.LoginName));
                }
            }

            lblExistingGroups.Text = sb.ToString();
        }

        protected void btnCreateGroupAndAddUsers_Click(object sender, EventArgs e)
        {
            cc.Load(cc.Web, web => web.CurrentUser);
            cc.ExecuteQuery();
            Microsoft.SharePoint.Client.User currentUser = cc.Web.CurrentUser;

            if (!cc.Web.GroupExists("Test"))
            {
                Group group = cc.Web.AddGroup("Test", "Test group", true);
                cc.Web.AddUserToGroup("Test", currentUser.LoginName);
            }
        }

        protected void btnRemoveGroup_Click(object sender, EventArgs e)
        {
            if (cc.Web.GroupExists("Test"))
            {
                cc.Web.RemoveGroup("Test");
            }
        }

        protected void btnRemoveUserFromGroup_Click(object sender, EventArgs e)
        {
            cc.Load(cc.Web, web => web.CurrentUser);
            cc.ExecuteQuery();
            Microsoft.SharePoint.Client.User currentUser = cc.Web.CurrentUser;
            if (cc.Web.GroupExists("Test"))
            {
                if (cc.Web.IsUserInGroup("Test", currentUser.LoginName))
                {
                    cc.Web.RemoveUserFromGroup("Test", currentUser.LoginName);
                }
            }
        }

        protected void btnAddContributePermissionLevel_Click(object sender, EventArgs e)
        {
            if (cc.Web.GroupExists("Test"))
            {
                cc.Web.AddPermissionLevelToGroup("Test", RoleType.Contributor);
            }
        }

        protected void btnAddReadPermissionLevel_Click(object sender, EventArgs e)
        {
            if (cc.Web.GroupExists("Test"))
            {
                cc.Web.AddPermissionLevelToGroup("Test", RoleType.Reader);
            }
        }

        protected void btnAddReadPermissionLevelToCurrentUser_Click(object sender, EventArgs e)
        {
            cc.Load(cc.Web, web => web.CurrentUser);
            cc.ExecuteQuery();
            Microsoft.SharePoint.Client.User currentUser = cc.Web.CurrentUser;
            cc.Web.AddPermissionLevelToUser(currentUser.LoginName, RoleType.Reader);
        }

        protected void btnRemoveReadPermissionLevel_Click(object sender, EventArgs e)
        {
            if (cc.Web.GroupExists("Test"))
            {
                cc.Web.RemovePermissionLevelFromGroup("Test", RoleType.Reader);
            }
        }

        protected void btnRemoveReadPermissionLevelFromCurrentUser_Click(object sender, EventArgs e)
        {
            cc.Load(cc.Web, web => web.CurrentUser);
            cc.ExecuteQuery();
            Microsoft.SharePoint.Client.User currentUser = cc.Web.CurrentUser;
            cc.Web.RemovePermissionLevelFromUser(currentUser.LoginName, RoleType.Reader);
        }
    }
}
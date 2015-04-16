using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace Provisioning.Cloud.Workflow.AppWebWeb
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
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                Response.Write(clientContext.Web.Title);
            }
        }

        protected void CreateSiteButton_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            //using (var clientContext = spContext.CreateAppOnlyClientContextForSPAppWeb())
            using ( var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                var web = clientContext.Web;
                clientContext.Load(web);
                var list = web.Lists.GetByTitle("SiteCreationRequests");
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                //var roleassignments = list.RoleAssignments;
                //clientContext.Load(roleassignments);
                //clientContext.ExecuteQuery();
                //clientContext.Load(roleassignments.Groups.GetById(3));
                //clientContext.Load(clientContext.Web.CurrentUser);
                //clientContext.ExecuteQuery();
                //var ownerGroup = roleassignments.Groups.GetById(3);
                //clientContext.Load(ownerGroup.Users);
                //clientContext.ExecuteQuery();
                //ownerGroup.Users.AddUser(clientContext.Web.CurrentUser);
                //ownerGroup.Update();
                //clientContext.ExecuteQuery();
                var newItemCreator = new ListItemCreationInformation();
                var newItem = list.AddItem(newItemCreator);
                newItem["Title"] = SiteName.Text;
                newItem["Approver"] = clientContext.Web.GetUserById(11);
                //newItem["Approver"] = clientContext.Web.CurrentUser;
                newItem["Approved"] = false;
                newItem.Update();
                clientContext.ExecuteQuery();
            }
        }


    }
}
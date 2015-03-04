using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace Provisioning.Cloud.WorkflowWeb
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

        }

        protected void CreateSiteButton_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
            {
                var web = clientContext.Web;
                clientContext.Load(web);
                var list = web.Lists.GetByTitle("SiteCreationRequests");
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                var newItemCreator = new ListItemCreationInformation();
                var newItem = list.AddItem(newItemCreator);
                newItem["Title"] = SiteName.Text;
                newItem["Approver"] = clientContext.Web.CurrentUser;
                newItem["Approved"] = false;
                newItem.Update();
                clientContext.ExecuteQuery();
            }
        }

    }
}
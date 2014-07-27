using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Core.OfficeWebWidgetsWeb
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
            if (!this.IsPostBack)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    clientContext.Load(clientContext.Web, web => web.CurrentUser);
                    clientContext.ExecuteQuery();

                    //Prefil the current user information
                    Microsoft.SharePoint.Client.User currentUser = clientContext.Web.CurrentUser;
                    List<SharePointUser> peoplePickerUsers = new List<SharePointUser>(1);
                    peoplePickerUsers.Add(new SharePointUser() { 
                        displayName = currentUser.Title, 
                        text = currentUser.Title,
                        email = currentUser.Email, 
                        isResolved = false,
                        loginName = currentUser.LoginName });
                    txtSiteOwner.Text = JsonUtility.Serialize<List<SharePointUser>>(peoplePickerUsers);
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<SharePointUser> ownersList = JsonUtility.Deserialize<List<SharePointUser>>(txtSiteOwner.Text);
            List<SharePointUser> backupOwnersList = JsonUtility.Deserialize<List<SharePointUser>>(txtBackupSiteOwners.Text);

            lblSiteOwner.Text = FormatOwners(ownersList);
            lblBackupSiteOwners.Text = FormatOwners(backupOwnersList);
        }

        private string FormatOwners(List<SharePointUser> owners)
        {
            StringBuilder sb = new StringBuilder();

            foreach (SharePointUser user in owners)
            {
                sb.Append(String.Format("Name: {0}, Login: {1}<BR/>", user.displayName, user.loginName));
            }

            return sb.ToString();
        }
    }
}
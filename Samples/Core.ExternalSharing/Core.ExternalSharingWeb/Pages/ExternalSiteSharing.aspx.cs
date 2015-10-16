using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.ExternalSharingWeb.Pages
{
    public partial class ExternalSiteSharing : System.Web.UI.Page
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

        protected void btnValidateEmail_Click(object sender, EventArgs e)
        {
            if (txtTargetEmail.Text.Length > 0)
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                using (var ctx = spContext.CreateUserClientContextForSPHost())
                {
                    string returnedValue = ctx.Web.ResolvePeoplePickerValueForEmail(txtTargetEmail.Text);
                    lblStatus.Text = returnedValue;
                }
            }
        }

        protected void btnShareSite_Click(object sender, EventArgs e)
        {
            if(txtTargetEmail.Text.Length > 0)
            { 
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                // Check if this is edit link or not
                ExternalSharingSiteOption shareType = SolveSelectedShareType();

                // Share a site for the given email address
                SharingResult result = ctx.Web.ShareSite(txtTargetEmail.Text, shareType,
                                                         true, "Here's a site shared for you.");

                // Output the created link
                lblStatus.Text = string.Format("Site sharing status: {0}", result.StatusCode.ToString());
            }
            }
            else
            {
                lblStatus.Text = "Please assign the email to target the site to.";
            }
        }

        /// <summary>
        /// Solve target sharing option based on UI selection
        /// </summary>
        /// <returns>Sharing style target</returns>
        private ExternalSharingSiteOption SolveSelectedShareType()
        {
            switch (rblSharingOptions.SelectedValue)
            {
                case "owner":
                    return ExternalSharingSiteOption.Owner;
                case "edit":
                    return ExternalSharingSiteOption.Edit;
                default:
                    return ExternalSharingSiteOption.View;
            }
        }

        protected void btnSharingStatus_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                // Get object sharing setting configuration from the site level
                ObjectSharingSettings result = ctx.Web.GetObjectSharingSettingsForSite();

                // For outputting the list of people site is being shared
                if (result.ObjectSharingInformation.SharedWithUsersCollection.Count > 0)
                {
                    string userList = "";
                    foreach (var item in result.ObjectSharingInformation.SharedWithUsersCollection)
                    {
                        userList += string.Format(" - {0}", item.Email);
                    }
                    lblStatus.Text = string.Format("Site shared with: {0}", userList);
                }
                else
                {
                    lblStatus.Text = string.Format("Site not shared with anyone");
                }
            }
        }


    }
}
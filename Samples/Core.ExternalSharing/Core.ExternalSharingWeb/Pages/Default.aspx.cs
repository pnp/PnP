using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core.ExternalSharingWeb
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
            //update the navigation links
            this.hplScenario1.NavigateUrl = String.Format("~/pages/ExternalSettingsAtSiteCollectionLevel.aspx?{0}", SharePointUrlParameters());
            this.hplScenario2.NavigateUrl = String.Format("~/pages/ExternalSiteSharing.aspx?{0}", SharePointUrlParameters());
            this.hplScenario3.NavigateUrl = String.Format("~/pages/ExternalSharingForDocument.aspx?{0}", SharePointUrlParameters());
        }

        private string SharePointUrlParameters()
        {
            return HttpUtility.ParseQueryString(this.Context.Request.Url.Query).ToString();
        }
    }
}
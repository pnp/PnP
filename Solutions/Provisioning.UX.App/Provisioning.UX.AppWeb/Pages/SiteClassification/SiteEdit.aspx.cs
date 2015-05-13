using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.UX.AppWeb.Pages.SiteClassification
{
    public partial class SiteEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var _spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            using(var _ctx = _spContext.CreateUserClientContextForSPHost())
            {
                _ctx.Load(_ctx.Web, web => web.Title);
                _ctx.ExecuteQuery();
                Response.Write(_ctx.Web.Title);
            }
        }
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
    }
}
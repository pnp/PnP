using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Provisioning.UX.AppWeb.Pages.SubSite
{
    public partial class newsbweb : System.Web.UI.Page
    {
        private ClientContext _ctx;

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
            var _spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            _ctx = _spContext.CreateUserClientContextForSPHost();

            if (!Page.IsPostBack)
            {
                if (this.DoesUserHavePermission())
                {
                    SetHiddenFields();
                    SetUI();
                }
            }
           
        }

        private void SetUI()
        {
            var _web = _ctx.Web;
            _ctx.Load(_web);
            _ctx.ExecuteQuery();
            this.labelHostURL.InnerHtml = _web.Url;
          //  this.lblHostSite.Text = _web.Url;
        }

        protected bool DoesUserHavePermission()
        {
            BasePermissions perms = new BasePermissions();
            perms.Set(PermissionKind.ManageSubwebs);
            ClientResult<bool> _permResult = _ctx.Web.DoesUserHavePermissions(perms);
            _ctx.ExecuteQuery();
            return _permResult.Value;
        }

        protected void Submit_Click(object sender, EventArgs e)
        {

        }

        private void SetHiddenFields()
        {
            string _url = Request.QueryString["SPHostUrl"];
            this.Url.Value = _url;
        }
    }
}
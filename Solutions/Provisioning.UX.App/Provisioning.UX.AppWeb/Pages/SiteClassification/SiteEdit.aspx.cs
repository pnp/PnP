using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
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
        private ClientContext _ctx;
        const string SITE_PROPERTY_DIVISION = "_site_props_division";
        const string SITE_PROPERTY_REGION = "_site_props_region";
        const string SITE_PROPERTY_FUNCTION = "_site_props_function";

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

        protected void Submit_Click(object sender, EventArgs e)
        {
            var _web = _ctx.Web;
           
            var _division = Request.Form["selectDivision"].ToString();
            var _updateDivision = string.Compare(this.lblDivision.Text, _division, StringComparison.InvariantCultureIgnoreCase) != 0;
            if (_updateDivision)
            {
                _web.SetPropertyBagValue(SITE_PROPERTY_DIVISION, _division);
            }

            var _region = Request.Form["selectRegions"].ToString();
            var _updateRegion = string.Compare(this.lblRegion.Text, _region, StringComparison.InvariantCultureIgnoreCase) != 0;
            if (_updateRegion)
            {
                _web.SetPropertyBagValue(SITE_PROPERTY_REGION, _region);
            }

            var _function = Request.Form["selectFunction"].ToString();
            var _updateFunction = string.Compare(this.lblFunction.Text, _function, StringComparison.InvariantCultureIgnoreCase) != 0;
            if (_updateFunction)
            {
                _web.SetPropertyBagValue(SITE_PROPERTY_FUNCTION, _function);
            }

            Response.Redirect(this.Url.Value);
        }

        private void SetHiddenFields()
        {
            //Another way is by setting with javascript
            string _url =  Request.QueryString["SPHostUrl"];
            this.Url.Value = _url;
        }

        private void SetUI()
        {
            var _web = _ctx.Web;
            var _regionProP = _web.GetPropertyBagValueString(SITE_PROPERTY_REGION, string.Empty);
            var _functionProp = _web.GetPropertyBagValueString(SITE_PROPERTY_FUNCTION, string.Empty);
            var _divisionProp = _web.GetPropertyBagValueString(SITE_PROPERTY_DIVISION, string.Empty);

            this.lblDivision.Text = _divisionProp;
            this.lblFunction.Text = _functionProp;
            this.lblRegion.Text = _regionProP;

            this.SetAvailableSitePolicy();
        }

        

        private void SetAvailableSitePolicy()
        {
            List<SitePolicyEntity> policies = _ctx.Web.GetSitePolicies();
            foreach (var policy in policies)
            {
                this.BusinessImpact.Items.Add(policy.Name);
            }
        }
        protected bool DoesUserHavePermission()
        {
            BasePermissions perms = new BasePermissions();
            perms.Set(PermissionKind.ManageWeb);
            ClientResult<bool> _permResult = _ctx.Web.DoesUserHavePermissions(perms);
            _ctx.ExecuteQuery();
            return _permResult.Value;
        }


    }
}
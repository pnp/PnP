using Core.SiteClassification.Common;
using OfficeDevPnP.Core.Entities;
using Microsoft.SharePoint.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
  //<AppPermissionRequests AllowAppOnlyPolicy="true">
  //  <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
  //</AppPermissionRequests>
namespace Core.SiteClassificationWeb.Pages
{
    public partial class Index : System.Web.UI.Page
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
            _ctx.ApplicationName = "AMSSITE_CLASSIFICATIONSAMPLE";
            if(!Page.IsPostBack)
            {    
                if(this.DoesUserHavePermission()) {
                    SetHiddenFields();
                    SetUI();
                }    
            }
        }

        private void SetUI()
        {
            SiteProfile _siteProfile =  GetSiteProfile(_ctx);
  
            if(_siteProfile.CustomProperties.ContainsKey(SiteClassificationKeys.AudienceReachKey))
            {
                SetAudience(_siteProfile.CustomProperties[SiteClassificationKeys.AudienceReachKey]);
            }

            this.SetExpirationDate(_siteProfile);
            this.SetSiteClassification(_siteProfile);
            this.SetAvaibleSitePolicies(_siteProfile);

        }

        protected SiteProfile GetSiteProfile(ClientContext ctx)
        {
            ISiteClassificationFactory _factory = SiteClassificationFactory.GetInstance();
            ISiteClassificationManager _manager = _factory.GetManager(ctx);
            var _siteProfile = _manager.GetSiteProfile(ctx);
            return _siteProfile;

        }
        protected bool DoesUserHavePermission()
        {
            BasePermissions perms = new BasePermissions();
            perms.Set(PermissionKind.ManageWeb);
            ClientResult<bool> _permResult = _ctx.Web.DoesUserHavePermissions(perms);
            _ctx.ExecuteQuery();
            return _permResult.Value; 
        }

        protected void SetHiddenFields()
        {  
            //Another way is by setting with javascript
            string _url =  Request.QueryString["SPHostUrl"];
            this.Url.Value = _url;
        }

        protected void SetAudience(string audienceReach)
        {

            switch (audienceReach)
            {
                case "Team":
                    this.AudienceScope_Team.Checked = true;
                    this.AudienceScope_Enterprise.Checked = false;
                    this.AudienceScope_Organization.Checked = false;
                    break;
                case "Enterprise":
                    this.AudienceScope_Team.Checked = false;
                    this.AudienceScope_Enterprise.Checked = true;
                    this.AudienceScope_Organization.Checked = false;
                    break;
                case "Organization":
                    this.AudienceScope_Team.Checked = true;
                    this.AudienceScope_Enterprise.Checked = false;
                    this.AudienceScope_Organization.Checked = true;
                    break;
                default:
                    this.AudienceScope_Team.Checked = true;
                    this.AudienceScope_Enterprise.Checked = false;
                    this.AudienceScope_Organization.Checked = false;
                    break;           
            }
            this.lblAudienceReach.Text = audienceReach;
        }

        protected void SetExpirationDate(SiteProfile profile)
        {
            if(profile.ExpirationDate != DateTime.MinValue)
            {
                this.lblExpirationDate.Text = String.Format("{0}", profile.ExpirationDate);
            }
            else
            {
                this.lblExpirationDate.Text = string.Format("{0}", "NO EXPIRATION DATE");
            }
            
        }

        protected void SetSiteClassification(SiteProfile profile)
        {
            if(!string.IsNullOrEmpty(profile.SitePolicy))
            {
                this.lblSitePolicy.Text = string.Format("{0}", profile.SitePolicy);
            }
            else
            {
                this.lblSitePolicy.Text = string.Format("{0}", "None");
            }
            
        }

        protected string GetSiteClassification()
        {
            if (!string.IsNullOrEmpty(Request.Form["BusinessImpact"]))
            {
                return Request.Form["BusinessImpact"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        protected void SetAvaibleSitePolicies(SiteProfile profile)
        {
            List<SitePolicyEntity> policies = _ctx.Web.GetSitePolicies();
            foreach(var policy in policies)
            {   
                this.BusinessImpact.Items.Add(policy.Name);
            }
            this.BusinessImpact.SelectedIndex = this.BusinessImpact.Items.IndexOf(this.BusinessImpact.Items.FindByText(profile.SitePolicy));
            
           
        }
        protected void Submit_Click(object sender, EventArgs e)
        {
            var _spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            
            SiteProfile _profile = new SiteProfile();
            _profile.SitePolicy = this.GetSiteClassification();

             Dictionary<string, string> _customProps = new Dictionary<string, string>();
            _customProps.Add(SiteClassificationKeys.AudienceReachKey, this.GetAudience());
            _customProps.Add(SiteClassificationKeys.BusinessImpactKey, _profile.SitePolicy);

            _profile.CustomProperties = _customProps;
         
            using (var _ctx = _spContext.CreateUserClientContextForSPHost())
             {
                ISiteClassificationFactory _factory = SiteClassificationFactory.GetInstance();
                ISiteClassificationManager _manager = _factory.GetManager(_ctx);
                _manager.SaveSiteProperties(_ctx, _profile);
                AddJsLink(_ctx, _ctx.Web);
             }

         

            Response.Redirect(this.Url.Value);
        }

        protected string GetAudience()
        {
            if (!string.IsNullOrEmpty(Request.Form["AudienceScope"]))
            {
                return Request.Form["AudienceScope"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public void AddJsLink(ClientContext ctx, Web web)
        {
            string scenarioUrl = String.Format("{0}://{1}:{2}/Scripts", this.Request.Url.Scheme,
                                                this.Request.Url.DnsSafeHost, this.Request.Url.Port);
            string revision = Guid.NewGuid().ToString().Replace("-", "");
            string jsLink = string.Format("{0}/{1}?rev={2}", scenarioUrl, "classifier.js", revision);

            StringBuilder scripts = new StringBuilder(@"
                var headID = document.getElementsByTagName('head')[0]; 
                var");

            scripts.AppendFormat(@"
                newScript = document.createElement('script');
                newScript.type = 'text/javascript';
                newScript.src = '{0}';
                headID.appendChild(newScript);", jsLink);
            string scriptBlock = scripts.ToString();

            var existingActions = web.UserCustomActions;
            ctx.Load(existingActions);
            ctx.ExecuteQuery();
            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Description == "PnPClassification" &&
                    action.Location == "ScriptLink")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                }
            }

            var newAction = existingActions.Add();
            newAction.Description = "PnPClassification";
            newAction.Location = "ScriptLink";

            newAction.ScriptBlock = scriptBlock;
            newAction.Update();
            ctx.Load(web, s => s.UserCustomActions);
            ctx.ExecuteQuery();
        }
    }
}
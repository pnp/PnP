using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Provisioning.UX.AppWeb.Models;

namespace Provisioning.UX.AppWeb.Pages.SiteClassification
{
    public partial class SiteEdit : System.Web.UI.Page
    {
        private ClientContext _ctx;
        private string isOnPrem = "false";
        private bool isSiteExternallyShared = false;
        private bool isTenantExternallyShared = false;
        private string _externallySharedStatus = "Off";
        private bool _propsExternallyShared;

        const string SITE_PROPERTY_DIVISION = "_site_props_division";
        const string SITE_PROPERTY_REGION = "_site_props_region";
        const string SITE_PROPERTY_FUNCTION = "_site_props_function";
        const string SITE_PROPERTY_ISONPREM = "_site_props_sponprem";
        const string SITE_PROPERTY_EXTERNAL_SHARING = "_site_props_externalsharing";

        protected void Page_Load(object sender, EventArgs e)
        {
            var _spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            _ctx = _spContext.CreateUserClientContextForSPHost();

            if (!Page.IsPostBack)
            {
                if (this.DoesUserHavePermission())
                {
                    SetHiddenFields();
                    CheckForOnPremAndSharing();

                    if(isOnPrem == "false")
                    {
                        CheckTenantExternalSharing();
                        if(isTenantExternallyShared)
                        {
                            CheckSiteExternalSharing();
                        }                        
                    }

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
            if (_division != "Select...") {
                var _updateDivision = string.Compare(this.lblDivision.Text, _division, StringComparison.InvariantCultureIgnoreCase) != 0;
                if (_updateDivision)
                {
                    _web.SetPropertyBagValue(SITE_PROPERTY_DIVISION, _division);
                }
            }

            var _region = Request.Form["selectRegions"].ToString();
            if (_region != "Select...")
            {
                var _updateRegion = string.Compare(this.lblRegion.Text, _region, StringComparison.InvariantCultureIgnoreCase) != 0;
                if (_updateRegion)
                {
                    _web.SetPropertyBagValue(SITE_PROPERTY_REGION, _region);
                }
            }

            var _function = Request.Form["selectFunction"].ToString();
            if (_function != "Select...")
            {
                var _updateSegment = string.Compare(this.lblFunction.Text, _function, StringComparison.InvariantCultureIgnoreCase) != 0;
                if (_updateSegment)
                {
                    _web.SetPropertyBagValue(SITE_PROPERTY_FUNCTION, _function);
                }
            }

            var _sitePolicyName = Request.Form["BusinessImpact"].ToString();
            if (_sitePolicyName != "Select...")
            {
                var _updateSitePolicy = String.Compare(this.lblSitePolicy.Text, _sitePolicyName, StringComparison.InvariantCultureIgnoreCase) != 0;
                if (_updateSitePolicy)
                {
                    AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
                    var _auth = new AppOnlyAuthenticationSite();
                    _auth.SiteUrl = this.Url.Value;
                    _siteService.Authentication = _auth;
                    _siteService.SetSitePolicy(_sitePolicyName);

                }
            }
                       
            var _toggle = this.toggleSharing.Checked;
            var newStatus = "false";
            _propsExternallyShared = _web.GetPropertyBagValueString(SITE_PROPERTY_EXTERNAL_SHARING, string.Empty).ToBoolean();

            if (_toggle != _propsExternallyShared)
            {
                if (!_toggle)
                {
                    newStatus = "false";
                }
                else
                {
                    newStatus = "true";
                }

                SetExternalSharing();
                _web.SetPropertyBagValue(SITE_PROPERTY_EXTERNAL_SHARING, newStatus);
            }

            System.Threading.Thread.Sleep(2000);
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

            var _ctxWeb = _ctx.Web;
            var _regionProP = _ctxWeb.GetPropertyBagValueString(SITE_PROPERTY_REGION, string.Empty);
            var _functionProp = _ctxWeb.GetPropertyBagValueString(SITE_PROPERTY_FUNCTION, string.Empty);
            var _divisionProp = _ctxWeb.GetPropertyBagValueString(SITE_PROPERTY_DIVISION, string.Empty);
            
            var _site = _ctx.Site;
            _ctx.Load(_ctx.Site, s => s.Owner.Title);
            _ctx.ExecuteQuery();
            this.siteOwner.Text = _ctx.Site.Owner.Title;

            this.lblDivision.Text = _divisionProp;
            this.lblFunction.Text = _functionProp;
            this.lblRegion.Text = _regionProP;

            if (isOnPrem == "false")
            {
                if (!isSiteExternallyShared)
                {
                    this.toggleSharing.Checked = false;
                }
                if (isSiteExternallyShared)
                {
                    this.toggleSharing.Checked = true;
                }
            }
            else
            {
                this.divExternalSharing.Visible = false;
            }       

            this.SetUXAvailableSitePolicy();
           
        }

        private void CheckForOnPremAndSharing()
        {
            var _web = _ctx.Web;
            isOnPrem = _web.GetPropertyBagValueString(SITE_PROPERTY_ISONPREM, string.Empty);
            _propsExternallyShared = _web.GetPropertyBagValueString(SITE_PROPERTY_EXTERNAL_SHARING, string.Empty).ToBoolean();
        }

        private void SetUXAvailableSitePolicy()
        {
            AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
            var _auth = new AppOnlyAuthenticationSite();
            _auth.SiteUrl = this.Url.Value;
            _siteService.Authentication = _auth;

            var _sitePolicies = _siteService.GetAvailablePolicies();
            foreach (var _sitePolicyEntity in _sitePolicies)
            {
                this.BusinessImpact.Items.Add(_sitePolicyEntity.Name);
            }

            var _appliedSitePolicy = _siteService.GetAppliedSitePolicy();
            if(_appliedSitePolicy != null)
            {
                this.lblSitePolicy.Text = _appliedSitePolicy.Name;
                var _expirationDate = _ctx.Web.GetSiteExpirationDate();
                this.lblExpirationDate.Text = _expirationDate == DateTime.MinValue ? String.Format("{0}", "None") : String.Format("{0}", _expirationDate);
            }
            else
            {
                this.lblSitePolicy.Text = string.Format("{0}", "None");
                //this.lblExpirationDate.Text = String.Format("{0}", "None");
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

        private void CheckTenantExternalSharing()
        {
            try
            {
                ConfigManager _manager = new ConfigManager();
                var _tenantAdminUrl = _manager.GetAppSettingsKey("TenantAdminUrl");

                AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
                var _auth = new AppOnlyAuthenticationSite();
                _auth.SiteUrl = _tenantAdminUrl;
                _siteService.Authentication = _auth;

                var _sharingResult = _siteService.IsTenantExternalSharingEnabled(_tenantAdminUrl);
                
                if(!_sharingResult)
                {
                    isTenantExternallyShared = false;                    
                }
                else
                {
                    isTenantExternallyShared = true;
                }               
            }
            catch(Exception _ex)
            {
                OfficeDevPnP.Core.Diagnostics.Log.Error("Office365SiteProvisioningService.IsTenantExternalSharingEnabled",
                   "There was an issue checking the status of tenant external sharing. Exception: {0}",
                   _ex);
            }

        }

        private void CheckSiteExternalSharing()
        {
            ConfigManager _manager = new ConfigManager();
            var _tenantAdminUrl = _manager.GetAppSettingsKey("TenantAdminUrl");

            AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
            _siteService.Authentication = new AppOnlyAuthenticationTenant();
            _siteService.Authentication.TenantAdminUrl = _tenantAdminUrl;
                        
            _siteService.UsingContext(ctx =>
            {
                try
                {
                    Tenant _tenant = new Tenant(ctx);
                    SiteProperties _siteProps = _tenant.GetSitePropertiesByUrl(this.Url.Value, false);
                    ctx.Load(_tenant);
                    ctx.Load(_siteProps);
                    ctx.ExecuteQuery();
                    

                    var _tenantSharingCapability = _tenant.SharingCapability;
                    var _siteSharingCapability = _siteProps.SharingCapability;
                    
                    if (_tenantSharingCapability != SharingCapabilities.Disabled)
                    {
                        if (_siteSharingCapability != SharingCapabilities.Disabled)
                        {
                            isSiteExternallyShared = true;

                            // Update UI                            
                            this.toggleSharing.Checked = true;                            
                        }
                        else
                        {
                            isSiteExternallyShared = false;

                            // Update UI                            
                            this.toggleSharing.Checked = false;                            
                        }
                    }
                    else
                    {
                        // Update UI                        
                        this.toggleSharing.Checked = false;                        
                    }

                }
                catch (Exception _ex)
                {
                    
                }

            });
        }

        private void SetExternalSharing()
        {
            if (isOnPrem == "false")
            {
                var _web = _ctx.Web;
                var _newStatus = string.Empty;

                ConfigManager _manager = new ConfigManager();
                var _tenantAdminUrl = _manager.GetAppSettingsKey("TenantAdminUrl");

                AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
                _siteService.Authentication = new AppOnlyAuthenticationTenant();
                _siteService.Authentication.TenantAdminUrl = _tenantAdminUrl;

                SiteInformation _srInfo = new SiteInformation();
                _srInfo.Url = this.Url.Value;
                if (this.toggleSharing.Checked == true)
                {
                    _srInfo.EnableExternalSharing = true;
                    _newStatus = "true";
                }
                else
                {
                    _srInfo.EnableExternalSharing = false;
                    _newStatus = "false";
                }               

                _siteService.SetExternalSharing(_srInfo);               

                //Update property bag as well
                _web.SetPropertyBagValue(SITE_PROPERTY_EXTERNAL_SHARING, _newStatus);

            }
            
        }

        private void UpdateSharingPropertyBag(string newStatus)
        {
            var _web = _ctx.Web;
            _web.SetPropertyBagValue(SITE_PROPERTY_EXTERNAL_SHARING, _externallySharedStatus);
        }


    }
}
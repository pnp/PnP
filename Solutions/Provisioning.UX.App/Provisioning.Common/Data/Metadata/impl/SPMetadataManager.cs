using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Provisioning.Common.Authentication;
using System.Diagnostics;
using Provisioning.Common.Utilities;
using Provisioning.Common.Metadata;
using Provisioning.Common.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;

namespace Provisioning.Common.Data.Metadata.Impl
{
    /// <summary>
    /// Implementation Class for working Metadata Repository
    /// </summary>
    class SPMetadataManager : AbstractModule, ISharePointClientService, IMetadataManager
    {
        
        const string SITE_PROPERTY_DIVISION = "_site_props_division";
        const string SITE_PROPERTY_REGION = "_site_props_region";
        const string SITE_PROPERTY_FUNCTION = "_site_props_function";
        const string SITE_PROPERTY_BUSINESS = "_site_props_business";
        const string SITE_PROPERTY_ISONPREM = "_site_props_sponprem";
        const string SITE_PROPERTY_EXTERNAL_SHARING = "_site_props_externalsharing";

        #region instance Members
        const string CAML_GET_ENABLED_CLASSIFICATIONS = "<View><Query><Where><Eq><FieldRef Name='SP_Enabled'/><Value Type='Text'>True</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
        const string CAML_GET_ENABLED_SITEMETADATA = "<View><Query><Where><Eq><FieldRef Name='SP_Enabled'/><Value Type='Text'>True</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";

        #endregion

        #region Properties
        /// <summary>
        /// Returns the implementation for AppOnlyAuthentication
        /// </summary>
        public IAuthentication Authentication
        {
            get
            {
                var _auth = new AppOnlyAuthenticationSite();
               _auth.SiteUrl = this.ConnectionString;
               return _auth;
            }

        }
        #endregion

        #region ISharePointClientService
        public void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        public void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = this.Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }
        #endregion

        #region IMetadataManager
        public ICollection<SiteClassification> GetAvailableSiteClassifications()
        {
            ICollection<SiteClassification> _returnResults = new List<SiteClassification>();
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                try
                {
                    var _web = ctx.Web;
                    ctx.Load(_web);
                    if (!_web.ListExists(SPDataConstants.LIST_TITLE_SITECLASSIFICATION))
                    {
                        var _message = String.Format("The List {0} does not exist in Site {1}",
                         SPDataConstants.LIST_TITLE_SITECLASSIFICATION,
                         ctx.Url);

                        Log.Fatal("SPMetadataManager.GetAvailableSiteClassifications", _message);
                        throw new DataStoreException(_message);
                    }
                  
                    var _camlQuery = new CamlQuery();
                    _camlQuery.ViewXml = CAML_GET_ENABLED_CLASSIFICATIONS;

                    var _list = ctx.Web.Lists.GetByTitle(SPDataConstants.LIST_TITLE_SITECLASSIFICATION);
                    var _listItemCollection = _list.GetItems(_camlQuery);
                    ctx.Load(_listItemCollection,
                        eachItem => eachItem.Include(
                            item => item,
                            item => item["ID"],
                            item => item["SP_Key"],
                            item => item["SP_Value"],
                            item => item["SP_DisplayOrder"],
                            item => item["SP_Enabled"],
                            item => item["SP_SiteExpirationMonths"],
                            item => item["SP_AddAllAuthenticatedUsers"]));
                    ctx.ExecuteQuery();
                 
                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "SPMetadataManager.GetAvailableSiteClassifications", _timespan.Elapsed);
   
                    foreach (ListItem _item in _listItemCollection)
                    {
                        var _classification = new SiteClassification()
                        {
                            Id  = _item.BaseGetInt("ID"),
                            Key = _item.BaseGet("SP_Key"),
                            Value = _item.BaseGet("SP_Value"),
                            DisplayOrder = _item.BaseGetInt("SP_DisplayOrder"),
                            Enabled = _item.BaseGet<bool>("SP_Enabled"),
                            ExpirationMonths = _item.BaseGetInt("SP_SiteExpirationMonths"),
                            AddAllAuthenticatedUsers = _item.BaseGet<bool>("SP_AddAllAuthenticatedUsers"),
                        };
                        _returnResults.Add(_classification);
                    }
                 
                  }
                catch(ServerException ex)
                {
                   //TODO LOG
                }
                catch(DataStoreException ex)
                {
                    throw;
                }
              
            });
            return _returnResults;
        }
        public SiteClassification GetSiteClassificationByName(string name)
        {
            throw new NotImplementedException();
        }
        public void CreateNewSiteClassification(SiteClassification classification)
        {
            throw new NotImplementedException();
        }
        public void UpdateSiteClassification(SiteClassification classification)
        {
            throw new NotImplementedException();
        }
        public bool DoesUserHavePermissions()
        {
            bool _returnResult = false;                
            UsingContext(ctx =>
            {                        
                var _web = ctx.Web;
                ctx.Load(_web);
                BasePermissions perms = new BasePermissions();
                perms.Set(PermissionKind.ManageWeb);
                ClientResult<bool> _permResult = ctx.Web.DoesUserHavePermissions(perms);
                ctx.ExecuteQuery();

                _returnResult = _permResult.Value;
            });

            return _returnResult;
        }                
        
        public ICollection<SiteMetadata> GetAvailableOrganizationalFunctions() { return GetSiteMetadataFromList("Functions"); }
        public ICollection<SiteMetadata> GetAvailableRegions() { return GetSiteMetadataFromList("Regions"); }
        public ICollection<SiteMetadata> GetAvailableDivisions() { return GetSiteMetadataFromList("Divisions"); }
        public ICollection<SiteMetadata> GetAvailableBusinessUnits() { return GetSiteMetadataFromList("Business"); }
        public ICollection<SiteMetadata> GetAvailableTimeZones() { return GetSiteMetadataFromList("TimeZone"); }
        public ICollection<SiteMetadata> GetAvailableSiteRegions() { return GetSiteMetadataFromList("Regions"); }
        public ICollection<SiteMetadata> GetAvailableLanguages() { return GetSiteMetadataFromList("Languages"); }        

        ICollection<SiteMetadata> GetSiteMetadataFromList(string listName)
        {
            ICollection<SiteMetadata> _returnResults = new List<SiteMetadata>();
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                try
                {
                    var _web = ctx.Web;
                    ctx.Load(_web);
                    if (!_web.ListExists(listName))
                    {
                        var _message = String.Format("The List {0} does not exist in Site {1}",
                         listName,
                         ctx.Url);

                        Log.Fatal("SPMetadataManager.GetSiteMetadataFromList", _message);
                        throw new DataStoreException(_message);
                    }

                    var _camlQuery = new CamlQuery();
                    _camlQuery.ViewXml = CAML_GET_ENABLED_SITEMETADATA;

                    var _list = ctx.Web.Lists.GetByTitle(listName);
                    var _listItemCollection = _list.GetItems(_camlQuery);
                    ctx.Load(_listItemCollection,
                        eachItem => eachItem.Include(
                            item => item,
                            item => item["ID"],
                            item => item["SP_Key"],
                            item => item["SP_Value"],
                            item => item["SP_DisplayOrder"],
                            item => item["SP_Enabled"]
                            ));
                    ctx.ExecuteQuery();

                    _timespan.Stop();
                    Log.TraceApi("SharePoint", "SPMetadataManager.GetSiteMetadataFromList", _timespan.Elapsed);

                    foreach (ListItem _item in _listItemCollection)
                    {
                        var _metadata = new SiteMetadata()
                        {
                            Id = _item.BaseGetInt("ID"),
                            Key = _item.BaseGet("SP_Key"),
                            Value = _item.BaseGet("SP_Value"),
                            DisplayOrder = _item.BaseGetInt("SP_DisplayOrder"),
                            Enabled = _item.BaseGet<bool>("SP_Enabled")
                        };
                        _returnResults.Add(_metadata);
                    }

                }
                catch (ServerException ex)
                {
                    //TODO LOG
                }
                catch (DataStoreException ex)
                {
                    throw;
                }

            });
            return _returnResults;
        }

        #endregion       

        public SiteEditMetadata GetSiteMetadata(SiteEditMetadata metadata)
        {
            // Check permissions
            bool _permsResult = DoesUserHavePermissions();

            Uri siteUri = new Uri(metadata.Url);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(metadata.Url, accessToken))
            {
                var _web = clientContext.Web;

                var _site = clientContext.Site;
                clientContext.Load(clientContext.Site, s => s.Owner.Title);
                clientContext.ExecuteQuery();
                metadata.SiteOwner.Name = _site.Owner.Title;

                clientContext.Load(clientContext.Site, s => s.Owner.Email);
                clientContext.ExecuteQuery();
                metadata.SiteOwner.Email = _site.Owner.Email;                

                // Get site configuration settings from property bag
                metadata.SharePointOnPremises = _web.GetPropertyBagValueString(SITE_PROPERTY_ISONPREM, string.Empty).ToBoolean();
                metadata.EnableExternalSharing = _web.GetPropertyBagValueString(SITE_PROPERTY_EXTERNAL_SHARING, string.Empty).ToBoolean();

                // Get business metadata settings from property bag
                metadata.Region = _web.GetPropertyBagValueString(SITE_PROPERTY_REGION, string.Empty);
                metadata.Function = _web.GetPropertyBagValueString(SITE_PROPERTY_FUNCTION, string.Empty);
                metadata.Division = _web.GetPropertyBagValueString(SITE_PROPERTY_DIVISION, string.Empty);
                metadata.BusinessUnit = _web.GetPropertyBagValueString(SITE_PROPERTY_BUSINESS, string.Empty);
            }
                       
            if (_permsResult)
            {               
                if (!metadata.SharePointOnPremises)
                {
                    metadata = CheckTenantSharingCapabilities(metadata);
                    if (metadata.TenantSharingEnabled)
                    {
                        // Get current sharing settings (ON/OFF)
                        metadata = CheckSiteSharingCapabilities(metadata);
                    }
                }
            }
            SetSitePolicy(metadata);
            return metadata;
        }

        public SiteEditMetadata SetSiteMetadata(SiteEditMetadata metadata)
        {
            // Check permissions
            bool _permsResult = DoesUserHavePermissions();

            Uri siteUri = new Uri(metadata.Url);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(metadata.Url, accessToken))
            {
                if (_permsResult)
                {
                    var _web = clientContext.Web;

                    // Set external sharing value
                    _web.SetPropertyBagValue(SITE_PROPERTY_EXTERNAL_SHARING, metadata.EnableExternalSharing.ToString());

                    // Set region value
                    _web.SetPropertyBagValue(SITE_PROPERTY_REGION, metadata.Region);

                    // Set function value
                    _web.SetPropertyBagValue(SITE_PROPERTY_FUNCTION, metadata.Function);

                    // Set division value
                    _web.SetPropertyBagValue(SITE_PROPERTY_DIVISION, metadata.Division);

                    // Set business unit value
                    _web.SetPropertyBagValue(SITE_PROPERTY_BUSINESS, metadata.BusinessUnit);

                    metadata = SetSitePolicy(metadata);
                    metadata = SetExternalSharing(metadata);
                }
            }
            return metadata;            
        }

        public SiteEditMetadata SetExternalSharing(SiteEditMetadata metadata)
        {
            try
            {
                if (!metadata.SharePointOnPremises)
                {
                    string _sharingStatus = string.Empty;

                    ConfigManager _manager = new ConfigManager();
                    var _tenantAdminUrl = _manager.GetAppSettingsKey("TenantAdminUrl");

                    AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
                    _siteService.Authentication = new AppOnlyAuthenticationTenant();
                    _siteService.Authentication.TenantAdminUrl = _tenantAdminUrl;

                    SiteInformation _srInfo = new SiteInformation();
                    _srInfo.Url = metadata.Url;
                    if (metadata.EnableExternalSharing)
                    {
                        _srInfo.EnableExternalSharing = true;
                        _sharingStatus = "true";
                    }
                    else
                    {
                        _srInfo.EnableExternalSharing = false;
                        _sharingStatus = "false";
                    }

                    _siteService.SetExternalSharing(_srInfo);

                    Uri siteUri = new Uri(metadata.Url);
                    string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
                    string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken(metadata.Url, accessToken))
                    {
                        var _web = clientContext.Web;
                        clientContext.Load(_web);

                        //Update property bag as well
                        _web.SetPropertyBagValue(SITE_PROPERTY_EXTERNAL_SHARING, _sharingStatus);
                        metadata.Success = true;
                    };
                }
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.SetSiteExternalSharingStatus", "There was an error processing the request. Exception: {0}", _ex);
                metadata.ErrorMessage = _ex.Message;
                return metadata;
            }

            return metadata;

        }

        private SiteEditMetadata CheckTenantSharingCapabilities(SiteEditMetadata metadata)
        {
            try
            {
                AppOnlyAuthenticationTenant _auth = new AppOnlyAuthenticationTenant();
                _auth.TenantAdminUrl = metadata.TenantAdminUrl;
                var _service = new Office365SiteProvisioningService();
                _service.Authentication = _auth;
                metadata.TenantSharingEnabled = _service.IsTenantExternalSharingEnabled(metadata.TenantAdminUrl);
                metadata.Success = true;
                return metadata;
            }
            catch (Exception _ex)
            {
                metadata.ErrorMessage = _ex.Message;
                OfficeDevPnP.Core.Diagnostics.Log.Error("SPMetadataManager.CheckTenantSharingCapabilities",
                   "There was an error processing the request. Exception: {0}",
                   _ex);
                return metadata;
            }            
        }

        private SiteEditMetadata CheckSiteSharingCapabilities(SiteEditMetadata metadata)
        {
            try
            {
                AppOnlyAuthenticationTenant _auth = new AppOnlyAuthenticationTenant();
                _auth.TenantAdminUrl = metadata.TenantAdminUrl;
                var _service = new Office365SiteProvisioningService();
                _service.Authentication = _auth;
                metadata.SiteSharingEnabled = _service.isSiteExternalSharingEnabled(metadata.Url);
                metadata.Success = true;
                return metadata;
            }
            catch (Exception _ex)
            {
                metadata.ErrorMessage = _ex.Message;
                OfficeDevPnP.Core.Diagnostics.Log.Error("SPMetadataManager.CheckTenantSharingCapabilities",
                   "There was an error processing the request. Exception: {0}",
                   _ex);
                return metadata;
            }
        }

        public SiteEditMetadata SetSitePolicy(SiteEditMetadata metadata)
        {
            AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
            var _auth = new AppOnlyAuthenticationSite();
            _auth.SiteUrl = metadata.Url;
            _siteService.Authentication = _auth;

            var _appliedSitePolicy = _siteService.GetAppliedSitePolicy();           
                        
            if (_appliedSitePolicy != null)
            {
                var _updateSitePolicy = String.Compare(metadata.SitePolicy, _appliedSitePolicy.Name, StringComparison.InvariantCultureIgnoreCase) != 0;
                if (_updateSitePolicy)
                {
                    _siteService.SetSitePolicy(metadata.AppliedSitePolicyName);
                }

                // Get applied site policy data
                Uri siteUri = new Uri(metadata.Url);
                string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
                string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

                using (var ctx = TokenHelper.GetClientContextWithAccessToken(metadata.Url, accessToken))
                {
                    var _web = ctx.Web;
                    var _expDate = ctx.Web.GetSiteExpirationDate();

                    metadata.AppliedSitePolicy = _appliedSitePolicy;
                    metadata.AppliedSitePolicyName = metadata.AppliedSitePolicy.Name;
                    metadata.AppliedSitePolicyExpirationDate = _expDate == DateTime.MinValue ? String.Format("{0}", "None") : String.Format("{0}", _expDate);

                };                
            }
            else
            {
                _siteService.SetSitePolicy(metadata.SitePolicy);
                metadata.AppliedSitePolicyName = metadata.SitePolicy;
            }
            return metadata;
        }        
    }
}

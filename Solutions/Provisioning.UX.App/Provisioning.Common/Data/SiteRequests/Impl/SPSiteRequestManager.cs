using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Provisioning.Common.Utilities;
using Provisioning.Common.Data.SiteRequests;
using System.Diagnostics;

namespace Provisioning.Common.Data.SiteRequests.Impl
{
    /// <summary>
    /// Implmentation class for the Site Request Repository that leverages SharePoint as the datasource.
    /// </summary>
    internal class SPSiteRequestManager : AbstractModule, ISiteRequestManager, ISharePointClientService
    {
        #region Private Instance Members
        private static readonly IConfigurationFactory _cf = ConfigurationFactory.GetInstance();
        private static readonly IAppSettingsManager _manager = _cf.GetAppSetingsManager();
        const string LOGGING_SOURCE = "SPSiteRequestManagerImpl"; 
        const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" StaticName=""{1}"" DisplayName=""{2}"" ID=""{3}"" {4}/>";
        const string CAML_NEWREQUEST_BY_URL = "<Query><Where><And><Eq><FieldRef Name=SP_Url'/><Value Type='Text'>{0}</Value></Eq><Eq><FieldRef Name='Status'/><Value Type='Text'>New</Value></Eq></And></Where></Query>";
        const string CAML_NEWREQUESTS = "<View><Query><Where><Eq><FieldRef Name='SP_ProvisioningStatus'/><Value Type='Text'>New</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
        const string CAML_GETREQUEST_BY_URL = "<View><Query><Where><Eq><FieldRef Name='SP_Url'/><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
        const string CAML_APPROVEDREQUESTS = "<View><Query><Where><Eq><FieldRef Name='SP_ProvisioningStatus'/><Value Type='Text'>Approved</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
        const string CAML_GETREQUESTSBYOWNER = "<View><Query><Where><Eq><FieldRef Name='SP_Owner' LookupId='True'/><Value Type='Int'>{0}</Value></Eq></Where></Query></View>";
        const string CAML_INCOMPLETEREQUESTS = "<View><Query><Where><And><And><Neq><FieldRef Name='SP_ProvisioningStatus'/><Value Type='Text'>Complete</Value></Neq><Neq><FieldRef Name='SP_ProvisioningStatus'/><Value Type='Text'>Approved</Value></Neq></And><Neq><FieldRef Name='SP_ProvisioningStatus'/><Value Type='Text'>New</Value></Neq></And></Where></Query><RowLimit>100</RowLimit></View>";
        #endregion

        #region Constructor
        public SPSiteRequestManager()
        {
        }
        #endregion

        #region Private Methods 
        /// <summary>
        /// Member to return SiteRequest from the SharePoint SiteRequest Repository
        /// </summary>
        /// <param name="camlQuery">Query Query to Execute</param>
        /// <returns></returns>
        private ICollection<SiteInformation> GetSiteRequestsByCaml(string camlQuery)
        {   
            List<SiteInformation> _siteRequests = new List<SiteInformation>();
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                var _camlQuery = new CamlQuery();
                _camlQuery.ViewXml = camlQuery;

               Log.Info("SPSiteRequestManager.GetSiteRequestsByCaml",
                    "Querying SharePoint Request Repository {0}, Caml Query {1}",
                    SiteRequestList.LISTURL,
                    _camlQuery.ViewXml);

                var _web = ctx.Web;
                ctx.Load(_web);

                if (!_web.ListExists(SiteRequestList.TITLE))
                {
                    var _message = String.Format("The List {0} does not exist in Site {1}",
                         SiteRequestList.TITLE,
                         _web.Url);
                    Log.Fatal("SPSiteRequestManager.GetSiteRequestsByCaml", _message);
                    throw new DataStoreException(_message);
                }

                var _list = ctx.Web.Lists.GetByTitle(SiteRequestList.TITLE);
                var _listItemCollection = _list.GetItems(_camlQuery);
                ctx.Load(_listItemCollection,
                     eachItem => eachItem.Include(
                     item => item,
                     item => item[SiteRequestFields.TITLE],
                     item => item[SiteRequestFields.DESCRIPTION_NAME],
                     item => item[SiteRequestFields.TEMPLATE_NAME],
                     item => item[SiteRequestFields.POLICY_NAME],
                     item => item[SiteRequestFields.URL_NAME],
                     item => item[SiteRequestFields.OWNER_NAME],
                     item => item[SiteRequestFields.ADD_ADMINS_NAME],
                     item => item[SiteRequestFields.LCID_NAME],
                     item => item[SiteRequestFields.EXTERNALSHARING_NAME],
                     item => item[SiteRequestFields.PROVISIONING_STATUS_NAME],
                     item => item[SiteRequestFields.ONPREM_REQUEST_NAME],
                     item => item[SiteRequestFields.LCID_NAME],
                     item => item[SiteRequestFields.TIMEZONE_NAME],
                     item => item[SiteRequestFields.BC_NAME],
                     item => item[SiteRequestFields.PROPS_NAME],
                     item => item[SiteRequestFields.STATUSMESSAGE_NAME]));
                ctx.ExecuteQuery();

                _timespan.Stop();
                Log.TraceApi("SharePoint", "SPSiteRequestManager.GetSiteRequestsByCaml", _timespan.Elapsed);

                foreach (ListItem _item in _listItemCollection)
                {
                    var _site = new SiteInformation()
                    {
                        Title = _item.BaseGet(SiteRequestFields.TITLE),
                        Description = _item.BaseGet(SiteRequestFields.DESCRIPTION_NAME),
                        Template = _item.BaseGet(SiteRequestFields.TEMPLATE_NAME),
                        SitePolicy = _item.BaseGet(SiteRequestFields.POLICY_NAME),
                        Url = _item.BaseGet(SiteRequestFields.URL_NAME),
                        SiteOwner = _item.BaseGetUser(SiteRequestFields.OWNER_NAME),
                        AdditionalAdministrators = _item.BaseGetUsers(SiteRequestFields.ADD_ADMINS_NAME),
                        EnableExternalSharing = _item.BaseGet<bool>(SiteRequestFields.EXTERNALSHARING_NAME),
                        RequestStatus = _item.BaseGet(SiteRequestFields.PROVISIONING_STATUS_NAME),
                        Lcid = _item.BaseGetUint(SiteRequestFields.LCID_NAME),
                        TimeZoneId = _item.BaseGetInt(SiteRequestFields.TIMEZONE_NAME),
                        SharePointOnPremises = _item.BaseGet<bool>(SiteRequestFields.ONPREM_REQUEST_NAME),
                        BusinessCase = _item.BaseGet(SiteRequestFields.BC_NAME),
                        SiteMetadataJson = _item.BaseGet(SiteRequestFields.PROPS_NAME),
                        RequestStatusMessage = _item.BaseGet(SiteRequestFields.STATUSMESSAGE_NAME)
                    };
                    _siteRequests.Add(_site);
                }
            });
            return _siteRequests;
        }

        private SiteInformation GetSiteRequestByCaml(string camlQuery, string filter)
        {
            SiteInformation _siteRequest = null;
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                CamlQuery _camlQuery = new CamlQuery();
                _camlQuery.ViewXml = string.Format(camlQuery, filter);

               Log.Info("SPSiteRequestManager.GetSiteRequestsByCaml", "Querying SharePoint Request Repository: {0}, Caml Query: {1} Filter: {2}",
                  SiteRequestList.LISTURL,
                  _camlQuery.ViewXml,
                  filter);

                var _web = ctx.Web;
                ctx.Load(_web);

                if (!_web.ListExists(SiteRequestList.TITLE))
                {
                    var _message = String.Format("The List {0} does not exist in Site {1}",
                          SiteRequestList.TITLE,
                          _web.Url);
                    Log.Fatal("SPSiteRequestManager.GetSiteRequestsByCaml", _message);
                    throw new DataStoreException(_message);
                }

                var _list = ctx.Web.Lists.GetByTitle(SiteRequestList.TITLE);
                var _listItemCollection = _list.GetItems(_camlQuery);

                ctx.Load(_listItemCollection,
                    eachItem => eachItem.Include(
                    item => item,
                    item => item[SiteRequestFields.TITLE],
                    item => item[SiteRequestFields.DESCRIPTION_NAME],
                    item => item[SiteRequestFields.TEMPLATE_NAME],
                    item => item[SiteRequestFields.POLICY_NAME],
                    item => item[SiteRequestFields.URL_NAME],
                    item => item[SiteRequestFields.OWNER_NAME],
                    item => item[SiteRequestFields.PROVISIONING_STATUS_NAME],
                    item => item[SiteRequestFields.ADD_ADMINS_NAME],
                    item => item[SiteRequestFields.LCID_NAME],
                    item => item[SiteRequestFields.EXTERNALSHARING_NAME],
                    item => item[SiteRequestFields.PROVISIONING_STATUS_NAME],
                    item => item[SiteRequestFields.ONPREM_REQUEST_NAME],
                    item => item[SiteRequestFields.LCID_NAME],
                    item => item[SiteRequestFields.TIMEZONE_NAME],
                    item => item[SiteRequestFields.BC_NAME],
                    item => item[SiteRequestFields.PROPS_NAME],
                    item => item[SiteRequestFields.STATUSMESSAGE_NAME]));
                ctx.ExecuteQuery();

                _timespan.Stop();
                Log.TraceApi("SharePoint", "SPSiteRequestManager.GetSiteRequestsByCaml", _timespan.Elapsed);

                if (_listItemCollection.Count > 0)
                {
                    ListItem _item = _listItemCollection.First();

                    _siteRequest = new SiteInformation()
                    {
                        Title = _item.BaseGet(SiteRequestFields.TITLE),
                        Description = _item.BaseGet(SiteRequestFields.DESCRIPTION_NAME),
                        Template = _item.BaseGet(SiteRequestFields.TEMPLATE_NAME),
                        SitePolicy = _item.BaseGet(SiteRequestFields.POLICY_NAME),
                        Url = _item.BaseGet(SiteRequestFields.URL_NAME),
                        SiteOwner = _item.BaseGetUser(SiteRequestFields.OWNER_NAME),
                        AdditionalAdministrators = _item.BaseGetUsers(SiteRequestFields.ADD_ADMINS_NAME),
                        EnableExternalSharing = _item.BaseGet<bool>(SiteRequestFields.EXTERNALSHARING_NAME),
                        RequestStatus = _item.BaseGet(SiteRequestFields.PROVISIONING_STATUS_NAME),
                        Lcid = _item.BaseGetUint(SiteRequestFields.LCID_NAME),
                        TimeZoneId = _item.BaseGetInt(SiteRequestFields.TIMEZONE_NAME),
                        SharePointOnPremises = _item.BaseGet<bool>(SiteRequestFields.ONPREM_REQUEST_NAME),
                        BusinessCase = _item.BaseGet( SiteRequestFields.BC_NAME),
                        SiteMetadataJson = _item.BaseGet( SiteRequestFields.PROPS_NAME),
                        RequestStatusMessage = _item.BaseGet(SiteRequestFields.STATUSMESSAGE_NAME)
                    };
                }
            });
            return _siteRequest;
        }
        #endregion

        #region ISharePointClientService Members
        /// <summary>
        /// Class used for working with the ClientContext
        /// </summary>
        /// <param name="action"></param>
        public virtual void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        /// <summary>
        /// Class used for working with the ClientContext
        /// </summary>
        /// <param name="action"></param>
        /// <param name="csomTimeOut"></param>
        public virtual void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the implementation for AppOnlyAuthentication
        /// </summary>
        public IAuthentication Authentication
        {
            get
            {
                return new AppOnlyAuthenticationSite();
            }
            
        }
        #endregion

        #region ISiteRequestManager Members
        public ICollection<SiteInformation> GetOwnerRequests(string email)
        {
            Log.Info("SPSiteRequestManager.GetOwnerRequests", "Entering GetOwnerRequests by email {0}", email);

            ICollection<SiteInformation> _returnResults = new List<SiteInformation>();
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                try
                {
                    var _user = ctx.Web.EnsureUser(email);
                    ctx.Load(_user);
                    ctx.ExecuteQuery();

                    if (_user != null) 
                    {
                        var _userID = _user.Id;
                        var camlString = string.Format(CAML_GETREQUESTSBYOWNER, _userID);
                        _returnResults = this.GetSiteRequestsByCaml(camlString);

                        _timespan.Stop();
                        Log.TraceApi("SharePoint", "SPSiteRequestManager.GetOwnerRequests", _timespan.Elapsed);
                    }
                    else
                    {
                        Log.Warning("SPSiteRequestManager.GetOwnerRequests", "GetOwnerRequests email {0} not found", email);
                    }
                }
                catch (Exception _ex) 
                {
                  //TODO LOG
                }
            });
            return _returnResults;
        }

        public void CreateNewSiteRequest(SiteInformation siteRequest)
        {
            Log.Info("SPSiteRequestManager.CreateNewSiteRequest", "Entering CreateNewSiteRequest requested url {0}", siteRequest.Url);
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
                var _web = ctx.Web;
                ctx.Load(_web);

                if (!_web.ListExists(SiteRequestList.TITLE))
                {
                    var _message = String.Format("The List {0} does not exist in Site {1}",
                         SiteRequestList.TITLE,
                         _web.Url);
                    Log.Fatal("SPSiteRequestManager.CreateNewSiteRequest", _message);
                    throw new DataStoreException(_message);
                }

                List list = _web.Lists.GetByTitle(SiteRequestList.TITLE);
                ListItemCreationInformation _listItemCreation = new ListItemCreationInformation();
                ListItem _record = list.AddItem(_listItemCreation);
                _record[SiteRequestFields.TITLE] = siteRequest.Title;
                _record[SiteRequestFields.DESCRIPTION_NAME] = siteRequest.Description;
                _record[SiteRequestFields.TEMPLATE_NAME] = siteRequest.Template;
                _record[SiteRequestFields.URL_NAME] = siteRequest.Url;
                _record[SiteRequestFields.LCID_NAME] = siteRequest.Lcid;
                _record[SiteRequestFields.TIMEZONE_NAME] = siteRequest.TimeZoneId;
                _record[SiteRequestFields.POLICY_NAME] = siteRequest.SitePolicy;
                _record[SiteRequestFields.EXTERNALSHARING_NAME] = siteRequest.EnableExternalSharing;
                _record[SiteRequestFields.ONPREM_REQUEST_NAME] = siteRequest.SharePointOnPremises;
                _record[SiteRequestFields.BC_NAME] = siteRequest.BusinessCase;
                _record[SiteRequestFields.PROPS_NAME] = siteRequest.SiteMetadataJson;
                //If Settings are set to autoapprove then automatically approve the requests
                if(_manager.GetAppSettings().AutoApprove) 
                {
                    _record[SiteRequestFields.PROVISIONING_STATUS_NAME] = SiteRequestStatus.Approved.ToString();
                    _record[SiteRequestFields.APPROVEDDATE_NAME] = DateTime.Now;
                }
                else 
                {
                    _record[SiteRequestFields.PROVISIONING_STATUS_NAME] = SiteRequestStatus.New.ToString();
                }
                
                FieldUserValue _siteOwner = FieldUserValue.FromUser(siteRequest.SiteOwner.Name);
                _record[SiteRequestFields.OWNER_NAME] = _siteOwner;
                
                //Additional Admins
                if(siteRequest.AdditionalAdministrators != null)
                {
                    if (siteRequest.AdditionalAdministrators.Count > 0)
                    {
                        FieldUserValue[] _additionalAdmins = new FieldUserValue[siteRequest.AdditionalAdministrators.Count];
                        int _index = 0;
                        foreach (SiteUser _user in siteRequest.AdditionalAdministrators)
                        {
                            FieldUserValue _adminFieldUser = FieldUserValue.FromUser(_user.Name);
                            _additionalAdmins[_index] = _adminFieldUser;
                            _index++;
                        }
                        _record[SiteRequestFields.ADD_ADMINS_NAME] = _additionalAdmins;
                    }
                }
                _record.Update();
                ctx.ExecuteQuery();
                _timespan.Stop();

                Log.TraceApi("SharePoint", "SPSiteRequestManager.CreateNewSiteRequest", _timespan.Elapsed);
                Log.Info("SPSiteRequestManager.CreateNewSiteRequest", PCResources.SiteRequestNew_Successful, siteRequest.Url);
            }
            );
        }

        public SiteInformation GetSiteRequestByUrl(string url)
        {
            Log.Info("SPSiteRequestManager.GetSiteRequestByUrl", "Entering GetSiteRequestByUrl url {0}", url);
            return this.GetSiteRequestByCaml(CAML_GETREQUEST_BY_URL, url);
        }

        public ICollection<SiteInformation> GetNewRequests()
        {
            Log.Info("SPSiteRequestManager.GetNewRequests", "Entering GetNewRequests");
            return this.GetSiteRequestsByCaml(CAML_NEWREQUESTS);
        }

        public ICollection<SiteInformation> GetApprovedRequests()
        {
            Log.Info("SPSiteRequestManager.GetNewRequests", "Entering GetApprovedRequests");
            return this.GetSiteRequestsByCaml(CAML_APPROVEDREQUESTS);
        }

        public ICollection<SiteInformation> GetIncompleteRequests()
        {
            Log.Info("SPSiteRequestManager.GetIncompleteRequests", "Entering GetIncompleteRequests");
            return this.GetSiteRequestsByCaml(CAML_INCOMPLETEREQUESTS);
        }

        public bool DoesSiteRequestExist(string url)
        {
            Log.Info("SPSiteRequestManager.DoesSiteRequestExist", "Entering DoesSiteRequestExist url {0}", url);
            var _result = this.GetSiteRequestByUrl(url);
            if(_result != null)
            {
                return true;
            }
            return false;
        }

        public void UpdateRequestStatus(string url, SiteRequestStatus status)
        {
            Log.Info("SPSiteRequestManager.UpdateRequestStatus", "Entering UpdateRequestStatus url {0} status {1}", url, status.ToString());
            this.UpdateRequestStatus(url, status, string.Empty);
        }

        public void UpdateRequestStatus(string url, SiteRequestStatus status, string statusMessage)
        {
            Log.Info("SPSiteRequestManager.UpdateRequestStatus", "Entering UpdateRequestStatus url {0} status {1} status message", url, status.ToString(), statusMessage);
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();
               
                var _web = ctx.Web;
                ctx.Load(_web);

                if (!_web.ListExists(SiteRequestList.TITLE)) {
                    var _message = String.Format("The List {0} does not exist in Site {1}",
                         SiteRequestList.TITLE,
                         _web.Url);
                    Log.Fatal("SPSiteRequestManager.UpdateRequestStatus", _message);
                    throw new DataStoreException(_message);
                }

                var _list = ctx.Web.Lists.GetByTitle(SiteRequestList.TITLE);
                var _query = new CamlQuery();
                _query.ViewXml = string.Format(CAML_GETREQUEST_BY_URL, url);
                
                ListItemCollection _itemCollection =_list.GetItems(_query);
                ctx.Load(_itemCollection);
                ctx.ExecuteQuery();

                if (_itemCollection.Count != 0) {
                    ListItem _item = _itemCollection.FirstOrDefault();
                    _item[SiteRequestFields.PROVISIONING_STATUS_NAME] = status.ToString();
                    _item[SiteRequestFields.STATUSMESSAGE_NAME] = statusMessage;

                    _item.Update();
                    ctx.ExecuteQuery();
                }

                _timespan.Stop();
                Log.Info("SPSiteRequestManager.UpdateRequestStatus", PCResources.SiteRequestUpdate_Successful, url, status.ToString());
                Log.TraceApi("SharePoint", "SPSiteRequestManager.UpdateRequestStatus", _timespan.Elapsed);
            });
        }

        public void UpdateRequestUrl(string url, string newUrl)
        {
            Log.Info("SPSiteRequestManager.UpdateRequestUrl", "Entering UpdateRequestUrl url {0} status {1} status message", url, newUrl);
            UsingContext(ctx =>
            {
                Stopwatch _timespan = Stopwatch.StartNew();

                var _web = ctx.Web;
                ctx.Load(_web);

                if (!_web.ListExists(SiteRequestList.TITLE))
                {
                    var _message = String.Format("The List {0} does not exist in Site {1}",
                         SiteRequestList.TITLE,
                         _web.Url);
                    Log.Fatal("SPSiteRequestManager.UpdateRequestUrl", _message);
                    throw new DataStoreException(_message);
                }

                var _list = ctx.Web.Lists.GetByTitle(SiteRequestList.TITLE);
                var _query = new CamlQuery();
                _query.ViewXml = string.Format(CAML_GETREQUEST_BY_URL, url);

                ListItemCollection _itemCollection = _list.GetItems(_query);
                ctx.Load(_itemCollection);
                ctx.ExecuteQuery();

                if (_itemCollection.Count != 0)
                {
                    ListItem _item = _itemCollection.FirstOrDefault();
                    _item[SiteRequestFields.URL_NAME] = newUrl;

                    
                    _item.Update();
                    ctx.ExecuteQuery();
                }

                _timespan.Stop();
                Log.Info("SPSiteRequestManager.UpdateRequestUrl", PCResources.SiteRequestUpdate_Successful, url, newUrl);
                Log.TraceApi("SharePoint", "SPSiteRequestManager.UpdateRequestUrl", _timespan.Elapsed);
            });
        }

        #endregion
    }
}

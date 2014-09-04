using Microsoft.SharePoint.Client;
using Patterns.Provisioning.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Patterns.Provisioning.Common.Data.Impl
{
    /// <summary>
    /// Implmentation class for the Site Request Repository that leverages SharePoint as the datasource
    /// </summary>
    internal class SpSiteSource : ISiteRequestManager
    {
        #region Private Instance Members
        const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" StaticName=""{1}"" DisplayName=""{2}"" ID=""{3}"" {4}/>";

        private bool _isFaulted = false;
        private ClientContext _authenticatedCtx;
        private string _listName;
        private static readonly string CAML_NEWREQUEST_BY_URL = "<Query><Where><And><Eq><FieldRef Name=SR_Url'/><Value Type='Text'>{0}</Value></Eq><Eq><FieldRef Name='Status'/><Value Type='Text'>New</Value></Eq></And></Where></Query>";
        private static readonly string CAML_NEWREQUESTS = "<View><Query><Where><Eq><FieldRef Name='SR_Status'/><Value Type='Text'>New</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";
        private static readonly string CAML_GETREQUEST_BY_URL = "<View><Query><Where><Eq><FieldRef Name='SR_Url'/><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>100</RowLimit></View>";               
        #endregion

        #region Private Members
        /// <summary>
        /// Initiliaze the object
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="listName"></param>
        internal void Initialize(ClientContext ctx, string listName)
        {
            try
            {
                var web = ctx.Web;
                var lists = web.Lists;
                ctx.Load(web);
                ctx.Load(lists, lc => lc.Where(l=>l.Title == listName));
                ctx.ExecuteQuery();

                this._authenticatedCtx = ctx;
                this._listName = listName;

                // create the list in the host web if it does not already exist
                if (lists.Count == 0) {
                    CreateSiteRequestsList();
                }
            }
            catch(IdcrlException _idlException)
            {
                this._isFaulted = true;
                throw new DataStoreException("ClientContext is invalid.", _idlException);
            }
            catch(Exception _ex)
            {
                this._isFaulted = true;
                throw new DataStoreException(_ex.Message, _ex);
            }
        }

        private void CreateSiteRequestsList() {
            var newList = new ListCreationInformation() {
                Title = this._listName,
                Description = Lists.SiteRepositoryDesc,
                TemplateType = (int)ListTemplateType.GenericList,
                Url = Lists.SiteRepositoryUrl
            };
            var list = this._authenticatedCtx.Web.Lists.Add(newList);
            this._authenticatedCtx.Load(list);
            this._authenticatedCtx.ExecuteQuery();
            
            // add fields (replace the second field with the display name
            AddFieldAsXml(list, SiteRequestFields.Description, SiteRequestFields.DescriptionDisplayName, SiteRequestFields.DescriptionId, "Note", options: AddFieldOptions.AddFieldCheckDisplayName);
            AddFieldAsXml(list, SiteRequestFields.Template, SiteRequestFields.TemplateDisplayName, SiteRequestFields.TemplateId);
            AddFieldAsXml(list, SiteRequestFields.Policy, SiteRequestFields.PolicyDisplayName, SiteRequestFields.PolicyId);
            AddFieldAsXml(list, SiteRequestFields.Url, SiteRequestFields.UrlDisplayName, SiteRequestFields.UrlId);
            AddFieldAsXml(list, SiteRequestFields.Owner, SiteRequestFields.OwnerDisplayName, SiteRequestFields.OwnerId, "User", "List='UserInfo' UserSelectionMode='0' ShowField='ImnName'");
            AddFieldAsXml(list, SiteRequestFields.AdditionalOwners, SiteRequestFields.AdditionalOwnersDisplayName, SiteRequestFields.AdditionalOwnersId, "UserMulti", "Mult='TRUE' List='UserInfo' UserSelectionMode='1' ShowField='ImnName'");
            AddFieldAsXml(list, SiteRequestFields.Lcid, SiteRequestFields.LcidDisplayName, SiteRequestFields.LcidId);
            AddFieldAsXml(list, SiteRequestFields.StatusMessage, SiteRequestFields.StatusMessageDisplayName, SiteRequestFields.StatusMessageId, "Note", options: AddFieldOptions.AddFieldCheckDisplayName);
            AddFieldAsXml(list, SiteRequestFields.TimeZone, SiteRequestFields.TimeZoneDisplayName, SiteRequestFields.TimeZoneId);
            AddFieldAsXml(list, SiteRequestFields.State, SiteRequestFields.StateDisplayName, SiteRequestFields.StatusId, additionalAttributes: "ReadOnly='TRUE'");

            list.Update();

            this._authenticatedCtx.ExecuteQuery();
        }

        Field AddFieldAsXml(List list, string fieldInternalName, string fieldDisplayName, Guid fieldId, string fieldType = "Text", string additionalAttributes = "", AddFieldOptions options = AddFieldOptions.AddFieldToDefaultView) {
            var fieldXml = string.Format(FIELD_XML_FORMAT, fieldType, fieldInternalName, fieldDisplayName, fieldId, additionalAttributes);
            var field = list.Fields.AddFieldAsXml(fieldXml, true, options | AddFieldOptions.AddFieldInternalNameHint);
            return field;
        }

        /// <summary>
        /// Check to Ensure the State of the object is valid
        /// </summary>
        private void EnsureStateOfObject()
        {
            if (this._isFaulted)
            {
                throw new DataStoreException("Manager is in a faulted stated.");
            }

        }

        /// <summary>
        /// Used to get a value from a list
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private string BaseSet(ListItem item, string fieldName)
        {
            return item[fieldName] == null ? String.Empty : item[fieldName].ToString();
        }

        /// <summary>
        /// Helper to return a string from a Url Field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private string BaseSetUrl(ListItem item, string fieldName)
        {
            return ((FieldUrlValue)(item[fieldName])).Url;
        }

        private int BaseSetInt(ListItem item, string fieldName)
        {
            return Convert.ToInt32(item[fieldName]);
        }
        /// <summary>
        /// Helper to return a uint from a string field
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private uint BaseSetUint(ListItem item, string fieldName)
        {
            object _temp = item[fieldName];
            uint _result = new uint();
            if (_temp != null)
            {
                uint.TryParse(item[fieldName].ToString(), out _result);
                return _result;
            }
            return _result;

        }

        /// <summary>
        /// Used to create a User Object to SharePointUser
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private SharePointUser BaseSetUser(ListItem item, string field)
        {
            SharePointUser _owner = new SharePointUser();
            var _fieldUser = ((FieldUserValue)(item[field]));
            User _user = this._authenticatedCtx.Web.EnsureUser(_fieldUser.LookupValue);
            this._authenticatedCtx.Load(_user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
            this._authenticatedCtx.ExecuteQuery();

            _owner.Email = _user.Email;
            _owner.Login = _user.LoginName;
            _owner.Name = _user.Title;
            return _owner;
        }

        /// <summary>
        /// Used to create a User Object to SharePoint Users
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private ICollection<SharePointUser> BaseSetUsers(ListItem item, string fieldName)
        {
            List<SharePointUser> _users = new List<SharePointUser>();
            foreach (FieldUserValue _userValue in item[fieldName] as FieldUserValue[])
            {
                User _user = this._authenticatedCtx.Web.EnsureUser(_userValue.LookupValue);
                this._authenticatedCtx.Load(_user, u => u.LoginName, u => u.Email, u => u.PrincipalType, u => u.Title);
                this._authenticatedCtx.ExecuteQuery();

                var _spUser = new SharePointUser()
                {
                    Email = _user.Email,
                    Login = _user.LoginName,
                    Name = _user.Title
                };
                _users.Add(_spUser);
            }

            return _users;
        }

        /// <summary>
        /// Returns the SharePoint List Repository for SiteRequests. If the list doesn't exist the list will be created.
        /// TODO FINISH IMPL
        /// </summary>
        /// <returns></returns>
        private List GetSharePointRepository()
        {
            var _ctx = this._authenticatedCtx;
            List _repositoryList;

            ExceptionHandlingScope _scope = new ExceptionHandlingScope(_ctx);
            using (_scope.StartScope())
            {
                using (_scope.StartTry())
                {
                    _repositoryList = _ctx.Web.Lists.GetByTitle(Lists.SiteRepositoryTitle);
                    _ctx.Load(_repositoryList);

                }
                using (_scope.StartCatch())
                {
                    //we assume the list doesnt so lets create it
                    var _listCreation = new ListCreationInformation()
                    {
                        Title = Lists.SiteRepositoryTitle,
                        TemplateType = (int)ListTemplateType.GenericList,
                        Description = Lists.SiteRepositoryDesc
                       
                    };

                    var _list = _ctx.Web.Lists.Add(_listCreation);
                    FieldText _templateField = _ctx.CastTo<FieldText>(_list.Fields.Add(SiteRequestFields.TemplateId, SiteRequestFields.Template, FieldType.Text, SiteRequestFields.Template, true));
                    FieldText _urlField = _ctx.CastTo<FieldText>(_list.Fields.Add(SiteRequestFields.UrlId, SiteRequestFields.Url, FieldType.Text, SiteRequestFields.Url, true));
                    FieldText _descField = _ctx.CastTo<FieldText>(_list.Fields.Add(SiteRequestFields.DescriptionId, SiteRequestFields.Description, FieldType.Text, SiteRequestFields.Description, true));
                    FieldUser _ownerField = _ctx.CastTo<FieldUser>(_list.Fields.Add(SiteRequestFields.OwnerId, SiteRequestFields.Owner, FieldType.User, SiteRequestFields.Owner, true));
                    _ownerField.SelectionMode = FieldUserSelectionMode.PeopleOnly;
                    _ownerField.Update();
                    FieldUser _additionOwnersFields = _ctx.CastTo<FieldUser>(_list.Fields.Add(SiteRequestFields.AdditionalOwnersId, SiteRequestFields.AdditionalOwners, FieldType.User, SiteRequestFields.AdditionalOwners, true));
                    _additionOwnersFields.AllowMultipleValues = true;
                    _additionOwnersFields.SelectionMode = FieldUserSelectionMode.PeopleOnly;
                    _additionOwnersFields.Update();
                    FieldText _policyField = _ctx.CastTo<FieldText>(_list.Fields.Add(SiteRequestFields.PolicyId, SiteRequestFields.Policy, FieldType.Text, SiteRequestFields.Policy, true));
                    FieldText _statusField = _ctx.CastTo<FieldText>(_list.Fields.Add(SiteRequestFields.StatusId, SiteRequestFields.State, FieldType.Text, SiteRequestFields.State, true));
                    FieldText _lcidField = _ctx.CastTo<FieldText>(_list.Fields.Add(SiteRequestFields.LcidId, SiteRequestFields.Lcid, FieldType.Text, SiteRequestFields.Lcid, true));
                    FieldNumber _timeZoneID = _ctx.CastTo<FieldNumber>(_list.Fields.Add(SiteRequestFields.TimeZoneId, SiteRequestFields.TimeZone, FieldType.Number, SiteRequestFields.TimeZone, true));
                    FieldText _MessageField = _ctx.CastTo<FieldText>(_list.Fields.Add(SiteRequestFields.StatusMessageId, SiteRequestFields.StatusMessage, FieldType.Text, SiteRequestFields.StatusMessage, true));
                    
                 
               
                }
                using (_scope.StartFinally())
                {
                    _repositoryList = _ctx.Web.Lists.GetByTitle(Lists.SiteRepositoryTitle);
                    _ctx.Load(_repositoryList);
                }
            }
            _ctx.ExecuteQuery();
            return _repositoryList;
          
        }
        #endregion

        #region ISiteRequestManager Members

        public void CreateNewSiteRequest(SiteRequestInformation siteRequest)
        {
            EnsureStateOfObject();
            if(this.DoesSiteRequestExist(siteRequest.Url))
            {
                throw new DataStoreException(String.Format("An item already exist with a url of {0}.", siteRequest.Url));
            }
            try
            { 
                Web _web = this._authenticatedCtx.Web;
                List _list = _web.Lists.GetByTitle(this._listName);
                ListItemCreationInformation _listItemCreation = new ListItemCreationInformation();
                ListItem _record = _list.AddItem(_listItemCreation);
                
                //Create the Record to insert
                _record[SiteRequestFields.Title] = siteRequest.Title;
               
                _record[SiteRequestFields.Description] = siteRequest.Description;
                _record[SiteRequestFields.Template] = siteRequest.Template;
                _record[SiteRequestFields.State] = SiteRequestStatus.New.ToString();
                _record[SiteRequestFields.Url] = siteRequest.Url;
                _record[SiteRequestFields.Lcid] = siteRequest.Lcid;
                _record[SiteRequestFields.TimeZone] = siteRequest.TimeZoneId;
                FieldUserValue _siteOwner = FieldUserValue.FromUser(siteRequest.SiteOwner.Email);
                _record[SiteRequestFields.Owner] = _siteOwner;
                
                if(!String.IsNullOrEmpty(siteRequest.SitePolicy))
                {
                    _record[SiteRequestFields.Policy] = siteRequest.SitePolicy;
                }
               
                //Additional Adminstrators
                if (siteRequest.AdditionalAdministrators != null)
                {
                    FieldUserValue[] _additionalAdmins = new FieldUserValue[siteRequest.AdditionalAdministrators.Count];
                    int _index = 0;
                    foreach (SharePointUser _user in siteRequest.AdditionalAdministrators)
                    {
                        FieldUserValue _adminFieldUser = FieldUserValue.FromUser(_user.Email);
                        _additionalAdmins[_index] = _adminFieldUser;
                        _index++;
                    }
                    _record[SiteRequestFields.AdditionalOwners] = _additionalAdmins;
                }
  
                _record.Update();
                this._authenticatedCtx.ExecuteQuery();
            }
            catch(Exception _ex)
            {
                throw new DataStoreException("Exception occured while inserting record.", _ex);
            }
        }

        public bool IsFaulted
        {
            get { return this._isFaulted; }
            internal set { this._isFaulted = value;}
        }

        public ICollection<SiteRequestInformation> GetNewRequests()
        {
            EnsureStateOfObject();
            List<SiteRequestInformation> _siteRequests = new List<SiteRequestInformation>();
            CamlQuery _caml = new CamlQuery();
            _caml.ViewXml = CAML_NEWREQUESTS;

            var _ctx = this._authenticatedCtx;
            var _web = _ctx.Web;
            var _list = _web.Lists.GetByTitle(this._listName);
            var _listItemCollection = _list.GetItems(_caml);

            _ctx.Load(_listItemCollection,
                eachItem => eachItem.Include(
                item => item,
                item => item[SiteRequestFields.Title],
                item => item[SiteRequestFields.Description],
                item => item[SiteRequestFields.Template],
                item => item[SiteRequestFields.Policy],
                item => item[SiteRequestFields.Url],
                item => item[SiteRequestFields.Owner],
                item => item[SiteRequestFields.AdditionalOwners],
                item => item[SiteRequestFields.Lcid]));
            _ctx.ExecuteQuery();

            foreach(ListItem _item in _listItemCollection)
            {
                var _site = new SiteRequestInformation()
                {
                  Title = this.BaseSet(_item, SiteRequestFields.Title),
                  Description = this.BaseSet(_item, SiteRequestFields.Description),
                  Template = this.BaseSet(_item, SiteRequestFields.Template),
                  SitePolicy = this.BaseSet(_item, SiteRequestFields.Policy),
                  Url = this.BaseSet(_item, SiteRequestFields.Url),
                  SiteOwner = this.BaseSetUser(_item, SiteRequestFields.Owner),
                  AdditionalAdministrators = this.BaseSetUsers(_item, SiteRequestFields.AdditionalOwners)
                };
                

                _siteRequests.Add(_site);  
            }

            return _siteRequests;
        }

        public SiteRequestInformation GetSiteRequestByUrl(string url)
        {
            ArgumentHelper.RequireNotNullOrEmpty(url, "url");
            EnsureStateOfObject();

            CamlQuery _caml = new CamlQuery();
            _caml.ViewXml = string.Format(CAML_GETREQUEST_BY_URL, url);

            var _ctx = this._authenticatedCtx;
            var _web = _ctx.Web;
            var _list = _web.Lists.GetByTitle(this._listName);
            var _listItemCollection = _list.GetItems(_caml);
          
            _ctx.Load(_listItemCollection,
                eachItem => eachItem.Include(
                item => item,
                item => item[SiteRequestFields.Title],
                item => item[SiteRequestFields.Description],
                item => item[SiteRequestFields.Template],
                item => item[SiteRequestFields.Policy],
                item => item[SiteRequestFields.Url],
                item => item[SiteRequestFields.Owner],
                item => item[SiteRequestFields.AdditionalOwners], 
                item => item[SiteRequestFields.TimeZone],
                item => item[SiteRequestFields.Lcid]));
            _ctx.ExecuteQuery();

            if(_listItemCollection.Count > 0)
            {
                ListItem _item = _listItemCollection.First();
            
                var _siteRequest = new SiteRequestInformation()
                {
                    Title = this.BaseSet(_item, SiteRequestFields.Title),
                    Description = this.BaseSet(_item, SiteRequestFields.Description),
                    Template = this.BaseSet(_item, SiteRequestFields.Template),
                    SitePolicy = this.BaseSet(_item, SiteRequestFields.Policy),
                    Url = this.BaseSet(_item, SiteRequestFields.Url),
                    SiteOwner = this.BaseSetUser(_item, SiteRequestFields.Owner),
                    AdditionalAdministrators = this.BaseSetUsers(_item, SiteRequestFields.AdditionalOwners),
                    Lcid = this.BaseSetUint(_item, SiteRequestFields.Lcid),
                    TimeZoneId = this.BaseSetInt(_item, SiteRequestFields.TimeZone),
                };
                return _siteRequest;
            }
            else
            {
                return null;
            }
        }

        public bool DoesSiteRequestExist(string url)
        {
            var _result = this.GetSiteRequestByUrl(url);
            if(_result != null)
            {
                return true;
            }
            return false;
        }

        public void UpdateRequestStatus(string url, SiteRequestStatus status)
        {
            this.UpdateRequestStatus(url, status, string.Empty);
        }

        public void UpdateRequestStatus(string url, SiteRequestStatus status, string statusMessage)
        {
            EnsureStateOfObject();
            ArgumentHelper.RequireNotNullOrEmpty(url, "url");

            Web _web = this._authenticatedCtx.Web;
            List _list = _web.Lists.GetByTitle(this._listName);

            CamlQuery _query = new CamlQuery();
            _query.ViewXml = string.Format(CAML_GETREQUEST_BY_URL, url);

            ListItemCollection _itemCollection = _list.GetItems(_query);
            this._authenticatedCtx.Load(_itemCollection);
            this._authenticatedCtx.ExecuteQuery();

            if(_itemCollection.Count != 0)
            {
                ListItem _item = _itemCollection.First();
                _item[SiteRequestFields.State] = status.ToString();
                if(!string.IsNullOrEmpty(statusMessage))
                {
                    _item[SiteRequestFields.StatusMessage] = statusMessage;
                }
                _item.Update();
                this._authenticatedCtx.ExecuteQuery();
            }
        }

        #endregion

    }
}



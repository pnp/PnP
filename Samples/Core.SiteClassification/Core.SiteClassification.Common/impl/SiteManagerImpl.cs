using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SiteClassification.Common.impl
{
    /// <summary>
    /// Implementation Class for the SiteClassificationManager. 
    /// </summary>
    internal class SiteClassificationImpl : ISiteClassificationManager
    {
        #region Internal Members
        internal void Initialize(ClientContext ctx)
        {
            try {
                var _web = ctx.Web;
                var lists = _web.Lists;
                ctx.Load(_web);
                ctx.Load(lists, lc => lc.Where(l => l.Title == SiteClassificationList.SiteClassificationListTitle));
                ctx.ExecuteQuery();
                
                if (lists.Count == 0) {
                    this.CreateSiteClassificationList(ctx); 
                }
            }
            catch(Exception)
            {

            }
        }
        #endregion

        #region Private Members
      
        /// <summary>
        /// We want to remove from the quick launch.
        /// http://blogs.technet.com/b/speschka/archive/2014/05/07/create-a-list-in-the-host-web-when-your-sharepoint-app-is-installed-and-remove-it-from-the-recent-stuff-list.aspx
        /// </summary>
        /// <param name="ctx">The ClientContext </param>
        /// <param name="listName">The List name to remove</param>
        private void RemoveFromQuickLaunch(ClientContext ctx, string listName)
        {
            Site _site = ctx.Site;
            Web _web = _site.RootWeb;

            ctx.Load(_web, x => x.Navigation, x => x.Navigation.QuickLaunch);
            ctx.ExecuteQuery();

            var _vNode = from NavigationNode _navNode in _web.Navigation.QuickLaunch
                        where _navNode.Title == "Recent"
                        select _navNode;

            NavigationNode _nNode = _vNode.First<NavigationNode>();
            ctx.Load(_nNode.Children);
            ctx.ExecuteQuery();
            var vcNode = from NavigationNode cn in _nNode.Children
                         where cn.Title == listName
                         select cn;
            NavigationNode _cNode = vcNode.First<NavigationNode>();
            _cNode.DeleteObject();

            ctx.ExecuteQuery();
        }
        private void CreateSiteClassificationList(ClientContext ctx)
        {
            var _newList = new ListCreationInformation()
            {
                Title = SiteClassificationList.SiteClassificationListTitle,
                Description = SiteClassificationList.SiteClassificationDesc,
                TemplateType = (int)ListTemplateType.GenericList,
                Url = SiteClassificationList.SiteClassificationUrl,
                QuickLaunchOption = QuickLaunchOptions.Off
            };

            if(!ctx.Web.ContentTypeExistsById(SiteClassificationContentType.SITEINFORMATION_CT_ID))
            {
                //ct
                ContentType _contentType = ctx.Web.CreateContentType(SiteClassificationContentType.SITEINFORMATION_CT_NAME,
                    SiteClassificationContentType.SITEINFORMATION_CT_DESC,
                    SiteClassificationContentType.SITEINFORMATION_CT_ID,
                    SiteClassificationContentType.SITEINFORMATION_CT_GROUP);

                FieldLink _titleFieldLink = _contentType.FieldLinks.GetById(new Guid("fa564e0f-0c70-4ab9-b863-0177e6ddd247"));
                _titleFieldLink.Required = false;
                _contentType.Update(false);

                //Key Field
                FieldCreationInformation fldCreate = new FieldCreationInformation(FieldType.Text)
                {
                    Id = SiteClassificationFields.FLD_KEY_ID,
                    InternalName = SiteClassificationFields.FLD_KEY_INTERNAL_NAME,
                    DisplayName = SiteClassificationFields.FLD_KEY_DISPLAY_NAME,
                    Group = SiteClassificationFields.FIELDS_GROUPNAME,
                };
                ctx.Web.CreateField(fldCreate);

                //value field
                fldCreate = new FieldCreationInformation(FieldType.Text)
                {
                    Id = SiteClassificationFields.FLD_VALUE_ID,
                    InternalName = SiteClassificationFields.FLD_VALUE_INTERNAL_NAME,
                    DisplayName = SiteClassificationFields.FLD_VALUE_DISPLAY_NAME,
                    Group = SiteClassificationFields.FIELDS_GROUPNAME,
                };
                ctx.Web.CreateField(fldCreate);

                //Add Key Field to content type
                ctx.Web.AddFieldToContentTypeById(SiteClassificationContentType.SITEINFORMATION_CT_ID, 
                    SiteClassificationFields.FLD_KEY_ID.ToString(), 
                    true);
                //Add Value Field to content type
                ctx.Web.AddFieldToContentTypeById(SiteClassificationContentType.SITEINFORMATION_CT_ID,
                    SiteClassificationFields.FLD_VALUE_ID.ToString(),
                    true);
            }
            var _list = ctx.Web.Lists.Add(_newList);
            _list.Hidden = true;
            _list.ContentTypesEnabled = true;
            _list.Update();
            ctx.Web.AddContentTypeToListById(SiteClassificationList.SiteClassificationListTitle, SiteClassificationContentType.SITEINFORMATION_CT_ID, true);
            this.CreateCustomPropertiesInList(_list);
            ctx.ExecuteQuery();
            this.RemoveFromQuickLaunch(ctx, SiteClassificationList.SiteClassificationListTitle);

        }

        private void CreateCustomPropertiesInList(List list)
        {
            IList<string> _listItemsToAdd = new List<string>();
            _listItemsToAdd.Add(SiteClassificationKeys.AudienceReachKey);
            _listItemsToAdd.Add(SiteClassificationKeys.BusinessImpactKey);

           foreach(var _itemToAdd in _listItemsToAdd)
           {
               ListItemCreationInformation _item = new ListItemCreationInformation();
               ListItem _record = list.AddItem(_item);
               _record[SiteClassificationFields.FLD_KEY_INTERNAL_NAME] = _itemToAdd;
               _record.Update();
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
        /// Used to key/value custom properities
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetCustomProfileProperties(ClientContext ctx)
        {
            CamlQuery _query = new CamlQuery();
            var _siteInfoList = ctx.Web.Lists.GetByTitle(SiteClassificationList.SiteClassificationListTitle);
            ListItemCollection _items = _siteInfoList.GetItems(_query);

            ctx.Load(_items,
                 eachItem => eachItem.Include(
                 item => item,
                 item => item[SiteClassificationFields.FLD_KEY_INTERNAL_NAME],
                 item => item[SiteClassificationFields.FLD_VALUE_INTERNAL_NAME]));
            ctx.ExecuteQuery();

            Dictionary<string, string> _siteInfo = new Dictionary<string, string>();

            if (_items.Count > 0)
            {
                foreach (ListItem _item in _items)
                {
                    _siteInfo.Add(BaseSet(_item, SiteClassificationFields.FLD_KEY_INTERNAL_NAME), BaseSet(_item, SiteClassificationFields.FLD_VALUE_INTERNAL_NAME));
                }
            }
            return _siteInfo;
        }
        #endregion

        public SiteProfile GetSiteProfile(ClientContext ctx)
        {
            var _siteProfile = new SiteProfile()
            {
              CustomProperties = this.GetCustomProfileProperties(ctx),
              ExpirationDate = ctx.Web.GetSiteExpirationDate()
            
            };
            
            SitePolicyEntity _sitePolicy = ctx.Web.GetAppliedSitePolicy();
            if(_sitePolicy != null)
            {
                _siteProfile.SitePolicy = _sitePolicy.Name;  
            }
  
            return _siteProfile;
        }
        public void SaveSiteProperties(ClientContext ctx, SiteProfile profile)
        {
            var _customProperties = profile.CustomProperties;

            ctx.Web.ApplySitePolicy(profile.SitePolicy);

            CamlQuery _query = new CamlQuery();
            var _siteInfoList = ctx.Web.Lists.GetByTitle(SiteClassificationList.SiteClassificationListTitle);
            ListItemCollection _items = _siteInfoList.GetItems(_query);

            ctx.Load(_items,
                 eachItem => eachItem.Include(
                 item => item,
                 item => item[SiteClassificationFields.FLD_KEY_INTERNAL_NAME],
                 item => item[SiteClassificationFields.FLD_VALUE_INTERNAL_NAME]));
            ctx.ExecuteQuery();

            if (_items.Count > 0)
            {
                foreach(var _item in _items)
                {
                    this.SetListItemValue(_siteInfoList, _item, _customProperties);
                }
                _siteInfoList.Update();
                ctx.ExecuteQuery();
            }
          
        }

        private void SetListItemValue(List list, ListItem listItem, Dictionary<string, string> updatedValues)
        {
            string _key = listItem[SiteClassificationFields.FLD_KEY_INTERNAL_NAME].ToString();

            switch(_key)
            {
                case SiteClassificationKeys.AudienceReachKey:
                    if(updatedValues.ContainsKey(SiteClassificationKeys.AudienceReachKey))
                    {
                        listItem[SiteClassificationFields.FLD_VALUE_INTERNAL_NAME] = updatedValues[SiteClassificationKeys.AudienceReachKey];
                        listItem.Update();
                    }
                    break;
                case SiteClassificationKeys.BusinessImpactKey:
                    if (updatedValues.ContainsKey(SiteClassificationKeys.BusinessImpactKey))
                    {
                        listItem[SiteClassificationFields.FLD_VALUE_INTERNAL_NAME] = updatedValues[SiteClassificationKeys.BusinessImpactKey];
                        listItem.Update();
                    }
                    break;
                default:
                    break;
            }



        }
    }
}

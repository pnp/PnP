using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorporateEvents.SharePointWeb.Models {
    /// <summary>
    /// Base class for all SP model objects
    /// </summary>
    public abstract class BaseListItem {
        internal const string TITLE = "Title";
        internal const string CREATED = "Created";
        internal const string MODIFIED = "Modified";
        static bool _fieldsRetrieved = false;
        static Dictionary<string, Field> _fields = new Dictionary<string, Field>();

        public BaseListItem() { IsNew = true; }
        public BaseListItem(ListItem item) : this() {
            ListItem = item;
            IsNew = false;
            // call initialize in subclasses here
        }

        // properties
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        protected ListItem ListItem { get; set; }
        protected bool IsNew { get; set; }

        // Abstract properties and methods
        protected abstract string ListTitle { get; }
        protected abstract string ContentTypeName { get; }
        protected abstract string[] FieldInternalNames { get; }
        protected abstract void SetProperties(ListItem item);
        protected abstract void ReadProperties(ListItem item);

        /// <summary>
        /// Saves the list item and sets its associated content type pending executeQuery.
        /// </summary>
        /// <param name="context">ClientContext object to enable saving a new list item.</param>
        /// <param name="executeQuery">Calls ExecuteQuery if true. Enables batching many items if false.</param>
        public void Save(ClientContext context, bool executeQuery = true) {
            var list = context.Web.GetListByTitle(ListTitle);
            if (!IsNew && Id > 0){
                ListItem = list.GetItemById(Id);
            }
            else{
                var listItemCreationInfo = new ListItemCreationInformation();
                ListItem = list.AddItem(listItemCreationInfo);
            }
            context.Load(ListItem);
            context.ExecuteQuery();

            // ensure that the fields have been added to the list
            EnsureFieldsRetrieved(ListItem);

            BaseSet(ListItem, TITLE, Title);
            // use if you want to override the created/modified date
            //BaseSet(ListItem, CREATED, Created);
            //BaseSet(ListItem, MODIFIED, Modified);

            if (!string.IsNullOrEmpty(ContentTypeName) && list.ContentTypeExistsById(ContentTypeName)) {
                var contentType = list.GetContentTypeByName(ContentTypeName);
                BaseSet(ListItem, "ContentTypeId", contentType.Id.StringValue);
            }

            // set the properties on the list item
            SetProperties(ListItem);

            ListItem.Update();

            if (executeQuery) {
                context.ExecuteQuery();
                ListItem.RefreshLoad();
                UpdateBaseProperties(ListItem);
                ReadProperties(ListItem);
            }
        }

        private void UpdateBaseProperties(ListItem item) {
            Id = item.Id;
            Title = (string)item[TITLE];
            Created = (DateTime)item[CREATED];
            Modified = (DateTime)item[MODIFIED];
        }

        /// <summary>
        /// Ensures that fields are retrieved for the item.
        /// Internal static _fields object is used to store the fields to be able to use the fields and determine their types.
        /// </summary>
        /// <param name="item">Input list item.</param>
        private void EnsureFieldsRetrieved(ListItem item) {
            if (_fieldsRetrieved)
                return;

            item.Context.Load(item.ParentList);
            item.Context.ExecuteQuery();
            _fields = item.ParentList.GetFields(FieldInternalNames).ToDictionary(f => f.StaticName.ToLowerInvariant());
            _fieldsRetrieved = true;
        }

        /// <summary>
        /// Initializes the object by setting all properties via the Read/SetProperties methods.
        /// </summary>
        /// <param name="item">Input list item.</param>
        protected void Initialize(ListItem item) {
            if (item != null && !item.ServerObjectIsNull.HasValue)
                throw new ArgumentNullException("item");

            UpdateBaseProperties(item);
            EnsureFieldsRetrieved(item);
            ReadProperties(item);
        }

        /// <summary>
        /// Encapsulates setting an item by internal name.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="internalName"></param>
        /// <param name="value"></param>
        protected void BaseSet(ListItem item, string internalName, object value) {
            //var field = _fields[internalName.ToLowerInvariant()];
            item[internalName] = value;
        }

        /// <summary>
        /// Encapsulates setting an item by internal name.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="internalName"></param>
        /// <param name="value"></param>
        protected void BaseSetTaxonomyField(ListItem item, string internalName, string label, Guid termId) {
            var field = _fields[internalName.ToLowerInvariant()];
            item.SetTaxonomyFieldValue(field.Id, label, termId);
        }

        protected T BaseGet<T>(ListItem item, string internalName){
            var field = _fields[internalName.ToLowerInvariant()];
            var value = item[field.InternalName];
            return (T)value;
        }

        protected T BaseGetEnum<T>(ListItem item, string internalName, T defaultValue) where T : struct {
            var valueString = BaseGet<string>(item, internalName);

            if (string.IsNullOrEmpty(valueString))
                return defaultValue;

            return valueString.ToEnum<T>();
        }

        protected T BaseGetEnum<T>(ListItem item, string internalName) where T : struct {
            return BaseGetEnum<T>(item, internalName, default(T));
        }
    }
}
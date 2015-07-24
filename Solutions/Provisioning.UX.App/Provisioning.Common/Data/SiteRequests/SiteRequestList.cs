using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.SiteRequests
{
    public static class SiteRequestList
    {
        const string FIELD_DESCRIPTION = "Description";
        const string DEFAULT_CTYPE_GROUP = "Site Provisioning Content Types";
        public static readonly string TITLE = "Site Requests";
        public static readonly string LISTURL = "Lists/SiteRequests";
        public static readonly string DESCRIPTION = "Repository for new Site Collection Requests";
        internal const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" DisplayName=""{2}"" ID=""{3}"" Group=""{4}"" {5}/>";
     
        public static readonly string CONTENTTYPE_NAME = "Site Request";
        public static readonly string CONTENTTYPE_DESCRIPTION = "Used to store site requests.";
        public static readonly string CONTENTTYPE_ID = "0x01008AFD1A3CDDC04ECAAED455326AC126FC";
       

        public static List CreateSharePointRepositoryList(Web web, string title, string description, string url)
        {
            List _requestList =  web.GetListByTitle(title);

            if(_requestList == null) //List Doesnt Existing
            {
                var _listCreation = new ListCreationInformation()
                {
                    Title = title,
                    TemplateType = (int)ListTemplateType.GenericList,
                    Description = description,
                    Url = url
                };
                _requestList = web.Lists.Add(_listCreation);
                web.Context.Load(_requestList);
                web.Context.ExecuteQuery();
            }

            var _fields = CreateListFields(web);

            var _contentID = CreateContentType(web, SiteRequestList.CONTENTTYPE_NAME,
                    SiteRequestList.CONTENTTYPE_DESCRIPTION,
                    SiteRequestList.DEFAULT_CTYPE_GROUP,
                    SiteRequestList.CONTENTTYPE_ID);

            //add fields to CT
            BindFieldsToContentType(web, SiteRequestList.CONTENTTYPE_ID, _fields);
            AddContentTypeToList(web, SiteRequestList.CONTENTTYPE_ID, SiteRequestList.TITLE, _fields);
            return _requestList;
        }

        static IEnumerable<Field> CreateListFields(Web web)
        {
            var fields = new List<Field>();
            var fieldsXml = new Dictionary<string, string>();

            fieldsXml.Add(SiteRequestFields.URL_NAME,
                 FormatField(
                 SiteRequestFields.URL_ID,
                 SiteRequestFields.URL_NAME,
                 SiteRequestFields.URL_TYPE,
                 SiteRequestFields.URL_DISPLAYNAME,
                 SiteRequestFields.URL_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.URL_ATTRIB));

            fieldsXml.Add(SiteRequestFields.DESCRIPTION_NAME,
                 FormatField(
                 SiteRequestFields.DESCRIPTION_ID,
                 SiteRequestFields.DESCRIPTION_NAME,
                 SiteRequestFields.DESCRIPTION_TYPE,
                 SiteRequestFields.DESCRIPTION_DISPLAYNAME,
                 SiteRequestFields.DESCRIPTION_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.DESCRIPTION_ATTRIB));


            fieldsXml.Add(SiteRequestFields.ONPREM_REQUEST_NAME,
                 FormatField(
                 SiteRequestFields.ONPREM_REQUEST_ID,
                 SiteRequestFields.ONPREM_REQUEST_NAME,
                 SiteRequestFields.ONPREM_REQEUST_TYPE,
                 SiteRequestFields.ONPREM_REQUEST_DISPLAYNAME,
                 SiteRequestFields.ONPREM_REQUEST_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.ONPREM_REQUEST_ATTRIB));

            fieldsXml.Add(SiteRequestFields.PROPS_NAME,
                 FormatField(
                 SiteRequestFields.PROPS_ID,
                 SiteRequestFields.PROPS_NAME,
                 SiteRequestFields.PROPS_TYPE,
                 SiteRequestFields.PROPS_DISPLAYNAME,
                 SiteRequestFields.PROPS_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.PROPS_ATTRIB));

            fieldsXml.Add(SiteRequestFields.BC_NAME,
                 FormatField(
                 SiteRequestFields.BC_ID,
                 SiteRequestFields.BC_NAME,
                 SiteRequestFields.BC_TYPE,
                 SiteRequestFields.BC_DISPLAYNAME,
                 SiteRequestFields.BC_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.BC_ATTRIB));

           fieldsXml.Add(SiteRequestFields.OWNER_NAME,
                  FormatField(
                  SiteRequestFields.OWNER_ID,
                  SiteRequestFields.OWNER_NAME,
                  SiteRequestFields.OWNER_TYPE,
                  SiteRequestFields.OWNER_DISPLAYNAME,
                  SiteRequestFields.OWNER_DESC,
                  SiteRequestFields.DEFAULT_FIELD_GROUP,
                  SiteRequestFields.OWNER_ATTRIB));
  
            fieldsXml.Add(SiteRequestFields.ADD_ADMINS_NAME,
                FormatField(SiteRequestFields.ADD_ADMINS_ID,
                SiteRequestFields.ADD_ADMINS_NAME,
                SiteRequestFields.ADD_ADMINS_TYPE,
                SiteRequestFields.ADD_ADMINS_DISPLAYNAME,
                SiteRequestFields.ADD_ADMINS_DESC,
                SiteRequestFields.DEFAULT_FIELD_GROUP,
                SiteRequestFields.ADD_ADMINS_ATTRIB));

            fieldsXml.Add(SiteRequestFields.POLICY_NAME,
                  FormatField(
                  SiteRequestFields.POLICY_ID,
                  SiteRequestFields.POLICY_NAME,
                  SiteRequestFields.POLICY_TYPE,
                  SiteRequestFields.POLICY_DISPLAYNAME,
                  SiteRequestFields.POLICY_DESC,
                  SiteRequestFields.DEFAULT_FIELD_GROUP,
                  SiteRequestFields.POLICY_ATTRIB));

            fieldsXml.Add(SiteRequestFields.TEMPLATE_NAME,
                   FormatField(
                   SiteRequestFields.TEMPLATE_ID,
                   SiteRequestFields.TEMPLATE_NAME,
                   SiteRequestFields.TEMPLATE_TYPE,
                   SiteRequestFields.TEMPLATE_DISPLAYNAME,
                   SiteRequestFields.TEMPLATED_DESC,
                   SiteRequestFields.DEFAULT_FIELD_GROUP,
                   SiteRequestFields.TEMPLATE_ATTRIB));

            fieldsXml.Add(SiteRequestFields.PROVISIONING_STATUS_NAME,
                  FormatField(
                  SiteRequestFields.PROVISIONING_STATUS_ID,
                  SiteRequestFields.PROVISIONING_STATUS_NAME,
                  SiteRequestFields.PROVISIONING_STATUS_TYPE,
                  SiteRequestFields.PROVISIONING_STATUS_DISPLAYNAME,
                  SiteRequestFields.PROVISIONING_STATUS_DESC,
                  SiteRequestFields.DEFAULT_FIELD_GROUP,
                  SiteRequestFields.PROVISIONING_STATUS_ATTRIB));

            fieldsXml.Add(SiteRequestFields.LCID_NAME,
                  FormatField(
                  SiteRequestFields.LCID_ID,
                  SiteRequestFields.LCID_NAME,
                  SiteRequestFields.LCID_TYPE,
                  SiteRequestFields.LCID_DISPLAYNAME,
                  SiteRequestFields.LCID_DESC,
                  SiteRequestFields.DEFAULT_FIELD_GROUP,
                  SiteRequestFields.LCID_ATTRIB));

            fieldsXml.Add(SiteRequestFields.TIMEZONE_NAME,
                 FormatField(
                 SiteRequestFields.TIMEZONE_ID,
                 SiteRequestFields.TIMEZONE_NAME,
                 SiteRequestFields.TIMEZONE_TYPE,
                 SiteRequestFields.TIMEZONE_DISPLAYNAME,
                 SiteRequestFields.TIMEZONE_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.TIMEZONE_ATTRIB));

            fieldsXml.Add(SiteRequestFields.APPROVEDDATE_NAME,
                  FormatField(
                  SiteRequestFields.APPROVEDATE_ID,
                  SiteRequestFields.APPROVEDDATE_NAME,
                  SiteRequestFields.APPROVEDATE_TYPE,
                  SiteRequestFields.APPROVEDDATE_DISPLAYNAME,
                  SiteRequestFields.APPROVEDATE_DESC,
                  SiteRequestFields.DEFAULT_FIELD_GROUP,
                  SiteRequestFields.APPROVEDATE_ATTRIB));

            fieldsXml.Add(SiteRequestFields.STATUSMESSAGE_NAME,
                 FormatField(
                 SiteRequestFields.STATUSMESSAGE_ID,
                 SiteRequestFields.STATUSMESSAGE_NAME,
                 SiteRequestFields.STATUSMESSAGE_TYPE,
                 SiteRequestFields.STATUSMESSAGE_DISPLAYNAME,
                 SiteRequestFields.STATUSMESSAGE_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.STATUSMESSAGE_ATTRIB));

            fieldsXml.Add(SiteRequestFields.EXTERNALSHARING_NAME,
                 FormatField(
                 SiteRequestFields.EXTERNALSHARING_ID,
                 SiteRequestFields.EXTERNALSHARING_NAME,
                 SiteRequestFields.EXTERNALSHARING_TYPE,
                 SiteRequestFields.EXTERNALSHARING_DISPLAYNAME,
                 SiteRequestFields.EXTERNALSHARING_DESC,
                 SiteRequestFields.DEFAULT_FIELD_GROUP,
                 SiteRequestFields.EXTERNALSHARING_ATTRIB));

            TryCreateFields(web, fields, fieldsXml);
            return fields;
        }

        static void TryCreateFields(Web web, List<Field> fields, Dictionary<string, string> fieldsXml)
        {
            web.Context.Load(web.Fields);
            web.Context.ExecuteQuery();

            foreach (var key in fieldsXml.Keys)
            {
                var field = web.Fields.GetFieldByName<Field>(key);
                if (field == null)
                {
                    field = web.Fields.AddFieldAsXml(fieldsXml[key], true, AddFieldOptions.AddFieldInternalNameHint);
                    field.StaticName = key;
                }
                fields.Add(field);
            }
            web.Context.ExecuteQuery();

            foreach (var field in fields)
            {
                web.Context.Load(field);
            }
            web.Context.ExecuteQuery();
        }
        static string CreateContentType(Web web, string name, string description, string group, string id) 
        {
            if (web.ContentTypeExistsById(id))
                return id;

            // add the description
            var ctypeCreationInfo = new ContentTypeCreationInformation()
            {
                Id = id,
                Description = description,
                Group = group,
                Name = name
            };
            web.ContentTypes.Add(ctypeCreationInfo);
            web.Context.ExecuteQuery();
            return id;
        }
        private static void BindFieldsToContentType(Web web, string contentTypeId, IEnumerable<Field> fields)
        {
            var contentType = web.GetContentTypeById(contentTypeId);

            var descriptionField = web.Fields.GetByInternalNameOrTitle(FIELD_DESCRIPTION);
            web.Context.Load(web.Fields);
            web.Context.Load(descriptionField);
            web.Context.Load(contentType.FieldLinks);
            web.Context.ExecuteQuery();

            // add the description field
            ((List<Field>)fields).Insert(0, descriptionField);

            // add any fields which have not be included
            var missingFields = fields.Except(from f in fields
                                              join fl in contentType.FieldLinks on f.StaticName equals fl.Name
                                              select f).ToList();

            foreach (var field in missingFields)
            {
                var fieldLinkCreationInfo = new FieldLinkCreationInformation()
                {
                    Field = field
                };
                contentType.FieldLinks.Add(fieldLinkCreationInfo);
            }
            contentType.Update(false);
            contentType.Context.ExecuteQuery();
        }
        private static void AddContentTypeToList(Web web, string contentTypeId, string listName, IEnumerable<Field> fields)
        {
            //Debug.WriteLine("CType: {0}, List: {1}", contentTypeId, listName);
            var list = web.GetListByTitle(listName);

            if (!list.ContentTypeExistsById(contentTypeId))
                web.AddContentTypeToListById(listName, contentTypeId);

            list.ContentTypesEnabled = true;
            list.Update();
            list.RefreshLoad();

            var contentType = web.ContentTypes.GetById(contentTypeId);
            var defaultView = list.DefaultView;
            web.Context.Load(contentType);
            web.Context.Load(defaultView);
            web.Context.Load(defaultView.ViewFields);
            web.Context.ExecuteQuery();

            foreach (var field in fields) {
                // add the fields to the default view
                if (field.FieldTypeKind != FieldType.Note &&
                    !defaultView.ViewFields.Contains(field.InternalName)) {
                    defaultView.ViewFields.Add(field.InternalName);
                    //Debug.WriteLine("  added to default view");
                }

                defaultView.Update();
            }
            web.Context.ExecuteQuery();
            // Item content type
            DeleteContentTypeFromList(web, list, "Item");
        }
        private static void DeleteContentTypeFromList(Web web, List list, string contentTypeName)
        {
            ContentType ct = list.GetContentTypeByName(contentTypeName);
            if (ct != null)
            {
                ct.DeleteObject();
                web.Context.ExecuteQuery();
            }
        }
        static string FormatField(Guid fieldId, string internalName, FieldType fieldType, string displayName, string description, string groupName, string attributes)
        {
            if (attributes == null)
                attributes = string.Empty;

            description = description.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
            attributes += string.Format(" Description=\"{0}\"", description);
            return FormatFieldXml(fieldId, internalName, fieldType.ToString(), displayName, groupName, attributes);
        }
        static string FormatFieldXml(Guid id, string internalName, string fieldType, string displayName, string group, string additionalXmlAttributes)
        {
            var newFieldCAML = string.Format(FIELD_XML_FORMAT, fieldType, internalName, displayName, id, group, additionalXmlAttributes);
            return newFieldCAML;
        }
    }
}

using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SharePoint.Client
{
    public static class FieldAndContentTypeExtensions
    {
        const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" DisplayName=""{2}"" ID=""{3}"" Group=""{4}"" {5}/>";
        /// <summary>
        /// Create field to web remotely
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Guid for the new field.</param>
        /// <param name="internalName">Internal name of the field</param>
        /// <param name="fieldType">Field type to be created.</param>
        /// <param name="displayName">The display name of hte field</param>
        /// <param name="group">The field group name</param>
        /// <returns>The newly created field or existing field.</returns>
        public static Field CreateField(this Web web, Guid id, string internalName, FieldType fieldType, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            return CreateField(web, id, internalName, fieldType.ToString(), displayName, group, additionalXmlAttributes, executeQuery);
        }

        /// <summary>
        /// Create field to web remotely
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Guid for the new field.</param>
        /// <param name="internalName">Internal name of the field</param>
        /// <param name="fieldType">Field type to be created.</param>
        /// <param name="displayName">The display name of hte field</param>
        /// <param name="group">The field group name</param>
        /// <returns>The newly created field or existing field.</returns>
        public static Field CreateField(this Web web, Guid id, string internalName, string fieldType, string displayName, string group, string additionalXmlAttributes = "", bool executeQuery = true)
        {
            FieldCollection fields = web.Fields;
            web.Context.Load(fields, fc => fc.Include(f => f.Id, f => f.InternalName));
            web.Context.ExecuteQuery();

            var field = fields.FirstOrDefault(f => f.Id == id || f.InternalName == internalName);

            if (field != null)
                throw new ArgumentException("id", "Field already exists");

            string newFieldCAML = string.Format(FIELD_XML_FORMAT, fieldType, internalName, displayName, id, group, additionalXmlAttributes);
            LoggingUtility.LogInformation("New Field as XML: " + newFieldCAML, EventCategory.FieldsAndContentTypes);
            field = fields.AddFieldAsXml(newFieldCAML, false, AddFieldOptions.AddFieldInternalNameHint);
            web.Update();

            if (executeQuery)
                web.Context.ExecuteQuery();

            return field;
        }

        /// <summary>
        /// Create field to web remotely
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="fieldAsXml">The XML declaration of SiteColumn definition</param>
        /// <returns>The newly created field or existing field.</returns>
        public static Field CreateField(this Web web, string fieldAsXml, bool executeQuery = true)
        {
            FieldCollection fields = web.Fields;
            web.Context.Load(fields);
            web.Context.ExecuteQuery();

            Field field = fields.AddFieldAsXml(fieldAsXml, false, AddFieldOptions.AddFieldInternalNameHint);
            web.Update();

            if (executeQuery)
                web.Context.ExecuteQuery();

            return field;
        }

        /// <summary>
        /// Can be used to create taxonomy field remotely to web. Associated to group and term set in the GetDefaultSiteCollectionTermStore 
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Unique Id for the taxonomy field</param>
        /// <param name="internalName">Internal Name of the field</param>
        /// <param name="displayName">Display name</param>
        /// <param name="group">Site column group</param>
        /// <param name="mmsGroupName">Taxonomy group </param>
        /// <param name="mmsTermSetName">Term set name</param>
        /// <returns>New taxonomy field</returns>
        public static Field CreateTaxonomyField(this Web web, Guid id, string internalName, string displayName, string group, string mmsGroupName, string mmsTermSetName)
        {
            // create a new MMS field
            var mmsField = web.CreateField(id, internalName, "TaxonomyFieldType", displayName, group, "ShowField=\"Term1033\"");
            web.WireUpTaxonomyField(mmsField.Id, mmsGroupName, mmsTermSetName);
            mmsField.Update();
            web.Context.ExecuteQuery();

            return mmsField;
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="field">Field to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        public static void WireUpTaxonomyField(this Web web, Field field, string mmsGroupName, string mmsTermSetName)
        {
            TermStore termStore = GetDefaultTermStore(web);

            if (termStore == null)
                throw new NullReferenceException("The default term store is not available.");

            if (string.IsNullOrEmpty(mmsTermSetName))
                throw new ArgumentNullException("mmsTermSetName", "The MMS term set is not specified.");

            // get the term group and term set
            TermGroup termGroup = termStore.Groups.GetByName(mmsGroupName);
            TermSet termSet = termGroup.TermSets.GetByName(mmsTermSetName);
            web.Context.Load(termStore);
            web.Context.Load(termSet);
            web.Context.ExecuteQuery();

            // set the SSP ID and Term Set ID on the taxonomy field
            var taxField = web.Context.CastTo<TaxonomyField>(field);
            taxField.SspId = termStore.Id;
            taxField.TermSetId = termSet.Id;
            taxField.Update();
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Wires up MMS field to the specified term set.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">Field ID to be wired up</param>
        /// <param name="mmsGroupName">Taxonomy group</param>
        /// <param name="mmsTermSetName">Term set name</param>
        public static void WireUpTaxonomyField(this Web web, Guid id, string mmsGroupName, string mmsTermSetName)
        {
            var field = web.Fields.GetById(id);
            web.Context.Load(field);
            web.WireUpTaxonomyField(field, mmsGroupName, mmsTermSetName);
        }


        /// <summary>
        /// Creates fields from feature elment xml file schema. XML file can contain one or many field definitions created using classic feature framework structure.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlFilePath">Absolute path to the xml location</param>
        /// <param name="skipFieldIfExists">If set to true and field exists, field is skipped. If set to false, exception is raised.</param>
        public static void CreateFieldsFromXMLFile(this Web web, string xmlFilePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);

            // Perform the action field creation
            CreateFieldsFromXML(web, xd);
        }

        /// <summary>
        /// Creates fields from feature elment xml file schema. XML file can contain one or many field definitions created using classic feature framework structure.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xmlFilePath">XML structure in string format</param>
        /// <param name="skipFieldIfExists">If set to true and field exists, field is skipped. If set to false, exception is raised.</param>
        public static void CreateFieldsFromXMLString(this Web web, string xmlStructure)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlStructure);

            // Perform the action field creation
            CreateFieldsFromXML(web, xd);
        }

        /// <summary>
        /// Creates field from xml structure which follows the classic feture framework structure
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site. Site columns should be created to root site.</param>
        /// <param name="xd">Actual XML document</param>
        /// <param name="skipFieldIfExists">If set to true and field exists, field is skipped. If set to false, exception is raised.</param>
        public static void CreateFieldsFromXML(this Web web, XmlDocument xmlDoc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("namespace", "http://schemas.microsoft.com/sharepoint/");

            XmlDocument xdocField = null;
            XmlNodeList fields = xmlDoc.SelectNodes("//namespace:Field", nsmgr);
            int count = fields.Count;
            foreach (XmlNode field in fields)
            {
                xdocField = new XmlDocument();
                xdocField.LoadXml(field.OuterXml);
                string fieldName = xdocField.SelectSingleNode("//namespace:Field", nsmgr).Attributes["Name"].Value;

                // IF field already existed, let's move on
                if (web.FieldExistsByName(fieldName))
                {
                    continue;
                }

                web.CreateField(field.OuterXml);
            }
        }


        public static void CreateContentTypeFromXMLFile(this Web web, string absolutePathToFile)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(absolutePathToFile);
            CreateContentTypeFromXML(web, xd);
        }

        public static void CreateContentTypeFromXMLString(this Web web, string xmlStructure)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlStructure);
            CreateContentTypeFromXML(web, xd);
        }

        public static void CreateContentTypeFromXML(this Web web, XmlDocument xmlDoc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("namespace", "http://schemas.microsoft.com/sharepoint/");

            XmlNodeList fields = xmlDoc.SelectNodes("//namespace:ContentType", nsmgr);
            int count = fields.Count;
            foreach (XmlNode ct in fields)
            {
                string ctid = ct.Attributes["ID"].Value;
                string name = ct.Attributes["Name"].Value;
                string description = ct.Attributes["Description"].Value;
                string group = ct.Attributes["Group"].Value;

                if (web.ContentTypeExistsByName(name))
                    continue;

                //Create CT
                web.CreateContentType(name, description, ctid, group);

                //Add fields to content type 
                XmlNodeList fieldRefs = ct.SelectNodes(".//namespace:FieldRef", nsmgr);
                XmlAttribute attr = null;
                foreach (XmlNode fr in fieldRefs)
                {
                    bool required = false;
                    bool hidden = false;
                    string frid = fr.Attributes["ID"].Value;
                    string frName = fr.Attributes["Name"].Value;
                    attr = fr.Attributes["Required"];
                    if (attr != null)
                    {
                        bool.TryParse(attr.Value, out required);
                    }
                    attr = fr.Attributes["Hidden"];
                    if (attr != null)
                    {
                        bool.TryParse(attr.Value, out hidden);
                    }
                    web.AddFieldToContentTypeById(ctid, frid, required, hidden);
                }
            }
        }

        public static void BindFieldsToTermSetsFromXMLFile(this Web web, string absolutePathToFile)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(absolutePathToFile);
        }

        public static void BindFieldsToTermSetsFromXMLString(this Web web, string xmlStructure)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlStructure);
        }

        public static void BindFieldsToTermSetsFromXML(this Web web, XmlDocument xmlDoc)
        {
            XmlNodeList fields = xmlDoc.SelectNodes("//MMSField");
            foreach (XmlNode mmsfield in fields)
            {
                string fieldGuid = mmsfield.Attributes["FieldGuid"].Value;
                string MMSGroupName = mmsfield.Attributes["MMSGroupName"].Value;
                string TermSet = mmsfield.Attributes["TermSet"].Value;

                FieldAndContentTypeExtensions.WireUpTaxonomyField(web, new Guid(fieldGuid), MMSGroupName, TermSet);
            }
        }


        /// <summary>
        /// Private method used for resolving taxonomy term set for taxonomy field
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns></returns>
        private static TermStore GetDefaultTermStore(Web web)
        {
            TermStore termStore = null;
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(web.Context);
            web.Context.Load(taxonomySession,
                ts => ts.TermStores.Include(
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name
                        )
                    )
                );
            web.Context.ExecuteQuery();
            if (taxonomySession != null)
            {
                termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            }

            return termStore;
        }

        /// <summary>
        /// Create new content type to web
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="name">Name of the content type</param>
        /// <param name="id">Complete ID for the content type</param>
        /// <param name="group">Group for the content type</param>
        /// <returns></returns>
        public static ContentType CreateContentType(this Web web, string name, string id, string group)
        {
            // Load the current collection of content types
            return CreateContentType(web, name, string.Empty, id, group);
        }


        /// <summary>
        /// Create new content type to web
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="name">Name of the content type</param>
        /// <param name="description">Description for the content type</param>
        /// <param name="id">Complete ID for the content type</param>
        /// <param name="group">Group for the content type</param>
        /// <returns></returns>
        public static ContentType CreateContentType(this Web web, string name, string description, string id, string group)
        {
            // Load the current collection of content types
            ContentTypeCollection contentTypes = web.ContentTypes;
            web.Context.Load(contentTypes);
            web.Context.ExecuteQuery();
            ContentTypeCreationInformation newCt = new ContentTypeCreationInformation();

            // Set the properties for the content type
            newCt.Name = name;
            newCt.Id = id;
            newCt.Description = description;
            newCt.Group = group;
            ContentType myContentType = contentTypes.Add(newCt);
            web.Context.ExecuteQuery();

            //Return the content type object
            return myContentType;
        }

        /// <summary>
        /// Associates field to contennt type
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeID"></param>
        /// <param name="fieldID"></param>
        public static void AddFieldToContentTypeById(this Web web, string contentTypeID, string fieldID, bool required = false, bool hidden = false)
        {
            // Get content type
            ContentType ct = web.GetContentTypeById(contentTypeID);
            web.Context.Load(ct);
            web.Context.Load(ct.FieldLinks);
            web.Context.ExecuteQuery();

            // Get field
            Field fld = web.Fields.GetById(new Guid(fieldID));

            // Add field association to content type
            AddFieldToContentType(web, ct, fld, required, hidden);
        }

        /// <summary>
        /// Associates field to content type
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeName"></param>
        /// <param name="fieldID"></param>
        public static void AddFieldToContentTypeByName(this Web web, string contentTypeName, Guid fieldID, bool required = false, bool hidden = false)
        {
            // Get content type
            ContentType ct = web.GetContentTypeByName(contentTypeName);
            web.Context.Load(ct);
            web.Context.Load(ct.FieldLinks);
            web.Context.ExecuteQuery();

            // Get field
            Field fld = web.Fields.GetById(fieldID);

            // Add field association to content type
            AddFieldToContentType(web, ct, fld, required, hidden);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentType"></param>
        /// <param name="field"></param>
        /// <param name="required"></param>
        /// <param name="hidden"></param>
        public static void AddFieldToContentType(this Web web, ContentType contentType, Field field, bool required = false, bool hidden = false)
        {
            FieldLinkCreationInformation fldInfo = new FieldLinkCreationInformation();
            fldInfo.Field = field;
            contentType.FieldLinks.Add(fldInfo);
            contentType.Update(true);
            web.Context.ExecuteQuery();

            web.Context.Load(field);
            web.Context.ExecuteQuery();

            if (required || hidden)
            {
                //Update FieldLink
                FieldLink flink = contentType.FieldLinks.GetById(field.Id);
                flink.Required = required;
                flink.Hidden = hidden;
                contentType.Update(true);
                web.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Adds content type to list
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listTitle"></param>
        /// <param name="contentTypeID"></param>
        /// <param name="defaultContent"></param>
        public static void AddContentTypeToListById(this Web web, string listTitle, string contentTypeId, bool defaultContent = false)
        {
            // Get content type instance
            ContentType contentType = GetContentTypeById(web, contentTypeId);
            // Add content type to list
            AddContentTypeToList(web, listTitle, contentType, defaultContent);
        }

        /// <summary>
        /// Adds content type to list
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listTitle"></param>
        /// <param name="contentTypeID"></param>
        /// <param name="defaultContent"></param>
        public static void AddContentTypeToListByName(this Web web, string listTitle, string contentTypeName, bool defaultContent = false)
        {
            // Get content type instance
            ContentType contentType = GetContentTypeByName(web, contentTypeName);
            // Add content type to list
            AddContentTypeToList(web, listTitle, contentType, defaultContent);
        }

        /// <summary>
        /// Adds content type to list
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listTitle"></param>
        /// <param name="contentType"></param>
        /// <param name="defaultContent">If set true, content type is updated to be default content type for the list</param>
        public static void AddContentTypeToList(this Web web, string listTitle, ContentType contentType, bool defaultContent = false)
        {
            // Get list instances
            List list = web.GetListByTitle(listTitle);
            // Add content type to list
            AddContentTypeToList(list, contentType, defaultContent);
        }

        /// <summary>
        /// Add content type to list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentTypeID"></param>
        /// <param name="defaultContent"></param>
        public static void AddContentTypeToListById(this List list, string contentTypeID, bool defaultContent = false)
        {
            Web web = list.ParentWeb;
            ContentType contentType = GetContentTypeById(web, contentTypeID);
            AddContentTypeToList(list, contentType, defaultContent);
        }

        /// <summary>
        /// Add content type to list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentTypeName"></param>
        /// <param name="defaultContent"></param>
        public static void AddContentTypeToListByName(this List list, string contentTypeName, bool defaultContent = false)
        {
            Web web = list.ParentWeb;
            ContentType contentType = GetContentTypeByName(web, contentTypeName);
            AddContentTypeToList(list, contentType, defaultContent);
        }

        /// <summary>
        /// Add content type to list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentType"></param>
        /// <param name="defaultContent"></param>
        public static void AddContentTypeToList(this List list, ContentType contentType, bool defaultContent = false)
        {
            list.ContentTypesEnabled = true;
            list.Update();
            list.Context.ExecuteQuery();
            ContentTypeCollection contentTypes = list.ContentTypes;
            list.Context.Load(contentTypes);
            list.Context.ExecuteQuery();


            foreach (ContentType ct in contentTypes)
            {
                if (ct.Name.ToLowerInvariant() == contentType.Name.ToString().ToLowerInvariant())
                {
                    // Already there, abort
                    return;
                }
            }

            contentTypes.AddExistingContentType(contentType);
            list.Context.ExecuteQuery();
            //set the default contenttype
            if (defaultContent)
            {
                SetDefaultContentTypeToList(list, contentType);
            }
        }

        /// <summary>
        /// Set default content type to list
        /// </summary>
        /// <param name="web"></param>
        /// <param name="list"></param>
        /// <param name="contentTypeId"></param>
        public static void SetDefaultContentTypeToList(this Web web, List list, string contentTypeId)
        {
            SetDefaultContentTypeToList(list, contentTypeId);
        }

        /// <summary>
        /// Set default content type to list
        /// </summary>
        /// <param name="web"></param>
        /// <param name="list"></param>
        /// <param name="contentType"></param>
        public static void SetDefaultContentTypeToList(this Web web, List list, ContentType contentType)
        {
            SetDefaultContentTypeToList(list, contentType.Id.ToString());
        }

        /// <summary>
        /// Set default content type to list
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listTitle"></param>
        /// <param name="contentTypeId"></param>
        public static void SetDefaultContentTypeToList(this Web web, string listTitle, string contentTypeId)
        {
            // Get list instances
            List list = web.GetListByTitle(listTitle);
            web.Context.Load(list);
            web.Context.ExecuteQuery();
            // Add content type to list
            SetDefaultContentTypeToList(list, contentTypeId);
        }

        /// <summary>
        /// Set's default content type list. 
        /// </summary>
        /// <remarks>Notice. Currently removes other content types from the list. Known issue</remarks>
        /// <param name="web"></param>
        /// <param name="listTitle"></param>
        /// <param name="contentType"></param>
        public static void SetDefaultContentTypeToList(this Web web, string listTitle, ContentType contentType)
        {
            SetDefaultContentTypeToList(web, listTitle, contentType.Id.ToString());
        }

        /// <summary>
        /// Set's default content type list. 
        /// </summary>
        /// <remarks>Notice. Currently removes other content types from the list. Known issue</remarks>
        /// <param name="list"></param>
        /// <param name="contentTypeId"></param>
        public static void SetDefaultContentTypeToList(this List list, string contentTypeId)
        {
            ContentTypeCollection ctCol = list.ContentTypes;
            list.Context.Load(ctCol);
            list.Context.ExecuteQuery();
            IList<ContentTypeId> newOrder = new List<ContentTypeId>();
            foreach (ContentType ct in ctCol)
            {
                if (ct.StringId.ToLowerInvariant().StartsWith(contentTypeId.ToLowerInvariant()))
                {
                    newOrder.Add(ct.Id);
                }
            }
            list.RootFolder.UniqueContentTypeOrder = newOrder;
            list.RootFolder.Update();
            list.Update();
            list.Context.ExecuteQuery();
        }

        /// <summary>
        /// Set default content type to list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentType"></param>
        public static void SetDefaultContentTypeToList(this List list, ContentType contentType)
        {
            SetDefaultContentTypeToList(list, contentType.Id.ToString());
        }

        /// <summary>
        /// Set localized labels for content type
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeName">Name of the content type</param>
        /// <param name="cultureName"></param>
        /// <param name="nameResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForContentType(this Web web, string contentTypeName, string cultureName, string nameResource, string descriptionResource)
        {
            ContentType contentType = web.GetContentTypeByName(contentTypeName);
            contentType.SetLocalizationForContentType(cultureName, nameResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for content type
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentTypeId"></param>
        /// <param name="cultureName"></param>
        /// <param name="nameResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForContentType(this List list, string contentTypeId, string cultureName, string nameResource, string descriptionResource)
        {
            ContentTypeCollection contentTypes = list.ContentTypes;
            list.Context.Load(contentTypes);
            list.Context.ExecuteQuery();
            ContentType contentType = contentTypes.GetById(contentTypeId);
            list.Context.ExecuteQuery();
            contentType.SetLocalizationForContentType(cultureName, nameResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for content type
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="cultureName"></param>
        /// <param name="nameResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForContentType(this ContentType contentType, string cultureName, string nameResource, string descriptionResource)
        {
            if (contentType.IsObjectPropertyInstantiated("TitleResource"))
            {
                contentType.Context.Load(contentType);
                contentType.Context.ExecuteQuery();
            }
            // Set translations for the culture
            contentType.NameResource.SetValueForUICulture(cultureName, nameResource);
            contentType.DescriptionResource.SetValueForUICulture(cultureName, descriptionResource);
            contentType.Update(true);
            contentType.Context.ExecuteQuery();
        }

        /// <summary>
        /// Set localized labels for field
        /// </summary>
        /// <param name="web"></param>
        /// <param name="siteColumnId"></param>
        /// <param name="cultureName"></param>
        /// <param name="titleResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForField(this Web web, Guid siteColumnId, string cultureName, string titleResource, string descriptionResource)
        {
            FieldCollection fields = web.Fields;
            Field fld = fields.GetById(siteColumnId);
            SetLocalizationForField(fld, cultureName, titleResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for field
        /// </summary>
        /// <param name="web"></param>
        /// <param name="siteColumnName"></param>
        /// <param name="cultureName"></param>
        /// <param name="titleResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForField(this Web web, string siteColumnName, string cultureName, string titleResource, string descriptionResource)
        {
            FieldCollection fields = web.Fields;
            Field fld = fields.GetByInternalNameOrTitle(siteColumnName);
            SetLocalizationForField(fld, cultureName, titleResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for field
        /// </summary>
        /// <param name="web"></param>
        /// <param name="siteColumn"></param>
        /// <param name="cultureName"></param>
        /// <param name="titleResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForField(this Web web, Field siteColumn, string cultureName, string titleResource, string descriptionResource)
        {
            SetLocalizationForField(siteColumn, cultureName, titleResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for field
        /// </summary>
        /// <param name="list"></param>
        /// <param name="siteColumnId"></param>
        /// <param name="cultureName"></param>
        /// <param name="titleResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForField(this List list, Guid siteColumnId, string cultureName, string titleResource, string descriptionResource)
        {
            FieldCollection fields = list.Fields;
            Field fld = fields.GetById(siteColumnId);
            SetLocalizationForField(fld, cultureName, titleResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for field
        /// </summary>
        /// <param name="list"></param>
        /// <param name="siteColumnName"></param>
        /// <param name="cultureName"></param>
        /// <param name="titleResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForField(this List list, string siteColumnName, string cultureName, string titleResource, string descriptionResource)
        {
            FieldCollection fields = list.Fields;
            Field fld = fields.GetByInternalNameOrTitle(siteColumnName);
            SetLocalizationForField(fld, cultureName, titleResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for field
        /// </summary>
        /// <param name="list"></param>
        /// <param name="siteColumn"></param>
        /// <param name="cultureName"></param>
        /// <param name="titleResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForField(this List list, Field siteColumn, string cultureName, string titleResource, string descriptionResource)
        {
            SetLocalizationForField(siteColumn, cultureName, titleResource, descriptionResource);
        }

        /// <summary>
        /// Set localized labels for field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="cultureName"></param>
        /// <param name="titleResource"></param>
        /// <param name="descriptionResource"></param>
        public static void SetLocalizationForField(this Field field, string cultureName, string titleResource, string descriptionResource)
        {
            if (field.IsObjectPropertyInstantiated("TitleResource"))
            {
                field.Context.Load(field);
                field.Context.ExecuteQuery();
            }
            // Set translations for the culture
            field.TitleResource.SetValueForUICulture(cultureName, titleResource);
            field.DescriptionResource.SetValueForUICulture(cultureName, descriptionResource);
            field.UpdateAndPushChanges(true);
            field.Context.ExecuteQuery();
        }

        /// <summary>
        /// Does content type exists in the web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public static bool ContentTypeExistsById(this Web web, string contentTypeId)
        {
            ContentTypeCollection ctCol = web.ContentTypes;
            web.Context.Load(ctCol);
            web.Context.ExecuteQuery();
            foreach (var item in ctCol)
            {
                if (item.Id.StringValue.ToLowerInvariant().StartsWith(contentTypeId.ToLowerInvariant()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Does content type exists in the web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public static bool ContentTypeExistsByName(this Web web, string contentTypeName)
        {
            ContentTypeCollection ctCol = web.ContentTypes;
            IEnumerable<ContentType> results = web.Context.LoadQuery<ContentType>(ctCol.Where(item => item.Name == contentTypeName));
            web.Context.ExecuteQuery();
            ContentType ct = results.FirstOrDefault();
            if (ct != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Does content type exist in web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listTitle"></param>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public static bool ContentTypeExistsById(this Web web, string listTitle, string contentTypeId)
        {
            List list = web.GetListByTitle(listTitle);
            return ContentTypeExistsById(list, contentTypeId);
        }

        /// <summary>
        /// Does content type exist in list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public static bool ContentTypeExistsById(this List list, string contentTypeId)
        {
            if (!list.ContentTypesEnabled)
            {
                return false;
            }

            ContentTypeCollection ctCol = list.ContentTypes;
            list.Context.Load(ctCol);
            list.Context.ExecuteQuery();

            foreach (var item in ctCol)
            {
                if (item.Id.StringValue.ToLowerInvariant().StartsWith(contentTypeId.ToLowerInvariant()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Does content type exist in ewb
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listTitle"></param>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public static bool ContentTypeExistsByName(this Web web, string listTitle, string contentTypeName)
        {
            List list = web.GetListByTitle(listTitle);
            return ContentTypeExistsByName(list, contentTypeName);
        }

        /// <summary>
        /// Does cotnent type exist in list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public static bool ContentTypeExistsByName(this List list, string contentTypeName)
        {
            if (!list.ContentTypesEnabled)
            {
                return false;
            }

            ContentTypeCollection ctCol = list.ContentTypes;
            IEnumerable<ContentType> results = list.Context.LoadQuery<ContentType>(ctCol.Where(item => item.Name == contentTypeName));
            list.Context.ExecuteQuery();
            if (results.FirstOrDefault() != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns if the field is found
        /// </summary>
        /// <param name="web"></param>
        /// <param name="fieldId">Guid for the field ID</param>
        /// <returns>True or false dependign on the field existance</returns>
        public static bool FieldExistsById(this Web web, Guid fieldId)
        {
            FieldCollection fields = web.Fields;
            web.Context.Load(fields);
            web.Context.ExecuteQuery();
            foreach (var item in fields)
            {
                if (item.Id.ToString().ToLowerInvariant() == fieldId.ToString().ToLowerInvariant())
                {
                    return true;
                }
            }
            return false;
        }




        /// <summary>
        /// Returns if the field is found
        /// </summary>
        /// <param name="web"></param>
        /// <param name="fieldName">String for the field internal name to be used as query criteria</param>
        /// <returns></returns>
        public static bool FieldExistsByName(this Web web, string fieldName)
        {
            FieldCollection fields = web.Fields;
            IEnumerable<Field> results = web.Context.LoadQuery<Field>(fields.Where(item => item.InternalName == fieldName));
            web.Context.ExecuteQuery();
            if (results.FirstOrDefault() != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Does field exist in web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static bool FieldExistsById(this Web web, string fieldId)
        {
            return FieldExistsById(web, new Guid(fieldId));
        }

        /// <summary>
        /// Returns if the field is found
        /// </summary>
        /// <param name="web"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static bool FieldExistsById(this List list, Guid fieldId)
        {
            FieldCollection fields = list.Fields;
            list.Context.Load(fields);
            list.Context.ExecuteQuery();
            foreach (var item in fields)
            {
                if (item.Id.ToString().ToLowerInvariant() == fieldId.ToString().ToLowerInvariant())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns if the field is found, query based on the ID
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public static bool FieldExistsById(this List list, string fieldId)
        {
            return FieldExistsById(list, new Guid(fieldId));
        }

        /// <summary>
        /// Field exists in list by name
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool FieldExistsByName(this List list, string fieldName)
        {
            FieldCollection fields = list.Fields;
            IEnumerable<Field> results = list.Context.LoadQuery<Field>(fields.Where(item => item.InternalName == fieldName));
            list.Context.ExecuteQuery();
            if (results.FirstOrDefault() != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Field exists in content type
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeName"></param>
        /// <param name="fieldName">Name of the content type</param>
        /// <returns></returns>
        public static bool FieldExistsByNameInContentType(this Web web, string contentTypeName, string fieldName)
        {
            ContentType ct = GetContentTypeByName(web, contentTypeName);
            FieldCollection fields = ct.Fields;
            IEnumerable<Field> results = ct.Context.LoadQuery<Field>(fields.Where(item => item.InternalName == fieldName));
            ct.Context.ExecuteQuery();
            if (results.FirstOrDefault() != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return content type by name
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeName"></param>
        /// <returns>Conten type object or null if was not found</returns>
        public static ContentType GetContentTypeByName(this Web web, string contentTypeName)
        {
            ContentTypeCollection ctCol = web.ContentTypes;
            IEnumerable<ContentType> results = web.Context.LoadQuery<ContentType>(ctCol.Where(item => item.Name == contentTypeName));
            web.Context.ExecuteQuery();
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Return conten type by Id
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public static ContentType GetContentTypeById(this Web web, string contentTypeId)
        {
            ContentTypeCollection ctCol = web.ContentTypes;
            web.Context.Load(ctCol);
            web.Context.ExecuteQuery();
            foreach (var item in ctCol)
            {
                if (item.Id.StringValue.ToLowerInvariant() == contentTypeId.ToLowerInvariant())
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Return content type by name
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentTypeName"></param>
        /// <returns>Conten type object or null if was not found</returns>
        public static ContentType GetContentTypeByName(this List list, string contentTypeName)
        {
            ContentTypeCollection ctCol = list.ContentTypes;
            IEnumerable<ContentType> results = list.Context.LoadQuery<ContentType>(ctCol.Where(item => item.Name == contentTypeName));
            list.Context.ExecuteQuery();
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Return content type by Id
        /// </summary>
        /// <param name="web"></param>
        /// <param name="contentTypeId"></param>
        /// <returns></returns>
        public static ContentType GetContentTypeById(this List list, string contentTypeId)
        {
            ContentTypeCollection ctCol = list.ContentTypes;
            list.Context.Load(ctCol);
            list.Context.ExecuteQuery();
            foreach (var item in ctCol)
            {
                if (item.Id.StringValue.ToLowerInvariant() == contentTypeId.ToLowerInvariant())
                {
                    return item;
                }
            }
            return null;
        }
    }
}

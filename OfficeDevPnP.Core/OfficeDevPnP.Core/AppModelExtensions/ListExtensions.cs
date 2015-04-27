using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Utilities;
using Microsoft.SharePoint.Client.WebParts;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that provides generic list creation and manipulation methods
    /// </summary>
    public static partial class ListExtensions
    {
        #region Event Receivers

        /// <summary>
        /// Registers a remote event receiver
        /// </summary>
        /// <param name="list">The list to process</param>
        /// <param name="name">The name of the event receiver (needs to be unique among the event receivers registered on this list)</param>
        /// <param name="url">The URL of the remote WCF service that handles the event</param>
        /// <param name="eventReceiverType"></param>
        /// <param name="synchronization"></param>
        /// <param name="force">If True any event already registered with the same name will be removed first.</param>
        /// <returns>Returns an EventReceiverDefinition if succeeded. Returns null if failed.</returns>
        public static EventReceiverDefinition AddRemoteEventReceiver(this List list, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, bool force)
        {
            return list.AddRemoteEventReceiver(name, url, eventReceiverType, synchronization, 1000, force);
        }

        /// <summary>
        /// Registers a remote event receiver
        /// </summary>
        /// <param name="list">The list to process</param>
        /// <param name="name">The name of the event receiver (needs to be unique among the event receivers registered on this list)</param>
        /// <param name="url">The URL of the remote WCF service that handles the event</param>
        /// <param name="eventReceiverType"></param>
        /// <param name="synchronization"></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="force">If True any event already registered with the same name will be removed first.</param>
        /// <returns>Returns an EventReceiverDefinition if succeeded. Returns null if failed.</returns>
        public static EventReceiverDefinition AddRemoteEventReceiver(this List list, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, int sequenceNumber, bool force)
        {
            var query = from receiver
                     in list.EventReceivers
                        where receiver.ReceiverName == name
                        select receiver;
            var receivers = list.Context.LoadQuery(query);
            list.Context.ExecuteQueryRetry();

            var receiverExists = receivers.Any();
            if (receiverExists && force)
            {
                var receiver = receivers.FirstOrDefault();
                receiver.DeleteObject();
                list.Context.ExecuteQueryRetry();
                receiverExists = false;
            }
            EventReceiverDefinition def = null;

            if (!receiverExists)
            {
                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.EventType = eventReceiverType;
                receiver.ReceiverUrl = url;
                receiver.ReceiverName = name;
                receiver.SequenceNumber = sequenceNumber;
                receiver.Synchronization = synchronization;
                def = list.EventReceivers.Add(receiver);
                list.Context.Load(def);
                list.Context.ExecuteQueryRetry();
            }
            return def;
        }

        /// <summary>
        /// Returns an event receiver definition
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EventReceiverDefinition GetEventReceiverById(this List list, Guid id)
        {
            IEnumerable<EventReceiverDefinition> receivers = null;
            var query = from receiver
                        in list.EventReceivers
                        where receiver.ReceiverId == id
                        select receiver;

            receivers = list.Context.LoadQuery(query);
            list.Context.ExecuteQueryRetry();
            if (receivers.Any())
            {
                return receivers.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an event receiver definition
        /// </summary>
        /// <param name="list">The list to process</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EventReceiverDefinition GetEventReceiverByName(this List list, string name)
        {
            IEnumerable<EventReceiverDefinition> receivers = null;
            var query = from receiver
                        in list.EventReceivers
                        where receiver.ReceiverName == name
                        select receiver;

            receivers = list.Context.LoadQuery(query);
            list.Context.ExecuteQueryRetry();
            if (receivers.Any())
            {
                return receivers.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region List Properties
        /// <summary>
        /// Sets a key/value pair in the web property bag
        /// </summary>
        /// <param name="list">The list to process</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">Integer value for the property bag entry</param>
        public static void SetPropertyBagValue(this List list, string key, int value)
        {
            SetPropertyBagValueInternal(list, key, value);
        }


        /// <summary>
        /// Sets a key/value pair in the list property bag
        /// </summary>
        /// <param name="list">List that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">String value for the property bag entry</param>
        public static void SetPropertyBagValue(this List list, string key, string value)
        {
            SetPropertyBagValueInternal(list, key, value);
        }


        /// <summary>
        /// Sets a key/value pair in the list property bag
        /// </summary>
        /// <param name="list">List that will hold the property bag entry</param>
        /// <param name="key">Key for the property bag entry</param>
        /// <param name="value">Value for the property bag entry</param>
        private static void SetPropertyBagValueInternal(List list, string key, object value)
        {
            var props = list.RootFolder.Properties;
            list.Context.Load(props);
            list.Context.ExecuteQueryRetry();

            props[key] = value;
            list.Update();
            list.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Get int typed property bag value. If does not contain, returns default value.
        /// </summary>
        /// <param name="list">List to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <param name="defaultValue"></param>
        /// <returns>Value of the property bag entry as integer</returns>
        public static int? GetPropertyBagValueInt(this List list, string key, int defaultValue)
        {
            object value = GetPropertyBagValueInternal(list, key);
            if (value != null)
            {
                return (int)value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get string typed property bag value. If does not contain, returns given default value.
        /// </summary>
        /// <param name="list">List to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <param name="defaultValue"></param>
        /// <returns>Value of the property bag entry as string</returns>
        public static string GetPropertyBagValueString(this List list, string key, string defaultValue)
        {
            object value = GetPropertyBagValueInternal(list, key);
            if (value != null)
            {
                return (string)value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Type independent implementation of the property gettter.
        /// </summary>
        /// <param name="list">List to read the property bag value from</param>
        /// <param name="key">Key of the property bag entry to return</param>
        /// <returns>Value of the property bag entry</returns>
        private static object GetPropertyBagValueInternal(List list, string key)
        {
            var props = list.RootFolder.Properties;
            list.Context.Load(props);
            list.Context.ExecuteQueryRetry();
            if (props.FieldValues.ContainsKey(key))
            {
                return props.FieldValues[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the given property bag entry exists
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="key">Key of the property bag entry to check</param>
        /// <returns>True if the entry exists, false otherwise</returns>
        public static bool PropertyBagContainsKey(this List list, string key)
        {
            var props = list.RootFolder.Properties;
            list.Context.Load(props);
            list.Context.ExecuteQueryRetry();
            if (props.FieldValues.ContainsKey(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Removes a content type from a list/library by name
        /// </summary>
        /// <param name="list">The list</param>
        /// <param name="contentTypeName">The content type name to remove from the list</param>
        /// <exception cref="System.ArgumentException">Thrown when contentTypeName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">contentTypeName is null</exception>
        public static void RemoveContentTypeByName(this List list, string contentTypeName)
        {
            if (string.IsNullOrEmpty(contentTypeName))
            {
                throw (contentTypeName == null)
                  ? new ArgumentNullException("contentTypeName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "contentTypeName");
            }

            ContentTypeCollection _cts = list.ContentTypes;
            list.Context.Load(_cts);

            IEnumerable<ContentType> _results = list.Context.LoadQuery<ContentType>(_cts.Where(item => item.Name == contentTypeName));
            list.Context.ExecuteQueryRetry();

            ContentType _ct = _results.FirstOrDefault();
            if (_ct != null)
            {
                _ct.DeleteObject();
                list.Update();
                list.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Adds a document library to a web. Execute Query is called during this implementation
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">Name of the library</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        /// <param name="urlPath"></param>
        /// <exception cref="System.ArgumentException">Thrown when listName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">listName is null</exception>
        public static List CreateDocumentLibrary(this Web web, string listName, bool enableVersioning = false, string urlPath = "")
        {
            if (string.IsNullOrEmpty(listName))
            {
                throw (listName == null)
                  ? new ArgumentNullException("listName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listName");
            }
            // Call actual implementation
            return CreateListInternal(web, null, (int)ListTemplateType.DocumentLibrary, listName, enableVersioning, urlPath: urlPath);
        }

        /// <summary>
        /// Checks if list exists on the particular site based on the list Title property.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list to be checked.</param>
        /// <exception cref="System.ArgumentException">Thrown when listTitle is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">listTitle is null</exception>
        /// <returns>True if the list exists</returns>
        public static bool ListExists(this Web web, string listTitle)
        {
            if (string.IsNullOrEmpty(listTitle))
            {
                throw (listTitle == null)
                  ? new ArgumentNullException("listTitle")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listTitle");
            }

            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
            web.Context.ExecuteQueryRetry();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if list exists on the particular site based on the list id property.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="id">The id of the list to be checked.</param>
        /// <exception cref="System.ArgumentException">Thrown when listTitle is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">listTitle is null</exception>
        /// <returns>True if the list exists</returns>
        public static bool ListExists(this Web web, Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("id");
            }

            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Id == id));
            web.Context.ExecuteQueryRetry();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a default list to a site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listType">Built in list template type</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        /// <param name="updateAndExecuteQuery">(Optional) Perform list update and executequery, defaults to true</param>
        /// <param name="urlPath">(Optional) URL to use for the list</param>
        /// <param name="enableContentTypes">(Optional) Enable content type management</param>
        /// <returns>The newly created list</returns>
        public static List CreateList(this Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "", bool enableContentTypes = false)
        {
            return CreateListInternal(web, null, (int)listType, listName, enableVersioning, updateAndExecuteQuery, urlPath, enableContentTypes);
        }

        /// <summary>
        /// Adds a custom list to a site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="featureId">Feature that contains the list template</param>
        /// <param name="listType">Type ID of the list, within the feature</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        /// <param name="updateAndExecuteQuery">(Optional) Perform list update and executequery, defaults to true</param>
        /// <param name="urlPath">(Optional) URL to use for the list</param>
        /// <param name="enableContentTypes">(Optional) Enable content type management</param>
        /// <returns>The newly created list</returns>
        public static List CreateList(this Web web, Guid featureId, int listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "", bool enableContentTypes = false)
        {
            return CreateListInternal(web, featureId, listType, listName, enableVersioning, updateAndExecuteQuery, urlPath, enableContentTypes);
        }

        private static List CreateListInternal(this Web web, Guid? templateFeatureId, int templateType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "", bool enableContentTypes = false)
        {
            Log.Info(CoreResources.ListExtensions_CreateList0Template12, listName, templateType, templateFeatureId.HasValue ? " (feature " + templateFeatureId.Value.ToString() + ")" : "");

            ListCollection listCol = web.Lists;
            ListCreationInformation lci = new ListCreationInformation();
            lci.Title = listName;
            lci.TemplateType = templateType;
            if (templateFeatureId.HasValue)
            {
                lci.TemplateFeatureId = templateFeatureId.Value;
            }
            if (!string.IsNullOrEmpty(urlPath))
            {
                lci.Url = urlPath;
            }

            List newList = listCol.Add(lci);

            if (enableVersioning)
            {
                newList.EnableVersioning = true;
                newList.EnableMinorVersions = true;
            }
            if (enableContentTypes)
            {
                newList.ContentTypesEnabled = true;
            }
            if (updateAndExecuteQuery)
            {
                newList.Update();
                web.Context.Load(listCol);
                web.Context.ExecuteQueryRetry();
            }

            return newList;
        }

        /// <summary>
        /// Enable/disable versioning on a list
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">List to operate on</param>
        /// <param name="enableVersioning">True to enable versioning, false to disable</param>
        /// <param name="enableMinorVersioning">Enable/Disable minor versioning</param>
        /// <param name="updateAndExecuteQuery">Perform list update and executequery, defaults to true</param>
        /// <exception cref="System.ArgumentException">Thrown when listName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">listName is null</exception>
        public static void UpdateListVersioning(this Web web, string listName, bool enableVersioning, bool enableMinorVersioning = true, bool updateAndExecuteQuery = true)
        {
            if (string.IsNullOrEmpty(listName))
            {
                throw (listName == null)
                  ? new ArgumentNullException("listName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listName");
            }

            List listToUpdate = web.Lists.GetByTitle(listName);
            listToUpdate.EnableVersioning = enableVersioning;
            listToUpdate.EnableMinorVersions = enableMinorVersioning;

            if (updateAndExecuteQuery)
            {
                listToUpdate.Update();
                web.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Enable/disable versioning on a list
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="enableVersioning">True to enable versioning, false to disable</param>
        /// <param name="enableMinorVersioning">Enable/Disable minor versioning</param>
        /// <param name="updateAndExecuteQuery">Perform list update and executequery, defaults to true</param>
        public static void UpdateListVersioning(this List list, bool enableVersioning, bool enableMinorVersioning = true, bool updateAndExecuteQuery = true)
        {
            list.EnableVersioning = enableVersioning;
            list.EnableMinorVersions = enableMinorVersioning;

            if (updateAndExecuteQuery)
            {
                list.Update();
                list.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Sets the default value for a managed metadata column in the specified list. This operation will not change existing items in the list
        /// </summary>
        /// <param name="web">Extension web</param>
        /// <param name="termName">Name of a specific term</param>
        /// <param name="listName">Name of list</param>
        /// <param name="fieldInternalName">Internal name of field</param>
        /// <param name="groupGuid">TermGroup Guid</param>
        /// <param name="termSetGuid">TermSet Guid</param>
        public static void UpdateTaxonomyFieldDefaultValue(this Web web, string termName, string listName, string fieldInternalName, Guid groupGuid, Guid termSetGuid)
        {
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(web.Context);
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            var termGroup = termStore.GetGroup(groupGuid);
            var termSet = termGroup.TermSets.GetById(termSetGuid);
            var terms = termSet.Terms;
            var term = web.Context.LoadQuery(termSet.Terms.Where(t => t.Name == termName));

            web.Context.ExecuteQueryRetry();

            var foundTerm = term.First();

            var list = web.GetListByTitle(listName);

            var fields = web.Context.LoadQuery(list.Fields.Where(f => f.InternalName == fieldInternalName));
            web.Context.ExecuteQueryRetry();

            var taxField = web.Context.CastTo<TaxonomyField>(fields.First());

            //The default value requires that the item is present in the TaxonomyHiddenList (which gives it it's WssId)
            //To solve this, we create a folder that we assign the value, which creates the listitem in the hidden list
            var item = list.AddItem(new ListItemCreationInformation()
            {
                UnderlyingObjectType = FileSystemObjectType.Folder,
                LeafName = string.Concat("Temporary_Folder_For_WssId_Creation_", DateTime.Now.ToFileTime().ToString())
            });

            item.SetTaxonomyFieldValue(taxField.Id, foundTerm.Name, foundTerm.Id);

            web.Context.Load(item);
            web.Context.ExecuteQueryRetry();

            dynamic val = item[fieldInternalName];

            //The folder has now served it's purpose and can safely be removed
            item.DeleteObject();

            taxField.DefaultValue = string.Format("{0};#{1}|{2}", val.WssId, val.Label, val.TermGuid);
            taxField.Update();

            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Sets JS link customization for a list form
        /// </summary>
        /// <param name="list">SharePoint list</param>
        /// <param name="pageType">Type of form</param>
        /// <param name="jslink">JSLink to set to the form. Set to empty string to remove the set JSLink customization.
        /// Specify multiple values separated by pipe symbol. For e.g.: ~sitecollection/_catalogs/masterpage/jquery-2.1.0.min.js|~sitecollection/_catalogs/masterpage/custom.js
        /// </param>
        public static void SetJSLinkCustomizations(this List list, PageType pageType, string jslink)
        {
            // Get the list form to apply the JS link
            Form listForm = list.Forms.GetByPageType(pageType);
            list.Context.Load(listForm, nf => nf.ServerRelativeUrl);
            list.Context.ExecuteQueryRetry();

            Microsoft.SharePoint.Client.File file = list.ParentWeb.GetFileByServerRelativeUrl(listForm.ServerRelativeUrl);
            LimitedWebPartManager wpm = file.GetLimitedWebPartManager(PersonalizationScope.Shared);
            list.Context.Load(wpm.WebParts, wps => wps.Include(wp => wp.WebPart.Title));
            list.Context.ExecuteQueryRetry();

            // Set the JS link for all web parts
            foreach (WebPartDefinition wpd in wpm.WebParts)
            {
                WebPart wp = wpd.WebPart;
                wp.Properties["JSLink"] = jslink;
                wpd.SaveWebPartChanges();

                list.Context.ExecuteQueryRetry();
            }
        }

#if !CLIENTSDKV15
        /// <summary>
        /// Can be used to set translations for different cultures. 
        /// </summary>
        /// <seealso cref="http://blogs.msdn.com/b/vesku/archive/2014/03/20/office365-multilingual-content-types-site-columns-and-site-other-elements.aspx"/>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list</param>
        /// <param name="cultureName">Culture name like en-us or fi-fi</param>
        /// <param name="titleResource">Localized Title string</param>
        /// <param name="descriptionResource">Localized Description string</param>
        /// <exception cref="System.ArgumentException">Thrown when listTitle, cultureName, titleResource, descriptionResource is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">listTitle, cultureName, titleResource, descriptionResource is null</exception>
        public static void SetLocalizationLabelsForList(this Web web, string listTitle, string cultureName, string titleResource, string descriptionResource)
        {
            if (string.IsNullOrEmpty(listTitle))
            {
                throw (listTitle == null)
                  ? new ArgumentNullException("listTitle")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listTitle");
            }
            if (string.IsNullOrEmpty(cultureName))
            {
                throw (cultureName == null)
                  ? new ArgumentNullException("cultureName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "cultureName");
            }
            if (string.IsNullOrEmpty(titleResource))
            {
                throw (titleResource == null)
                  ? new ArgumentNullException("titleResource")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "titleResource");
            }
            if (string.IsNullOrEmpty(descriptionResource))
            {
                throw (descriptionResource == null)
                  ? new ArgumentNullException("descriptionResource")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "descriptionResource");
            }

            List list = web.GetList(listTitle);
            SetLocalizationLabelsForList(list, cultureName, titleResource, descriptionResource);
        }
#endif

#if !CLIENTSDKV15
        /// <summary>
        /// Can be used to set translations for different cultures. 
        /// </summary>
        /// <example>
        ///     list.SetLocalizationForSiteLabels("fi-fi", "Name of the site in Finnish", "Description in Finnish");
        /// </example>
        /// <seealso cref="http://blogs.msdn.com/b/vesku/archive/2014/03/20/office365-multilingual-content-types-site-columns-and-site-other-elements.aspx"/>
        /// <param name="list">List to be processed </param>
        /// <param name="cultureName">Culture name like en-us or fi-fi</param>
        /// <param name="titleResource">Localized Title string</param>
        /// <param name="descriptionResource">Localized Description string</param>
        public static void SetLocalizationLabelsForList(this List list, string cultureName, string titleResource, string descriptionResource)
        {
            list.TitleResource.SetValueForUICulture(cultureName, titleResource);
            list.DescriptionResource.SetValueForUICulture(cultureName, descriptionResource);
            list.Update();
            list.Context.ExecuteQueryRetry();
        }
#endif

        /// <summary>
        /// Returns the GUID id of a list
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">List to operate on</param>
        /// <exception cref="System.ArgumentException">Thrown when listName is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">listName is null</exception>
        public static Guid GetListID(this Web web, string listName)
        {
            if (string.IsNullOrEmpty(listName))
            {
                throw (listName == null)
                  ? new ArgumentNullException("listName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listName");
            }

            List listToQuery = web.Lists.GetByTitle(listName);
            web.Context.Load(listToQuery, l => l.Id);
            web.Context.ExecuteQueryRetry();

            return listToQuery.Id;
        }

        /// <summary>
        /// Get list by using Title
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list to return</param>
        /// <returns>Loaded list instance matching to title or null</returns>
        /// <exception cref="System.ArgumentException">Thrown when listTitle is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">listTitle is null</exception>
        public static List GetListByTitle(this Web web, string listTitle)
        {
            if (string.IsNullOrEmpty(listTitle))
            {
                throw (listTitle == null)
                  ? new ArgumentNullException("listTitle")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listTitle");
            }
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
            web.Context.ExecuteQueryRetry();
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Get list by using Url
        /// </summary>
        /// <param name="web">Web (site) to be processed</param>
        /// <param name="webRelativeUrl">Url of list relative to the web (site), e.g. lists/testlist</param>
        /// <returns></returns>
        public static List GetListByUrl(this Web web, string webRelativeUrl)
        {
            if (string.IsNullOrEmpty(webRelativeUrl))
                throw new ArgumentNullException("webRelativeUrl");

            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }
            var listServerRelativeUrl = UrlUtility.Combine(web.ServerRelativeUrl, webRelativeUrl);

            var foundList = web.GetList(listServerRelativeUrl);
            web.Context.Load(foundList, l => l.DefaultViewUrl, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden, l => l.RootFolder);
            try
            {
                web.Context.ExecuteQueryRetry();
            }
            catch (ServerException se)
            {
                if (se.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    foundList = null;
                }
                else
                {
                    throw;
                }
            }

            return foundList;
        }

        #region List Permissions

        /// <summary>
        /// Set custom permission to the list
        /// </summary>
        /// <param name="list">List on which permission to be set</param>
        /// <param name="user">Built in user</param>
        /// <param name="roleType">Role type</param>
        public static void SetListPermission(this List list, BuiltInIdentity user, RoleType roleType)
        {
            Principal permissionEntity = null;

            // Get the web for list
            Web web = list.ParentWeb;
            list.Context.Load(web);
            list.Context.ExecuteQueryRetry();

            switch (user)
            {
                case BuiltInIdentity.Everyone:
                    {
                        permissionEntity = web.EnsureUser("c:0(.s|true");
                        break;
                    }
                case BuiltInIdentity.EveryoneButExternalUsers:
                    {
                        string userIdentity = string.Format("c:0-.f|rolemanager|spo-grid-all-users/{0}", web.GetAuthenticationRealm());
                        permissionEntity = web.EnsureUser(userIdentity);
                        break;
                    }
            }

            list.SetListPermission(permissionEntity, roleType);
        }

        /// <summary>
        /// Set custom permission to the list
        /// </summary>
        /// <param name="list">List on which permission to be set</param>
        /// <param name="principal">SharePoint Group or User</param>
        /// <param name="roleType">Role type</param>
        public static void SetListPermission(this List list, Principal principal, RoleType roleType)
        {
            // Get the web for list
            Web web = list.ParentWeb;
            list.Context.Load(web);
            list.Context.ExecuteQueryRetry();

            // Stop inheriting permissions
            list.BreakRoleInheritance(true, false);

            // Get role type
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(roleType);
            RoleDefinitionBindingCollection rdbColl = new RoleDefinitionBindingCollection(web.Context);
            rdbColl.Add(roleDefinition);

            // Set custom permission to the list
            list.RoleAssignments.Add(principal, rdbColl);
            list.Context.ExecuteQueryRetry();
        }

        #endregion

        #region List view

        /// <summary>
        /// Creates list views based on specific xml structure from file
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listUrl"></param>
        /// <param name="filePath"></param>
        public static void CreateViewsFromXMLFile(this Web web, string listUrl, string filePath)
        {
            if (string.IsNullOrEmpty(listUrl))
                throw new ArgumentNullException("listUrl");

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);
            CreateViewsFromXML(web, listUrl, xd);
        }

        /// <summary>
        /// Creates views based on specific xml structure from string
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listUrl"></param>
        /// <param name="xmlString"></param>
        public static void CreateViewsFromXMLString(this Web web, string listUrl, string xmlString)
        {
            if (string.IsNullOrEmpty(listUrl))
                throw new ArgumentNullException("listUrl");

            if (string.IsNullOrEmpty(xmlString))
                throw new ArgumentNullException("xmlString");

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlString);
            CreateViewsFromXML(web, listUrl, xd);
        }

        /// <summary>
        /// Create list views based on xml structure loaded to memory
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listUrl"></param>
        /// <param name="xmlDoc"></param>
        public static void CreateViewsFromXML(this Web web, string listUrl, XmlDocument xmlDoc)
        {
            if (string.IsNullOrEmpty(listUrl))
                throw new ArgumentNullException("listUrl");

            if (xmlDoc == null)
                throw new ArgumentNullException("xmlDoc");

            // Get instances to the list
            List list = web.GetList(listUrl);
            web.Context.Load(list);
            web.Context.ExecuteQueryRetry();

            // Execute the actual xml based creation
            list.CreateViewsFromXML(xmlDoc);
        }

        /// <summary>
        /// Create list views based on specific xml structure in external file
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filePath"></param>
        public static void CreateViewsFromXMLFile(this List list, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);
            list.CreateViewsFromXML(xd);
        }

        /// <summary>
        /// Create list views based on specific xml structure in string 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="xmlString"></param>
        public static void CreateViewsFromXMLString(this List list, string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
                throw new ArgumentNullException("xmlString");

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlString);
            list.CreateViewsFromXML(xd);
        }

        /// <summary>
        /// Actual implementation of the view creation logic based on given xml
        /// </summary>
        /// <param name="list"></param>
        /// <param name="xmlDoc"></param>
        public static void CreateViewsFromXML(this List list, XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                throw new ArgumentNullException("xmlDoc");

            // Convert base type to string value used in the xml structure
            string listType = list.BaseType.ToString();
            // Get only relevant list views for matching base list type
            XmlNodeList listViews = xmlDoc.SelectNodes("ListViews/List[@Type='" + listType + "']/View");
            int count = listViews.Count;
            foreach (XmlNode view in listViews)
            {
                string name = view.Attributes["Name"].Value;
                ViewType type = (ViewType)Enum.Parse(typeof(ViewType), view.Attributes["ViewTypeKind"].Value);
                string[] viewFields = view.Attributes["ViewFields"].Value.Split(',');
                uint rowLimit = uint.Parse(view.Attributes["RowLimit"].Value);
                bool defaultView = bool.Parse(view.Attributes["DefaultView"].Value);
                string query = view.SelectSingleNode("./ViewQuery").InnerText;

                //Create View
                list.CreateView(name, type, viewFields, rowLimit, defaultView, query);
            }
        }

        /// <summary>
        /// Create view to existing list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="viewName"></param>
        /// <param name="viewType"></param>
        /// <param name="viewFields"></param>
        /// <param name="rowLimit"></param>
        /// <param name="setAsDefault"></param>
        /// <param name="query"></param>
        /// <param name="personal"></param>
        public static View CreateView(this List list,
                                      string viewName,
                                      ViewType viewType,
                                      string[] viewFields,
                                      uint rowLimit,
                                      bool setAsDefault,
                                      string query = null,
                                      bool personal = false)
        {
            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentNullException("viewName");

            ViewCreationInformation viewCreationInformation = new ViewCreationInformation();
            viewCreationInformation.Title = viewName;
            viewCreationInformation.ViewTypeKind = viewType;
            viewCreationInformation.RowLimit = rowLimit;
            viewCreationInformation.ViewFields = viewFields;
            viewCreationInformation.PersonalView = personal;
            viewCreationInformation.SetAsDefaultView = setAsDefault;
            if (!string.IsNullOrEmpty(query))
            {
                viewCreationInformation.Query = query;
            }

            View view = list.Views.Add(viewCreationInformation);
            list.Context.Load(view);
            list.Context.ExecuteQueryRetry();

            return view;
        }

        /// <summary>
        /// Gets a view by Id
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        /// <returns>returns null if not found</returns>
        public static View GetViewById(this List list, Guid id)
        {
            id.ValidateNotNullOrEmpty("id");

            try
            {
                var view = list.Views.GetById(id);

                list.Context.Load(view);
                list.Context.ExecuteQueryRetry();

                return view;
            }
            catch (ServerException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a view by Name
        /// </summary>
        /// <param name="list"></param>
        /// <param name="name"></param>
        /// <returns>returns null if not found</returns>
        public static View GetViewByName(this List list, string name)
        {
            name.ValidateNotNullOrEmpty("name");

            try
            {
                var view = list.Views.GetByTitle(name);

                list.Context.Load(view);
                list.Context.ExecuteQueryRetry();

                return view;
            }
            catch (ServerException)
            {
                return null;
            }

        }

        #endregion

        private static void SetDefaultColumnValuesImplementation(this List list, IEnumerable<IDefaultColumnValue> columnValues)
        {
            using (var clientContext = list.Context as ClientContext)
            {
                try
                {
                    var values = columnValues.ToList<IDefaultColumnValue>();

                    clientContext.Load(list.RootFolder);
                    clientContext.Load(list.RootFolder.Folders);
                    clientContext.ExecuteQueryRetry();

                    var xMetadataDefaults = new XElement("MetadataDefaults");

                    while (values.Any())
                    {
                        // Get the first entry 
                        var defaultColumnValue = values.First();
                        var path = defaultColumnValue.FolderRelativePath;
                        if (string.IsNullOrEmpty(path))
                        {
                            // Assume root folder
                            path = "/";
                        }
                        if (path.Equals("/"))
                        {
                            path = list.RootFolder.ServerRelativeUrl;
                        }
                        else
                        {
                            path = UrlUtility.Combine(list.RootFolder.ServerRelativeUrl, path);
                        }
                        // Find all in the same path:
                        var defaultColumnValuesInSamePath = columnValues.Where(x => x.FolderRelativePath == defaultColumnValue.FolderRelativePath);
                        path = Uri.EscapeUriString(path);

                        var xATag = new XElement("a", new XAttribute("href", path));

                        foreach (var defaultColumnValueInSamePath in defaultColumnValuesInSamePath)
                        {
                            var fieldName = defaultColumnValueInSamePath.FieldInternalName;
                            var fieldStringBuilder = new StringBuilder();
                            if (defaultColumnValueInSamePath.GetType() == typeof(DefaultColumnTermValue))
                            {
                                // Term value
                                foreach (var term in ((DefaultColumnTermValue)defaultColumnValueInSamePath).Terms)
                                {
                                    if (!term.IsPropertyAvailable("Id") || !term.IsPropertyAvailable("Name"))
                                    {
                                        clientContext.Load(term, t => t.Id, t => t.Name);
                                        clientContext.ExecuteQueryRetry();
                                    }
                                    var wssId = list.ParentWeb.GetWssIdForTerm(term);
                                    fieldStringBuilder.AppendFormat("{0};#{1}|{2};#", wssId, term.Name, term.Id);
                                }
                                var xDefaultValue = new XElement("DefaultValue", new XAttribute("FieldName", fieldName));
                                var fieldString = fieldStringBuilder.ToString().TrimEnd(new char[] { ';', '#' });
                                xDefaultValue.SetValue(fieldString);
                                xATag.Add(xDefaultValue);
                            }
                            else
                            {
                                // Text value
                                var fieldString = fieldStringBuilder.Append(((DefaultColumnTextValue)defaultColumnValueInSamePath).Text);
                                var xDefaultValue = new XElement("DefaultValue", new XAttribute("FieldName", fieldName));
                                xDefaultValue.SetValue(fieldString);
                                xATag.Add(xDefaultValue);
                            }
                            xMetadataDefaults.Add(xATag);
                            values.Remove(defaultColumnValueInSamePath);
                        }
                    }

                    var formsFolder = list.RootFolder.Folders.FirstOrDefault(x => x.Name == "Forms");
                    if (formsFolder != null)
                    {
                        var xmlSB = new StringBuilder();
                        XmlWriterSettings xmlSettings = new XmlWriterSettings();
                        xmlSettings.OmitXmlDeclaration = true;
                        xmlSettings.NewLineHandling = NewLineHandling.None;
                        xmlSettings.Indent = false;

                        using (var xmlWriter = XmlWriter.Create(xmlSB, xmlSettings))
                        {
                            xMetadataDefaults.Save(xmlWriter);
                        }

                        var objFileInfo = new FileCreationInformation();
                        objFileInfo.Url = "client_LocationBasedDefaults.html";
                        objFileInfo.ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlSB.ToString()));

                        objFileInfo.Overwrite = true;
                        formsFolder.Files.Add(objFileInfo);
                        clientContext.ExecuteQueryRetry();
                    }

                    // Add the event receiver if not already there
                    if (list.GetEventReceiverByName("LocationBasedMetadataDefaultsReceiver ItemAdded") == null)
                    {
                        EventReceiverDefinitionCreationInformation eventCi = new EventReceiverDefinitionCreationInformation();
                        eventCi.Synchronization = EventReceiverSynchronization.DefaultSynchronization;
                        eventCi.EventType = EventReceiverType.ItemAdded;
#if !CLIENTSDKV15
                        eventCi.ReceiverAssembly = "Microsoft.Office.DocumentManagement, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";
#else
                        eventCi.ReceiverAssembly = "Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";
#endif
                        eventCi.ReceiverClass = "Microsoft.Office.DocumentManagement.LocationBasedMetadataDefaultsReceiver";
                        eventCi.ReceiverName = "LocationBasedMetadataDefaultsReceiver ItemAdded";
                        eventCi.SequenceNumber = 1000;

                        list.EventReceivers.Add(eventCi);

                        list.Update();

                        clientContext.ExecuteQueryRetry();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error applying default column values", ex);
                }
            }
        }

        /// <summary>
        /// <para>Sets default values for column values.</para>
        /// <para>In order to for instance set the default Enterprise Metadata keyword field to a term, add the enterprise metadata keyword to a library (internal name "TaxKeyword")</para>
        /// <para> </para>
        /// <para>Column values are defined by the DefaultColumnValue class that has 3 properties:</para>
        /// <para>RelativeFolderPath : / to set a default value for the root of the document library, or /foldername to specify a subfolder</para>
        /// <para>FieldInternalName : The name of the field to set. For instance "TaxKeyword" to set the Enterprise Metadata field</para>
        /// <para>Terms : A collection of Taxonomy terms to set</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="columnValues"></param>
        public static void SetDefaultColumnValues(this List list, IEnumerable<IDefaultColumnValue> columnValues)
        {

            using (var clientContext = list.Context as ClientContext)
            {
                clientContext.Load(list.RootFolder);
                clientContext.Load(list.RootFolder.Folders);
                clientContext.ExecuteQueryRetry();
                TaxonomySession taxSession = TaxonomySession.GetTaxonomySession(clientContext);
                // Check if default values file is present
                var formsFolder = list.RootFolder.Folders.FirstOrDefault(x => x.Name == "Forms");
                List<IDefaultColumnValue> existingValues = new List<IDefaultColumnValue>();

                if (formsFolder != null)
                {
                    var configFile = formsFolder.Files.GetByUrl("client_LocationBasedDefaults.html");
                    clientContext.Load(configFile, c => c.Exists);
                    bool fileExists = false;
                    try
                    {
                        clientContext.ExecuteQueryRetry();
                        fileExists = true;
                    }
                    catch { }

                    if (fileExists)
                    {
                        var streamResult = configFile.OpenBinaryStream();
                        clientContext.ExecuteQueryRetry();
                        XDocument document = XDocument.Load(streamResult.Value);
                        var values = from a in document.Descendants("a") select a;

                        List<DefaultColumnTermValue> defaultColumnTermValues = new List<DefaultColumnTermValue>();

                        foreach (var value in values)
                        {
                            var href = value.Attribute("href").Value;
                            href = Uri.UnescapeDataString(href);
                            href = href.Replace(list.RootFolder.ServerRelativeUrl, "/");
                            var defaultValues = from d in value.Descendants("DefaultValue") select d;
                            foreach (var defaultValue in defaultValues)
                            {
                                var fieldName = defaultValue.Attribute("FieldName").Value;

                                var field = list.Fields.GetByInternalNameOrTitle(fieldName);
                                clientContext.Load(field);
                                clientContext.ExecuteQueryRetry();
                                if (field.FieldTypeKind == FieldType.Text)
                                {
                                    var textValue = defaultValue.Value;
                                    DefaultColumnTextValue defaultColumnTextValue = new DefaultColumnTextValue()
                                    {
                                        FieldInternalName = fieldName,
                                        FolderRelativePath = href,
                                        Text = textValue
                                    };
                                    existingValues.Add(defaultColumnTextValue);
                                }
                                else
                                {
                                    var termsIdentifier = defaultValue.Value;

                                    var terms = termsIdentifier.Split(new string[] { ";#" }, StringSplitOptions.None);

                                    List<Term> existingTerms = new List<Term>();
                                    for (int q = 1; q < terms.Length; q++)
                                    {
                                        var termIdString = terms[q].Split(new char[] { '|' })[1];
                                        var term = taxSession.GetTerm(new Guid(termIdString));
                                        clientContext.Load(term, t => t.Id, t => t.Name);
                                        clientContext.ExecuteQueryRetry();
                                        existingTerms.Add(term);
                                        q++; // Skip one
                                    }

                                    DefaultColumnTermValue defaultColumnTermValue = new DefaultColumnTermValue()
                                    {
                                        FieldInternalName = fieldName,
                                        FolderRelativePath = href,
                                    };
                                    existingTerms.ForEach(t => defaultColumnTermValue.Terms.Add(t));

                                    existingValues.Add(defaultColumnTermValue);
                                }
                            }

                        }
                    }
                }

                List<IDefaultColumnValue> termsList = columnValues.Union(existingValues, new DefaultColumnTermValueComparer()).ToList();

                list.SetDefaultColumnValuesImplementation(termsList);
            }
        }

        private class DefaultColumnTermValueComparer : IEqualityComparer<IDefaultColumnValue>
        {
            public bool Equals(IDefaultColumnValue x, IDefaultColumnValue y)
            {
                if (ReferenceEquals(x, y)) return true;

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                    return false;

                return x.FieldInternalName == y.FieldInternalName && x.FolderRelativePath == y.FolderRelativePath;
            }

            public int GetHashCode(IDefaultColumnValue defaultValue)
            {
                if (ReferenceEquals(defaultValue, null)) return 0;

                int hashFolder = defaultValue.FolderRelativePath == null ? 0 : defaultValue.FolderRelativePath.GetHashCode();

                int hashFieldInternalName = defaultValue.FieldInternalName.GetHashCode();

                return hashFolder ^ hashFieldInternalName;
            }
        }

    }
}

using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that provides generic list creation and manipulation methods
    /// </summary>
    public static class ListExtensions
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
        public static EventReceiverDefinition RegisterRemoteEventReceiver(this List list, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, bool force)
        {
            var query = from receiver
                     in list.EventReceivers
                        where receiver.ReceiverName == name
                        select receiver;
            list.Context.LoadQuery(query);
            list.Context.ExecuteQuery();

            var receiverExists = query.Any();
            if (receiverExists && force)
            {
                var receiver = query.FirstOrDefault();
                receiver.DeleteObject();
                list.Context.ExecuteQuery();
                receiverExists = false;
            }
            EventReceiverDefinition def = null;

            if (!receiverExists)
            {
                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.EventType = eventReceiverType;
                receiver.ReceiverUrl = url;
                receiver.ReceiverName = name;
                receiver.Synchronization = synchronization;
                def = list.EventReceivers.Add(receiver);
                list.Context.Load(def);
                list.Context.ExecuteQuery();
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
            list.Context.ExecuteQuery();
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
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EventReceiverDefinition GetEventReceiverByName(this List list, string name)
        {
            IEnumerable<EventReceiverDefinition> receivers = null;
            var query = from receiver
                        in list.EventReceivers
                        where receiver.ReceiverName == name
                        select receiver;

            receivers = list.Context.LoadQuery(query);
            list.Context.ExecuteQuery();
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
        /// <summary>
        /// Removes a content type from a list/library by name
        /// </summary>
        /// <param name="list">The list</param>
        /// <param name="contentTypeName">The content type name to remove from the list</param>
        /// <exception cref="System.ArgumentException">Thrown when a arguement is null or <see cref="String.Empty"/></exception>
        public static void RemoveContentTypeByName(this List list, string contentTypeName)
        {
            if (string.IsNullOrEmpty(contentTypeName))
            {
                throw new ArgumentException(string.Format(Constants.EXCEPTION_MSG_INVALID_ARG, "contentTypeName"));
            }

            ContentTypeCollection _cts = list.ContentTypes;
            list.Context.Load(_cts);

            IEnumerable<ContentType> _results = list.Context.LoadQuery<ContentType>(_cts.Where(item => item.Name == contentTypeName));
            list.Context.ExecuteQuery();

            ContentType _ct = _results.FirstOrDefault();
            if (_ct != null)
            {
                _ct.DeleteObject();
                list.Update();
                list.Context.ExecuteQuery();
            }
        }
        /// <summary>
        /// Removes a content type from a list/library 
        /// </summary>
        /// <param name="list">The list</param>
        /// <param name="contentTypeName">The content type name to remove from the list</param>
        [Obsolete("Use RemoveContentTypeByName")]
        public static void RemoveContentType(this List list, string contentTypeName)
        {
            ContentTypeCollection _cts = list.ContentTypes;
            list.Context.Load(_cts);

            IEnumerable<ContentType> _results = list.Context.LoadQuery<ContentType>(_cts.Where(item => item.Name == contentTypeName));
            list.Context.ExecuteQuery();

            ContentType _ct = _results.FirstOrDefault();
            if (_ct != null)
            {
                _ct.DeleteObject();
                list.Update();
                list.Context.ExecuteQuery();
            }
        }
        /// <summary>
        /// Adds a list to a site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listType">Type of the list</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        /// <param name="updateAndExecuteQuery">Perform list update and executequery, defaults to true</param>
        public static void AddList(this Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            // Call actual implementation
            CreateListInternal(web, listType, listName, enableVersioning, updateAndExecuteQuery, urlPath);
        }

        /// <summary>
        /// Adds a document library to a site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">Name of the library</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        public static void AddDocumentLibrary(this Web web, string listName, bool enableVersioning = false, string urlPath = "")
        {
            // Call actual implementation
            CreateListInternal(web, ListTemplateType.DocumentLibrary, listName, enableVersioning, urlPath: urlPath);
        }

        /// <summary>
        /// Adds a document library to a site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">Name of the library</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        public static void CreateDocumentLibrary(this Web web, string listName, bool enableVersioning = false, string urlPath = "")
        {
            // Call actual implementation
            CreateListInternal(web, ListTemplateType.DocumentLibrary, listName, enableVersioning, urlPath: urlPath);
        }


        /// <summary>
        /// Checks if list exists on the particular site based on the list Title property.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list to be checked.</param>
        /// <returns></returns>
        public static bool ListExists(this Web web, string listTitle)
        {
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
            web.Context.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a list to a site
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listType">Type of the list</param>
        /// <param name="featureID">Feature guid that brings this list type</param>
        /// <param name="listName">Name of the list</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        /// <param name="updateAndExecuteQuery">Perform list update and executequery, defaults to true</param>
        public static bool AddList(this Web web, int listType, Guid featureID, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            bool created = false;

            ListCollection listCollection = web.Lists;
            web.Context.Load(listCollection, lists => lists.Include(list => list.Title).Where(list => list.Title == listName));
            web.Context.ExecuteQuery();

            if (listCollection.Count == 0)
            {
                ListCollection listCol = web.Lists;
                ListCreationInformation lci = new ListCreationInformation();
                lci.Title = listName;
                lci.TemplateFeatureId = featureID;
                lci.TemplateType = listType;

                if (!string.IsNullOrEmpty(urlPath))
                    lci.Url = urlPath;

                List newList = listCol.Add(lci);

                if (enableVersioning)
                {
                    newList.EnableVersioning = true;
                    newList.EnableMinorVersions = true;
                }

                if (updateAndExecuteQuery)
                {
                    newList.Update();
                    web.Context.Load(listCol);
                    web.Context.ExecuteQuery();
                }

                created = true;
            }

            return created;
        }

        public static void CreateList(this Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            // Call actual implementation
            CreateListInternal(web, listType, listName, enableVersioning, updateAndExecuteQuery, urlPath);
        }

        private static void CreateListInternal(this Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            ListCollection listCol = web.Lists;
            ListCreationInformation lci = new ListCreationInformation();
            lci.Title = listName;
            lci.TemplateType = (int)listType;

            if (!string.IsNullOrEmpty(urlPath))
                lci.Url = urlPath;

            List newList = listCol.Add(lci);

            if (enableVersioning)
            {
                newList.EnableVersioning = true;
                newList.EnableMinorVersions = true;
            }

            if (updateAndExecuteQuery)
            {
                newList.Update();
                web.Context.Load(listCol);
                web.Context.ExecuteQuery();
            }

        }

        /// <summary>
        /// Enable/disable versioning on a list
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">List to operate on</param>
        /// <param name="enableVersioning">True to enable versioning, false to disable</param>
        /// <param name="enableMinorversioning">Enable/Disable minor versioning</param>
        /// <param name="updateAndExecuteQuery">Perform list update and executequery, defaults to true</param>
        public static void UpdateListVersioning(this Web web, string listName, bool enableVersioning, bool enableMinorVersioning = true, bool updateAndExecuteQuery = true)
        {
            List listToUpdate = web.Lists.GetByTitle(listName);
            listToUpdate.EnableVersioning = enableVersioning;
            listToUpdate.EnableMinorVersions = enableMinorVersioning;

            if (updateAndExecuteQuery)
            {
                listToUpdate.Update();
                web.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Enable/disable versioning on a list
        /// </summary>
        /// <param name="list">List to be processed</param>
        /// <param name="enableVersioning">True to enable versioning, false to disable</param>
        /// <param name="enableMinorversioning">Enable/Disable minor versioning</param>
        /// <param name="updateAndExecuteQuery">Perform list update and executequery, defaults to true</param>
        public static void UpdateListVersioning(this List list, bool enableVersioning, bool enableMinorVersioning = true, bool updateAndExecuteQuery = true)
        {
            list.EnableVersioning = enableVersioning;
            list.EnableMinorVersions = enableMinorVersioning;

            if (updateAndExecuteQuery)
            {
                list.Update();
                list.Context.ExecuteQuery();
            }

        }


        /// <summary>
        /// Can be used to set translations for different cultures. 
        /// </summary>
        /// <seealso cref="http://blogs.msdn.com/b/vesku/archive/2014/03/20/office365-multilingual-content-types-site-columns-and-site-other-elements.aspx"/>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">Title of the list </param>
        /// <param name="cultureName">Culture name like en-us or fi-fi</param>
        /// <param name="titleResource">Localized Title string</param>
        /// <param name="descriptionResource">Localized Description string</param>
        public static void SetLocalizationLabelsForList(this Web web, string listTitle, string cultureName, string titleResource, string descriptionResource)
        {
            List list = web.GetList(listTitle);
            SetLocalizationLabelsForList(list, cultureName, titleResource, descriptionResource);
        }

        /// <summary>
        /// Can be used to set translations for different cultures. 
        /// </summary>
        /// <example>
        ///     list.SetLocalizationForSiteLabels("fi-fi", "Name of the site in Finnish", "Description in Finnish");
        /// </example>
        /// <seealso cref="http://blogs.msdn.com/b/vesku/archive/2014/03/20/office365-multilingual-content-types-site-columns-and-site-other-elements.aspx"/>
        /// <param name="list">List to be processed </param>
        /// <param name="listName">Title of the list </param>
        /// <param name="cultureName">Culture name like en-us or fi-fi</param>
        /// <param name="titleResource">Localized Title string</param>
        /// <param name="descriptionResource">Localized Description string</param>
        public static void SetLocalizationLabelsForList(this List list, string cultureName, string titleResource, string descriptionResource)
        {
            list.TitleResource.SetValueForUICulture(cultureName, titleResource);
            list.DescriptionResource.SetValueForUICulture(cultureName, descriptionResource);
            list.Update();
            list.Context.ExecuteQuery();
        }

        /// <summary>
        /// Returns the GUID id of a list
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listName">List to operate on</param>
        public static Guid GetListID(this Web web, string listName)
        {
            Guid ret = Guid.NewGuid();

            List listToQuery = web.Lists.GetByTitle(listName);
            web.Context.Load(listToQuery, l => l.Id);
            web.Context.ExecuteQuery();

            return listToQuery.Id;
        }

        /// <summary>
        /// Get list by using Title
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list to return</param>
        /// <returns>Loaded list instance matching to title or null</returns>
        public static List GetListByTitle(this Web web, string listTitle)
        {
            ListCollection lists = web.Lists;
            IEnumerable<List> results = web.Context.LoadQuery<List>(lists.Where(list => list.Title == listTitle));
            web.Context.ExecuteQuery();
            return results.FirstOrDefault();
        }

        public static List GetListByUrl(this Web web, string siteRelativeUrl)
        {
            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }
            if (!siteRelativeUrl.StartsWith("/")) siteRelativeUrl = "/" + siteRelativeUrl;
            siteRelativeUrl = web.ServerRelativeUrl + siteRelativeUrl;
            IEnumerable<List> lists = web.Context.LoadQuery(
                web.Lists
                    .Include(l => l.DefaultViewUrl, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden, l => l.RootFolder));

            web.Context.ExecuteQuery();

            List foundList = lists.Where(l => l.RootFolder.ServerRelativeUrl.ToLower().StartsWith(siteRelativeUrl.ToLower())).FirstOrDefault();

            if (foundList != null)
            {
                return foundList;
            }
            else
            {
                return null;
            }

        }

        [Obsolete("Use CreateListViewsFormXMLFile")]
        public static void CreateListVewsFromXMLFile(this Web web, string listUrl, string filePath)
        {
            CreateListViewsFromXMLFile(web, listUrl, filePath);
        }

        /// <summary>
        /// Creates list views based on specific xml structure from file
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listUrl"></param>
        /// <param name="filePath"></param>
        public static void CreateListViewsFromXMLFile(this Web web, string listUrl, string filePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);
            CreateListViewsFromXML(web, listUrl, xd);
        }

        [Obsolete("Use CreateListViewsFromXMLString")]
        public static void CreateListVewsFromXMLString(this Web web, string listUrl, string xmlString)
        {
            CreateListViewsFromXMLString(web, listUrl, xmlString);
        }

        /// <summary>
        /// Creates views based on specific xml structure from string
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listUrl"></param>
        /// <param name="xmlString"></param>
        public static void CreateListViewsFromXMLString(this Web web, string listUrl, string xmlString)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlString);
            CreateListViewsFromXML(web, listUrl, xd);
        }

        [Obsolete("Use CreateListViewFromXML")]
        public static void CreateListVewsFromXML(this Web web, string listUrl, XmlDocument xmlDoc)
        {
            CreateListViewsFromXML(web, listUrl, xmlDoc);
        }

        /// <summary>
        /// Create list views based on xml structure loaded to memory
        /// </summary>
        /// <param name="web"></param>
        /// <param name="listUrl"></param>
        /// <param name="xmlDoc"></param>
        public static void CreateListViewsFromXML(this Web web, string listUrl, XmlDocument xmlDoc)
        {
            // Get instances to the list
            List list = web.GetList(listUrl);
            web.Context.Load(list);
            web.Context.ExecuteQuery();

            // Execute the actual xml based creation
            list.CreateListViewsFromXML(xmlDoc);
        }

        [Obsolete("Use CreateListViewsFromXMLFile")]
        public static void CreateListVewsFromXMLFile(this List list, string filePath)
        {
            CreateListViewsFromXMLFile(list, filePath);
        }

        /// <summary>
        /// Create list views based on specific xml structure in external file
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filePath"></param>
        public static void CreateListViewsFromXMLFile(this List list, string filePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);
            list.CreateListViewsFromXML(xd);
        }

        [Obsolete("Use CreateListViewsFromXMLString")]
        public static void CreateListVewsFromXMLString(this List list, string xmlString)
        {
            CreateListViewsFromXMLString(list, xmlString);
        }

        /// <summary>
        /// Create list views based on specific xml structure in string 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="xmlString"></param>
        public static void CreateListViewsFromXMLString(this List list, string xmlString)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlString);
            list.CreateListViewsFromXML(xd);
        }

        [Obsolete("Use CreateListViewsFormXML")]
        public static void CreateListVewsFromXML(this List list, XmlDocument xmlDoc)
        {
            CreateListViewsFromXML(list, xmlDoc);
        }

        /// <summary>
        /// Actual implementation of the view creation logic based on given xml
        /// </summary>
        /// <param name="list"></param>
        /// <param name="xmlDoc"></param>
        public static void CreateListViewsFromXML(this List list, XmlDocument xmlDoc)
        {
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
                list.CreateListView(name, type, viewFields, rowLimit, defaultView, query);
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
        public static void CreateListView(this List list, string viewName, ViewType viewType, string[] viewFields, uint rowLimit, bool setAsDefault, string query = null, bool personal = false)
        {
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

            list.Views.Add(viewCreationInformation);
            list.Context.ExecuteQuery();
        }
    }
}

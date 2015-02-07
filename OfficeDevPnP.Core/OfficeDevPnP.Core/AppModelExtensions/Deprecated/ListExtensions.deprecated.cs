using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Entities;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that provides generic list creation and manipulation methods
    /// </summary>
    public static partial class ListExtensions
    {
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
        [Obsolete("Use List.AddRemoteEventReceiver()")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static EventReceiverDefinition RegisterRemoteEventReceiver(this List list, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, bool force)
        {
            return list.AddRemoteEventReceiver(name, url, eventReceiverType, synchronization, force);
        }

        [Obsolete("Prefer CreateList()")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void AddList(this Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            if (string.IsNullOrEmpty(listName))
            {
                throw (listName == null)
                  ? new ArgumentNullException("listName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listName");
            }

            // Call actual implementation
            CreateListInternal(web, null, (int)listType, listName, enableVersioning, updateAndExecuteQuery, urlPath);
        }

        [Obsolete("Please use the CreateDocumentLibrary method")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void AddDocumentLibrary(this Web web, string listName, bool enableVersioning = false, string urlPath = "")
        {
            if (string.IsNullOrEmpty(listName))
            {
                throw (listName == null)
                  ? new ArgumentNullException("listName")
                  : new ArgumentException(CoreResources.Exception_Message_EmptyString_Arg, "listName");
            }
            // Call actual implementation
            CreateListInternal(web, null, (int)ListTemplateType.DocumentLibrary, listName, enableVersioning, urlPath: urlPath);
        }

        [Obsolete("Prefer CreateList()")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool AddList(this Web web, int listType, Guid featureID, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
        {
            bool created = false;

            ListCollection listCollection = web.Lists;
            web.Context.Load(listCollection, lists => lists.Include(list => list.Title).Where(list => list.Title == listName));
            web.Context.ExecuteQueryRetry();

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
                    web.Context.ExecuteQueryRetry();
                }

                created = true;
            }

            return created;
        }

        /// <summary>
        /// <para>Sets default values for column values.</para>
        /// <para>In order to for instance set the default Enterprise Metadata keyword field to a term, add the enterprise metadata keyword to a library (internal name "TaxKeyword")</para>
        /// <para> </para>
        /// <para>Column values are defined by the DefaultColumnValue class that has 3 properties:</para>
        /// <para>RelativeFolderPath : / to set a default value for the root of the document library, or /foldername to specify a subfolder</para>
        /// <para>FieldInternalName : The name of the field to set. For instance "TaxKeyword" to set the Enterprise Metadata field</para>
        /// <para>TermPaths : A collection of string values to set in the shape of TermGroup|TermSet|Term </para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="columnValues"></param>
        [Obsolete("Use SetDefaultColumnValues(IEnumerable<IDefaultColumnValue> columnValues) instead")]
        public static void SetDefaultColumnValues(this List list, IEnumerable<DefaultColumnTermPathValue> columnValues)
        {
            using (var clientContext = list.Context as ClientContext)
            {
                List<DefaultColumnTermValue> newValues = new List<DefaultColumnTermValue>();
                foreach (var value in columnValues)
                {

                    DefaultColumnTermValue newValue = new DefaultColumnTermValue();
                    newValue.FieldInternalName = value.FieldInternalName;
                    newValue.FolderRelativePath = value.FolderRelativePath;

                    foreach (var termpath in value.TermPaths)
                    {
                        var term = clientContext.Site.GetTaxonomyItemByPath(termpath) as Term;
                        newValue.Terms.Add(term);
                    }
                    newValues.Add(newValue);
                }
                list.SetDefaultColumnValues(newValues);
            }
        }

    }
}

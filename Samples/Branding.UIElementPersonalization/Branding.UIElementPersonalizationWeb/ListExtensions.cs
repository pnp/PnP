using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Branding.UIElementPersonalizationWeb
{
    public static class ListExtensions
    {
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
        /// <param name="listName">Name of the list</param>
        /// <param name="enableVersioning">Enable versioning on the list</param>
        /// <param name="updateAndExecuteQuery">Perform list update and executequery, defaults to true</param>
        public static void AddList(this Web web, ListTemplateType listType, string listName, bool enableVersioning, bool updateAndExecuteQuery = true, string urlPath = "")
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


    }
}
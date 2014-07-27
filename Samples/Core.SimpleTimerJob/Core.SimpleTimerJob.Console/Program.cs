using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SimpleTimerJob.Console
{
    class Program
    {

        private static string SharePointPrincipal = "00000003-0000-0ff1-ce00-000000000000";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            Uri siteUri = new Uri(ConfigurationManager.AppSettings["url"]);

            //Get the realm for the URL
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);

            //Get the access token for the URL.  
            //   Requires this app to be registered with the tenant
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                siteUri.Authority, realm).AccessToken;

            //Get client context with access token
            using (var ctx =
                TokenHelper.GetClientContextWithAccessToken(
                    siteUri.ToString(), accessToken))
            {
                // Let's create a list to the host web and add a new entry for each execution
                if (!ListExists(ctx.Web, "RemoteOperation"))
                {
                    AddList(ctx.Web, ListTemplateType.GenericList, "RemoteOperation");
                }

                // Add new execution entry to the list time stamp
                // Assume that the web has a list named "Announcements". 
                List list = ctx.Web.Lists.GetByTitle("RemoteOperation");
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem newItem = list.AddItem(itemCreateInfo);
                newItem["Title"] = string.Format("New {0}", DateTime.Now.ToLongTimeString());
                newItem.Update();

                ctx.ExecuteQuery();
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
        public static void AddList(Web web, ListTemplateType listType, string listName)
        {
            ListCollection listCol = web.Lists;
            ListCreationInformation lci = new ListCreationInformation();
            lci.Title = listName;
            lci.TemplateType = (int)listType;
            List newList = listCol.Add(lci);
        }


        /// <summary>
        /// Checks if list exists on the particular site based on the list Title property.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="listTitle">Title of the list to be checked.</param>
        /// <returns></returns>
        public static bool ListExists(Web web, string listTitle)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;

namespace OfficeDevPnP.SPOnline.Core
{
    public static class SPOWikiPage
    {
        public static void SetWikiPageContent(string pageUrl, string content, Web web, ClientContext clientContext)
        {
            //pageUrl = Utils.Urls.CombineUrl(web, pageUrl);
            File file = clientContext.Web.GetFileByServerRelativeUrl(pageUrl);

            clientContext.Load(file, f => f.ListItemAllFields);
            clientContext.ExecuteQuery();

            ListItem item = file.ListItemAllFields;

            item["WikiField"] = content;

            item.Update();

            clientContext.ExecuteQuery();
        }

        public static string GetWikiPageContent(string serverRelativePageUrl, Web web, ClientContext clientContext)
        {
            serverRelativePageUrl = Utils.Urls.CombineUrl(web, serverRelativePageUrl);

            File file = clientContext.Web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            clientContext.Load(file, f => f.ListItemAllFields);
            clientContext.ExecuteQuery();

            ListItem item = file.ListItemAllFields;

            string content = item["WikiField"] as string;

            return content;
        }

        public static void AddWikiPage(string serverRelativePageUrl, Web web, ClientContext clientContext, string content = null)
        {
            //serverRelativePageUrl = Utils.Urls.CombineUrl(web, serverRelativePageUrl);
            string folderName = serverRelativePageUrl.Substring(0, serverRelativePageUrl.LastIndexOf("/"));
            Folder folder = web.GetFolderByServerRelativeUrl(folderName);
            File file = folder.Files.AddTemplateFile(serverRelativePageUrl, TemplateFileType.WikiPage);

            clientContext.ExecuteQuery();
            if (content != null)
            {
                SetWikiPageContent(serverRelativePageUrl, content, web, clientContext);
            }
        }

        public static void RemoveWikiPage(string serverRelativePageUrl, Web web, ClientContext clientContext)
        {
            serverRelativePageUrl = Utils.Urls.CombineUrl(web, serverRelativePageUrl);

            File file = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            file.DeleteObject();

            clientContext.ExecuteQuery();
        }
    }
}

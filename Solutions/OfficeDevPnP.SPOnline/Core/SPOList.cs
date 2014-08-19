using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Reflection;

namespace OfficeDevPnP.SPOnline.Core
{
    public static class SPOList
    {

        public static Guid GetFieldId(List list, string fieldName)
        {
            ClientContext clientContext = list.Context as ClientContext;
            Field field = list.Fields.GetByInternalNameOrTitle(fieldName);
            clientContext.Load(field);
            clientContext.ExecuteQuery();

            return field.Id;
        }

        public static View AddView(List list, string title, string camlQuery, string[] viewFields, ViewType viewType, uint rowLimit, bool personal, bool setAsDefault, ClientContext clientContext)
        {
            ViewCreationInformation vInfo = new ViewCreationInformation();
            vInfo.Title = title;
            if (!string.IsNullOrEmpty(camlQuery)) vInfo.Query = camlQuery;
            vInfo.ViewFields = viewFields;
            vInfo.ViewTypeKind = viewType;
            vInfo.SetAsDefaultView = setAsDefault;
            vInfo.PersonalView = personal;
            vInfo.RowLimit = rowLimit;

            View view = list.Views.Add(vInfo);
            clientContext.Load(view, v => v.Id, v => v.ViewQuery, v => v.Title, v => v.ViewFields, v => v.ViewType, v => v.DefaultView, v => v.PersonalView, v => v.RowLimit);
            clientContext.ExecuteQuery();
            return view;
        }

        public static List CreateList(string title, string description, string url, ListTemplateType templateType, Web web, QuickLaunchOptions quicklaunchOptions)
        {
            ClientContext clientContext = web.Context as ClientContext;

            ListCreationInformation createInfo = new ListCreationInformation();

            createInfo.Title = title;
            createInfo.TemplateType = (int)templateType;

            createInfo.QuickLaunchOption = quicklaunchOptions;

            if (!string.IsNullOrEmpty(description))
            {
                createInfo.Description = description;
            }

            if (!string.IsNullOrEmpty(url))
            {
                createInfo.Url = url;
            }

            // clientContext.Load(web.Lists);

            List newList = web.Lists.Add(createInfo);


            clientContext.ExecuteQuery();

            clientContext.Load(newList);
            clientContext.ExecuteQuery();

            return newList;
        }



        public static List GetListById(Guid id, Web web, ClientContext context)
        {

            List foundList = web.Lists.GetById(id);

            context.Load(foundList, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden);

            context.ExecuteQuery();

            return foundList;

        }

        public static List GetListByTitle(string title, Web web, ClientContext context)
        {
            List foundList = web.Lists.GetByTitle(title);

            context.Load(foundList, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden);
            context.ExecuteQuery();

            return foundList;
        }

        public static IEnumerable<List> GetLists(Web web, ClientContext clientContext)
        {
            ListCollection lists = web.Lists;
            clientContext.Load(lists, lc => lc.Include(l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden));

            clientContext.ExecuteQuery();

            foreach (List list in lists)
            {
                yield return list;
            }
        }

        public static List GetListByUrl(string siteRelativeUrl, Web web, ClientContext context)
        {
            context.Load(web, w=> w.ServerRelativeUrl);
            context.ExecuteQuery();
            if (!siteRelativeUrl.StartsWith("/")) siteRelativeUrl = "/" + siteRelativeUrl;
            siteRelativeUrl = web.ServerRelativeUrl + siteRelativeUrl;
            IEnumerable<List> lists = context.LoadQuery(
                web.Lists
                    .Include(l => l.DefaultViewUrl, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden, l => l.RootFolder));

            context.ExecuteQuery();


            List foundList = lists.Where(l => l.RootFolder.ServerRelativeUrl.ToLower().StartsWith(siteRelativeUrl.ToLower())).FirstOrDefault();
            //List foundList = lists.Where(l => l.DefaultViewUrl.ToLower().IndexOf(url.ToLower()) != -1).FirstOrDefault();

            if (foundList != null)
            {
                return foundList;
            }
            else
            {
                return null;
            }

        }

        public static List<View> GetViews(List list, ClientContext clientContext)
        {
            clientContext.Load(list.Views, views => views.IncludeWithDefaultProperties(view => view.ViewFields));
            clientContext.ExecuteQuery();

            return list.Views.ToList<View>();
        }

    }
}

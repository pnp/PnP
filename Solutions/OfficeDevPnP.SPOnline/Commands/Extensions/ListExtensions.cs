using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.SPOnline.Commands
{
    public static class ListExtensions
    {
        public static List GetList(this Web web, SPOListPipeBind identity)
        {
            List list = null;
            if (identity.List != null)
            {
                list = identity.List;
            }
            else if (identity.Id != Guid.Empty)
            {
                list = web.Lists.GetById(identity.Id);
            }
            else if (!string.IsNullOrEmpty(identity.Title))
            {
                list = web.GetListByTitle(identity.Title);
                if (list == null)
                {
                    list = web.GetListByUrl(identity.Title);
                }
            }

            web.Context.Load(list, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden);
            web.Context.ExecuteQuery();
            return list;
        }

        public static IEnumerable<SPOList> GetLists(this Web web)
        {
            var lists = web.Context.LoadQuery(web.Lists.IncludeWithDefaultProperties(l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden));

            web.Context.ExecuteQuery();


            return lists.Select(x => new SPOList(x));
         }
    }
}

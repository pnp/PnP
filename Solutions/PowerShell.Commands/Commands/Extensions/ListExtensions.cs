using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    public static class ListExtensions
    {
        public static List GetList(this Web web, ListPipeBind identity)
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
            if (list != null)
            {
                web.Context.Load(list, l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden, l => l.ContentTypesEnabled);
                web.Context.ExecuteQueryRetry();
            }
            return list;
        }
    }
}

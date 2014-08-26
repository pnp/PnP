using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Commands.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOView")]
    public class GetView : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Url of the list.")]
        public SPOListPipeBind List;

        [Parameter(Mandatory = false)]
        public SPOViewPipeBind Identity;

        protected override void ExecuteCmdlet()
        {

            if (List != null)
            {
                var list = this.SelectedWeb.GetList(List);
                if (list != null)
                {
                    IQueryable<ViewEntity> query = null;
                    View view = null;
                    if (Identity != null)
                    {
                        if (Identity.Id != Guid.Empty)
                        {
                            view = list.GetViewById(Identity.Id);
                            
                        }
                        else if (!string.IsNullOrEmpty(Identity.Title))
                        {
                            view = list.GetViewByName(Identity.Title);
                        }
                    }
                    else
                    {
                        var views = ClientContext.LoadQuery(list.Views.IncludeWithDefaultProperties(v => v.ViewFields));
                        ClientContext.ExecuteQuery();
                        query = from v in views.AsQueryable()
                                select new ViewEntity()
                                {
                                    ViewFields = v.ViewFields,
                                    DefaultView = v.DefaultView,
                                    Id = v.Id,
                                    PersonalView = v.PersonalView,
                                    Query = v.ViewQuery,
                                    RowLimit = v.RowLimit,
                                    Title = v.Title,
                                    ViewType = v.ViewType,
                                    _contextObject = v
                                };
                    }
                    if (query != null)
                    {
                        if (query.Count() == 1)
                        {
                            WriteObject(query.First());
                        }
                        else
                        {
                            foreach (var v in query)
                            {
                                WriteObject(v);
                            }
                        }
                    }
                    else if (view != null)
                    {
                        WriteObject(new ViewEntity()
                        {
                            ViewFields = view.ViewFields,
                            DefaultView = view.DefaultView,
                            Id = view.Id,
                            PersonalView = view.PersonalView,
                            Query = view.ViewQuery,
                            RowLimit = view.RowLimit,
                            Title = view.Title,
                            ViewType = view.ViewType,
                            _contextObject = view
                        });
                    }
                }
            }

        }
    }

}

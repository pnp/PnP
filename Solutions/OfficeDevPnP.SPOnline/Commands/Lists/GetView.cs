using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
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
                    IQueryable<SPOnlineView> query = null;
                    View view = null;
                    if (Identity != null)
                    {


                        if (Identity.Id != Guid.Empty)
                        {
                            view = SPO.SPOList.GetViews(list, ClientContext).Where(v => v.Id == Identity.Id).FirstOrDefault();
                        }
                        else if (!string.IsNullOrEmpty(Identity.Title))
                        {
                            view = SPO.SPOList.GetViews(list, ClientContext).Where(v => v.Title == Identity.Title).FirstOrDefault();
                        }
                    }
                    else
                    {
                        var views = SPO.SPOList.GetViews(list, ClientContext);
                        query = from v in views.AsQueryable()
                                select new SPOnlineView(v);
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
                        WriteObject(new SPOnlineView(view));
                    }
                }
            }

        }
    }

}

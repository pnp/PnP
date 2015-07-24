using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;
using System.Collections.Generic;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOView")]
    [CmdletHelp("Returns one or all views from a list", Category = "Lists")]
    public class GetView : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Url of the list.")]
        public ListPipeBind List;

        [Parameter(Mandatory = false)]
        public ViewPipeBind Identity;

        protected override void ExecuteCmdlet()
        {

            if (List != null)
            {
                var list = List.GetList(SelectedWeb);
                if (list != null)
                {
                    View view = null;
                    IEnumerable<View> views = null;
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
                        views = ClientContext.LoadQuery(list.Views.IncludeWithDefaultProperties(v => v.ViewFields));
                        ClientContext.ExecuteQueryRetry();
                        
                    }
                    if (views != null && views.Any())
                    {
                        WriteObject(views,true);
                    }
                    else if (view != null)
                    {
                        WriteObject(view);
                    }
                }
            }

        }
    }

}

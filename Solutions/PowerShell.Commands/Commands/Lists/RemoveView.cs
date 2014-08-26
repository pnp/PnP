using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOView")]
    public class RemoveView : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Title of the list.")]
        public SPOViewPipeBind Identity = new SPOViewPipeBind();

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 1, HelpMessage = "The ID or Url of the list.")]
        public SPOListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (List != null)
            {
                var list = this.SelectedWeb.GetList(List);

                if (list != null)
                {
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
                        else if (Identity.View != null)
                        {
                            view = Identity.View;
                        }
                        if (view != null)
                        {
                            if (Force || ShouldContinue(string.Format(Properties.Resources.RemoveView0, view.Title), Properties.Resources.Confirm))
                            {
                                view.DeleteObject();
                                ClientContext.ExecuteQuery();
                            }
                        }
                    }
                }
            }
        }
    }

}

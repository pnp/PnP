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
    [Cmdlet(VerbsCommon.Get, "SPOList")]
    [CmdletHelp("Returns a List object", DetailedDescription = "Returns a list object. Due to limitation of the PowerShell environment the command does not return a full Client List object. In order to access the full client list, use the ContextObject property of the return lists.")]
    [CmdletExample(Code = "PS:> Get-SPOList", Remarks = "Returns all lists in the current web", SortOrder = 1)]
    [CmdletExample(Code = "PS:> Get-SPOList -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe", Remarks = "Returns a list with the given id.", SortOrder = 2)]
    [CmdletExample(Code = "PS:> Get-SPOList -Identity /Lists/Announcements", Remarks = "Returns a list with the given url.", SortOrder = 3)]
    public class GetList : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Url of the list.")]
        public SPOListPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity != null)
            {
                var list = this.SelectedWeb.GetList(Identity);
                WriteObject(new SPOList(list));

            }
            else
            {
                var lists = this.SelectedWeb.GetLists();
                WriteObject(lists);
            }
        }
    }

}

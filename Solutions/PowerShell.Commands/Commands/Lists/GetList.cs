using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOList")]
    [CmdletHelp("Returns a List object", DetailedDescription = "Returns a list object.", Category = "Lists")]
    [CmdletExample(
        Code = "PS:> Get-SPOList", 
        Remarks = "Returns all lists in the current web", 
        SortOrder = 1)]
    [CmdletExample(
        Code = "PS:> Get-SPOList -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe", 
        Remarks = "Returns a list with the given id.", 
        SortOrder = 2)]
    [CmdletExample(
        Code = "PS:> Get-SPOList -Identity /Lists/Announcements", 
        Remarks = "Returns a list with the given url.", 
        SortOrder = 3)]
    public class GetList : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Url of the list.")]
        public ListPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity != null)
            {
                var list = SelectedWeb.GetList(Identity);
                WriteObject(list);

            }
            else
            {
                var lists = ClientContext.LoadQuery(SelectedWeb.Lists.IncludeWithDefaultProperties(l => l.Id, l => l.BaseTemplate, l => l.OnQuickLaunch, l => l.DefaultViewUrl, l => l.Title, l => l.Hidden));
                ClientContext.ExecuteQueryRetry();
                WriteObject(lists,true);
            }
        }
    }

}

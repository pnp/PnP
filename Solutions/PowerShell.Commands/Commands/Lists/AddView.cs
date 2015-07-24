using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOView")]
    [CmdletHelp("Adds a view to a list", Category = "Lists")]
    [CmdletExample(
        Code = @"Add-SPOView -List ""Demo List"" -Title ""Demo View"" -Fields ""Title"",""Address""",
        SortOrder = 1)]
    public class AddView : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Url of the list.")]
        public ListPipeBind List;

        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = false)]
        public string Query;

        [Parameter(Mandatory = true)]
        public string[] Fields;

        [Parameter(Mandatory = false)]
        public ViewType ViewType = ViewType.None;

        [Parameter(Mandatory = false)]
        public uint RowLimit = 30;

        [Parameter(Mandatory = false)]
        public SwitchParameter Personal;

        [Parameter(Mandatory = false)]
        public SwitchParameter SetAsDefault;

        protected override void ExecuteCmdlet()
        {
            List list = null;
            if (List != null)
            {
                list = List.GetList(SelectedWeb);
            }
            if (list != null)
            {
                var view = list.CreateView(Title, ViewType, Fields, RowLimit, SetAsDefault, Query, Personal);

                WriteObject(view);
            }
        }
    }

}

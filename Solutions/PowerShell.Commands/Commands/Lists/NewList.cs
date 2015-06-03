using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "SPOList")]
    [CmdletHelp("Creates a new list", Category = "Lists")]
    [CmdletExample(Code = "PS:> New-SPOList -Title Announcements -Template Announcements", SortOrder = 1)]
    [CmdletExample(Code = @"PS:> New-SPOList -Title ""Demo List"" -Url ""DemoList"" -Template Announcements", SortOrder = 2, Remarks = "Create a list with a title that is different from the url")]
    public class NewList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = true, HelpMessage = "The type of list to create.")]
        public ListTemplateType Template;

        [Parameter(Mandatory = false, HelpMessage = "If set, will override the url of the list.")]
        public string Url = null;

        [Parameter(Mandatory = false)]
        public SwitchParameter EnableVersioning;

        [Parameter(Mandatory = false, HelpMessage = "Obsolete", DontShow = true)]
        public QuickLaunchOptions QuickLaunchOptions;

        [Parameter(Mandatory = false)]
        public SwitchParameter EnableContentTypes;

        [Parameter(Mandatory = false)]
        public SwitchParameter OnQuickLaunch;

        protected override void ExecuteCmdlet()
        {
            var list = SelectedWeb.CreateList(Template, Title, EnableVersioning, true, Url, EnableContentTypes);
            if (OnQuickLaunch)
            {
                list.OnQuickLaunch = true;
                list.Update();
                ClientContext.ExecuteQueryRetry();
            }
        }
    }

}

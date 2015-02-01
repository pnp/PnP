using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "SPOWeb")]
    [CmdletHelp("Creates a new subweb to the current web")]
    [CmdletExample(Code = @"
PS:> New-SPOWeb -Title ""Project A Web"" -Url projectA -Description ""Information about Project A"" -Locale 1033 -Template ""STS#0""", Remarks = "Creates a new subweb under the current web with url projectA", SortOrder = 1)]
    public class NewWeb : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage="The title of the new web")]
        public string Title;

        [Parameter(Mandatory = true, HelpMessage="The Url of the new web")]
        public string Url;

        [Parameter(Mandatory = false, HelpMessage="The description of the new web")]
        public string Description = string.Empty;

        [Parameter(Mandatory = false)]
        public int Locale = 1033;

        [Parameter(Mandatory = true, HelpMessage="The site definition template to use for the new web, e.g. STS#0")]
        public string Template = string.Empty;

        [Parameter(Mandatory = false, HelpMessage="By default the subweb will inherit its security from its parent, specify this switch to break this inheritance")]
        public SwitchParameter BreakInheritance = false;

        [Parameter(Mandatory = false, HelpMessage="Specifies whether the site inherits navigation.")]
        public SwitchParameter InheritNavigation = true;
        protected override void ExecuteCmdlet()
        {
            var web = SelectedWeb.CreateWeb(Title, Url, Description, Template, Locale, !BreakInheritance,InheritNavigation);
            ClientContext.Load(web, w => w.Id, w => w.Url);
            ClientContext.ExecuteQuery();
            WriteObject(web);
        }

    }
}

using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
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
    [Cmdlet(VerbsCommon.New, "SPOList")]
    [CmdletHelp("Creates a new list")]
    [CmdletExample(Code = "PS:> New-SPOList -Title Announcements -Template Announcements", SortOrder = 1)]
    [CmdletExample(Code = "PS:> New-SPOList -Title \"Demo List\" -Url \"DemoList\" -Template Announcements", SortOrder = 2, Remarks = "Create a list with a title that is different from the url")]
    public class NewList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = true, HelpMessage = "The type of list to create.")]
        public ListTemplateType Template;

        [Parameter(Mandatory = false, HelpMessage = "If set, will override the url of the list.")]
        public string Url = null;

        [Parameter(Mandatory = false, HelpMessage = "The description of the list.")]
        public string Description = null;

        [Parameter(Mandatory = false)]
        public QuickLaunchOptions QuickLaunchOptions;

        protected override void ExecuteCmdlet()
        {
            SPO.SPOList.CreateList(Title, Description, Url, Template, this.SelectedWeb, QuickLaunchOptions);
        }
    }

}

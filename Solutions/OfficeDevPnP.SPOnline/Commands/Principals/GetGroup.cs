using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.SPOnline.Commands.Principals
{
    [Cmdlet(VerbsCommon.Get, "SPOGroup")]
    [CmdletHelp("Returns a specific group or all groups.")]
    [CmdletExample(Code = @"
PS:> Get-SPOGroup
", SortOrder = 1)]
    [CmdletExample(Code = @"
PS:> Get-SPOGroup -Name 'Site Members'
", SortOrder = 2)]
    public class GetGroup : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The name of the group")]
        public string Name = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (string.IsNullOrEmpty(Name))
            {
                var groups  = ClientContext.LoadQuery(this.SelectedWeb.SiteGroups.IncludeWithDefaultProperties(g => g.Users));
                ClientContext.ExecuteQuery();
                WriteObject(groups);
            }
            else
            {
                var group = this.SelectedWeb.SiteGroups.GetByName(Name);

                ClientContext.Load(group);
                ClientContext.Load(group.Users);

                ClientContext.ExecuteQuery();

                WriteObject(group);
            }
        }
    }



}

using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using System.Management.Automation;

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
    public class GetGroup : SPOCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The name of the group")]
        public string Name = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (string.IsNullOrEmpty(Name))
            {
                WriteObject(SPOnline.Core.SPOGroup.GetGroups(ClientContext.Web));
            }
            else
            {
                WriteObject(SPOnline.Core.SPOGroup.GetGroup(Name, ClientContext.Web));
            }
        }
    }



}

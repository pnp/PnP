using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands.Principals
{
    [Cmdlet(VerbsCommon.Remove, "SPOUserFromGroup")]
    [CmdletHelp("Removes a user from a group")]
    [CmdletExample(Code = @"
PS:> Remove-SPOUserFromGroup -LogonName user@company.com -GroupName 'Marketing Site Members'
")]
    public class RemoveUserFromGroup : SPOCmdlet
    {

        [Parameter(Mandatory = true, HelpMessage = "A valid logon name of a user")]
        public string LogonName = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "A valid group name")]
        public string GroupName = string.Empty;

        protected override void ExecuteCmdlet()
        {
            SPOnline.Core.SPOGroup.RemoveUserFromGroup(LogonName, GroupName, ClientContext.Web);

        }
    }
}

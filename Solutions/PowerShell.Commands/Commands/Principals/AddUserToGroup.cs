using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Add, "SPOUserToGroup")]
    [CmdletHelp("Adds a user to a group", Category = "User and group management")]
    [CmdletExample(
        Code = @"PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 'Marketing Site Members'",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 5",
        Remarks = "Add the specified user to the group with Id 5",
        SortOrder = 2)]
    public class AddUserToGroup : SPOWebCmdlet
    {

        [Parameter(Mandatory = true, HelpMessage = "The login name of the user")]
        [Alias("LogonName")]
        public string LoginName;

        [Parameter(Mandatory = true, HelpMessage = "The group id, group name or group object to add the user to.", ValueFromPipeline = true)]
        public GroupPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var group = Identity.GetGroup(SelectedWeb);

            SelectedWeb.AddUserToGroup(group, LoginName);
        }
    }
}

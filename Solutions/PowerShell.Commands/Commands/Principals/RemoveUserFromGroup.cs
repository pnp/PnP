using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Remove, "SPOUserFromGroup")]
    [CmdletHelp("Removes a user from a group", Category = "User and group management")]
    [CmdletExample(
        Code = @"PS:> Remove-SPOUserFromGroup -LoginName user@company.com -GroupName 'Marketing Site Members'",
        SortOrder = 1)]
    public class RemoveUserFromGroup : SPOWebCmdlet
    {

        [Parameter(Mandatory = true, HelpMessage = "A valid login name of a user")]
        [Alias("LogonName")]
        public string LoginName = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "A valid group name")]
        public string GroupName = string.Empty;

        protected override void ExecuteCmdlet()
        {
            try
            {
                Group group = SelectedWeb.SiteGroups.GetByName(GroupName);
                User user = SelectedWeb.SiteUsers.GetByEmail(LoginName);
                ClientContext.Load(user);
                ClientContext.Load(group);
                ClientContext.ExecuteQueryRetry();
                SelectedWeb.RemoveUserFromGroup(group, user);
            }
            catch
            {
                Group group = SelectedWeb.SiteGroups.GetByName(GroupName);
                User user = SelectedWeb.SiteUsers.GetByLoginName(LoginName);
                ClientContext.Load(user);
                ClientContext.Load(group);
                ClientContext.ExecuteQueryRetry();
                SelectedWeb.RemoveUserFromGroup(group, user);
            }
        }
    }
}

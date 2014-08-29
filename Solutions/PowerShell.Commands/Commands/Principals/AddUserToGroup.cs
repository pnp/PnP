using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Add, "SPOUserToGroup")]
    [CmdletHelp("Adds a user to a group")]
    [CmdletExample(Code = @"
    PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 'Marketing Site Members'
    ")]
    [CmdletExample(Code = @"
    PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 5
    ", Remarks = "Add the specified user to the group with Id 5")]
    public class AddUserToGroup : SPOWebCmdlet
    {

        [Parameter(Mandatory = true, HelpMessage = "The login name of the user")]
        [Alias("LogonName")]
        public string LoginName;

        [Parameter(Mandatory = true, HelpMessage = "The group id, group name or group object to add the user to.", ValueFromPipeline = true)]
        public GroupPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Id != -1)
            {
                this.SelectedWeb.AddUserToGroup(Identity.Id, LoginName);
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                this.SelectedWeb.AddUserToGroup(Identity.Name, LoginName);
            }
            else if (Identity.Group != null)
            {
                this.SelectedWeb.AddUserToGroup(Identity.Group, LoginName);
            }
        }
    }
}

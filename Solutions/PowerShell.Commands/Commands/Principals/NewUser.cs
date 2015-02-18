using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet("New", "SPOUser")]
    [CmdletHelp("Adds a user to the build-in Site User Info List and returns a user object")]
    [CmdletExample(Code = @"
PS:> New-SPOUser -LogonName user@company.com
")]
    public class NewUser : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        [Alias("LogonName")]
        public string LoginName = string.Empty;

        protected override void ExecuteCmdlet()
        {
            var user = SelectedWeb.EnsureUser(LoginName);
            ClientContext.Load(user, u => u.Email, u => u.Id, u => u.IsSiteAdmin, u => u.Groups, u => u.PrincipalType, u => u.Title, u => u.IsHiddenInUI, u => u.UserId, u => u.LoginName);
            ClientContext.ExecuteQueryRetry();
            WriteObject(user);
        }
    }
}

using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands.Principals
{
    [Cmdlet("New", "SPOUser")]
    [CmdletHelp("Adds a user to the build-in Site User Info List and returns a user object")]
    [CmdletExample(Code = @"
PS:> New-SPOUser -LogonName user@company.com
")]
    public class NewUser : SPOCmdlet
    {
        [Parameter(Mandatory = true)]
        public string LogonName = string.Empty;

        protected override void ExecuteCmdlet()
        {
            WriteObject(SPOnline.Core.SPOUser.EnsureUser(LogonName, ClientContext.Web));
        }
    }
}

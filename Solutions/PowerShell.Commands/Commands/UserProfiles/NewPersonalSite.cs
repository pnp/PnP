using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;

#if !CLIENTSDKV15
namespace OfficeDevPnP.PowerShell.Commands.UserProfiles
{

    [Cmdlet(VerbsCommon.New, "SPOPersonalSite")]
    [CmdletHelp(@"Office365 only: Creates a personal / OneDrive For Business site", Category = "User Profiles")]

    public class NewPersonalSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The email address of the user", Position = 0)]
        public string[] Email;

        protected override void ExecuteCmdlet()
        {
            ProfileLoader profileLoader = ProfileLoader.GetProfileLoader(ClientContext);
            profileLoader.CreatePersonalSiteEnqueueBulk(Email);
            ClientContext.ExecuteQueryRetry();
        }
    }
}
#endif
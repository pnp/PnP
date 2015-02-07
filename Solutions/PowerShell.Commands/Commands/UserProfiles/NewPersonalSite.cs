using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using OfficeDevPnP.PowerShell.Commands.Base;

#if !CLIENTSDKV15
namespace OfficeDevPnP.PowerShell.Commands.UserProfiles
{

    [Cmdlet(VerbsCommon.New, "SPOPersonalSite")]
    public class NewPersonalSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The email address of the user", Position=0)]
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
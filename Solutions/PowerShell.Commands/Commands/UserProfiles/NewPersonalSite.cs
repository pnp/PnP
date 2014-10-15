using Microsoft.SharePoint.Client.UserProfiles;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

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
            ClientContext.ExecuteQuery();
        }
    }
}
#endif
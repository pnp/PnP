using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;

namespace OfficeDevPnP.PowerShell.Commands.UserProfiles
{
    [Cmdlet(VerbsCommon.Get, "SPOUserProfileProperty")]
    [CmdletHelp(@"Office365 only: Uses the tenant API to retrieve site information.

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Requires a connection to a SharePoint Tenant Admin site.")]
    [CmdletExample(Code = @"
PS:> Get-SPOUserProfileProperty -Account 'user@domain.com'", Remarks = "Returns the profile properties for the specified user")]
    [CmdletExample(Code = @"
PS:> Get-SPOUserProfileProperty -Account 'user@domain.com','user2@domain.com'", Remarks = "Returns the profile properties for the specified users")]
    public class GetUserProfileProperty : SPOAdminCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The account of the user, formatted either as a login name, or as a claims identity, e.g. i:0#.f|membership|user@domain.com", Position = 0)]
        public string[] Account;

        protected override void ExecuteCmdlet()
        {
            var peopleManager = new PeopleManager(ClientContext);

            foreach (var acc in Account)
            {
                var result = Tenant.EncodeClaim(acc);
                ClientContext.ExecuteQueryRetry();
                var properties = peopleManager.GetPropertiesFor(result.Value);
                ClientContext.Load(properties);
                ClientContext.ExecuteQueryRetry();
                WriteObject(properties);
            }
        }
    }
}

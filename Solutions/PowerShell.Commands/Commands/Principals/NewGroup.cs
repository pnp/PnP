using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet("New", "SPOGroup")]
    [CmdletHelp("Adds a user to the build-in Site User Info List and returns a user object")]
    [CmdletExample(Code = @"
PS:> New-SPOUser -LogonName user@company.com
")]
    public class NewGroup : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Title = string.Empty;

        [Parameter(Mandatory = false)]
        public string Description;

        [Parameter(Mandatory = false)]
        public string Owner;

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowRequestToJoinLeave;

        [Parameter(Mandatory = false)]
        public SwitchParameter AutoAcceptRequestToJoinLeave;



        protected override void ExecuteCmdlet()
        {
            User groupOwner = null;
            if (!string.IsNullOrEmpty(Owner))
            {
                groupOwner = this.SelectedWeb.EnsureUser(Owner);
            }
            GroupCreationInformation groupCI = new GroupCreationInformation();
            groupCI.Title = Title;
            groupCI.Description = Description;

            var group = this.SelectedWeb.SiteGroups.Add(groupCI);

            ClientContext.Load(group);
            ClientContext.Load(group.Users);
            ClientContext.ExecuteQuery();

            if (AllowRequestToJoinLeave)
                group.AllowRequestToJoinLeave = true;

            if (AutoAcceptRequestToJoinLeave)
                group.AutoAcceptRequestToJoinLeave = true;

            if (!string.IsNullOrEmpty(Owner))
                group.Owner = groupOwner;

            group.Update();
            ClientContext.ExecuteQuery();
            WriteObject(group);

            
        }
    }
}

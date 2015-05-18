using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOCustomAction")]
    [CmdletHelp("Adds a custom action to a web", Category =  "Branding")]
    public class AddCustomAction : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Title = string.Empty;

        [Parameter(Mandatory = true)]
        public string Description = string.Empty;

        [Parameter(Mandatory = true)]
        public string Group = string.Empty;

        [Parameter(Mandatory = true)]
        public string Location = string.Empty;

        [Parameter(Mandatory = true)]
        public int Sequence = 0;

        [Parameter(Mandatory = true)]
        public string Url = string.Empty;

        [Parameter(Mandatory = false)]
        public List<PermissionKind> Rights = new List<PermissionKind>();

        [Parameter(Mandatory = false)]
        public CustomActionScope Scope = CustomActionScope.Web;


        protected override void ExecuteCmdlet()
        {
            var permissions = new BasePermissions();
            foreach (var kind in Rights)
            {
                permissions.Set(kind);
            }
            var ca = new CustomActionEntity { Description = Description, Location = Location, Group = Group, Sequence = Sequence, Title = Title, Url = Url, Rights = new BasePermissions() };

            foreach (var permission in Rights)
            {
                ca.Rights.Set(permission);
            }

            if (Scope == CustomActionScope.Web)
            {
                SelectedWeb.AddCustomAction(ca);
            }
            else
            {
                ClientContext.Site.AddCustomAction(ca);
            }
        }
    }
}

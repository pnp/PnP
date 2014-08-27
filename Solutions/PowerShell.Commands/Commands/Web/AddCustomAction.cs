using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.Core.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOCustomAction")]
    [CmdletHelp("Adds a custom action to a web")]
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

        protected override void ExecuteCmdlet()
        {
            BasePermissions permissions = new BasePermissions();
            foreach (PermissionKind kind in Rights)
            {
                permissions.Set(kind);
            }
            CustomActionEntity ca = new CustomActionEntity();
            ca.Description = Description;
            ca.Location = Location;
            ca.Group = Group;
            ca.Sequence = Sequence;
            ca.Title = Title;
            ca.Url = Url;
            ca.Rights = new BasePermissions();

            foreach(var permission in Rights)
            {
                ca.Rights.Set(permission);
            }

            this.SelectedWeb.AddCustomAction(ca);
        }
    }
}

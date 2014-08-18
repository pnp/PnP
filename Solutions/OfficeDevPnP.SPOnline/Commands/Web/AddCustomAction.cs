using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOCustomAction")]
    [CmdletHelp("Adds a custom action to a web")]
    public class AddCustomAction : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name = string.Empty;

        [Parameter(Mandatory = true)]
        public string Group = string.Empty;

        [Parameter(Mandatory = true)]
        public string Location = string.Empty;

        [Parameter(Mandatory = true)]
        public int Sequence = 0;

        [Parameter(Mandatory = true)]
        public string Title = string.Empty;

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
            SPOnline.Core.SPOWeb.AddCustomAction(this.SelectedWeb, Title, Group, Location, Name, Sequence, Url, permissions, ClientContext);
        }
    }
}

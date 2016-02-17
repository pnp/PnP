// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;
using System.ComponentModel.Design;

namespace Perficient.Provisioning.VSTools
{
    static class PkgCmdIDList
    {
        private const uint cmdidDeployItemWithPnP = 0x100;
        private const uint cmdidPnPToolsToggle = 0x101;
        private const uint cmdidEditConnection = 0x102;
        private const uint cmdidDeployFolderWithPnP = 0x103;

        public static CommandID DeployItemCommandID = new CommandID(GuidList.guidProvisioning_VSToolsCmdSet, (int)cmdidDeployItemWithPnP);
        public static CommandID DeployFolderCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningFolderCmdSet, (int)cmdidDeployFolderWithPnP);

        public static CommandID ToggleToolsCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningProjectCmdSet, (int)cmdidPnPToolsToggle);
        public static CommandID EditConnCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningProjectCmdSet, (int)cmdidEditConnection);
    };
}
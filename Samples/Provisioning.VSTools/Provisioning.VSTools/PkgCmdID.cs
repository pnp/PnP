// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;
using System.ComponentModel.Design;

namespace Perficient.Provisioning.VSTools
{
    static class PkgCmdIDList
    {
        //public const uint cmdidHackathon = 0x100;
        //public const uint cmdidPnPToolsToggle = 0x101;
        //public const uint cmdidProvisionConfig = 0x102;

        //public const uint cmdidDeployFolderWithPNP = 0x103;

        public const uint cmdidDeployItemWithPnP = 0x100;
        public const uint cmdidPnPToolsToggle = 0x101;
        public const uint cmdidEditConnection = 0x102;
        public const uint cmdidDeployFolderWithPnP = 0x103;

        public static CommandID DeployItemCommandID = new CommandID(GuidList.guidProvisioning_VSToolsCmdSet, (int)cmdidDeployItemWithPnP);
        public static CommandID DeployFolderCommandID = new CommandID(GuidList.guidProvisioning_VSToolsCmdSet, (int)cmdidDeployFolderWithPnP);

        public static CommandID ToggleToolsCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningProjectCmdSet, (int)cmdidPnPToolsToggle);
        public static CommandID EditConnCommandID = new CommandID(GuidList.guidPnPTemplateProvisioningProjectCmdSet, (int)cmdidEditConnection);
    };
}
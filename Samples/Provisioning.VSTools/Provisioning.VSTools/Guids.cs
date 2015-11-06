// Guids.cs
// MUST match guids.h
using System;

namespace Perficient.Provisioning.VSTools
{
    static class GuidList
    {
        public const string guidProvisioning_VSToolsPkgString = "627e9c4a-f6e1-4391-96f0-3164202e10e3";
        public const string guidProvisioning_VSToolsCmdSetString = "52b48f09-8811-4ede-b47f-a305ce95e2bf";
        public const string guidToolWindowPersistanceString = "217ba122-45b3-4dac-803c-ea71882d229c";
        public const string guidPnPTemplateProvisioningProjectCmdSetString = "a1ae027b-0948-4098-ad4e-b03a7b622856";
        public const string guidPnPTemplateProvisioningFolderCmdSetString = "8ab0a17b-a1dd-4d7e-989d-10961175ce7c";

        public static readonly Guid guidProvisioning_VSToolsCmdSet = new Guid(guidProvisioning_VSToolsCmdSetString);
        public static readonly Guid guidPnPTemplateProvisioningProjectCmdSet = new Guid(guidPnPTemplateProvisioningProjectCmdSetString);
        public static readonly Guid guidPnPTemplateProvisioningFolderCmdSet = new Guid(guidPnPTemplateProvisioningFolderCmdSetString);

    };
}
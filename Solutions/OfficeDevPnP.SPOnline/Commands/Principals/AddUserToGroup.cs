using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands.Principals
{
    [Cmdlet(VerbsCommon.Add, "SPOUserToGroup")]
    [CmdletHelp("Adds a user to a group")]
    [CmdletExample(Code = @"
    PS:> Add-SPOUserToGroup -LogonName user@company.com -Identity 'Marketing Site Members'
    ")]
    [CmdletExample(Code = @"
    PS:> Add-SPOUserToGroup -LogonName user@company.com -Identity 5
    ", Remarks = "Add the specified user to the group with Id 5")]
    public class AddUserToGroup : SPOCmdlet
    {

        [Parameter(Mandatory = true, HelpMessage = "The logon name of the user")]
        public string LogonName;

        [Parameter(Mandatory = true, HelpMessage = "The group id, or group object to add the user to.", ValueFromPipeline = true)]
        public GroupPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Id != -1)
            {
                SPOnline.Core.SPOGroup.AddUserToGroup(LogonName, Identity.Id, ClientContext.Web);
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                SPOnline.Core.SPOGroup.AddUserToGroup(LogonName, Identity.Name, ClientContext.Web);
            }
            else if (Identity.Group != null)
            {
                SPOnline.Core.SPOGroup.AddUserToGroup(LogonName, Identity.Group);
            }

        }
    }
}

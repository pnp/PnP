using OfficeDevPnP.SPOnline.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOAppSideLoading")]
    public class SetAppSideLoading : SPOCmdlet
    {
        [Parameter(ParameterSetName = "On", Mandatory = true)]
        public SwitchParameter On;

        [Parameter(ParameterSetName = "Off", Mandatory = true)]
        public SwitchParameter Off;
        protected override void ExecuteCmdlet()
        {
            if (On)
            {
                OfficeDevPnP.SPOnline.Core.SPOSite.EnableAppSideLoading(ClientContext);
            }
            else
            {
                OfficeDevPnP.SPOnline.Core.SPOSite.DisableAppSideLoading(ClientContext);
            }
        }

    }
}

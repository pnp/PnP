using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOSubWebs")]
    public class GetSubWebs : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline=true, Position=0)]
        public WebPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var webs = ClientContext.LoadQuery(this.SelectedWeb.Webs);
            ClientContext.ExecuteQuery();
            WriteObject(webs, true);

        }

    }
}

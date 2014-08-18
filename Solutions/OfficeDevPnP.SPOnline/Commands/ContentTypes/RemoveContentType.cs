using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOContentType")]
    public class RemoveContentType : SPOWebCmdlet
    {

        [Parameter(Mandatory = true)]
        public SPOContentTypePipeBind Identity;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (Force || ShouldContinue(Properties.Resources.RemoveContentType, Properties.Resources.Confirm))
            {
                if (!string.IsNullOrEmpty(Identity.Id))
                {
                    SPOnline.Core.SPOContentType.RemoveContentTypeById(Identity.Id, SelectedWeb, ClientContext);
                }
                else
                {
                    SPOnline.Core.SPOContentType.RemoveContentTypeByName(Identity.Id, SelectedWeb, ClientContext);
                }
            }
        }
    }
}

using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.PowerShell.Commands.Entities;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Add, "SPOContentTypeToList")]
    [CmdletHelp("Adds a new content type to a list")]
    public class AddContentTypeToList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public SPOListPipeBind List;

        [Parameter(Mandatory = true)]
        public SPOContentTypePipeBind ContentType;

        [Parameter(Mandatory = false)]
        public SwitchParameter DefaultContentType;

        protected override void ExecuteCmdlet()
        {
            ContentType ct = null;
            List list = this.SelectedWeb.GetList(List);

            if (ContentType.ContentType == null)
            {
                if (ContentType.Id != null)
                {
                    ct = this.SelectedWeb.GetContentTypeById(ContentType.Id);
                }
                else if (ContentType.Name != null)
                {
                    ct = this.SelectedWeb.GetContentTypeByName(ContentType.Name);
                }
            }
            else
            {
                ct = ContentType.ContentType;
            }
            if (ct != null)
            {
                this.SelectedWeb.AddContentTypeToList(list.Title, ct, DefaultContentType);
            }
        }

    }
}

using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using OfficeDevPnP.PowerShell.Commands.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOContentType")]
    public class GetContentType : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public SPOContentTypePipeBind Identity;

        protected override void ExecuteCmdlet()
        {

            if (Identity != null)
            {
                ContentType ct = null;
                if (!string.IsNullOrEmpty(Identity.Id))
                {
                    ct = this.SelectedWeb.GetContentTypeById(Identity.Id);
                }
                else
                {
                    ct = this.SelectedWeb.GetContentTypeByName(Identity.Name);
                }
                if (ct != null)
                {

                    WriteObject(new ContentTypeEntity(ct));
                }
            }
            else
            {
                List<ContentType> cts = new List<ContentType>();
                ClientContext.Load(this.SelectedWeb.ContentTypes);
                ClientContext.ExecuteQuery();

                var spocts = from ct in this.SelectedWeb.ContentTypes select new ContentTypeEntity(ct);
                WriteObject(spocts, true);
            }
        }
    }
}

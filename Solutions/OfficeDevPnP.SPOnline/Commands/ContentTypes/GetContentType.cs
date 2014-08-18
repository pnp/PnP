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
    [Cmdlet(VerbsCommon.Get, "SPOContentType")]
    public class GetContentType : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public SPOContentTypePipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var cts = from ct in SPOnline.Core.SPOContentType.GetContentTypes(this.SelectedWeb, ClientContext)
                      select new SPOContentType(ct);
            if (Identity != null)
            {
                if (!string.IsNullOrEmpty(Identity.Id))
                {
                    var ct = from c in cts where c.Id.ToLower() == Identity.Id.ToLower() select c;
                    if (ct.FirstOrDefault() != null)
                    {
                        WriteObject(ct.FirstOrDefault());
                    }
                }
                else
                {
                    var ct = from c in cts where c.Name.ToLower() == Identity.Name.ToLower() select c;
                    if (ct.FirstOrDefault() != null)
                    {
                        WriteObject(ct.FirstOrDefault());
                    }
                }
            }
            else
            {
                WriteObject(cts, true);
            }
        }
    }
}

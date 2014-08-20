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

    [Cmdlet(VerbsCommon.Add, "SPOFieldToContentType")]
    public class AddFieldToContentType : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public Field Field;

        [Parameter(Mandatory = true)]
        public SPOContentTypePipeBind ContentType;

        [Parameter(Mandatory = false)]
        public SwitchParameter Required;

        [Parameter(Mandatory = false)]
        public SwitchParameter Hidden;

        protected override void ExecuteCmdlet()
        {
            if (ContentType.ContentType != null)
            {
                this.SelectedWeb.AddFieldToContentType(ContentType.ContentType, Field, Required, Hidden);
            }
            else if (!string.IsNullOrEmpty(ContentType.Id))
            {
                //var cts = SPOnline.Core.SPOContentType.GetContentTypes(this.SelectedWeb, ClientContext);

                if (!string.IsNullOrEmpty(ContentType.Id))
                {
                    var ct = this.SelectedWeb.GetContentTypeById(ContentType.Id);
                    if (ct != null)
                    {
                        this.SelectedWeb.AddFieldToContentType(ct, Field, Required, false);
                    }
                }
                else
                {
                    var ct = this.SelectedWeb.GetContentTypeByName(ContentType.Name);
                    if (ct != null)
                    {
                        this.SelectedWeb.AddFieldToContentType(ct, Field, Required, Hidden);
                    }
                }
            }
        }


    }
}

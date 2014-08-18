using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOSubWebs")]
    public class GetSubWebs : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public SPOWebPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var webs = SPOWeb.GetSubWebs(this.SelectedWeb, ClientContext).AsQueryable();
            var query = from web in webs
                        select new SPOnlineWeb(web);
            WriteObject(query.ToList(), true);

        }

    }
}

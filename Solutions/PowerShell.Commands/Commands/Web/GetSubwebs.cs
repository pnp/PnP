using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Core;
using OfficeDevPnP.PowerShell.Commands.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOSubWebs")]
    public class GetSubWebs : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public SPOWebPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            List<Web> webs = new List<Web>();
            ClientContext.Load(this.SelectedWeb.Webs);

            ClientContext.ExecuteQuery();
            foreach (var w in this.SelectedWeb.Webs)
            {
                webs.Add(w);
            }
            
            var query = from web in webs
                        select new WebEntity(web);
            WriteObject(query.ToList(), true);

        }

    }
}

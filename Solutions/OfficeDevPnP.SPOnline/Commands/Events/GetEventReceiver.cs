using OfficeDevPnP.SPOnline.Core;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOEventReceiver")]
    public class GetEventReceiver : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "List")]
        public SPOListPipeBind List;

        //[Parameter(Mandatory = false, ParameterSetName = "Web")]
        //public SPOWebPipeBind Web;

        [Parameter(Mandatory = false)]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "List")
            {
                Microsoft.SharePoint.Client.List list = null;

                if (List.List != null)
                {
                    list = List.List;
                }
                else if (List.Id != Guid.Empty)
                {
                    list = SPO.SPOList.GetListById(List.Id, SelectedWeb, ClientContext);
                }
                else if (!string.IsNullOrEmpty(List.Title))
                {
                    list = SPO.SPOList.GetListByTitle(List.Title, SelectedWeb, ClientContext);
                }

                if (list != null)
                {
                    if (Identity == null)
                    {
                        WriteObject(SPOEvents.GetEventReceivers(list, ClientContext));
                    }
                    else
                    {

                        WriteObject(SPOEvents.GetEventReceivers(list, Identity.Id, ClientContext));
                    }
                }
            }
            else
            {
                Microsoft.SharePoint.Client.Web web = SelectedWeb;
                if (Web != null)
                {
                    if (Web.Web != null)
                    {
                        web = Web.Web;
                    }
                    else if (Web.Id != Guid.Empty)
                    {
                        web = ClientContext.Site.OpenWebById(Web.Id);
                        ClientContext.Load(web);
                        ClientContext.ExecuteQuery();
                    }
                    else if (!string.IsNullOrEmpty(Web.Url))
                    {
                        web = ClientContext.Site.OpenWeb(Web.Url);
                        ClientContext.Load(web);
                        ClientContext.ExecuteQuery();
                    }
                }
                if (web != null)
                {
                    if (Identity == null)
                    {
                        WriteObject(SPOEvents.GetEventReceivers(web, ClientContext));
                    }
                    else
                    {

                        WriteObject(SPOEvents.GetEventReceivers(web, Identity.Id, ClientContext));
                    }
                }
            }

        }
    }
}



using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOEventReceiver")]
    [CmdletHelp("Returns all or a specific event receiver", Category = "Event Receivers")]
    [CmdletExample(
      Code = @"PS:> Get-SPOEventReceiver",
      Remarks = @"This will return all registered event receivers on the current web", SortOrder = 1)]
    [CmdletExample(
      Code = @"PS:> Get-SPOEventReceiver -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22",
      Remarks = @"This will return a specific registered event receivers from the current web", SortOrder = 2)]
    [CmdletExample(
      Code = @"PS:> Get-SPOEventReceiver -List ""ProjectList""",
      Remarks = @"This will return all registered event receivers in the list with the name ProjectList", SortOrder = 3)]
    [CmdletExample(
      Code = @"PS:> Get-SPOEventReceiver -List ""ProjectList"" -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22",
      Remarks = @"This will return a specific registered event receiver in the list with the name ProjectList", SortOrder = 4)]
    public class GetEventReceiver : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "List")]
        public ListPipeBind List;

        [Parameter(Mandatory = false)]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "List")
            {
                var list = List.GetList(SelectedWeb);

                if (list != null)
                {
                    if (Identity == null)
                    {
                        var query = ClientContext.LoadQuery(list.EventReceivers);
                        ClientContext.ExecuteQueryRetry();
                        WriteObject(query, true);
                    }
                    else
                    {
                        WriteObject(list.GetEventReceiverById(Identity.Id));
                    }
                }
            }
            else
            {
                if (Identity == null)
                {
                    var query = ClientContext.LoadQuery(SelectedWeb.EventReceivers);
                    ClientContext.ExecuteQueryRetry();
                    WriteObject(query, true);
                }
                else
                {
                    WriteObject(SelectedWeb.GetEventReceiverById(Identity.Id));
                }
            }

        }
    }
}



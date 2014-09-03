using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;


namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOList")]
    public class RemoveList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Title of the list.")]
        public ListPipeBind Identity = new ListPipeBind();

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;
        protected override void ExecuteCmdlet()
        {
            if (Identity != null)
            {
                var list = this.SelectedWeb.GetList(Identity);
                if (list != null)
                {
                    if (Force || ShouldContinue(Properties.Resources.RemoveList, Properties.Resources.Confirm))
                    {
                        list.DeleteObject();
                        ClientContext.ExecuteQuery();
                    }
                }
            }
        }
    }

}

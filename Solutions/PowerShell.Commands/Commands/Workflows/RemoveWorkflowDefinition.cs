using System;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsCommon.Remove, "SPOWorkflowDefinition")]
    public class RemoveWorkflowDefinition : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The subscription to remove", Position = 0)]
        public WorkflowDefinitionPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Definition != null)
            {
                Identity.Definition.Delete();
            }
            else if (Identity.Id != Guid.Empty)
            {
                var definition = SelectedWeb.GetWorkflowDefinition(Identity.Id);
                if (definition != null)
                    definition.Delete();
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                var definition = SelectedWeb.GetWorkflowDefinition(Identity.Name);
                if (definition != null)
                    definition.Delete();
            }
        }
    }

}

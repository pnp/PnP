using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsCommon.Add, "SPOWorkflowSubscription")]
    public class AddWorkflowSubscription : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The name of the subscription")]
        public string Name;

        [Parameter(Mandatory = true, HelpMessage = "The name of the workflow definition")]
        public string DefinitionName;

        [Parameter(Mandatory = true, HelpMessage = "The list to add the workflow to")]
        public ListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter StartManually = true;

        [Parameter(Mandatory = false)]
        public SwitchParameter StartOnCreated;
        
        [Parameter(Mandatory = false)]
        public SwitchParameter StartOnChanged;

        [Parameter(Mandatory = true)]
        public string HistoryListName;

        [Parameter(Mandatory = true)]
        public string TaskListName;

        [Parameter(Mandatory = false)]
        public Dictionary<string,string> AssociationValues;
        
        protected override void ExecuteCmdlet()
        {
            var list = SelectedWeb.GetList(List);

            list.AddWorkflowSubscription(DefinitionName,Name,StartManually,StartOnCreated,StartOnChanged,HistoryListName,TaskListName, AssociationValues);
        }
    }

}

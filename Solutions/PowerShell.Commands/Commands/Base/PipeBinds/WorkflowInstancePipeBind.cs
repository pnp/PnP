using System;
using Microsoft.SharePoint.Client.WorkflowServices;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class WorkflowInstancePipeBind
    {
        private readonly WorkflowInstance _instance;
        private readonly Guid _id;

        public WorkflowInstancePipeBind()
        {
            _instance = null;
            _id = Guid.Empty;
        }

        public WorkflowInstancePipeBind(WorkflowInstance instance)
        {
            _instance = instance;
        }

        public WorkflowInstancePipeBind(Guid guid)
        {
            _id = guid;
        }

        public WorkflowInstancePipeBind(string id)
        {
            _id = Guid.Parse(id);
        }

        public Guid Id
        {
            get { return _id; }
        }

        public WorkflowInstance Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}

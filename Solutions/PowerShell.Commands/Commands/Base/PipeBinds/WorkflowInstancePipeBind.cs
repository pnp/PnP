using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class WorkflowInstancePipeBind
    {
        private WorkflowInstance _instance;
        private Guid _id;

        public WorkflowInstancePipeBind()
        {
            _instance = null;
            _id = Guid.Empty;
        }

        public WorkflowInstancePipeBind(WorkflowInstance instance)
        {
            this._instance = instance;
        }

        public WorkflowInstancePipeBind(Guid guid)
        {
            this._id = guid;
        }

        public WorkflowInstancePipeBind(string id)
        {
            this._id = Guid.Parse(id);
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

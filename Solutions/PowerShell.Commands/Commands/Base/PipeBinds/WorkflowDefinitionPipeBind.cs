using System;
using Microsoft.SharePoint.Client.WorkflowServices;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class WorkflowDefinitionPipeBind
    {
        private readonly WorkflowDefinition _def;
        private readonly Guid _id;
        private readonly string _name;

        public WorkflowDefinitionPipeBind()
        {
            _def = null;
            _id = Guid.Empty;
            _name = string.Empty;
        }

        public WorkflowDefinitionPipeBind(WorkflowDefinition def)
        {
            _def = def;
        }

        public WorkflowDefinitionPipeBind(Guid guid)
        {
            _id = guid;
        }

        public WorkflowDefinitionPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                _name = id;
            }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public WorkflowDefinition Definition
        {
            get
            {
                return _def;
            }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}

using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class WorkflowSubscriptionPipeBind
    {
        private WorkflowSubscription _sub;
        private Guid _id;
        private string _name;

        public WorkflowSubscriptionPipeBind()
        {
            _sub = null;
            _id = Guid.Empty;
            _name = string.Empty;
        }

        public WorkflowSubscriptionPipeBind(WorkflowSubscription sub)
        {
            _sub = sub;
        }

        public WorkflowSubscriptionPipeBind(Guid guid)
        {
            _id = guid;
        }

        public WorkflowSubscriptionPipeBind(string id)
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

        public WorkflowSubscription Subscription
        {
            get
            {
                return _sub;
            }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}

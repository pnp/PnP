using System;
using Microsoft.SharePoint.Client.WorkflowServices;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class WorkflowSubscriptionPipeBind
    {
        private readonly WorkflowSubscription _sub;
        private readonly Guid _id;
        private readonly string _name;

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

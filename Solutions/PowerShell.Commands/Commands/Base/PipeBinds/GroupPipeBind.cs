using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class GroupPipeBind
    {
        private readonly int _id = -1;
        private readonly Group _group;
        private readonly string _name;
        public int Id
        {
            get
            {
                return _id;
            }
        }
        public Group Group
        {
            get
            {
                return _group;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        internal GroupPipeBind()
        {
        }

        public GroupPipeBind(int id)
        {
            _id = id;
        }

        public GroupPipeBind(Group group)
        {
            _group = group;
        }

        public GroupPipeBind(string name)
        {
            _name = name;
        }

    }
}

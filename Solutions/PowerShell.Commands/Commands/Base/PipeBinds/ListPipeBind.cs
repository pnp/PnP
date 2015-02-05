using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class ListPipeBind
    {
        private List _list;
        private Guid _id;
        private string _name;

        public ListPipeBind()
        {
            _list = null;
            _id = Guid.Empty;
            _name = string.Empty;
        }

        public ListPipeBind(List list)
        {
            _list = list;
        }

        public ListPipeBind(Guid guid)
        {
            _id = guid;
        }

        public ListPipeBind(string id)
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

        public List List
        {
            get
            {
                return _list;
            }
        }

        public string Title
        {
            get { return _name; }
        }
    }
}

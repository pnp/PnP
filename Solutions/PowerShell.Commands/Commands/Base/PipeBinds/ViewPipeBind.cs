using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public class ViewPipeBind
    {
        private View _view;
        private Guid _id;
        private string _name;

        public ViewPipeBind()
        {
            _view = null;
            _id = Guid.Empty;
            _name = string.Empty;
        }

        public ViewPipeBind(View view)
        {
            _view = view;
        }

        public ViewPipeBind(Guid guid)
        {
            _id = guid;
        }

        public ViewPipeBind(string id)
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

        public View View
        {
            get
            {
                return _view;
            }
        }

        public string Title
        {
            get { return _name; }
        }
    }
}

using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Entities;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public class SPOViewPipeBind
    {
        private ViewEntity _viewEntity;
        private View _view;
        private Guid _id;
        private string _name;

        public SPOViewPipeBind()
        {
            _viewEntity = null;
            _view = null;
            _id = Guid.Empty;
            _name = string.Empty;
        }

        public SPOViewPipeBind(ViewEntity view)
        {
            this._viewEntity = view;
        }

        public SPOViewPipeBind(View view)
        {
            this._view = view;
        }

        public SPOViewPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public SPOViewPipeBind(string id)
        {
            if (!Guid.TryParse(id, out _id))
            {
                this._name = id;
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
                if (_viewEntity != null)
                {
                    return _viewEntity.GetContextObject();
                }
                else
                {
                    return _view;
                }
            }
        }

        public string Title
        {
            get { return _name; }
        }
    }
}

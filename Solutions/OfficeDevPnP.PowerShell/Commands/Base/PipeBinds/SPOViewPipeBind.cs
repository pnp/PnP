using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public class SPOViewPipeBind
    {
        private SPOnlineView _onlineView;
        private View _view;
        private Guid _id;
        private string _name;

        public SPOViewPipeBind()
        {
            _onlineView = null;
            _view = null;
            _id = Guid.Empty;
            _name = string.Empty;
        }

        public SPOViewPipeBind(SPOnlineView view)
        {
            this._onlineView = view;
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
                if (_onlineView != null)
                {
                    return _onlineView.ContextObject;
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

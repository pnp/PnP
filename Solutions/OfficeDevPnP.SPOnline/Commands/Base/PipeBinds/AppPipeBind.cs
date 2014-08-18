using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.SPOnline.Commands.Base.PipeBinds
{
    public sealed class AppPipeBind
    {
        private AppInstance _appInstance;
        private Guid _id;

        public AppPipeBind(AppInstance instance)
        {
            this._appInstance = instance;
        }

        public AppPipeBind(Guid guid)
        {
            this._id = guid;
        }

        public AppPipeBind(string id)
        {
            this._id = new Guid(id);
        }

        public Guid Id
        {
            get { return _id; }
        }

        public AppInstance Instance
        {
            get { return _appInstance; }
        }
    }
}

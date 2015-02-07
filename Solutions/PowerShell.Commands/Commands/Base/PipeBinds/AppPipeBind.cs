using Microsoft.SharePoint.Client;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Base.PipeBinds
{
    public sealed class AppPipeBind
    {
        private readonly AppInstance _appInstance;
        private readonly Guid _id;

        public AppPipeBind(AppInstance instance)
        {
            _appInstance = instance;
        }

        public AppPipeBind(Guid guid)
        {
            _id = guid;
        }

        public AppPipeBind(string id)
        {
            _id = new Guid(id);
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

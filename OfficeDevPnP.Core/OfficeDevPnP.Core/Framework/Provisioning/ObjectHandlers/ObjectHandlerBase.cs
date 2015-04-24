using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public abstract class ObjectHandlerBase
    {
        private bool _reportProgress = true;
        public abstract string Name { get; }
        public bool ReportProgress
        {
            get { return _reportProgress; }
            set { _reportProgress = value; }
        }

        public abstract void ProvisionObjects(Web web, ProvisioningTemplate template);

        public abstract ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo);
    }
}

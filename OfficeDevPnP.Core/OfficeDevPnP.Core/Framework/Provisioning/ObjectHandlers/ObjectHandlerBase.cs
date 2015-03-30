using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public abstract class ObjectHandlerBase
    {
        public abstract void ProvisionObjects(Web web, ProvisioningTemplate template);

        public abstract ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplate baseTemplate);
    }
}

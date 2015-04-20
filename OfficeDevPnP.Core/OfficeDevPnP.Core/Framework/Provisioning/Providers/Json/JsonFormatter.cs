using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Json
{
    public class JsonFormatter : ITemplateFormatter
    {
        public void Initialize(TemplateProviderBase provider)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(System.IO.Stream template)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream ToFormattedTemplate(Model.ProvisioningTemplate template)
        {
            throw new NotImplementedException();
        }

        public Model.ProvisioningTemplate ToProvisioningTemplate(System.IO.Stream template)
        {
            throw new NotImplementedException();
        }

        public Model.ProvisioningTemplate ToProvisioningTemplate(System.IO.Stream template, String identifier)
        {
            throw new NotImplementedException();
        }
    }
}

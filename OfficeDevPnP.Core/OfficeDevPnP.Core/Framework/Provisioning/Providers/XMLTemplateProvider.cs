using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers
{
    /// <summary>
    /// Provider for xml based configurations
    /// </summary>
    public class XMLTemplateProvider : TemplateProviderBase
    {
        public override List<ProvisioningTemplate> GetTemplates()
        {
            return new List<ProvisioningTemplate>();
        }

        public override ProvisioningTemplate GetTemplate(string identifyer)
        {
            return new ProvisioningTemplate();
        }

        public override void Save(ProvisioningTemplate template)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string identifyer)
        {
            throw new NotImplementedException();
        }
    }
}

using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers
{
    public abstract class TemplateProviderBase
    {
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private bool _supportSave = false;
        private bool _supportDelete = false;
        private FileConnectorBase _connector = null;

        public Dictionary<string, string> Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        public virtual bool SupportsSave
        {
            get
            {
                return _supportSave;
            }
        }

        public virtual bool SupportsDelete
        {
            get
            {
                return _supportDelete;
            }
        }

        public virtual FileConnectorBase Connector
        {
            get
            {
                return _connector;
            }
            set
            {
                _connector = value;
            }
        }

        public abstract List<ProvisioningTemplate> GetTemplates();

        public abstract ProvisioningTemplate GetTemplate(string identifyer);

        public abstract void Save(ProvisioningTemplate template);

        public abstract void Delete(string identifyer);
    }
}

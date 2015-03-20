using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{
    public abstract class FileConnectorBase
    {
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();

        public virtual Dictionary<string, string> Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        public abstract string GetTemplateDefinition();
    }
}

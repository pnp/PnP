using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class DataRow
    {
        private Dictionary<string, string> _values = new Dictionary<string, string>();

        public Dictionary<string, string> Values
        {
            get { return _values;  }
            private set { _values = value; }
        }

        public DataRow(Dictionary<string, string> values)
        {
            foreach (var key in values.Keys)
            {
                Values.Add(key,values[key]);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class AdditionalGroup
    {
        private List<User> _members = new List<User>();

        public string Name { get; set; }

        public bool Description { get; set; }

        public List<User> Members
        {
            get { return _members; }
            private set { _members = value; }
        }
    }
}

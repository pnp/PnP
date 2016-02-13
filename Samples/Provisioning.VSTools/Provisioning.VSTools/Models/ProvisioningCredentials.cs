using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Perficient.Provisioning.VSTools.Models
{
    public class ProvisioningCredentials
    {
        public string Username { get; set; }

        [XmlIgnore]
        internal string Password
        {
            get
            {
                var securePw = Helpers.SecureStringHelper.DecryptString(this.SecurePassword);
                return Helpers.SecureStringHelper.ToInsecureString(securePw);
            }
            set
            {
                var securePw = Helpers.SecureStringHelper.ToSecureString(value);
                this.SecurePassword = Helpers.SecureStringHelper.EncryptString(securePw);
            }
        }

        public string SecurePassword { get; set; }

        public string AuthType { get { return "Office365"; } }
    }
}

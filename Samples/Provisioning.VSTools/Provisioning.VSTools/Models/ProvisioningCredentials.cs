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

        public void SetSecurePassword(string password)
        {
            var securePw = Helpers.SecureStringHelper.ToSecureString(password);
            this.SecurePassword = Helpers.SecureStringHelper.EncryptString(securePw);
        }

        public System.Security.SecureString GetSecurePassword()
        {
            return Helpers.SecureStringHelper.DecryptString(this.SecurePassword);
        }

        public string SecurePassword { get; set; }

        public string AuthType { get { return "Office365"; } }
    }
}

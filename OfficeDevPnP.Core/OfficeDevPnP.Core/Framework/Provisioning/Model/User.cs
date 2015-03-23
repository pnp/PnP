using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that defines a User or group in the provisioning template
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// The User email Address or the group name.
        /// </summary>
        public string Name { get; set; }
    }
}

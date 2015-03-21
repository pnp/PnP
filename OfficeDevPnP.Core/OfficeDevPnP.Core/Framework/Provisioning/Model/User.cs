using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that defines a User or group in the provisioning template
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// The User Email Address or the Group name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
    }
}

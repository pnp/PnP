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
    public partial class User : BaseModelEntity
    {
        #region Properties

        /// <summary>
        /// The User email Address or the group name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Comparison code

        public override int CompareTo(Object obj)
        {
            User other = obj as User;

            if (other == null)
            {
                return (1);
            }

            if (this.Name == other.Name)
            {
                return (0);
            }
            else
            {
                return (-1);
            }
        }

        public override int GetHashCode()
        {
            return (String.Format("{0}",
                this.Name).GetHashCode());
        }

        #endregion
    }
}

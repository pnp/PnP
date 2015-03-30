using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that represents an Feature.
    /// </summary>
    public class Feature : IEquatable<Feature>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the feature ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets if the feature should be deactivated
        /// </summary>
        public bool Deactivate { get; set; }

        #endregion

        #region Comparison code
        
        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}",
                this.Deactivate,
                this.ID).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Feature))
            {
                return (false);
            }
            return (Equals((Feature)obj));
        }

        public bool Equals(Feature other)
        {
            return (this.Deactivate == other.Deactivate &&
                this.ID == other.ID);
        }

        #endregion
    }
}

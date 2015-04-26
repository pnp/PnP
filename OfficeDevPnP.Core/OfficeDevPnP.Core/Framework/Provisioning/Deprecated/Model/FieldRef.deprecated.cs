using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Represents a Field XML Markup that is used to define information about a field
    /// </summary>
    public partial class FieldRef
    {
        #region Will be deprecated in June 2015 release


        /// <summary>
        /// Gets ot sets the ID of the referenced field
        /// </summary>
         [Obsolete("Use Id to set the identity of the object. This deprecated property will be removed in the June 2015 release.")]
        public Guid ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        #endregion
    }
}

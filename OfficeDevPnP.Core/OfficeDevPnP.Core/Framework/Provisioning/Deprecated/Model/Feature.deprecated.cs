using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that represents an Feature.
    /// </summary>
    public partial class Feature
    {
        #region Will be deprecated in June 2015 release
      
        [Obsolete("Use Id to set the identity of the object. This deprecated property will be removed in the June 2015 release.")]
        public Guid ID { get { return _id; } set { _id = value; } }


        #endregion
    }
}

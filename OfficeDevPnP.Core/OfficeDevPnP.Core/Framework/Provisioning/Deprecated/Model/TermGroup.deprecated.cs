using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public partial class TermGroup
    {

        #region Will be deprecated in June 2015 release

        [Obsolete("Use Id to set the identity of the object. This deprecated property will be removed in the June 2015 release.")]
        public Guid ID
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion
    }
}
using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
   
    public partial class ContentTypeBinding : IEquatable<ContentTypeBinding>
    {
        #region Will be deprecated in June 2015 release

        /// <summary>
        /// Gets or Sets the Content Type ID 
        /// </summary>
        [Obsolete("Use Id to set the identity of the object. This deprecated property will be removed in the June 2015 release.")]
        public string ContentTypeID
        {
            get { return _contentTypeId; }
            set { _contentTypeId = value; }
        }

        #endregion
    }
}
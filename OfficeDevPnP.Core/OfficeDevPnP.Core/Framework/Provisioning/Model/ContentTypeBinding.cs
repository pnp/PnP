using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for Content Type Binding in the Provisioning Template 
    /// </summary>
    public partial class ContentTypeBinding : IEquatable<ContentTypeBinding>
    {
        #region Private Members

        private string _contentTypeId;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the Content Type ID 
        /// </summary>
        public string ContentTypeId { get { return _contentTypeId; } set { _contentTypeId = value; } }

        /// <summary>
        /// Gets or Sets if the Content Type should be the default Content Type in the library
        /// </summary>
        public bool Default { get; set; }
        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}",
                this.ContentTypeId,
                this.Default).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ContentTypeBinding))
            {
                return (false);
            }
            return (Equals((ContentTypeBinding)obj));
        }

        public bool Equals(ContentTypeBinding other)
        {
            return (this.ContentTypeId == other.ContentTypeId &&
                this.Default == other.Default);
        }

        #endregion
    }
}

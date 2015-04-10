using System;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// This class holds deprecated ListInstance properties and methods
    /// </summary>
    public partial class ListInstance : IEquatable<ListInstance>
    {
        #region Will be deprecated in June 2015 release
        /// <summary>
        /// Gets or sets whether to remove the default content type from the list
        /// </summary>
        [Obsolete("Use RemoveExistingContentTypes instead")]
        public bool RemoveDefaultContentType { get; set; }

        #endregion
    }
}

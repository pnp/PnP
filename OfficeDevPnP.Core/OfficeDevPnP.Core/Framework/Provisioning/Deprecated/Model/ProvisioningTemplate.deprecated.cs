using System;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Extensions;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for the Provisioning Template
    /// </summary>
    public partial class ProvisioningTemplate
    {
        #region Will be deprecated in June 2015 release
         [Obsolete("Use Id to set the identity of the object. This deprecated property will be removed in the June 2015 release.")]
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }
        #endregion
    }
}

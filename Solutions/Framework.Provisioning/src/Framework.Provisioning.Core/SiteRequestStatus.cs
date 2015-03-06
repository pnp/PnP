using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core
{
    /// <summary>
    /// Enum for SiteRequestStatus
    /// </summary>
    public enum SiteRequestStatus
    {
        Complete,
        Exception,
        New,
        Processing,
        Pending,
        Approved
    }
}

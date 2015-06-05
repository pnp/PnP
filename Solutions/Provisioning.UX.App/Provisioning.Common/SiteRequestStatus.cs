using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common
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

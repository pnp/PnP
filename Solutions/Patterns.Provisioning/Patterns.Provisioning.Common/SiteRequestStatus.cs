using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Provisioning.Common
{
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

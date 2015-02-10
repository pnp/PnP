using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Framework.TimerJob.Enums
{
    /// <summary>
    /// Type of authentication, supports Office365, NetworkCredentials (on-premises) and AppOnly (both Office 365 as On-premises)
    /// </summary>
    public enum AuthenticationType
    {
        Office365 = 0,
        NetworkCredentials = 1,
        AppOnly = 2
    }
}

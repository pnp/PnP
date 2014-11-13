using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core
{
    /// <summary>
    /// Specify which user roles can declare and undeclare record status manually
    /// </summary>
    public enum EcmRecordDeclarationBy
    {
        Unknown = 0,
        /// <summary>
        /// All list contributors and administrators
        /// </summary>
        AllListContributors = 1,
        /// <summary>
        /// Only list administrators
        /// </summary>
        OnlyAdmins = 2,
        /// <summary>
        /// Only policy actions
        /// </summary>
        OnlyPolicy = 3
    }
}

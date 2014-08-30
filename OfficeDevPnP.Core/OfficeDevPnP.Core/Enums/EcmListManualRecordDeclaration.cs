
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core
{
    /// <summary>
    /// Specifies whether this list should allow the manual declaration of records.  When manual record declaration 
    /// is unavailable, records can only be declared through a policy or workflow.
    /// </summary>
    public enum EcmListManualRecordDeclaration
    {
        /// <summary>
        /// Use the site collection defaults
        /// </summary>
        UseSiteCollectionDefaults = 1,
        /// <summary>
        /// Always allow to manual declare records in this list
        /// </summary>
        AlwaysAllowManualDeclaration = 2,
        /// <summary>
        /// Never allow to manual declare records in this list
        /// </summary>
        NeverAllowManualDeclaration = 3
    }
}

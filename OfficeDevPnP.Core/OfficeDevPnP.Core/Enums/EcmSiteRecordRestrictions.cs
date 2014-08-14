using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core
{
    /// <summary>
    /// Specify restrictions to place on a document or item once it has been declared as a record.  Changing this setting 
    /// will not affect items which have already been declared records.  
    /// </summary>
    public enum EcmSiteRecordRestrictions
    {
        /// <summary>
        /// Records are no more restricted than non-records
        /// </summary>
        None = 1,
        /// <summary>
        /// Records can be edited but not deleted
        /// </summary>
        BlockDelete = 16,
        /// <summary>
        /// Records cannot be edited or deleted. Any change will require the record declaration to be revoked
        /// </summary>
        BlockEdit = 256
    }
}

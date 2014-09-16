using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core
{
    /// <summary>
    /// A class that returns strings that represent identifiers (IDs) for built-in content types.
    /// </summary>
    public static class BuiltInContentTypeId
    {
        /// <summary>
        /// Contains the content identifier (ID) for the DocumentSet content type. To get content type from a list, use BestMatchContentTypeId().
        /// </summary>
        public const string DocumentSet = "0x0120D520";
    }
}

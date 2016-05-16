using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines the importance of an email message
    /// </summary>
    public enum ItemImportance
    {
        /// <summary>
        /// Low importance
        /// </summary>
        Low,
        /// <summary>
        /// Normal importance, default value
        /// </summary>
        Normal,
        /// <summary>
        /// High importance
        /// </summary>
        High,
    }
}
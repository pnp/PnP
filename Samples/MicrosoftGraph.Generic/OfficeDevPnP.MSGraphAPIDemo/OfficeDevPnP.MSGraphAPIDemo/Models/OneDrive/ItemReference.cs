using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a reference to a drive item
    /// </summary>
    public class ItemReference : BaseModel
    {
        public String DriveId { get; set; }
        public String Path { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines an image file in OneDrive for Business
    /// </summary>
    public class Image
    {
        public Int32 Height { get; set; }
        public Int32 Width { get; set; }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a file in OneDrive for Business
    /// </summary>
    public class File
    {
        public Hashes Hashes { get; set; }
        public String MimeType { get; set; }
    }
}
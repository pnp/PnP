using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class SharingLink
    {
        public Identity Application { get; set; }
        public String Type { get; set; }
        public String WebUrl { get; set; }
    }
}
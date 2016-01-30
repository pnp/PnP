using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class SharingInvitation
    {
        public String Email { get; set; }
        public String RedeemedBy { get; set; }
        public Boolean SignInRequired { get; set; }
    }
}
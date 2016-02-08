using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class Conversation : BaseModel
    {
        public String Topic { get; set; }
        public Boolean HasAttachments { get; set; }
        public DateTimeOffset LastDeliveredDateTime { get; set; }
        public List<String> UniqueSenders { get; set; }
        public String Preview { get; set; }
    }
}
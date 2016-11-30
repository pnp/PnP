using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office365.Connectors.Models
{
    public class ConnectionError
    {
        public String GroupName { get; set; }

        public String State { get; set; }

        public String Error { get; set; }
    }
}
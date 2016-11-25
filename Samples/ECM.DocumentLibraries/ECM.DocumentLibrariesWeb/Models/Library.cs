using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECM.DocumentLibrariesWeb.Models
{
    public class Library
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool VerisioningEnabled { get; set; }

    }
}
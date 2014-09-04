using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Patterns.Provisioning.UIWeb {
    public class WizardModel {
        [Required]
        public string SiteName { get; set; }

        [Required]
        public string SiteUrl { get; set; }

        [Required]
        public string Template { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string OtherOwners { get; set; }
    }
}
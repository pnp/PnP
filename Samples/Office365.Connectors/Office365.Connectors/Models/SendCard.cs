using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Office365.Connectors.Models
{
    public class SendCard
    {
        public List<Connection> Connections { get; set; }

        [Required(ErrorMessage = "The Target Group is required")]
        public String WebHookUrl { get; set; }

        [Required(ErrorMessage = "The Card is required")]
        public String CardJson { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SPOGraphConsumer.Models
{
    public class SiteInfoViewModel
    {
        [DisplayName("Created Date Time")]
        public DateTime CreatedDateTime { get; set; }

        [DisplayName("Description")]
        public String Description { get; set; }

        [DisplayName("Display Name")]
        public String DisplayName { get; set; }

        [DisplayName("Graph Id")]
        public String Id { get; set; }

        [DisplayName("Last Modified Date Time")]
        public DateTime LastModifiedDateTime { get; set; }

        [DisplayName("Name")]
        public String Name { get; set; }

        [DisplayName("WebUrl")]
        public String WebUrl { get; set; }
    }
}
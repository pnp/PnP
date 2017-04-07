using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCTaxonomyPickerWeb.Models
{
    public class TermSetQueryModel
    {       
        public string Id { get; set; }  
        public string Name { get; set; }     
        public bool UseKeywords { get; set; }
        public bool UseHashtags { get; set; }
        public int LCID { get; set; }
    }
}
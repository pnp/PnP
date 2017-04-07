using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCTaxonomyPickerWeb.Models
{
    public class TermQueryModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string TermSetId { get; set; }     
        public string ParentTermId { get; set; }
        public int LCID { get; set; }
    }
}
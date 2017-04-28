using MVCTaxonomyPickerWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCTaxonomyPickerWeb.Models
{
    public class DemoModel
    { 
        [DisplayName("Demo"),
         Required(ErrorMessage = "Demo3 is required")]
        [UIHint("TaxonomyPicker")]
        public List<PickerTermModel> Demo { get; set; }

        [DisplayName("Demo1"), 
        Required(ErrorMessage = "Demo3 is required")]       
        [UIHint("TaxonomyPicker")]
        public List<PickerTermModel> Demo1 { get; set; }

        [DisplayName("Demo2")]      
        [UIHint("TaxonomyPicker")]
        public List<PickerTermModel> Demo2 { get; set; }

        [DisplayName("Demo3")]
        [UIHint("TaxonomyPicker")]
        public List<PickerTermModel> Demo3 { get; set; }
    }
}
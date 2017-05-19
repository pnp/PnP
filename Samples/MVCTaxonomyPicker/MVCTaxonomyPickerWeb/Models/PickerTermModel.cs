using System.Collections.Generic;

namespace MVCTaxonomyPickerWeb.Models
{
    public class PickerTermModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PathOfTerm { get; set; }            
        public List<PickerTermModel> Terms { get; set; }  
        public List<PickerLabelModel> Labels { get; set; }
        public int Level { get; set; }        
    }
}
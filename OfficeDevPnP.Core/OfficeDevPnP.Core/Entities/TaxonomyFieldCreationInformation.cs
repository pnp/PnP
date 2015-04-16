using Microsoft.SharePoint.Client.Taxonomy;

namespace OfficeDevPnP.Core.Entities
{
    public class TaxonomyFieldCreationInformation : FieldCreationInformation
    {
        private bool _multiValue = false;
       
        public bool MultiValue 
        {
            get {
                return _multiValue;
            }
            set
            {
                if (value)
                {
                    FieldType = "TaxonomyFieldTypeMulti";
                }
                else
                {
                    FieldType = "TaxonomyFieldType";
                }
                _multiValue = value;
            }
        }

        public TaxonomyItem TaxonomyItem { get; set; }

        public TaxonomyFieldCreationInformation()
            : base("TaxonomyFieldType")
        { }

    }

}

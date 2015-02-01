using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    this.FieldType = "TaxonomyFieldTypeMulti";
                }
                else
                {
                    this.FieldType = "TaxonomyFieldType";
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

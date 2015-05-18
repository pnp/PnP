using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECM.DocumentSetsWeb.Models
{
    public class AddDocumentSetViewModel
    {
        private List<string> _fieldNames = new List<string>();

        public List<string> FieldNames
        {
            get { return _fieldNames; }
            set { _fieldNames = value; }
        }

        public string ContentTypeName { get; set; }
        public string DocumentLibName { get; set; }
    }
}
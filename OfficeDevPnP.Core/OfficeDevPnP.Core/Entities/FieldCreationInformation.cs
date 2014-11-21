using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Entities
{
    public class FieldCreationInformation
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string InternalName { get; set; }
        public bool AddToDefaultView { get; set;}
        public IEnumerable<KeyValuePair<string, string>> AdditionalAttributes { get; set; }
        public string FieldType { get; private set; }
        public string Group { get; set; }


        public FieldCreationInformation(string fieldType)
        {
            this.FieldType = fieldType;
        }

        public FieldCreationInformation(FieldType fieldType)
        {
            this.FieldType = fieldType.ToString();
        }
    }

}

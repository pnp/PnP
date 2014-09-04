using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static class FieldExtensions
    {
        private const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" DisplayName=""{2}"" ID=""{3}"" />";
  
        public static Field Add(this FieldCollection fields, Guid id, string internalName, FieldType fieldType, string displayName, bool addtoDefaultView)
        {
            string _fieldXml = string.Format(FIELD_XML_FORMAT, fieldType, internalName, displayName, id);
            return fields.AddFieldAsXml(_fieldXml, addtoDefaultView, AddFieldOptions.DefaultValue);

        }
      

    }
}

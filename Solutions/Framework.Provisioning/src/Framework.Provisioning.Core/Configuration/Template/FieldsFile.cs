using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    /// <summary>
    /// TODO
    /// </summary>
    public class FieldsFile
    {
        [XmlAttribute]
        public string FileName { get; set; }

        /// <summary>
        /// Returns a collection of Fields to provision
        /// </summary>
        /// <returns></returns>
        public List<Field> GetFields()
        { 
            List<Field> _fields = new List<Field>();
           
            if(!string.IsNullOrEmpty(this.FileName))
            {
                var _path = PathHelper.GetAssemblyDirectory();
                var _fileName = this.FileName;
                string _filePath = string.Format("{0}{1}{2}", _path, @"\", _fileName);

                XDocument _doc = XDocument.Load(_filePath);

                XElement fields = _doc.Element("Fields");
                foreach (var fieldElement in fields.Elements())
                {
                    Field _field = new Field();
                    _field.SchemaXml = fieldElement.ToString();
                    _fields.Add(_field);
                }
       
            }
            return _fields;
        }
    }
}

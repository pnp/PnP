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
    public class ContentTypeFile
    {
        /// <summary>
        /// TODO
        /// </summary>
        [XmlAttribute]
        public string FileName { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public List<ContentType> GetContentTypeDeclaration()
        {
            List<ContentType> _contentTypes = new List<ContentType>();

            if(!string.IsNullOrEmpty(this.FileName))
            {
                var _path = PathHelper.GetAssemblyDirectory();
                var _fileName = this.FileName;
                string _filePath = string.Format("{0}{1}{2}", _path, @"\", _fileName);

                XDocument _doc = XDocument.Load(_filePath);
                XElement fields = _doc.Element("ContentTypes");
                foreach (var element in fields.Elements())
                {
                    ContentType _field = new ContentType();
                    _field.SchemaXml = element.ToString();
                    _contentTypes.Add(_field);
                }
            }

            return _contentTypes;

        }
    }
}

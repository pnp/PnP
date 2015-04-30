using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Provisioning.Common.Data.Templates
{
    [XmlRoot(ElementName = "TemplateConfiguration")]
    public class XMLSiteTemplateData
    {
        #region private members
        private List<Template> _templates = new List<Template>();
        #endregion

        #region Properties
        [XmlArray(ElementName = "Templates")]
        [XmlArrayItem("Template", typeof(Template))]
        public List<Template> Templates
        {
            get
            {
                return _templates ;
            }
            set { _templates = value; }
        }

        #endregion
    }
}

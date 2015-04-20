using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Provisioning.Common.Configuration.Template
{
    /// <summary>
    /// TemplateConfiguration Object for Working with Templates
    /// </summary>
    [XmlRoot(ElementName = "TemplateConfiguration")]
    public partial class TemplateConfiguration
    {
        #region private members
        private List<Template> _templates;

        #endregion

        #region Properties
        [XmlArray(ElementName = "Templates")]
        [XmlArrayItem("Template", typeof(Template))]
        public List<Template> Templates {
            get 
            {
                return _templates ?? (_templates = new List<Template>());
            } 
            set { _templates = value; }
        }

        #endregion
    }
}

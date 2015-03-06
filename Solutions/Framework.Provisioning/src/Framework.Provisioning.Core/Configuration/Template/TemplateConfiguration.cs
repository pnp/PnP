using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    /// <summary>
    /// TemplateConfiguration Object for Working with Templates
    /// </summary>
    [XmlRoot(ElementName = "TemplateConfiguration")]
    public partial class TemplateConfiguration
    {
        #region private members
        private List<Template> _templates;
        private List<BrandingPackage> _brandingPackages = new List<BrandingPackage>();
        private List<CustomAction> _customActions;
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

        [XmlArray(ElementName = "BrandingPackages")]
        [XmlArrayItem("BrandingPackage", typeof(BrandingPackage))]
        public List<BrandingPackage> BrandingPackage
        {
            get
            {
                return _brandingPackages;
            }
            set { _brandingPackages = value; }
        }

        [XmlArray(ElementName = "CustomActions")]
        [XmlArrayItem("CustomAction", typeof(CustomAction))]
        public List<CustomAction> CustomActions
        {
            get
            {
                return _customActions ?? (_customActions = new List<CustomAction>());
            }
            set { _customActions = value; }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    [XmlRoot(ElementName = "CustomActions")]
    public class CustomActions
    {
        private List<CustomAction> _siteCustomActions = new List<CustomAction>();
        private List<CustomAction> _webCustomActions = new List<CustomAction>();

        #region Properties
        [XmlArray(ElementName = "SiteCustomActions")]
        [XmlArrayItem("CustomAction", typeof(CustomAction))]
        public List<CustomAction> SiteCustomActions
        {
            get
            {
                return this._siteCustomActions;
            }
            set { this._siteCustomActions = value; }
        }

        [XmlArray(ElementName = "WebCustomActions")]
        [XmlArrayItem("CustomAction", typeof(CustomAction))]
        public List<CustomAction> WebCustomActions
        {
            get
            {
                return this._webCustomActions;
            }
            set { this._webCustomActions = value; }
        }

        #endregion

    }
}

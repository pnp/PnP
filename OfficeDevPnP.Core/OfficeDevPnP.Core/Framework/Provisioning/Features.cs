using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning
{
    /// <summary>
    /// Domain Object that is used in the Site Template for OOB Features
    /// </summary>
    [XmlRoot(ElementName = "Features")]
    public partial class Features
    {
        private List<SiteFeature> _siteFeatures = new List<SiteFeature>();
        private List<WebFeature> _webFeatures = new List<WebFeature>();

        #region Properties
        [XmlArray(ElementName = "SiteFeatures")]
        [XmlArrayItem("Feature", typeof(SiteFeature))]
        public List<SiteFeature> SiteFeatures
        {
            get
            {
                return this._siteFeatures;
            }
            set { this._siteFeatures = value; }
        }

        [XmlArray(ElementName = "WebFeatures")]
        [XmlArrayItem("Feature", typeof(WebFeature))]
        public List<WebFeature> WebFeatures
        {
            get
            {
                return this._webFeatures;
            }
            set { this._webFeatures = value; }
        }

        #endregion
    }
}
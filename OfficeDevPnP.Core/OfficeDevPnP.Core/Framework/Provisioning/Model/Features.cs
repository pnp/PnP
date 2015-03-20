using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that is used in the Site Template for OOB Features
    /// </summary>
    public partial class Features
    {
        private List<Feature> _siteFeatures = new List<Feature>();
        private List<Feature> _webFeatures = new List<Feature>();

        #region Properties
        public List<Feature> SiteFeatures
        {
            get
            {
                return this._siteFeatures;
            }
            private set { this._siteFeatures = value; }
        }

        public List<Feature> WebFeatures
        {
            get
            {
                return this._webFeatures;
            }
            private set { this._webFeatures = value; }
        }

        #endregion
    }
}
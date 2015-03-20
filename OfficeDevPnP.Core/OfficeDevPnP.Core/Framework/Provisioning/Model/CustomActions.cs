using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class CustomActions
    {
        private List<CustomAction> _siteCustomActions;
        private List<CustomAction> _webCustomActions;

        public CustomActions()
        {
            this._siteCustomActions = new List<CustomAction>();
            this._webCustomActions = new List<CustomAction>();
        }

        #region Properties
        public List<CustomAction> SiteCustomActions
        {
            get
            {
                return this._siteCustomActions;
            }
            private set { this._siteCustomActions = value; }
        }

        public List<CustomAction> WebCustomActions
        {
            get
            {
                return this._webCustomActions;
            }
            private set { this._webCustomActions = value; }
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class View
    {
        #region Private Members
        private string _schemaXml = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value that specifies the XML Schema representing the View type.
        /// </summary>
        public string SchemaXml
        {
            get { return this._schemaXml; }
            set { this._schemaXml = value; }
        }

        #endregion
    }
}

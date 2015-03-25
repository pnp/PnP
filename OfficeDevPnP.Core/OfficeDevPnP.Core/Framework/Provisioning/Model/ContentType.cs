using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object used in the Provisioning template that defines a Content Type
    /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/ms463449.aspx"/>
    /// </summary>
    public class ContentType
    {
        #region Private Members
        private string _schemaXML = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets value that specifies the XML Schema representing the content type.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/ms463449.aspx"/>
        /// </summary>
        public string SchemaXml
        {
            get { return this._schemaXML; }
            set { this._schemaXML = value; }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public partial class ContentType
    {
        #region Will be deprecated in June 2015 release

        #region Private Members
        private string _schemaXML = string.Empty;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets value that specifies the XML Schema representing the content type.
        /// <seealso>
        ///     <cref>https://msdn.microsoft.com/en-us/library/office/ms463449.aspx</cref>
        /// </seealso>
        /// </summary>
        [Obsolete("Use the other properties in this object to specify the content type. This deprecated property will be removed in the June 2015 release.")]
        public string SchemaXml
        {
            get { return this._schemaXML; }
            set { this._schemaXML = value; }
        }

        /// <summary>
        /// The ID of the Content Type
        /// </summary>
        [Obsolete("Use Id to set the identity of the object. This deprecated property will be removed in the June 2015 release.")]
        public string ID { get { return _id; } set { _id = value; } }
        #endregion
        #endregion
    }
}

using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class View : IEquatable<View>
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

        #region Comparison code

        public override int GetHashCode()
        {
            XElement element = XElement.Parse(this.SchemaXml);
            if (element.Attribute("Name") != null)
            {
                Guid nameGuid = Guid.Empty;
                if (Guid.TryParse(element.Attribute("Name").Value, out nameGuid))
                {
                    // Temporary remove guid
                    element.Attribute("Name").Remove();
                }
            }
            if (element.Attribute("Url") != null)
            {
                element.Attribute("Url").Remove();
            }
            if (element.Attribute("ImageUrl") != null)
            {
                var index = element.Attribute("ImageUrl").Value.IndexOf("rev=",StringComparison.InvariantCultureIgnoreCase);
               
                if (index > -1)
                {
                    // Remove ?rev=23 in url
                    Regex regex = new Regex("\\?rev=([0-9])\\w+");
                    element.SetAttributeValue("ImageUrl",regex.Replace(element.Attribute("ImageUrl").Value, ""));
                }
            }
            //return (String.Format("{0}",
//                this.SchemaXml).GetHashCode()); 
            return element.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is View))
            {
                return (false);
            }
            return (Equals((View)obj));
        }

        public bool Equals(View other)
        {
            XElement currentXml = XElement.Parse(this.SchemaXml);
            XElement otherXml = XElement.Parse(other.SchemaXml);
            if (currentXml.Attribute("Name") != null)
            {
                Guid nameGuid = Guid.Empty;
                if (Guid.TryParse(currentXml.Attribute("Name").Value, out nameGuid))
                {
                    // Temporary remove guid
                    currentXml.Attribute("Name").Remove();
                }
            }
            if (currentXml.Attribute("Url") != null)
            {
                currentXml.Attribute("Url").Remove();
            }
            if (otherXml.Attribute("Name") != null)
            {
                Guid nameGuid = Guid.Empty;
                if (Guid.TryParse(otherXml.Attribute("Name").Value, out nameGuid))
                {
                    // Temporary remove guid
                    otherXml.Attribute("Name").Remove();
                }
            }
            if (otherXml.Attribute("Url") != null)
            {
                otherXml.Attribute("Url").Remove();
            }
            
            return (XNode.DeepEquals(currentXml, otherXml));
        }

        #endregion
    }
}

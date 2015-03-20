using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for custom action.
    /// </summary>
    public partial class CustomAction
    {
        private int _rightsValue = 0;

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public string Group { get; set; }
        [XmlAttribute]
        public string Location { get; set; }
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public int Sequence { get; set; }
        
        [XmlIgnore]
        public BasePermissions Rights { get; set; }

        [XmlAttribute("Rights")]
        public int RightsValue {
            get
            {
                return this._rightsValue;
            }
            set 
            {
                this._rightsValue = value;
                BasePermissions _bp = new BasePermissions();
                if(Enum.IsDefined(typeof(PermissionKind), value))
                {
                    var _pk = (PermissionKind)value;
                    _bp.Set(_pk);
                    this.Rights = _bp;
                }
            }
        }

        [XmlAttribute]
        public string Url { get; set; }
        [XmlAttribute]
        public bool Enabled { get; set; }
        [XmlAttribute]
        public string ScriptBlock { get; set; }
        [XmlAttribute]
        public string ImageUrl { get; set; }
        [XmlAttribute]
        public string ScriptSrc { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that is used in the site template
    /// </summary>
    [XmlRoot(ElementName = "Security")]
    public partial class SiteSecurity
    {
        #region Private
        private List<User> _additionalAdministrators = new List<User>();
        private List<User> _additionalOwners = new List<User>();
        private List<User> _additionalMembers = new List<User>();
        private List<User> _additionalVisitors = new List<User>();
        #endregion

        #region Properties
        [XmlArray(ElementName = "AdditionalAdministrators")]
        [XmlArrayItem("User", typeof(User))]
        public List<User> AdditionalAdministrators
        {
            get
            {
                return _additionalAdministrators;
            }
            private set { _additionalAdministrators = value; }
        }

        [XmlArray(ElementName = "AdditionalOwners")]
        [XmlArrayItem("User", typeof(User))]
        public List<User> AdditionalOwners
        {
            get
            {
                return _additionalOwners;
            }
            private set { _additionalOwners = value; }
        }

        [XmlArray(ElementName = "AdditionalMembers")]
        [XmlArrayItem("User", typeof(User))]
        public List<User> AdditionalMembers
        {
            get
            {
                return _additionalMembers;
            }
            private set { _additionalMembers = value; }
        }

        [XmlArray(ElementName = "AdditionalVistors")]
        [XmlArrayItem("User", typeof(User))]
        public List<User> AdditionalVisitors
        {
            get
            {
                return _additionalVisitors;
            }
            private set { _additionalVisitors = value; }
        }
        #endregion

    }
}

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
        private List<AdditionalAdministrator> _additionalAdministrators;
        private List<Owner> _additionalOwners;
        private List<Member> _additionalMembers;
        private List<Vistor> _additionalVisitors;
        #endregion

        #region Properties
        [XmlArray(ElementName = "AdditionalAdministrators")]
        [XmlArrayItem("User", typeof(AdditionalAdministrator))]
        public List<AdditionalAdministrator> AdditionalAdministrators
        {
            get
            {
                return _additionalAdministrators ?? (_additionalAdministrators = new List<AdditionalAdministrator>());
            }
            set { _additionalAdministrators = value; }
        }

        [XmlArray(ElementName = "AdditionalOwners")]
        [XmlArrayItem("User", typeof(Owner))]
        public List<Owner> AdditionalOwners
        {
            get
            {
                return _additionalOwners ?? (_additionalOwners = new List<Owner>());
            }
            set { _additionalOwners = value; }
        }

        [XmlArray(ElementName = "AdditionalMembers")]
        [XmlArrayItem("User", typeof(Member))]
        public List<Member> AdditionalMembers
        {
            get
            {
                return _additionalMembers ?? (_additionalMembers = new List<Member>());
            }
            set { _additionalMembers = value; }
        }

        [XmlArray(ElementName = "AdditionalVistors")]
        [XmlArrayItem("User", typeof(Vistor))]
        public List<Vistor> AdditionalVisitors
        {
            get
            {
                return _additionalVisitors ?? (_additionalVisitors = new List<Vistor>());
            }
            set { _additionalVisitors = value; }
        }
        #endregion

    }
}

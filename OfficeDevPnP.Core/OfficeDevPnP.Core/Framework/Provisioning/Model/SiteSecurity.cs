using System.Collections.Generic;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that is used in the site template
    /// </summary>
    public partial class SiteSecurity
    {
        #region Private
        private List<User> _additionalAdministrators = new List<User>();
        private List<User> _additionalOwners = new List<User>();
        private List<User> _additionalMembers = new List<User>();
        private List<User> _additionalVisitors = new List<User>();
        private List<AdditionalGroup> _additionalGroups = new List<AdditionalGroup>();
        #endregion

        #region Properties
        /// <summary>
        /// A Collection of users that are associated as site collection adminsitrators
        /// </summary>
        public List<User> AdditionalAdministrators
        {
            get { return _additionalAdministrators; }
            private set { _additionalAdministrators = value; }
        }

        /// <summary>
        /// A Collection of users that are associated to the sites owners group
        /// </summary>
        public List<User> AdditionalOwners
        {
            get { return _additionalOwners; }
            private set { _additionalOwners = value; }
        }

        /// <summary>
        /// A Collection of users that are associated to the sites members group
        /// </summary>
        public List<User> AdditionalMembers
        {
            get { return _additionalMembers; }
            private set { _additionalMembers = value; }
        }

        /// <summary>
        /// A Collection of users taht are associated to the sites visitors group
        /// </summary>
        public List<User> AdditionalVisitors
        {
            get { return _additionalVisitors; }
            private set { _additionalVisitors = value; }
        }

        /// <summary>
        /// A Collection of users taht are associated to the sites visitors group
        /// </summary>
        public List<AdditionalGroup> AdditionalGroups
        {
            get { return _additionalGroups; }
            private set { _additionalGroups = value; }
        }

        #endregion
    }
}

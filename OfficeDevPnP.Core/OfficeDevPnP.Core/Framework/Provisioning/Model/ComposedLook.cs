using System;
using OfficeDevPnP.Core.Framework.Provisioning.Model.Comparers;
using OfficeDevPnP.Core.Framework.Provisioning.Model.HashFormatters;
using OfficeDevPnP.Core.Framework.Provisioning.Model.Attributes;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that defines a Composed Look in the Provision Template
    /// </summary>
    public partial class ComposedLook : ModelBase<ComposedLook>
    {
        static ComposedLook()
        {
            Empty = new ComposedLook();
        }
        public ComposedLook()
        {
            ObjectComparer = ComposedLookComparer.GetComparer(this);
        }

        private static ComposedLook _empty;

        public static ComposedLook Empty
        {
            private set { _empty = value; }
            get { return (_empty); }
        }

        #region Properties
        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [HashCodeIdentifier]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ColorFile
        /// </summary>
        [HashCodeIdentifier]
        public string ColorFile { get; set; }

        /// <summary>
        /// Gets or sets the FontFile
        /// </summary>
        [HashCodeIdentifier]
        public string FontFile { get; set; }

        /// <summary>
        /// Gets or sets the Background Image 
        /// </summary>
        [HashCodeIdentifier]
        public string BackgroundFile { get; set; }

        /// <summary>
        /// Gets or sets the MasterPage for the Composed Look
        /// </summary>
        [HashCodeIdentifier]
        public string MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the Site Logo
        /// </summary>
        [HashCodeIdentifier]
        public string SiteLogo { get; set; }

        /// <summary>
        /// Gets or sets the AlternateCSS
        /// </summary>
        [HashCodeIdentifier]
        public string AlternateCSS { get; set; }

        /// <summary>
        /// Gets or sets the Version of the ComposedLook.
        /// </summary>
        [HashCodeIdentifier]
        public int Version { get; set; }

        #endregion
    }
}

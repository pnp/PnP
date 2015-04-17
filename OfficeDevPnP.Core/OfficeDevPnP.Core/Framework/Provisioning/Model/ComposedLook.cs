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
        public static ComposedLook Empty
        {
            get { return ComposedLookBuilder.Build(); }
        }
        private class ComposedLookBuilder
        {
            private readonly ComposedLook cl;
            public ComposedLook Cl { get { return cl; } }

            private ComposedLookBuilder()
            {
                this.cl = new ComposedLook();
            }

            public static ComposedLook Build()
            {
                return new ComposedLookBuilder().Cl;
            }
        }
        static ComposedLook()
        {
        }
        public ComposedLook()
        {
            InstanceEquator = new ComposedLookEquator().GetEquator(this);
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

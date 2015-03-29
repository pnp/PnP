using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that defines a Composed Look in the Provision Template
    /// </summary>
    public partial class ComposedLook : BaseModelEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ColorFile
        /// </summary>
        public string ColorFile { get; set; }

        /// <summary>
        /// Gets or sets the FontFile
        /// </summary>
        public string FontFile { get; set; }

        /// <summary>
        /// Gets or sets the Background Image 
        /// </summary>
        public string BackgroundFile { get; set; }

        /// <summary>
        /// Gets or sets the MasterPage for the Composed Look
        /// </summary>
        public string MasterPage { get; set; }

        /// <summary>
        /// Gets or sets the Site Logo
        /// </summary>
        public string SiteLogo { get; set; }

        /// <summary>
        /// Gets or sets the AlternateCSS
        /// </summary>
        public string AlternateCSS { get; set; }

        /// <summary>
        /// Gets or sets the Version of the ComposedLook.
        /// </summary>
        public int Version { get; set; }

        #endregion

        #region Comparison code

        public override int CompareTo(Object obj)
        {
            ComposedLook other = obj as ComposedLook;

            if (other == null)
            {
                return (1);
            }

            if (this.AlternateCSS == other.AlternateCSS &&
                this.BackgroundFile == other.BackgroundFile &&
                this.ColorFile == other.ColorFile &&
                this.FontFile == other.FontFile &&
                this.MasterPage == other.MasterPage &&
                this.Name == other.Name &&
                this.SiteLogo == other.SiteLogo)
            {
                if (this.Version == other.Version)
                {
                    return (0);
                }
                else if (this.Version > other.Version)
                {
                    return (1);
                }
                else
                {
                    return (-1);
                }
            }
            else
            {
                return (-1);
            }
        }

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|",
                this.AlternateCSS,
                this.BackgroundFile,
                this.ColorFile,
                this.FontFile,
                this.MasterPage,
                this.Name,
                this.SiteLogo,
                this.Version
                ).GetHashCode());
        }

        #endregion
    }
}

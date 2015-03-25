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
    public partial class ComposedLook
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
    }
}

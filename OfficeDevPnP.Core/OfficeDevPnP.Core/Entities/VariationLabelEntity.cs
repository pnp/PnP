using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Entities
{
    /// <summary>
    /// Class represents variation label
    /// </summary>
    public class VariationLabelEntity
    {
        /// <summary>
        /// The variation label title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The variation label description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The flag to control display name
        /// </summary>
        public string FlagControlDisplayName { get; set; }

        /// <summary>
        /// The variation label language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The variation label locale
        /// </summary>
        public uint Locale { get; set; }

        /// <summary>
        /// The hierarchy creation mode
        /// </summary>
        public string HierarchyCreationMode { get; set; }

        /// <summary>
        /// Set as source variation
        /// </summary>
        public bool IsSource { get; set; }
    }

}

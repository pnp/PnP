using System.Collections.Generic;

namespace OfficeDevPnP.Core.Entities
{
    /// <summary>
    /// Specifies a default column value for a document library
    /// </summary>
    public class DefaultColumnTermPathValue
    {
        /// <summary>
        /// The Path of the folder, Rootfolder of the document library is "/" 
        /// </summary>
        public string FolderRelativePath { get; set; }

        /// <summary>
        /// The internal name of the field
        /// </summary>
        public string FieldInternalName { get; set; }

        /// <summary>
        /// Taxonomy paths in the shape of "TermGroup|TermSet|Term"
        /// </summary>
        public IList<string> TermPaths { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        public DefaultColumnTermPathValue()
        {
            TermPaths = new List<string>();
        }
    }
}

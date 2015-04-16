using System.Collections.Generic;
using Microsoft.SharePoint.Client.Taxonomy;

namespace OfficeDevPnP.Core.Entities
{
    public interface IDefaultColumnValue
    {
        string FolderRelativePath { get; set; }
        string FieldInternalName { get; set; }
    }
    
    /// <summary>
    /// Specifies a default column value for a document library
    /// </summary>
    public class DefaultColumnTermValue : DefaultColumnValue
    {
      
        /// <summary>
        /// Taxonomy paths in the shape of "TermGroup|TermSet|Term"
        /// </summary>
        public IList<Term> Terms { get; private set; }

        public DefaultColumnTermValue()
        {
            Terms = new List<Term>();
        }
    }

    public class DefaultColumnTextValue : DefaultColumnValue
    {
        public string Text { get; set; }
    }

    public class DefaultColumnValue : IDefaultColumnValue
    {
        /// <summary>
        /// The Path of the folder, Rootfolder of the document library is "/" 
        /// </summary>
        public string FolderRelativePath { get; set; }

        /// <summary>
        /// The internal name of the field
        /// </summary>
        public string FieldInternalName { get; set; }

    }
}

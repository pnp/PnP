using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class File : BaseModelEntity
    {
        #region Properties

        public string Src { get; set; }
        
        public string Folder { get; set; }

        public bool Overwrite { get; set; }

        #endregion

        #region Comparison code

        public override int CompareTo(Object obj)
        {
            File other = obj as File;

            if (other == null)
            {
                return (1);
            }

            if (this.Folder == other.Folder &&
                this.Overwrite == other.Overwrite &&
                this.Src == other.Src)
            {
                return (0);
            }
            else
            {
                return (-1);
            }
        }

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.Folder,
                this.Overwrite,
                this.Src).GetHashCode());
        }

        #endregion
    }
}

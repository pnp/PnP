using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class File : IEquatable<File>
    {
        #region Properties

        public string Src { get; set; }
        
        public string Folder { get; set; }

        public bool Overwrite { get; set; }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.Folder,
                this.Overwrite,
                this.Src).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is File))
            {
                return (false);
            }
            return (Equals((File)obj));
        }

        public bool Equals(File other)
        {
            return (this.Folder == other.Folder &&
                this.Overwrite == other.Overwrite &&
                this.Src == other.Src);
        }

        #endregion
    }
}

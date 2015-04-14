using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class WebPart : IEquatable<WebPart>
    {
        #region Properties
        public uint? Row { get; set; }

        public uint? Column { get; set; }

        public string Title { get; set; }

        public string Contents { get; set; }

        public string Zone { get; set; }

        public uint? Index { get; set; }
        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.Row,
                this.Column,
                this.Contents).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is File))
            {
                return (false);
            }
            return (Equals((File)obj));
        }

        public bool Equals(WebPart other)
        {
            return (this.Row == other.Row &&
                this.Column == other.Column &&
                this.Contents == other.Contents);
        }

        #endregion
    }
}

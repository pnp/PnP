using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for Extensiblity Call out
    /// </summary>
    public class Provider : IEquatable<Provider>
    {
        #region Properties

        public bool Enabled
        {
            get;
            set;
        }

        public string Assembly
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }
        
        public string Configuration { get; set; }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}",
                this.Assembly,
                this.Configuration,
                this.Enabled,
                this.Type).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Provider))
            {
                return (false);
            }
            return (Equals((Provider)obj));
        }

        public bool Equals(Provider other)
        {
            return (this.Assembly == other.Assembly &&
                this.Configuration == other.Configuration &&
                this.Enabled == other.Enabled &&
                this.Type == other.Type);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Framework.Provisioning.Model.Comparers;
using OfficeDevPnP.Core.Framework.Provisioning.Model.HashFormatters;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public abstract class ModelBase<T> : IEquatable<T>
    {
        public Func<T, bool> ObjectComparer { get; set; }

        public ModelBase() { }
        public override int GetHashCode()
        {
            return HashFormatter<ModelBase<T>>.GetFormatter(this);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is T))
            {
                return(false);
            }
            return (Equals((ModelBase<T>)obj));
        }

        public bool Equals(T other)
        {
            return ObjectComparer(other);
        }
    }
}

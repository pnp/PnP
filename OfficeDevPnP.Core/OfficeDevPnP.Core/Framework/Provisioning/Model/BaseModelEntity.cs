using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public abstract class BaseModelEntity: IComparable
    {
        public override bool Equals(object obj)
        {
            return (((IComparable)this).CompareTo(obj) == 0);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public abstract int CompareTo(object obj);
    }
}

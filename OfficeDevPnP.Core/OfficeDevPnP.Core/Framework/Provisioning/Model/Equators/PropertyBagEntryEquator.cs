using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model.Comparers
{
    public class PropertyBagEntryEquator : IEqualityComparer<PropertyBagEntry>
    {
        public Func<PropertyBagEntry, bool> GetEquator(PropertyBagEntry model)
        {
            return (other) =>
            {
                return (model.Key == other.Key && model.Value == other.Value);
            };
        }

        public bool Equals(PropertyBagEntry x, PropertyBagEntry y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(PropertyBagEntry obj)
        {
            return obj.GetHashCode();
        }
    }
}

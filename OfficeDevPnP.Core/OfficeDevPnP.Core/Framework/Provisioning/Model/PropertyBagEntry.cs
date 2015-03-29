using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class PropertyBagEntry : BaseModelEntity
    {
        #region Properties

        public string Key { get; set; }
            
        public string Value { get; set; }

        #endregion

        #region Comparison code

        public override int CompareTo(Object obj)
        {
            PropertyBagEntry other = obj as PropertyBagEntry;

            if (other == null)
            {
                return (1);
            }

            if (this.Key == other.Key &&
                this.Value == other.Value)
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
            return (String.Format("{0}|{1}",
                this.Key,
                this.Value).GetHashCode());
        }

        #endregion
    }
}

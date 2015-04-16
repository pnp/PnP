using OfficeDevPnP.Core.Framework.Provisioning.Model.Attributes;
using OfficeDevPnP.Core.Framework.Provisioning.Model.Comparers;
using OfficeDevPnP.Core.Framework.Provisioning.Model.HashFormatters;
using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class PropertyBagEntry : ModelBase<PropertyBagEntry>
    {
        [HashCodeIdentifier]
        public string Key { get; set; }
        [HashCodeIdentifier]
        public string Value { get; set; }
        public PropertyBagEntry()
        {
            InstanceEquator = new PropertyBagEntryEquator().GetEquator(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
{
    [ConfigurationCollection(typeof(Module))]
    public class ModuleElementCollection : ConfigurationElementCollection
    {
        internal const string PropertyName = "Module";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        protected override string ElementName
        {
            get
            {
                return PropertyName;
            }
        }
        protected override bool IsElementName(string elementName)
        {
            return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        }
        public override bool IsReadOnly()
        {
            return false;
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new Module();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Module)(element)).Name;
        }

        public Module this[int idx]
        {
            get
            {
                return (Module)BaseGet(idx);
            }
        }

        public new Module this[string key]
        {
            get
            {
                return base.BaseGet(key) as Module;
            }
        }
    }
}

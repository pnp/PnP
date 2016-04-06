using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddinsConfiguration
{
    public class AddinSection : ConfigurationSection
    {
        internal const string AddinSectionSectionName = "addinSection";
        internal const string AddinSectionSectionPath = "addinSection";
        internal const string XmlnsPropertyName = "xmlns";
        internal const string AddinsPropertyName = "addins";

        public AddinSection Instance
        {
            get
            {
                return ((AddinSection)(ConfigurationManager.GetSection(AddinSectionSectionPath)));
            }
        }

        [ConfigurationProperty(XmlnsPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public string Xmlns
        {
            get
            {
                return ((string)(base[XmlnsPropertyName]));
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }


        [Description("The Addins.")]
        [ConfigurationProperty(AddinsPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public AddIns Addins
        {
            get
            {
                return ((AddIns)(base[AddinsPropertyName]));
            }
            set
            {
                base[AddinsPropertyName] = value;
            }
        }
    }

    [ConfigurationCollection(typeof(AddIn), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate, AddItemName = AddInPropertyName)]
    public class AddIns : ConfigurationElementCollection
    {
        internal const string AddInPropertyName = "addin";

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
                return AddInPropertyName;
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return (elementName == AddInPropertyName);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AddIn)(element)).Name;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AddIn();
        }

        public AddIn this[int index]
        {
            get
            {
                return ((AddIn)(BaseGet(index)));
            }
        }

        public AddIn this[object Name]
        {
            get
            {
                return ((AddIn)(BaseGet(Name)));
            }
        }

        public void Add(AddIn addin)
        {
            base.BaseAdd(addin);
        }

        public void Remove(AddIn addin)
        {
            BaseRemove(GetElementKey(addin));
        }

        public AddIn GetItemAt(int index)
        {
            return ((AddIn)(BaseGet(index)));
        }

        public AddIn GetItemByKey(string Name)
        {
            return ((AddIn)(BaseGet(Name)));
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }

    public class AddIn : ConfigurationElement
    {
        internal const string UrlPropertyName = "Url";
        internal const string NamePropertyName = "Name";
        internal const string ClientIdPropertyName = "ClientId";
        

        public override bool IsReadOnly()
        {
            return false;
        }

        [Description("The Url.")]
        [ConfigurationProperty(UrlPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false)]
        public virtual string Url
        {
            get
            {
                return ((string)(base[UrlPropertyName]));
            }
            set
            {
                base[UrlPropertyName] = value;
            }
        }

        [Description("The Name.")]
        [ConfigurationProperty(NamePropertyName, IsRequired = true, IsKey = true, IsDefaultCollection = false)]
        public virtual string Name
        {
            get
            {
                return ((string)(base[NamePropertyName]));
            }
            set
            {
                base[NamePropertyName] = value;
            }
        }

        [Description("The ClientId.")]
        [StringValidator(InvalidCharacters = ".", MaxLength = 36)]
        [ConfigurationProperty(ClientIdPropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false)]
        public virtual string ClientId
        {
            get
            {
                return ((string)(base[ClientIdPropertyName]));
            }
            set
            {
                base[ClientIdPropertyName] = value;
            }
        }

        
    }
}

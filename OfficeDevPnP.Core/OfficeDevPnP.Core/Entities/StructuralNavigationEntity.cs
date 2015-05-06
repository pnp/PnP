using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Entities
{
    public class StructuralNavigationEntity
    {
        public StructuralNavigationEntity()
        {
            MaxDynamicItems = 20;
            ShowSubsites = true;
            ShowPages = false;
        }

        public bool ManagedNavigation { get; internal set; }
        public bool ShowSubsites { get; set; }
        public bool ShowPages { get; set; }
        public uint MaxDynamicItems { get; set; }
        public bool ShowSiblings { get; set; }

    }
}

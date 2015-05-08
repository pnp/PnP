using OfficeDevPnP.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Entities
{
    public class AreaNavigationEntity
    {

        public AreaNavigationEntity()
        {
            GlobalNavigation = new StructuralNavigationEntity();
            CurrentNavigation = new StructuralNavigationEntity();
        }

        public StructuralNavigationEntity GlobalNavigation { get; set; }

        public StructuralNavigationEntity CurrentNavigation { get; set; }

        public StructuralNavigationSorting Sorting { get; set; }

        public Boolean SortAscending { get; set; }

        public StructuralNavigationSortBy SortBy { get; set; }

    }
}

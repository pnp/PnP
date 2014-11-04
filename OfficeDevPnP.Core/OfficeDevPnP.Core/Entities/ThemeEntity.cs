using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Entities
{
    public class ThemeEntity
    {
        public string MasterPage { get; set; }

        public string CustomMasterPage { get; set; }

        public string Theme { get; set; }

        public string BackgroundImage { get; set; }

        public string Font { get; set; }
    }
}

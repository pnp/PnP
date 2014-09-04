using OfficeAMS.Core.Entities;
using Patterns.Provisioning.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Patterns.Provisioning
{
    public class SiteCreatedEventArgs: EventArgs
    {
        private SiteEntity siteInformation;

        public SiteCreatedEventArgs(SiteEntity siteInformation)
        {
            this.siteInformation = siteInformation;
        }

        public SiteEntity SiteInformation
        {
            get
            {
                return this.siteInformation;
            }
        }
    }
}

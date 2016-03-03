using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Extensibility.Providers
{
    public class PublishingPageWebPart : WebPart
    {
        public string DefaultViewDisplayName { get; set; }
        public bool IsListViewWebPart
        {
            get
            {
                return DefaultViewDisplayName != null;
            }            
        }
    }
}

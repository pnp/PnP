using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model.Comparers
{
    public static class ComposedLookComparer
    {
        public static Func<ComposedLook, bool> GetComparer(ComposedLook cl)
        {
            return (other) =>
            {
                return (cl.AlternateCSS == other.AlternateCSS &&
                    cl.BackgroundFile == other.BackgroundFile &&
                    cl.ColorFile == other.ColorFile &&
                    cl.FontFile == other.FontFile &&
                    cl.MasterPage == other.MasterPage &&
                    cl.Name == other.Name &&
                    cl.SiteLogo == other.SiteLogo &&
                    cl.Version == other.Version);
            };
        }
    }
}

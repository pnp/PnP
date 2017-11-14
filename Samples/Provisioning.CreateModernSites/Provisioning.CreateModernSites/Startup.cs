using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace Provisioning.CreateModernSites
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

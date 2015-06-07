using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Office365AddIn.GroupAuthorization.Startup))]
namespace Office365AddIn.GroupAuthorization
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

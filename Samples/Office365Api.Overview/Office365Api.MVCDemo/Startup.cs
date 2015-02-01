using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Office365Api.MVCDemo.Startup))]
namespace Office365Api.MVCDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

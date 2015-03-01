using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Office365Api.WebFormsDemo.Startup))]
namespace Office365Api.WebFormsDemo
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

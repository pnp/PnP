using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureAD.RedisCacheUserProfile.Startup))]
namespace AzureAD.RedisCacheUserProfile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

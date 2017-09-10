using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeDevPnP.Core.Framework.Authentication;
using OfficeDevPnP.Core.Framework.Authentication.Events;

namespace AspNetCore.Mvc.StarterWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //Add Session to the service collection
            services.AddSession();

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = SharePointAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = SharePointAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = SharePointAuthenticationDefaults.AuthenticationScheme;             
            })
            //OPTIONAL
            ////.AddCookie(options =>
            ////{
            ////    options.Cookie.HttpOnly = false; //set to false so we can read it from JavaScript
            ////    options.Cookie.Expiration = TimeSpan.FromDays(14);
            ////})
            .AddSharePoint(options =>
            {
                options.ClientId = Configuration["SharePointAuthentication:ClientId"];
                options.ClientSecret = Configuration["SharePointAuthentication:ClientSecret"];
                //OPTIONAL
                ////options.CookieAuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                //Handle events raised by the auth handler
                options.Events = new SharePointAuthenticationEvents()
                {
                    OnAuthenticationSucceeded = succeededContext => Task.FromResult<object>(null),
                    OnAuthenticationFailed = failedContext => Task.FromResult<object>(null)                    
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Required to store SP Cache Key session data
            app.UseSession();

            // Added to configure authentication. SharePoint authentication is enabled in ConfigureServices for .net core 2
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private void ConfigureSharePointAuthentication(IApplicationBuilder app)
        {
            //required to store SP Cache Key session data
            //must also call AddSession in the IServiceCollection
            app.UseSession();
        }
    }
}
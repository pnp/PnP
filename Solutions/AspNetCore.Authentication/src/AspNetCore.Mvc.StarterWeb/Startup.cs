using System;
using System.Collections.Generic;
using System.Linq;
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
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
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

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //Added to enable SharePoint authentication
            ConfigureSharePointAuthentication(app);
        }

        private void ConfigureSharePointAuthentication(IApplicationBuilder app)
        {
            //required to store SP Cache Key session data
            app.UseSession();

            //UseCookieAuthentication is required to do client session management
            //It is set to use the Authentication schema of our middleware
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AutomaticAuthenticate = true,
                CookieHttpOnly = false, //set to false so we can read it from JavaScript
                AutomaticChallenge = false,
                AuthenticationScheme = "AspNet.ApplicationCookie",
                ExpireTimeSpan = System.TimeSpan.FromDays(14),
                LoginPath = "/account/login"
            }
            );

            //Add SharePoint authentication capabilities
            app.UseSharePointAuthentication(
                new SharePointAuthenticationOptions()
                {
                    CookieAuthenticationScheme = "AspNet.ApplicationCookie",
                    //I really don't like how config settings are retrieved, but that is how the ASP.NET guys do it in their samples
                    ClientId = Configuration["SharePointAuthentication:ClientId"],
                    ClientSecret = Configuration["SharePointAuthentication:ClientSecret"],
                    //Handle events thrown by the auth handler
                    Events = new SharePointAuthenticationEvents()
                    {
                        OnAuthenticationSucceeded = succeededContext =>
                        {
                            return Task.FromResult<object>(null);
                        },
                        OnAuthenticationFailed = failedContext =>
                        {
                            return Task.FromResult<object>(null);
                        }
                    }
                }
            );
        }
    }
}

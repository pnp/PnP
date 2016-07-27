using System;
using System.IO;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeDevPnP.Core.Framework.Authentication;
using Microsoft.AspNet.Authentication.Cookies;
using OfficeDevPnP.Core.Framework.Authentication.Events;
using System.Threading.Tasks;

namespace AspNet5.Mvc6.StarterWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCaching();
            services.AddSession(o => { o.IdleTimeout = TimeSpan.FromSeconds(3600); });

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            #region Logging
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            #endregion

            app.UseStaticFiles();

            //Configuer SSL, only needed due to Kestrel and server/host management in ASP.NET Core
            WebServerConfig.ConfigureSSL(
                app,
                Path.Combine(env.WebRootPath, Configuration["WebServerSettings:CertificateFilePath"]),
                Configuration["WebServerSettings:CertificatePassword"]
            );

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

            //set up MVC routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.StarterWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 5000, listenOptions =>
                    {
                        listenOptions.UseHttps(@"..\..\certificates\localhost_ssl.pfx", "pass@word1");
                        listenOptions.NoDelay = true;
                    });

                })
                .UseStartup<Startup>();

            return webHostBuilder.Build();
        }
    }
}
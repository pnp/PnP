using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Filter;
using AspNetCore.Mvc.StarterWeb.Extensions;

namespace AspNetCore.Mvc.StarterWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = new WebHostBuilder()
                    .UseKestrel(options =>
                        {
                            options.UseHttps(@"..\..\certificates\localhost_ssl.pfx", "pass@word1");
                            options.NoDelay = true;

                            //I use this to get rid of SSL errors, feel free to remove it.
                            IConnectionFilter prevFilter = options.ConnectionFilter ?? new NoOpConnectionFilter();
                            options.ConnectionFilter = new IgnoreSslErrorsConnectionFilter(prevFilter);
                        }
                    )
                    .UseUrls("https://localhost:5000")
                    .UseContentRoot(Directory.GetCurrentDirectory());

                host.UseStartup<Startup>();

                var webHost = host.Build();

                webHost.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
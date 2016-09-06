using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCore.Mvc.StarterWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("https://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration() not needed here 
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
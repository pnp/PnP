namespace AspNet5.Mvc6.StarterWeb
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Server.Kestrel.Https;
    using System.Net;
    using System.Net.Security;

    public class WebServerConfig
    {
        /// <summary>
        /// Configures Kestrel to use HTTPS with the provided certificate path and file
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance of the application</param>
        /// <param name="certPath">The file system path to the certificate file</param>
        /// <param name="password">The password of the certificate</param>
        public static void ConfigureSSL(IApplicationBuilder app, string certPath, string password)
        {
            if (string.IsNullOrEmpty(certPath))
            {
                throw new ArgumentException("The provided certificate path is invalid.");
            }

            app.UseKestrelHttps(new X509Certificate2(certPath, password));
        }

        /// <summary>
        /// Calling this method will register a callback to successfully 
        /// validate any certificate and ignore certificate errors
        /// </summary>
        public static void IgnoreSslErrors()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate,
                        X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        }
    }
}
namespace OfficeDevPnP.Core.Framework.Authentication
{
    using System;
    using Microsoft.AspNetCore.Builder;

    public static class SharePointAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="SharePointAuthenticationMiddleware"/> middleware to the specified 
        /// <see cref="IApplicationBuilder"/>, which enables SharePoint OAuth token processing capabilities.
        /// This middleware understands appropriately
        /// formatted and secured tokens from SharePoint which appear in the request header.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseSharePointAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<SharePointAuthenticationMiddleware>();
        }

        /// <summary>
        /// Adds the <see cref="SharePointAuthenticationMiddleware"/> middleware to the specified 
        /// <see cref="IApplicationBuilder"/>, which enables SharePoint OAuth token processing capabilities.
        /// This middleware understands appropriately
        /// formatted and secured tokens from SharePoint, which appear in the request header.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">A  <see cref="SharePointAuthenticationOptions"/> that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseSharePointAuthentication(this IApplicationBuilder app, SharePointAuthenticationOptions options)
        {
            if (app == null) { throw new ArgumentNullException(nameof(app)); }
            if (options == null) { throw new ArgumentNullException(nameof(options)); }

            return app.UseMiddleware<SharePointAuthenticationMiddleware>(options);
        }
    }
}
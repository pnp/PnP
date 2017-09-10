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
        [Obsolete("UseSharePointAuthentication is obsolete. Configure SharePoint authentication with AddAuthentication().AddSharePoint in ConfigureServices. See https://go.microsoft.com/fwlink/?linkid=845470 for more details.", error: true)]
        public static IApplicationBuilder UseSharePointAuthentication(this IApplicationBuilder app)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
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
        [Obsolete("UseSharePointAuthentication is obsolete. Configure SharePoint authentication with AddAuthentication().AddSharePoint in ConfigureServices. See https://go.microsoft.com/fwlink/?linkid=845470 for more details.", error: true)]
        public static IApplicationBuilder UseSharePointAuthentication(this IApplicationBuilder app, SharePointAuthenticationOptions options)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }
    }
}
namespace OfficeDevPnP.Core.Framework.Authentication
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Text.Encodings.Web;

    /// <summary>
    /// An ASP.NET Core middleware for authenticating users using SharePoint.
    /// </summary>
    public class SharePointAuthenticationMiddleware :
        AuthenticationMiddleware<SharePointAuthenticationOptions>
    {
        private readonly RequestDelegate _nextMiddleware;

        /// <summary>
        /// Initializes a new <see cref="SharePointAuthenticationMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the HTTP pipeline to invoke.</param>
        /// <param name="loggerFactory"></param>
        /// <param name="encoder"></param>
        /// <param name="options">Configuration options for the middleware.</param>
        public SharePointAuthenticationMiddleware(
            RequestDelegate nextMiddleware,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IOptions<SharePointAuthenticationOptions> options)
            : base(nextMiddleware, options, loggerFactory, encoder)
        {
            if (nextMiddleware == null) { throw new ArgumentNullException(nameof(nextMiddleware)); }
            if (loggerFactory == null) { throw new ArgumentNullException(nameof(loggerFactory)); }
            if (encoder == null) { throw new ArgumentNullException(nameof(encoder)); }
            if (options == null) { throw new ArgumentNullException(nameof(options)); }

            _nextMiddleware = nextMiddleware;
        }

        /// <summary>
        /// Provides the <see cref="AuthenticationHandler{T}"/> object for processing authentication-related requests.
        /// </summary>
        /// <returns>An <see cref="AuthenticationHandler{T}"/> configured with the <see cref="SharePointAuthenticationOptions"/> supplied to the constructor.</returns>
        protected override AuthenticationHandler<SharePointAuthenticationOptions> CreateHandler()
        {
            return new SharePointAuthenticationHandler();
        }
    }
}
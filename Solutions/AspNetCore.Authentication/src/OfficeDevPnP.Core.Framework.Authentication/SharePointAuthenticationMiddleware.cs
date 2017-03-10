namespace OfficeDevPnP.Core.Framework.Authentication
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Net.Http;
    using System.Text.Encodings.Web;

    /// <summary>
    /// An ASP.NET Core middleware for authenticating users using SharePoint.
    /// </summary>
    public class SharePointAuthenticationMiddleware :
        AuthenticationMiddleware<SharePointAuthenticationOptions>
    {
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
            IOptions<SharedAuthenticationOptions> sharedOptions,
            IOptions<SharePointAuthenticationOptions> options)
            : base(nextMiddleware, options, loggerFactory, encoder)
        {
            if (nextMiddleware == null) { throw new ArgumentNullException(nameof(nextMiddleware)); }
            if (loggerFactory == null) { throw new ArgumentNullException(nameof(loggerFactory)); }
            if (encoder == null) { throw new ArgumentNullException(nameof(encoder)); }
            if (options == null) { throw new ArgumentNullException(nameof(options)); }

            if (string.IsNullOrEmpty(Options.SignInScheme))
            {
                Options.SignInScheme = sharedOptions.Value.SignInScheme;
            }

            if (string.IsNullOrEmpty(Options.SignInScheme))
            {
                throw new ArgumentException("Options.SignInScheme is required.");
            }

            Backchannel = new HttpClient(Options.BackchannelHttpHandler ?? new HttpClientHandler());
            Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("OfficeDev PnP ASP.NET Core Authentication middleware");
            Backchannel.Timeout = Options.BackchannelTimeout;
            Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB 
        }

        protected HttpClient Backchannel { get; private set; }

        /// <summary>
        /// Provides the <see cref="AuthenticationHandler{T}"/> object for processing authentication-related requests.
        /// </summary>
        /// <returns>An <see cref="AuthenticationHandler{T}"/> configured with the <see cref="SharePointAuthenticationOptions"/> supplied to the constructor.</returns>
        protected override AuthenticationHandler<SharePointAuthenticationOptions> CreateHandler()
        {
            return new SharePointAuthenticationHandler(Backchannel);
        }
    }
}
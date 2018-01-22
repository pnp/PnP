namespace OfficeDevPnP.Core.Framework.Authentication
{
    using System;
    using System.Net.Http;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Options;
    /// <summary>
    /// Used to setup defaults for the SharePointAuthenticationOptions.
    /// </summary>
    public class SharePointAuthenticationPostConfigureOptions : IPostConfigureOptions<SharePointAuthenticationOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public SharePointAuthenticationPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        public void PostConfigure(string name, SharePointAuthenticationOptions options)
        {
            options.AuthenticationProperties = options.AuthenticationProperties ?? new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(10),
                IsPersistent = false,
                AllowRefresh = false
            };

            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;
            if (options.Backchannel == null)
            {
                options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
                options.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("OfficeDev PnP ASP.NET Core Authentication handler");
                options.Backchannel.Timeout = options.BackchannelTimeout;
                options.Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
            }                        
        }
    }
}

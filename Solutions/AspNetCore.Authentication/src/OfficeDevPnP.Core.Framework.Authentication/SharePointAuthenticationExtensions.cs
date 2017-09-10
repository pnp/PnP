namespace OfficeDevPnP.Core.Framework.Authentication
{
    using System;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
        
    public static class SharePointAuthenticationExtensions
    {
        public static AuthenticationBuilder AddSharePoint(this AuthenticationBuilder builder, Action<SharePointAuthenticationOptions> configureOptions)
            => builder.AddSharePoint(SharePointAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddSharePoint(this AuthenticationBuilder builder, string authenticationScheme, Action<SharePointAuthenticationOptions> configureOptions)
            => builder.AddSharePoint(authenticationScheme, SharePointAuthenticationDefaults.DisplayName, configureOptions);


        public static AuthenticationBuilder AddSharePoint(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<SharePointAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<SharePointAuthenticationOptions>, SharePointAuthenticationPostConfigureOptions>());
            return builder.AddRemoteScheme<SharePointAuthenticationOptions, SharePointAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

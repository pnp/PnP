namespace OfficeDevPnP.Core.Framework.Authentication.Events
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    public class AuthenticationFailedContext : BaseSharePointAuthenticationContext
    {
        public AuthenticationFailedContext(HttpContext context, AuthenticationScheme scheme, SharePointAuthenticationOptions options, AuthenticationProperties properties)
            : base(context, scheme, options, properties) { }
    }
}
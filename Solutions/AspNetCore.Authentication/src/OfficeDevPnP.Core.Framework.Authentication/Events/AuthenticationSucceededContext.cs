namespace OfficeDevPnP.Core.Framework.Authentication.Events
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;

    public class AuthenticationSucceededContext : BaseSharePointAuthenticationContext
    {
        public AuthenticationSucceededContext(HttpContext context, AuthenticationScheme scheme, SharePointAuthenticationOptions options, AuthenticationProperties properties)
               : base(context, scheme, options, properties) { }

        public SharePointContext SharePointContext { get; set; }
    }
}

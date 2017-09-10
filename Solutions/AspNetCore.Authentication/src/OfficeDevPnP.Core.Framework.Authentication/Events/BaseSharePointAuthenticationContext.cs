namespace OfficeDevPnP.Core.Framework.Authentication.Events
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using System;

    public class BaseSharePointAuthenticationContext : RemoteAuthenticationContext<SharePointAuthenticationOptions>
    {
        public BaseSharePointAuthenticationContext(HttpContext context, AuthenticationScheme scheme, SharePointAuthenticationOptions options, AuthenticationProperties properties)
            : base(context, scheme, options, properties) { }
        
        /// <summary>
        /// Gets or set the <see cref="Ticket"/> to return if this event signals it handled the event.
        /// </summary>
        public AuthenticationTicket Ticket { get; set; }
    }
}

namespace OfficeDevPnP.Core.Framework.Authentication.Events
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using System;

    public class BaseSharePointAuthenticationContext : BaseControlContext
    {
        public BaseSharePointAuthenticationContext(HttpContext context, SharePointAuthenticationOptions options)
            : base(context)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;
        }

        public SharePointAuthenticationOptions Options { get; }
    }
}

using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Authentication.Events
{
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

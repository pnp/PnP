using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Filter;

namespace AspNetCore.Mvc.StarterWeb.Extensions
{
    public class IgnoreSslErrorsConnectionFilter : IConnectionFilter
    {
        private readonly IConnectionFilter _previousFilter;

        public IgnoreSslErrorsConnectionFilter(IConnectionFilter previousFilter)
        {
            if (previousFilter == null)
            {
                throw new ArgumentNullException(nameof(previousFilter));
            }

            _previousFilter = previousFilter;
        }

        public async Task OnConnectionAsync(ConnectionFilterContext context)
        {
            try
            {
                await _previousFilter.OnConnectionAsync(context);
            }
            catch (System.IO.IOException ioEx)
            {
                //compares by exception message for the time being, but needs a better solution

                //SSL Certificate error
                if (!ioEx.Message.Equals("Authentication failed because the remote party has closed the transport stream."))
                {
                    //throw ioEx; //do something
                }

                //non-SSL request
                if (!ioEx.Message.Equals("The handshake failed due to an unexpected packet format."))
                {
                    //throw ioEx; //do something
                }

                throw ioEx;
            }
        }
    }
}

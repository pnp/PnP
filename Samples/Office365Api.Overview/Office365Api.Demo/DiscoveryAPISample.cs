using Microsoft.Office365.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    static class DiscoveryAPISample
    {
        // Do not make static in Web apps; store it in session or in a cookie instead
        static string _lastLoggedInUser;
        //static DiscoveryContext _discoveryContext;
        public static DiscoveryContext _discoveryContext
        {
            get;
            set;
        }
        
        // Discovery service supports MyFiles, Mail, Contacts and Calendar
        public static async Task<CapabilityDiscoveryResult> DiscoverMyFiles()
        {
            if (_discoveryContext == null)
            {
                _discoveryContext = await DiscoveryContext.CreateAsync();
            }

            var dcr = await _discoveryContext.DiscoverCapabilityAsync("MyFiles");
            return dcr;
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverMail()
        {
            if (_discoveryContext == null)
            {
                _discoveryContext = await DiscoveryContext.CreateAsync();
            }

            var dcr = await _discoveryContext.DiscoverCapabilityAsync("Mail");
            return dcr;
        }
    }
}

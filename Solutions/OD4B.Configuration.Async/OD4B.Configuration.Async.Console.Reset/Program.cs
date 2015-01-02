using Microsoft.SharePoint.Client;
using OD4B.Configuration.Async.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OD4B.Configuration.Async.Console.Reset
{
    /// <summary>
    /// Can be used to reset the current settings in OD4B sites compared on what this sample is doing.
    /// Make sure that your app ID and secret matches what is expected.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            Uri url = new Uri("https://vesaj-my.sharepoint.com/personal/vesaj_veskuonline_com");

             //get the new site collection
            string realm = TokenHelper.GetRealmFromTargetUrl(url);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, url.Authority, realm).AccessToken;
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(url.ToString(), token))
            {
                // Uncomment the one you need for testing/reset
                // Apply(ctx, url);
                Reset(ctx);
            }
        }

        private static void Apply(ClientContext ctx, Uri url)
        {
            // Set configuration object properly for setting the config
            SiteModificationConfig config = new SiteModificationConfig()
            {
                SiteUrl = url.ToString(),
                JSFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\OneDriveConfiguration.js"),
                ThemeName = "Garage",
                ThemeColorFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Themes\\Garage\\garagewhite.spcolor"),
                ThemeBGFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Themes\\Garage\\garagebg.jpg"),
                ThemeFontFile = ""
            };

            new SiteModificationManager().ApplySiteConfiguration(ctx, config);
        }

        private static void Reset(ClientContext ctx)
        {
            new SiteModificationManager().ResetSiteConfiguration(ctx);
        }
    }
}

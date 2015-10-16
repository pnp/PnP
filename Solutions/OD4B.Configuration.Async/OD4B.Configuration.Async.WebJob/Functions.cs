using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using OD4B.Configuration.Async.Common;

namespace OD4B.Configuration.Async.WebJob
{
    /// <summary>
    /// Actual functions to process the queued actions
    /// </summary>
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage(
            [QueueTrigger(SiteModificationManager.StorageQueueName)] 
            SiteModificationData request, TextWriter log)
        {
            Uri url = new Uri(request.SiteUrl);

            //Connect to the OD4B site using App Only token
            string realm = TokenHelper.GetRealmFromTargetUrl(url);
            var token = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal, url.Authority, realm).AccessToken;

            using (var ctx = TokenHelper.GetClientContextWithAccessToken(
                url.ToString(), token))
            {
                // Set configuration object properly for setting the config
                SiteModificationConfig config = new SiteModificationConfig()
                {
                    SiteUrl = url.ToString(),
                    JSFile = Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources\\OneDriveConfiguration.js"),
                    ThemeName = "Garage",
                    ThemeColorFile = 
                        Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources\\Themes\\Garage\\garagewhite.spcolor"),
                    ThemeBGFile = 
                        Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources\\Themes\\Garage\\garagebg.jpg"),
                    ThemeFontFile = "" // Ignored in this case, but could be also set
                };

                new SiteModificationManager().ApplySiteConfiguration(ctx, config);
            }
        }
    }
}

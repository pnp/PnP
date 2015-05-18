using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using OD4B.Configuration.Async.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OD4B.Configuration.Async.Console.SendMessage
{
    /// <summary>
    /// Can be used to send messages to the queue for processing for debugging purposes
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string accountId = "vesaj@veskuonline.com";
            string siteUrl = "https://vesaj-my.sharepoint.com/personal/vesaj_veskuonline_com";

            new SiteModificationManager().AddConfigRequestToQueue(accountId, siteUrl, 
                                            CloudConfigurationManager.GetSetting("StorageConnectionString"));

        }
    }
}

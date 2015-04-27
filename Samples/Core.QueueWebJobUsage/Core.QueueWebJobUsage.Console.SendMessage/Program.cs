using Core.QueueWebJobUsage.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.QueueWebJobUsage.Console.SendMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            string requestorName = "Keyser Söze";
            string siteUrl = "https://vesaj.sharepoint.com/sites/dev";

            SiteModifyRequest request = new SiteModifyRequest() { RequestorName = requestorName, SiteUrl = siteUrl };

            new SiteManager().AddAsyncOperationRequestToQueue(request,
                                                              ConfigurationManager.AppSettings["StorageConnectionString"]);
        }
    }
}

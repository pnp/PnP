using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Job
{
    class AzureWebJobHelper
    {

        public const string AZUREWEBJOBNAME = "AzureWebJob_Name";
        public const string AZUREWEBSITENAME = "AzureWebJob_SiteName";
        public const string AZUREWEBSITEUSERNAME = "AzureWebJob_UserName";
        public const string AZUREWEBSITEPASSWORD = "AzureWebJob_Password";
        /// <summary>
        /// Use this method to keep alive the continous Azure Web Jobs when using the Free tier.
        /// Note: Using the free tier should not be the end goal and does not guarantee any SLAs
        /// </summary>
        /// <param name="webjobUrl"></param>
        /// <param name="userName"></param>
        /// <param name="userPWD"></param>
        /// <returns></returns>
        public static JObject GetWebjobState(string webjobUrl, string userName, string userPWD)
        {
            try {
                using (HttpClient client = new HttpClient())
                {
                    Log.Info("Provisioning.Job.AzureWebJobHelper.GetWebjobState", "Pinging Azure Web Job SCM {0}.", webjobUrl);

                    string auth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ':' + userPWD));
                    client.DefaultRequestHeaders.Add("authorization", auth);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var data = client.GetStringAsync(webjobUrl).Result;
                    var result = JsonConvert.DeserializeObject(data) as JObject;
                    Log.Info("Provisioning.Job.AzureWebJobHelper.GetWebjobState", "Ping result: {0}.", result);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Provisioning.Job.AzureWebJobHelper.GetWebjobState", "Pinging failed: " + ex.ToString());
                return null;
            }
        }

    }
}

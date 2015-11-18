using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC = System.Configuration;


namespace Provisioning.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            var _spj = new SiteProvisioningJob();
            _spj.ProcessSiteRequests();

            // Send keep-alive if this is an Azure Web Job (AzureWebJob_Name must be set to initiate this ping)
            if (SC.ConfigurationManager.AppSettings.AllKeys.Contains(AzureWebJobHelper.AZUREWEBJOBNAME))
            {
                AzureWebJobHelper.GetWebjobState(
                    string.Format("https://{0}.scm.azurewebsites.net/api/continuouswebjobs/{1}", 
                        SC.ConfigurationManager.AppSettings.Get(AzureWebJobHelper.AZUREWEBSITENAME),
                        SC.ConfigurationManager.AppSettings.Get(AzureWebJobHelper.AZUREWEBJOBNAME)),
                    SC.ConfigurationManager.AppSettings.Get(AzureWebJobHelper.AZUREWEBSITEUSERNAME),
                    SC.ConfigurationManager.AppSettings.Get(AzureWebJobHelper.AZUREWEBSITEPASSWORD)
                    );
            }
                
            

        }
    }
}

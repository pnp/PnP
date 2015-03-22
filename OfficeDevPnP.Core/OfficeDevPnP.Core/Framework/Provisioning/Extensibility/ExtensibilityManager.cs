using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Extensibility
{
    /// <summary>
    ///  Provisioning Framework Component that is used for invoking custom providers during the provisioning process.
    /// </summary>
    public class ExtensibilityManager
    {
        /// <summary>
        /// Method to Invoke Custom Provisioning Providers
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="provider"></param>
        /// <param name="template"></param>
        public void ExecuteCallout(ClientContext ctx, Provider provider, ProvisioningTemplate template)
        {
            if (string.IsNullOrWhiteSpace(provider.Assembly))
            {
                //TODO USE RESOURCE FILE
                Log.Warning("OfficeDevPnP.Core.Framework.Provisioning.Extensibility.ExtensibilityManager.ExecuteCallout", "Provider.Assembly did not contain a value.");
                return;
            }

            if(string.IsNullOrWhiteSpace(provider.Type))
            {
                //TODO USE RESOURCE FILE
                Log.Warning("OfficeDevPnP.Core.Framework.Provisioning.Extensibility.ExtensibilityManager.ExecuteCallout", "Provider.Type  did not contain a value.");
                return;
            }

            try
            {
                //TODO USE RESOURCE FILES
                Log.Info("OfficeDevPnP.Core.Framework.Provisioning.Extensibility.ExtensibilityManager.ExecuteCallout",
                    "Begin Invoke, Assembly {0}, Type {1}",
                    provider.Assembly,
                    provider.Type);

                var _instance = (IProvisioningExtensibility)Activator.CreateInstance(provider.Assembly, provider.Type).Unwrap();
                _instance.ProcessRequest(ctx, template, provider.Configuration);

                Log.Info("OfficeDevPnP.Core.Framework.Provisioning.Extensibility.ExtensibilityManager.ExecuteCallout",
                 "Provider Invoke Successful, Assembly {0}, Type {1}",
                 provider.Assembly,
                 provider.Type);
            }
            catch(Exception ex)
            {
                //TODO USE RESOURCE FILE THROW CUSTOM EXCEPTION
                Log.Error("OfficeDevPnP.Core.Framework.Provisioning.Extensibility.ExtensibilityManager.ExecuteCallout", 
                    "There was an exception invoking custom provider. Assembly: {0}, Type: {1}. Exception {2}", 
                    provider.Assembly, 
                    provider.Type, 
                    ex);
                throw;
            }
        }
    }
}

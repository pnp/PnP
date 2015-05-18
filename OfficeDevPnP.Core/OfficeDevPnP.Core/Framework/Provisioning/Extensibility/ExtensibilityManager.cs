using System;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.Extensibility
{
    /// <summary>
    ///  Provisioning Framework Component that is used for invoking custom providers during the provisioning process.
    /// </summary>
    public class ExtensibilityManager
    {
        /// <summary>
        /// Method to Invoke Custom Provisioning Providers. 
        /// Ensure the ClientContext is not dispose in the custom provider.
        /// </summary>
        /// <param name="ctx">Authenticated ClientContext that is passed to teh custom provider.</param>
        /// <param name="provider">A custom Extensibility Provisioning Provider</param>
        /// <param name="template">ProvisioningTemplate that is passed to the custom provider</param>
        /// <exception cref="ExtensiblityPipelineException"></exception>
        /// <exception cref="ArgumentException">Provider.Assembly or Provider.Type is NullOrWhiteSpace></exception>
        /// <exception cref="ArgumentNullException">ClientContext is Null></exception>
        public void ExecuteExtensibilityCallOut(ClientContext ctx, Provider provider, ProvisioningTemplate template)
        {
            var _loggingSource = "OfficeDevPnP.Core.Framework.Provisioning.Extensibility.ExtensibilityManager.ExecuteCallout";

            if (ctx == null) {
                throw new ArgumentNullException(CoreResources.Provisioning_Extensibility_Pipeline_ClientCtxNull);
            }

            if (string.IsNullOrWhiteSpace(provider.Assembly)) {
                throw new ArgumentException(CoreResources.Provisioning_Extensibility_Pipeline_Missing_AssemblyName);
            }

            if(string.IsNullOrWhiteSpace(provider.Type)) {
                throw new ArgumentException(CoreResources.Provisioning_Extensibility_Pipeline_Missing_TypeName);
            }

            try
            {
                Log.Info(_loggingSource,
                    CoreResources.Provisioning_Extensibility_Pipeline_BeforeInvocation,
                    provider.Assembly,
                    provider.Type);

                var _instance = (IProvisioningExtensibilityProvider)Activator.CreateInstance(provider.Assembly, provider.Type).Unwrap();
                _instance.ProcessRequest(ctx, template, provider.Configuration);

                Log.Info(_loggingSource,
                    CoreResources.Provisioning_Extensibility_Pipeline_Success,
                    provider.Assembly,
                    provider.Type);
            }
            catch(Exception ex)
            {
                string _message = string.Format(
                    CoreResources.Provisioning_Extensibility_Pipeline_Exception, 
                    provider.Assembly, 
                    provider.Type, 
                    ex);
                Log.Error(_loggingSource, _message);
                throw new ExtensiblityPipelineException(_message, ex);
             
            }
        }
    }
}

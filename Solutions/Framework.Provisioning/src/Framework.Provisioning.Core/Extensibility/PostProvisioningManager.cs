using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Extensibility
{
    /// <summary>
    /// Class to handle Post Provisioning call out to custom Post Provivision Providers. You 
    /// must implement <see cref="IPostProvisioningProvider"/> and have the assembly deployed with the Provisioning Engine.
    /// </summary>
    public class PostProvisioningManager
    {
        /// <summary>
        /// Member to call out to the Extensibility Provider. The Assemlby must implement <see cref="IPostProvisioningProvider"/> and the
        /// assembly must be located in the same directory.
        /// </summary>
        /// <param name="postProvider"></param>
        /// <param name="siteInfo"></param>
        public void Execute(Provider postProvider, SiteRequestInformation siteInfo)
        {
            string assemblyName = postProvider.Assembly;
            string typeName = postProvider.Type;
            try
            {
                var instance = (IPostProvisioningProvider)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                Log.Info("Framework.Provisioning.Core.Extensibility.PostProvisioningManager.Execute", "Preparing to invoke, Assembly {0}. Type {1}.", assemblyName, typeName);
                instance.ProcessRequest(siteInfo, postProvider.Configuration );
                Log.Info("Framework.Provisioning.Core.Extensibility.PostProvisioningManager.Execute", "Provider call out successful, Assembly {0}. Type {1}.", assemblyName, typeName);
          
            }
            catch(Exception ex)
            {
                Log.Fatal("Framework.Provisioning.Core.Extensibility.PostProvisioningManager.Execute", "There was an exception invoking Provider. Assembly {0}. Type {1}. Exception {2}", assemblyName, typeName, ex);
                throw;
            }

        }
    }
}

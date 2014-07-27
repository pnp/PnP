using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
// Can be found in Microsoft.Web.Administration.dll, located in C:\Windows\System32\inetsrv
using Microsoft.Web.Administration;
using System.IO;
using System.Diagnostics;

namespace Contoso.Core.CloudServices.Web
{
    /// <summary>
    /// Entry point for the Azure Web role
    /// </summary>
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            
            // Only change application pool account when running in Azure, no need to change this for the emulator as the emulator requires you to run with administrative privileges
            if (!RoleEnvironment.IsEmulated)
            {
                //Use the SetAppPoolIdentity method in case you want to use the tenant administration CSOM library in combination with 
                //specifying credentials via the SharePointOnlineCredentials class
                //SetAppPoolIdentity();
            }

            return base.OnStart();
        }

        /// <summary>
        /// This method iterates over the used application pools of the Azure Cloud Services Web role and changes the application pool account from NETWORK to SYSTEM. This is
        /// needed to make to code work with the Microsoft Online Services Sign In Assistant
        /// </summary>
        private void SetAppPoolIdentity()
        {

            Action<string> iis7fix = (appPoolName) =>
            {
                bool committed = false;
                while (!committed)
                {
                    try
                    {
                        using (ServerManager sm = new ServerManager())
                        {
                            var applicationPool = sm.ApplicationPools[appPoolName];
                            applicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.LocalSystem;
                            sm.CommitChanges();
                            committed = true;
                        }
                    }
                    catch (FileLoadException fle)
                    {
                        Trace.TraceError("Trying again because: " + fle.Message);
                    }
                }
            };

            var sitename = RoleEnvironment.CurrentRoleInstance.Id + "_Web";
            var appPoolNames = new ServerManager().Sites[sitename].Applications.Select(app => app.ApplicationPoolName).ToList();
            appPoolNames.ForEach(iis7fix);
        }
    }

}

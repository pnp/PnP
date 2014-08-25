using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Core
{
    [Obsolete("Use OfficeDev/PnP.Core")]
    public static class SPOApp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        [Obsolete("Use OfficeDev/PnP.Core")]
        public static ClientObjectList<AppInstance> GetAppInstances(ClientContext clientContext)
        {
            ClientObjectList<AppInstance> instances = Microsoft.SharePoint.Client.AppCatalog.GetAppInstances(clientContext, clientContext.Web);
            clientContext.Load(instances);
            clientContext.ExecuteQuery();

            return instances;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        [Obsolete("Use OfficeDev/PnP.Core")]
        public static AppInstance LoadAndInstallApp(Stream stream, Web web, bool LoadOnly, int LCID)
        {
            ClientContext clientContext = web.Context as ClientContext;
            AppInstance appInstance = null;
            if(LoadOnly)
            {
                appInstance = web.LoadApp(stream, LCID);
            }
            else
            {
                appInstance = web.LoadAndInstallAppInSpecifiedLocale(stream, LCID);
            }
            
            clientContext.Load(appInstance);
            clientContext.ExecuteQuery();

            return appInstance;
        }

        [Obsolete("Use OfficeDev/PnP.Core")]
        public static void UninstallApp(Guid appId, ClientContext clientContext)
        {
            var instances = GetAppInstances(clientContext) as ClientObjectList<AppInstance>;
            var instance = instances.FirstOrDefault(a => a.Id == appId);
            if(instance != null)
            {
                UninstallApp(instance, clientContext);
            }
        }

        [Obsolete("Use OfficeDev/PnP.Core")]
        public static void UninstallApp(AppInstance appInstance, ClientContext clientContext)
        {
            appInstance.Uninstall();
            clientContext.ExecuteQuery();
        }
    }
}

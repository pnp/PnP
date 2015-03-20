using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;


namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectFiles : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var context = web.Context as ClientContext;

            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                context.Load(web, w => w.ServerRelativeUrl);
                context.ExecuteQueryRetry();
            }

            foreach (var file in template.Files)
            {
                var fileInfo = new FileInfo(file.Src);
                var folder = web.EnsureFolderPath(file.TargetFolder);

                if (System.IO.File.Exists(file.Src))
                {
                    folder.UploadFile(fileInfo.Name, fileInfo.FullName, file.Overwrite);
                }
                else
                {
                    Log.Error("Source File {0} does not exist",file.Src);
                }
            }
           
        }


        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            // Impossible to return all files in the site currently

            return template;
        }
    }
}

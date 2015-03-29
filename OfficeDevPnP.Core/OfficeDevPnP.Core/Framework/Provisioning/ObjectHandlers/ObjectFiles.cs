using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;


namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectFiles : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            TokenParser parser = new TokenParser(web);
            var context = web.Context as ClientContext;

            if (!web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                context.Load(web, w => w.ServerRelativeUrl);
                context.ExecuteQueryRetry();
            }

            foreach (var file in template.Files)
            {

                var folderName = parser.Parse(file.Folder);

                if (folderName.ToLower().StartsWith((web.ServerRelativeUrl.ToLower())))
                {
                    folderName = folderName.Substring(web.ServerRelativeUrl.Length);
                }
                

                var folder = web.EnsureFolderPath(folderName);

                using (var stream = template.Connector.GetFileStream(file.Src))
                {
                    folder.UploadFile(file.Src, stream, true);
                }

            }

        }


        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {
            // Impossible to return all files in the site currently

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (baseTemplate != null)
            {
                template = CleanupEntities(template, baseTemplate);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {

            return template;
        }
    }
}

using System.Linq;
using System.Web.Configuration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectFiles : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template, TokenParser parser)
        {
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

                Microsoft.SharePoint.Client.File targetFile = null;

                if (file.Create)
                {

                    using (var stream = template.Connector.GetFileStream(file.Src))
                    {
                        targetFile = folder.UploadFile(file.Src, stream, file.Overwrite);
                    }
                }
                else
                {
                    // Get a reference to an existing file
                    targetFile = folder.GetFile(file.Src);
                }

                if (file.WebParts != null && file.WebParts.Any())
                {
                    if (!targetFile.IsPropertyAvailable("ServerRelativeUrl"))
                    {
                        web.Context.Load(targetFile, f => f.ServerRelativeUrl);
                        web.Context.ExecuteQuery();
                    }
                    foreach (var webpart in file.WebParts)
                    {
                        var wpEntity = new WebPartEntity();
                        wpEntity.WebPartTitle = webpart.Title;
                        wpEntity.WebPartXml = parser.Parse(webpart.Contents);
                        wpEntity.WebPartZone = webpart.Zone;
                        wpEntity.WebPartIndex = (int) webpart.Order;

                        web.AddWebPartToWebPartPage(targetFile.ServerRelativeUrl, wpEntity);
                    }
                }

            }

        }


        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            // Impossible to return all files in the site currently

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
        {

            return template;
        }
    }
}

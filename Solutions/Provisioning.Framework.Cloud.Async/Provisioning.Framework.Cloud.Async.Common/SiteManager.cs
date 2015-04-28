using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Framework.Cloud.Async.Common
{
    public class SiteManager
    {

        #region CONSTANTS

        public const string StorageQueueName = "pnpsiterequests";

        #endregion

        #region STORAGE QUEUE HANDLING

        /// <summary>
        /// Used to add new storage queue entry.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="siteUrl"></param>
        /// <param name="storageConnectionString"></param>
        public void AddConfigRequestToQueue(SiteCollectionRequest siteRequest, string storageConnectionString)
        {
            CloudStorageAccount storageAccount =
                                CloudStorageAccount.Parse(storageConnectionString);

            // Get queue... create if does not exist.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue =
                queueClient.GetQueueReference(SiteManager.StorageQueueName);
            queue.CreateIfNotExists();

            // Add entry to queue
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(siteRequest)));

        }

        #endregion

        #region SITE CREATION PROCESSING

        public string ProcessSiteCreationRequest(ClientContext ctx, SiteCollectionRequest siteRequest)
        {
            // Resolve full URL 
            var webFullUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", siteRequest.TenantName, siteRequest.ManagedPath, siteRequest.Url);

            // Resolve the actual SP template to use
            string siteTemplate = SolveActualTemplate(siteRequest);

            Tenant tenant = new Tenant(ctx);
            if (tenant.SiteExists(webFullUrl))
            {
                // Abort... can't proceed, URL taken.
                throw new InvalidDataException(string.Format("site already existed with same URL as {0}. Process aborted.", webFullUrl));
            }
            else
            {
                // Create new site collection with storage limits and settings from the form
                tenant.CreateSiteCollection(webFullUrl,
                                            siteRequest.Title,
                                            siteRequest.Owner,
                                            siteTemplate,
                                            (int)siteRequest.StorageMaximumLevel,
                                            (int)(siteRequest.StorageMaximumLevel * 0.75),
                                            siteRequest.TimeZoneId,
                                            0,
                                            0,
                                            siteRequest.Lcid);

                return webFullUrl;
            }
        }

        /// <summary>
        /// Applies actual template on top of given site URL. 
        /// </summary>
        /// <param name="webFullUrl"></param>
        /// <param name="siteRequest"></param>
        public void ApplyCustomTemplateToSite(ClientContext ctx, SiteCollectionRequest siteRequest, string resourcesPath)
        {
            // Template to be applied to site
            ProvisioningTemplate template = null;

            // Apply modification to provided site
            switch (siteRequest.ProvisioningType)
            {
                case SiteProvisioningType.Identity:

                    // Get template from xml file
                    XMLFileSystemTemplateProvider provider = new XMLFileSystemTemplateProvider(resourcesPath, "");
                    template = provider.GetTemplate(siteRequest.TemplateId);

                    break;
                case SiteProvisioningType.TemplateSite:

                    // Get template from existing site
                    using (ClientContext cc2 = ctx.Clone(siteRequest.TemplateId))
                    {

                        // Specify null as base template since we do want "everything" in this case
                        ProvisioningTemplateCreationInformation creationInfo = new ProvisioningTemplateCreationInformation(cc2.Web);
                        creationInfo.BaseTemplate = cc2.Web.GetBaseTemplate();
                        creationInfo.PersistComposedLookFiles = true;
                        creationInfo.FileConnector = new FileSystemConnector(resourcesPath, "");

                        // Get template from existing site
                        template = cc2.Web.GetProvisioningTemplate(creationInfo);
                    }
                    break;
                default:
                    break;
            }

            // Apply template to the site
            template.Connector = new FileSystemConnector(resourcesPath, "");
            ctx.Web.ApplyProvisioningTemplate(template);
        }

        /// <summary>
        /// Solves the used template based on the request object values
        /// </summary>
        /// <param name="siteRequest"></param>
        /// <returns></returns>
        private static string SolveActualTemplate(SiteCollectionRequest siteRequest)
        {
            // Currently we do not resolve the actual selection to root template, only team site supported by this solution.
            return "STS#0";
        }

        #endregion
    }
}

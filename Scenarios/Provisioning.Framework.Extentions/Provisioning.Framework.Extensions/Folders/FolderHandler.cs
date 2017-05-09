using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Microsoft.SharePoint.Client.Publishing.Navigation;
using Microsoft.SharePoint.Client.Taxonomy;
using SP = Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;

namespace Provisioning.Framework.Extensions
{
    class FolderHandler : IProvisioningExtensibilityProvider
    {
        public void ProcessRequest(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            var folders = !string.IsNullOrEmpty(configurationData) ? XmlHelper.ReadXmlString<FolderProvisionSchema.FolderList>(configurationData) : null;
            foreach (var folder in folders.Folder)
            {
                EnsureForlder(ctx, folder.List, folder.Path);
            }
        }

        private void EnsureForlder(ClientContext ctx, string listTitle, string path)
        {
            List list = ctx.Web.Lists.GetByTitle(listTitle);
            ctx.Load(list, l => l.RootFolder);
            ctx.Web.EnsureFolder(list.RootFolder, path);
        }
    }
}

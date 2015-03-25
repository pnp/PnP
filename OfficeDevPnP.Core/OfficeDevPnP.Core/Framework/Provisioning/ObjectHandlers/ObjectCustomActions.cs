using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.UPAWebService;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectCustomActions : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var context = web.Context as ClientContext;
            var site = context.Site;

            var webCustomActions = template.CustomActions.WebCustomActions;
            var siteCustomActions = template.CustomActions.SiteCustomActions;


            ProvisionCustomActionImplementation(web, webCustomActions);
            ProvisionCustomActionImplementation(site, siteCustomActions);


        }

        private void ProvisionCustomActionImplementation(object parent, List<CustomAction> customActions)
        {
            Web web = null;
            Site site = null;
            if (parent is Site)
            {
                site = parent as Site;
            }
            else
            {
                web = parent as Web;
            }
            foreach (var customAction in customActions)
            {
                var caExists = false;
                if (site != null)
                {
                    caExists = site.CustomActionExists(customAction.Name);
                }
                else
                {
                    caExists = web.CustomActionExists(customAction.Name);
                }
                if (!caExists)
                {
                    var customActionEntity = new CustomActionEntity();
                    customActionEntity.CommandUIExtension = customAction.CommandUIExtension;
                    customActionEntity.Description = customAction.Description;
                    customActionEntity.Group = customAction.Group;
                    customActionEntity.ImageUrl = customAction.ImageUrl;
                    customActionEntity.Location = customAction.Location;
                    customActionEntity.Name = customAction.Name;
                    customActionEntity.RegistrationId = customAction.RegistrationId;
                    customActionEntity.RegistrationType = customAction.RegistrationType;
                    customActionEntity.Remove = customAction.Remove;
                    customActionEntity.Rights = customAction.Rights;
                    customActionEntity.ScriptBlock = customAction.ScriptBlock;
                    customActionEntity.ScriptSrc = customAction.ScriptSrc;
                    customActionEntity.Sequence = customAction.Sequence;
                    customActionEntity.Title = customAction.Title;
                    customActionEntity.Url = customAction.Url;

                    if (site != null)
                    {
                        site.AddCustomAction(customActionEntity);
                    }
                    else
                    {
                        web.AddCustomAction(customActionEntity);
                    }
                }
            }

        }

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var context = web.Context as ClientContext;
            var webCustomActions = web.GetCustomActions();
            var siteCustomActions = context.Site.GetCustomActions();

            var customActions = new CustomActions();
            foreach (var customAction in webCustomActions)
            {
                customActions.WebCustomActions.Add(CopyUserCustomAction(customAction));
            }
            foreach (var customAction in siteCustomActions)
            {
                customActions.SiteCustomActions.Add(CopyUserCustomAction(customAction));
            }

            template.CustomActions = customActions;

            return template;
        }

        private CustomAction CopyUserCustomAction(UserCustomAction userCustomAction)
        {
            var customAction = new CustomAction();
            customAction.Description = userCustomAction.Description;
            customAction.Enabled = true;
            customAction.Group = userCustomAction.Group;
            customAction.ImageUrl = userCustomAction.ImageUrl;
            customAction.Location = userCustomAction.Location;
            customAction.Name = userCustomAction.Name;
            customAction.Rights = userCustomAction.Rights;
            customAction.ScriptBlock = userCustomAction.ScriptBlock;
            customAction.ScriptSrc = userCustomAction.ScriptSrc;
            customAction.Sequence = userCustomAction.Sequence;
            customAction.Title = userCustomAction.Title;
            customAction.Url = userCustomAction.Url;
            customAction.RegistrationId = userCustomAction.RegistrationId;
            customAction.RegistrationType = userCustomAction.RegistrationType;
            customAction.CommandUIExtension = userCustomAction.CommandUIExtension;

            return customAction;
        }
    }
}

using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectCustomActions : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Custom Actions"; }
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_CustomActions);

            var context = web.Context as ClientContext;
            var site = context.Site;

            // if this is a sub site then we're not enabling the site collection scoped custom actions
            if (!web.IsSubSite())
            {
                var siteCustomActions = template.CustomActions.SiteCustomActions;
                ProvisionCustomActionImplementation(site, siteCustomActions);
            }

            var webCustomActions = template.CustomActions.WebCustomActions;
            ProvisionCustomActionImplementation(web, webCustomActions);
        }

        private void ProvisionCustomActionImplementation(object parent, List<CustomAction> customActions)
        {
            Web web = null;
            Site site = null;
            if (parent is Site)
            {
                site = parent as Site;

                // Switch parser context;
                TokenParser.Rebase(site.RootWeb);
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
                    customActionEntity.CommandUIExtension = customAction.CommandUIExtension.ToParsedString();
                    
                    
                    customActionEntity.Description = customAction.Description;
                    customActionEntity.Group = customAction.Group;
                    customActionEntity.ImageUrl = customAction.ImageUrl.ToParsedString();
                    customActionEntity.Location = customAction.Location;
                    customActionEntity.Name = customAction.Name;
                    customActionEntity.RegistrationId = customAction.RegistrationId;
                    customActionEntity.RegistrationType = customAction.RegistrationType;
                    customActionEntity.Remove = customAction.Remove;
                    customActionEntity.Rights = customAction.Rights;
                    customActionEntity.ScriptBlock = customAction.ScriptBlock;
                    customActionEntity.ScriptSrc = customAction.ScriptSrc.ToParsedString();
                    customActionEntity.Sequence = customAction.Sequence;
                    customActionEntity.Title = customAction.Title;
                    customActionEntity.Url = customAction.Url.ToParsedString();

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

            // Rebase parser context to current web
            TokenParser.Rebase(web);
        }

        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            var context = web.Context as ClientContext;
            bool isSubSite = web.IsSubSite();
            var webCustomActions = web.GetCustomActions();
            var siteCustomActions = context.Site.GetCustomActions();

            var customActions = new CustomActions();
            foreach (var customAction in webCustomActions)
            {
                customActions.WebCustomActions.Add(CopyUserCustomAction(customAction));
            }
            
            // if this is a sub site then we're not creating entities for site collection scoped custom actions
            if (!isSubSite)
            {
                foreach (var customAction in siteCustomActions)
                {
                    customActions.SiteCustomActions.Add(CopyUserCustomAction(customAction));
                }
            }

            template.CustomActions = customActions;

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate, isSubSite);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate, bool isSubSite)
        {
            if (!isSubSite)
            {
                foreach (var customAction in baseTemplate.CustomActions.SiteCustomActions)
                {
                    int index = template.CustomActions.SiteCustomActions.FindIndex(f => f.Name.Equals(customAction.Name));

                    if (index > -1)
                    {
                        template.CustomActions.SiteCustomActions.RemoveAt(index);
                    }
                }
            }

            foreach (var customAction in baseTemplate.CustomActions.WebCustomActions)
            {
                int index = template.CustomActions.WebCustomActions.FindIndex(f => f.Name.Equals(customAction.Name));

                if (index > -1)
                {
                    template.CustomActions.WebCustomActions.RemoveAt(index);
                }
            }
            
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

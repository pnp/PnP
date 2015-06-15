using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectCustomActions : ObjectHandlerBase
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

            // Switch parser context back to it's original context
            TokenParser.Rebase(web);
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

                // Switch parser context
                TokenParser.Rebase(web);
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
                    var customActionEntity = new CustomActionEntity
                    {
                        CommandUIExtension = customAction.CommandUIExtension.ToParsedString(),
                        Description = customAction.Description,
                        Group = customAction.Group,
                        ImageUrl = customAction.ImageUrl.ToParsedString(),
                        Location = customAction.Location,
                        Name = customAction.Name,
                        RegistrationId = customAction.RegistrationId,
                        RegistrationType = customAction.RegistrationType,
                        Remove = customAction.Remove,
                        Rights = customAction.Rights,
                        ScriptBlock = customAction.ScriptBlock.ToParsedString(),
                        ScriptSrc = customAction.ScriptSrc.ToParsedString("~site","~sitecollection"),
                        Sequence = customAction.Sequence,
                        Title = customAction.Title,
                        Url = customAction.Url.ToParsedString()
                    };

                    if (site != null)
                    {
                        site.AddCustomAction(customActionEntity);
                    }
                    else
                    {
                        web.AddCustomAction(customActionEntity);
                    }
                }
                else
                {
                    UserCustomAction existingCustomAction = null;
                    if (site != null)
                    {
                        existingCustomAction = site.GetCustomActions().FirstOrDefault(c => c.Name == customAction.Name);
                    }
                    else
                    {
                        existingCustomAction = web.GetCustomActions().FirstOrDefault(c => c.Name == customAction.Name);
                    }
                    if (existingCustomAction != null)
                    {
                        var isDirty = false;
                        if (existingCustomAction.CommandUIExtension != customAction.CommandUIExtension.ToParsedString())
                        {
                            existingCustomAction.CommandUIExtension = customAction.CommandUIExtension.ToParsedString();
                            isDirty = true;
                        }
                        if (existingCustomAction.Description != customAction.Description)
                        {
                            existingCustomAction.Description = customAction.Description;
                            isDirty = true;
                        }
                        if (existingCustomAction.Group != customAction.Group)
                        {
                            existingCustomAction.Group = customAction.Group;
                            isDirty = true;
                        }
                        if (existingCustomAction.ImageUrl != customAction.ImageUrl.ToParsedString())
                        {
                            existingCustomAction.ImageUrl = customAction.ImageUrl.ToParsedString();
                            isDirty = true;
                        }
                        if (existingCustomAction.Location != customAction.Location)
                        {
                            existingCustomAction.Location = customAction.Location;
                            isDirty = true;
                        }
                        if (existingCustomAction.RegistrationId != customAction.RegistrationId)
                        {
                            existingCustomAction.RegistrationId = customAction.RegistrationId;
                            isDirty = true;
                        }
                        if (existingCustomAction.RegistrationType != customAction.RegistrationType)
                        {
                            existingCustomAction.RegistrationType = customAction.RegistrationType;
                            isDirty = true;
                        }
                        if (existingCustomAction.ScriptBlock != customAction.ScriptBlock.ToParsedString())
                        {
                            existingCustomAction.ScriptBlock = customAction.ScriptBlock.ToParsedString();
                            isDirty = true;
                        }
                        if (existingCustomAction.ScriptSrc != customAction.ScriptSrc.ToParsedString("~site","~sitecollection"))
                        {
                            existingCustomAction.ScriptSrc = customAction.ScriptSrc.ToParsedString();
                            isDirty = true;
                        }
                        if (existingCustomAction.Title != customAction.Title.ToParsedString())
                        {
                            existingCustomAction.Title = customAction.Title.ToParsedString();
                            isDirty = true;
                        }
                        if (existingCustomAction.Url != customAction.Url.ToParsedString())
                        {
                            existingCustomAction.Url = customAction.Url.ToParsedString();
                            isDirty = true;
                        }
                        if (isDirty)
                        {
                            existingCustomAction.Update();
                            existingCustomAction.Context.ExecuteQueryRetry();
                        }
                    }
                }
            }
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

        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.CustomActions.SiteCustomActions.Any() || template.CustomActions.WebCustomActions.Any();
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                var context = web.Context as ClientContext;
                var webCustomActions = web.GetCustomActions();
                var siteCustomActions = context.Site.GetCustomActions();

                _willExtract = webCustomActions.Any() || siteCustomActions.Any();
            }
            return _willExtract.Value;
        }
    }
}

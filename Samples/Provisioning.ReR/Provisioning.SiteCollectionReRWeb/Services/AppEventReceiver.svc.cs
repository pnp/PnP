using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Reflection;
using Contoso.Provisioning.SiteCollectionCreationWeb.Models;


namespace Contoso.Provisioning.SiteCollectionCreationWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {
        #region Private Instance Members
        const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" StaticName=""{1}"" DisplayName=""{2}"" ID=""{3}"" {4}/>";
        const string RECEIVER_NAME = "PP_SC_ITEMUPDATED";
  
        #endregion

        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:
                    this.HandleAppInstall(properties);
                    result.Status = SPRemoteEventServiceStatus.Continue;
                    break;
                case SPRemoteEventType.AppUninstalling:
                    this.HandleAppUnInstall(properties);
                    result.Status = SPRemoteEventServiceStatus.Continue;
                    break;
             }
            return result;
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            switch(properties.EventType)
            {
                case SPRemoteEventType.ItemUpdated:
                    HandleItemUpdated(properties);
                    break;
            }
        }

        private void HandleItemUpdated(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if(clientContext != null)
                {
                    List requestList = clientContext.Web.Lists.GetById(properties.ItemEventProperties.ListId);
                    ListItem item = requestList.GetItemById(properties.ItemEventProperties.ListItemId);
                    clientContext.Load(item);
                    clientContext.ExecuteQuery();

                    if (String.Compare(item[SiteRequestFields.State].ToString(), "Approved", true) == 0)
                    {
                        try
                        {
                            string site_title = item[SiteRequestFields.Title].ToString();
                            string site_description = item[SiteRequestFields.Description].ToString();
                            string site_template = item[SiteRequestFields.Template].ToString();
                            string site_url = item[SiteRequestFields.Url].ToString();
                            SharePointUser site_owner = LabHelper.BaseSetUser(clientContext, item, SiteRequestFields.Owner);
                            LabHelper.CreateSiteCollection(clientContext, site_url, site_template, site_title, site_description, site_owner.Email);
                            item[SiteRequestFields.State] = "COMPLETED";
                        }
                        catch(Exception ex)
                        {
                            item[SiteRequestFields.State] = "ERROR";
                            item[SiteRequestFields.StatusMessage] = ex.Message;
                        }
                        item.Update();
                        clientContext.ExecuteQuery();
                    }
                }
            }
        }
        private void HandleAppUnInstall(SPRemoteEventProperties properties)
        {
             using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
             {
                 this.CleanUp(clientContext);
             }
        }
        private void HandleAppInstall(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
            {
                this.CreateSiteRequestList(clientContext);
            }
        }
        private void CleanUp(ClientContext ctx)
        {
            List _requestList = ctx.Web.Lists.GetByTitle(Lists.SiteRepositoryTitle);
            ctx.Load(_requestList, p => p.EventReceivers);
            ctx.ExecuteQuery();

            var rer = _requestList.EventReceivers.Where(
                e => e.ReceiverName == RECEIVER_NAME).FirstOrDefault();

            try
            {
                System.Diagnostics.Trace.WriteLine("Removing ItemAdded receiver at " + rer.ReceiverUrl);

                //This will fail when deploying via F5, but works when deployed to production
                rer.DeleteObject();
                ctx.ExecuteQuery();
            }
            catch(Exception _ex)
            {
                System.Diagnostics.Trace.WriteLine(_ex.Message);
            }
            
        }
        private void CreateSiteRequestList(ClientContext ctx)
        {
            bool rerExists = false;
            var web = ctx.Web;
        
            ctx.Load(ctx.Web.Lists,
                    lists => lists.Include(
                    list => list.Title,
                     list => list.EventReceivers).Where
                (list => list.Title == Lists.SiteRepositoryTitle));
            ctx.ExecuteQuery();

            List requestList = ctx.Web.Lists.FirstOrDefault();
            if(requestList == null)
            { 
                var newList = new ListCreationInformation()
                {
                    Title = Lists.SiteRepositoryTitle,
                    Description = Lists.SiteRepositoryDesc,
                    TemplateType = (int)ListTemplateType.GenericList,
                    Url = Lists.SiteRepositoryUrl
                };
                requestList = ctx.Web.Lists.Add(newList);
                ctx.Load(requestList);
                ctx.ExecuteQuery();

                // add fields (replace the second field with the display name
                AddFieldAsXml(requestList, SiteRequestFields.Description, SiteRequestFields.DescriptionDisplayName, SiteRequestFields.DescriptionId, "Note", options: AddFieldOptions.AddFieldCheckDisplayName);
                AddFieldAsXml(requestList, SiteRequestFields.Template, SiteRequestFields.TemplateDisplayName, SiteRequestFields.TemplateId);
                AddFieldAsXml(requestList, SiteRequestFields.Policy, SiteRequestFields.PolicyDisplayName, SiteRequestFields.PolicyId);
                AddFieldAsXml(requestList, SiteRequestFields.Url, SiteRequestFields.UrlDisplayName, SiteRequestFields.UrlId);
                AddFieldAsXml(requestList, SiteRequestFields.Owner, SiteRequestFields.OwnerDisplayName, SiteRequestFields.OwnerId, "User", "List='UserInfo' UserSelectionMode='0' ShowField='ImnName'");
                AddFieldAsXml(requestList, SiteRequestFields.AdditionalOwners, SiteRequestFields.AdditionalOwnersDisplayName, SiteRequestFields.AdditionalOwnersId, "UserMulti", "Mult='TRUE' List='UserInfo' UserSelectionMode='1' ShowField='ImnName'");
                AddFieldAsXml(requestList, SiteRequestFields.Lcid, SiteRequestFields.LcidDisplayName, SiteRequestFields.LcidId);
                AddFieldAsXml(requestList, SiteRequestFields.StatusMessage, SiteRequestFields.StatusMessageDisplayName, SiteRequestFields.StatusMessageId, "Note", options: AddFieldOptions.AddFieldCheckDisplayName);
                AddFieldAsXml(requestList, SiteRequestFields.TimeZone, SiteRequestFields.TimeZoneDisplayName, SiteRequestFields.TimeZoneId);
                AddFieldAsXml(requestList, SiteRequestFields.State, SiteRequestFields.StateDisplayName, SiteRequestFields.StatusId);

                requestList.Update();
                ctx.ExecuteQuery();
            }
            else
            {
                foreach (var rer in requestList.EventReceivers)
                {
                    if (rer.ReceiverName == RECEIVER_NAME)
                    {
                        rerExists = true;
                        System.Diagnostics.Trace.WriteLine("Found existing ItemAdded receiver at " + rer.ReceiverUrl);
                    }
                }
            }
            if (!rerExists)
            {
                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.EventType = EventReceiverType.ItemUpdated;

                //Get WCF URL where this message was handled
                OperationContext op = OperationContext.Current;
                Message msg = op.RequestContext.RequestMessage;

                receiver.ReceiverUrl = msg.Headers.To.ToString();
                receiver.ReceiverName = RECEIVER_NAME;
                //Add the new event receiver to a list in the host web
                requestList.EventReceivers.Add(receiver);

                ctx.ExecuteQuery();
            }
        }

        //Helper Member to add fields
        Field AddFieldAsXml(List list, string fieldInternalName, string fieldDisplayName, Guid fieldId, string fieldType = "Text", string additionalAttributes = "", AddFieldOptions options = AddFieldOptions.AddFieldToDefaultView)
        {
            var fieldXml = string.Format(FIELD_XML_FORMAT, fieldType, fieldInternalName, fieldDisplayName, fieldId, additionalAttributes);
            var field = list.Fields.AddFieldAsXml(fieldXml, true, options | AddFieldOptions.AddFieldInternalNameHint);
            return field;
        }

    }
}

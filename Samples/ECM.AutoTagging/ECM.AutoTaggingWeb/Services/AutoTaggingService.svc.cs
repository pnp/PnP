using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Collections;
using System.Diagnostics;

namespace ECM.AutoTaggingWeb.Services
{
    public class AutoTaggingService : IRemoteEventService
    {
        /// <summary>
        /// Handles events that occur before an action occurs, such as when a user adds or deletes a list item.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <returns>Holds information returned from the remote event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult _result = new SPRemoteEventResult();

            try 
            { 
                switch(properties.EventType)
                {
                    case SPRemoteEventType.ItemAdded:
                        HandleAutoTaggingItemAdded(properties);
                        break;
                    case SPRemoteEventType.ItemAdding:
                        HandleAutoTaggingItemAdding(properties, _result);
                        break;
                }
                _result.Status = SPRemoteEventServiceStatus.Continue;
            }
            catch(Exception)
            {
                //You should log here.               
            }
            return _result;
        }

        /// <summary>
        /// Handles events that occur after an action occurs, such as after a user adds an item to a list or deletes an item from a list.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to Handle the ItemAdding Event
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="result"></param>
        public void HandleAutoTaggingItemAdding(SPRemoteEventProperties properties,SPRemoteEventResult result)
        {
            using (ClientContext ctx = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (ctx != null)
                {
                    var itemProperties = properties.ItemEventProperties;
                    var _userLoginName = properties.ItemEventProperties.UserLoginName;
                    var _afterProperites = itemProperties.AfterProperties;
                    if(!_afterProperites.ContainsKey(ScenarioHandler.FLD_CLASSIFICATION_INTERNAL_NAME))
                    {
                        string _classficationToSet = ProfileHelper.GetProfilePropertyFor(ctx, _userLoginName, Constants.UPA_CLASSIFICATION_PROPERTY);
                        if(!string.IsNullOrEmpty(_classficationToSet))
                        { 
                            var _formatTaxonomy = AutoTaggingHelper.GetTaxonomyFormat(ctx, _classficationToSet);
                            result.ChangedItemProperties.Add(ScenarioHandler.FLD_CLASSIFICATION_INTERNAL_NAME, _formatTaxonomy);
                        }
                    }
                }
            }
        }
   
        /// <summary>
        /// Used to handle the ItemAdded event.
        /// </summary>
        /// <param name="properties"></param>
        public void HandleAutoTaggingItemAdded(SPRemoteEventProperties properties)
        {
            using (ClientContext ctx = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (ctx != null)
                {
                    string _userLoginName = properties.ItemEventProperties.UserLoginName;
                    List _library = ctx.Web.Lists.GetById(properties.ItemEventProperties.ListId);
                    var _itemToUpdate = _library.GetItemById(properties.ItemEventProperties.ListItemId);
                    ctx.Load(_itemToUpdate);
                    ctx.ExecuteQuery();

                    Hashtable _model = new Hashtable();
                    string _classficationToSet = ProfileHelper.GetProfilePropertyFor(ctx, _userLoginName, Constants.UPA_CLASSIFICATION_PROPERTY);
                    if (!string.IsNullOrEmpty(_classficationToSet))
                    {
                        _model.Add(ScenarioHandler.FLD_CLASSIFICATION_INTERNAL_NAME, _classficationToSet);
                        AutoTaggingHelper.SetTaxonomyField(ctx, _itemToUpdate, _model);
                    }
                }
            }
        }
    }
}

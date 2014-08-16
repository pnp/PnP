# ECM.AutoTagging #

## Applies to ##

- Office 365 Multi-Tenant 
- Office 365 Dedicated
- SharePoint 2013 

### Version history ###

1.0  | August 10th 2014 | Initial release

## Authors ##
Frank Marasco (Microsoft) 

## Disclaimer ##

THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.


## Overview ##
This sample demonstrates how to auto tag a document that will populate meta-data at the time the document is upload to SharePoint. This solution will enable more refined search results and assist with classification of content.  A Remote Event Receiver (ReR) will be used to retrieve the properties from the users profile and update the meta-data of the document at the time of document upload. The user will will have option of override those auto populated properties.  

This samples demonstrates the following:

- Adding a Fields and Content Types to the host web
- Creation of Taxonomy Fields programmatically
- Creating Two Libraries with the associated content type that are deployed to the host web
- Register a ItemAdding & ItemAdded Remote Event Receiver in the host web
- Removal of the Remote Event Receiver
- Retrieval of User Profile Properties.
- Setting Taxonomy Fields programmatically 

## Permissions ##
AppOnly Permissions are used in this solution

- Tenant: 	FullControl 
- Taxonomy: Read
- Social: 	Read
 
 
## Adding Fields and Content Types ##
To create the fields and content types the below code leverages OfficeDevPnP.Core. See OfficeDevPnP.Core for implementation details. We chose to create the fields and content types programmatically, this gives you greating control of adding new fields as you desire, as well as gives you more control to implmenent localized verisions of your fields.
 		
	//Check the fields
	if (!ctx.Web.FieldExistsById(FLD_CLASSIFICATION_ID))
	{
    	ctx.Web.CreateTaxonomyField(FLD_CLASSIFICATION_ID,
                                FLD_CLASSIFICATION_INTERNAL_NAME,
                                FLD_CLASSIFICATION_DISPLAY_NAME,
                                FIELDS_GROUP_NAME,
                                TAXONOMY_GROUP,
                                TAXONOMY_TERMSET_CLASSIFICATION_NAME);
	}

	//check the content type
	if (!ctx.Web.ContentTypeExistsById(CONTOSODOCUMENT_CT_ID))
	{
	    ctx.Web.CreateContentType(CONTOSODOCUMENT_CT_NAME,
	                              CT_DESC, CONTOSODOCUMENT_CT_ID,
	                              CT_GROUP);
	}
	
	//associate fields to content types
	if (!ctx.Web.FieldExistsByNameInContentType(CONTOSODOCUMENT_CT_NAME, FLD_CLASSIFICATION_INTERNAL_NAME))
	{
	    ctx.Web.AddFieldToContentTypeById(CONTOSODOCUMENT_CT_ID,
	                                      FLD_CLASSIFICATION_ID.ToString(),
	                                      false);
	}


## Create Document Library ##
To create a document library we use the following code. We are again, leveraging core to provide this functionality. The following code will create the library, enable versioning and remove the default Document content type. 

	if (!ctx.Web.ListExists(library.Title))
	{
		ctx.Web.AddList(ListTemplateType.DocumentLibrary, library.Title, false);
		List _list = ctx.Web.GetListByTitle(library.Title);
		if (!string.IsNullOrEmpty(library.Description))
		{
		    _list.Description = library.Description;
		}
	
	if (library.VerisioningEnabled)
	{
	_list.EnableVersioning = true;
	}
	
	_list.ContentTypesEnabled = true;
	_list.RemoveContentTypeByName("Document");
	_list.Update();
	
	
	ctx.Web.AddContentTypeToListById(library.Title, associateContentTypeID, true);
	ctx.Web.Context.ExecuteQuery();
	
	}
	else
	{
	throw new Exception("A list, survey, discussion board, or document library with the specified title already exists in this Web site.  Please choose another title.");
	}

## Registering the Remote Event Receiver in the host web ##
We will register two remote event receivers, which are ItemAdding and ItemAdded to two separate libraries.


	public static void AddEventReceiver(ClientContext ctx, List list, EventReceiverDefinitionCreationInformation eventReceiverInfo)
    {
        if (!DoesEventReceiverExist(eventReceiverInfo.ReceiverName, ctx, list))
        {
            list.EventReceivers.Add(eventReceiverInfo);
            ctx.ExecuteQuery();
        }
    }

We want to make sure that when we add the event receiver that one doesn't already exist. The following code demonstrates how to check if the receiver exists by name.

    public static bool DoesEventReceiverExistByName(ClientContext ctx, List list, string eventReceiverName )
    {
        bool _doesExist = false;
        ctx.Load(list, lib => lib.EventReceivers);
        ctx.ExecuteQuery();

        var _rer = list.EventReceivers.Where(e => e.ReceiverName == eventReceiverName).FirstOrDefault();
        if (_rer != null) {
            _doesExist = true;
        }

        return _doesExist;
    }

The following code is used to help us create a new EventReceiverDefinitionCreationInformation object. Make sure you change your ReceiverUrl to match your environment.

    public static EventReceiverDefinitionCreationInformation CreateEventReciever(string receiverName, EventReceiverType type)
    {
        EventReceiverDefinitionCreationInformation _rer = new EventReceiverDefinitionCreationInformation();
        _rer.EventType = type;
        _rer.ReceiverName = receiverName;
        _rer.ReceiverClass = "ECM.AutoTaggingWeb.Services.AutoTaggingService";
        _rer.ReceiverUrl = "https://amsecm.azurewebsites.net/Services/AutoTaggingService.svc";
        _rer.Synchronization = EventReceiverSynchronization.Synchronous;
        return _rer;
    }

## Removing the Remote Event Receiver in the host web ##
The following code is use to remove the event receiver from the list.
  		
	public static void RemoveEventReceiver(ClientContext ctx, List list, string receiverName)
    {
        ctx.Load(list, lib => lib.EventReceivers);
        ctx.ExecuteQuery();

        var _rer = list.EventReceivers.Where(e => e.ReceiverName == receiverName).FirstOrDefault();
        if(_rer != null)
        {
            _rer.DeleteObject();
            ctx.ExecuteQuery();
        }
    }


## ItemAdding Remote Event Receiver ##
Implementation class for ItemAdding. The ItemAdding member uses the result ChangeItemProperties to update the taxonomy field. We need to check if the the document already contains the properties for the Taxonomy Field, in this scenario we don't want to update the field. When debugging the code you will notice that if the property is already supplied the format is **2;#MYTERNNAME|74972ac9-3183-4775-b232-cd6de3569c65** This is the WssID, the value of the term and the GUID.
   			
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

 To get the term value in this format, we are going to use the below code that queries the TaxonomyHiddenList. Warning, DO NOT modify this list, you can read. Remember, you can look but don't touch.

 		
		public static string GetTaxonomyFormat(ClientContext ctx, string term)
        { 
            if(string.IsNullOrEmpty(term))
            {
                throw new ArgumentException(string.Format(EXCEPTION_MSG_INVALID_ARG, "term"));
            }
            string _result = string.Empty;
            var _list = ctx.Web.Lists.GetByTitle(TAXONOMY_HIDDEN_LIST_NAME);
            CamlQuery _caml = new CamlQuery();

            _caml.ViewXml = string.Format(TAXONOMY_CAML_QRY, term);
            var _listItemCollection = _list.GetItems(_caml);

            ctx.Load(_listItemCollection,
                eachItem => eachItem.Include(
                    item => item,
                    item => item.Id,
                    item => item[TAXONOMY_FIELDS_IDFORTERM]));
            ctx.ExecuteQuery();

            if (_listItemCollection.Count > 0)
            {
                var _item = _listItemCollection.FirstOrDefault();
                var _wssId = _item.Id;
                var _termId = _item[TAXONOMY_FIELDS_IDFORTERM].ToString(); ;
                _result = string.Format(TAXONOMY_FORMATED_STRING, _wssId, term, _termId);
            }

            return _result;
        }


## ItemAdded Remote Event Receiver ##
Implementation class for ItemAdded. The ItemAdded event receiver is implemented Synchronously. This implementation queries the list and updates the Taxonomy field in the list.

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

The AutoTagginghelper.SetTaxonomyField implementation uses the following code. The code is more expensive since we have to query the FieldCollection, get the Taxonomy Field that you want to update and then finally search for the Term by name to retrieve the Term Id. 

 		
	public static void SetTaxonomyField(ClientContext ctx, ListItem listItem, Hashtable model)
    {
        FieldCollection _fields = listItem.ParentList.Fields;
        ctx.Load(_fields);
        ctx.ExecuteQuery();

        foreach(var _key in model.Keys)
        {
           var _termName = model[_key].ToString();
           TaxonomyField _field = ctx.CastTo<TaxonomyField>(_fields.GetByInternalNameOrTitle(_key.ToString()));
           ctx.Load(_field);
           ctx.ExecuteQuery();
           Guid _id = _field.TermSetId;
           string _termID = AutoTaggingHelper.GetTermIdByName(ctx, _termName, _id );
           var _termValue = new TaxonomyFieldValue()
           {
               Label = _termName,
               TermGuid = _termID,
               WssId = -1
           };

           _field.SetFieldValueByValue(listItem, _termValue);
           listItem.Update();
           ctx.ExecuteQuery();
        }
    }

## Recommendations ##
While your testing the two scenarios, you will noticed that the ItemAdding implementation is more responsive, this is due to how we are getting the term, the guid and its wssId, as well query int the list item. We have one call vs four in the ItemAdded implementation.  If your use case matches this scenario, then I would recommend that you use ItemAdding instead of ItemAdded (Synchronously).  You should also make sure you code is as efficient as possible. Another possible solution which is more efficient is to use ItemAdded asynchronously and queue the actions so that we are not blocking the user in the UI.


## Dependencies ##
- 	Microsoft.SharePoint.Client
-   Microsoft.SharePoint.Client.Runtime
-   Microsoft.SharePoint.Client.Taxonomy
-   Microsoft.SharePoint.Client.UserProfiles
-   [Setting up provider hosted app to Windows Azure for Office365 tenant](http://blogs.msdn.com/b/vesku/archive/2013/11/25/setting-up-provider-hosted-app-to-windows-azure-for-office365-tenant.aspx)




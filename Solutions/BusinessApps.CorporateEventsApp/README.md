# Corporate Events #

### Summary ###
Corporate Events add-in is reference composite solution demonstrating usage of Core component capabilities to provide centralized corporate evetn management system.

Solution is build using ASP.net MVC model.


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
BusinessApps.CorporateEvents | Suman Chakrabarti, Brian Michely, Frank Marasco

### Version history ###
Version  | Date | Comments
---------|------|---------
1.0  | September 19th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# SCENARIO 1 #
This application is a sample of how to approach a line-of-business application such as a corporate events add-in. This application creates a number of add-in parts for displaying 

## CONFIGURATION ##
The configuration of the application utilizes the __CorporateEvents.SharePointWeb.Models.DataInitializer__ class. The /Home page of the application will route the user to the /Home/Config page to begin initialization of the application. The initializer will create the following:

### Lists ###
- Corporate Events
- Event Registration
- Event Speakers
- Event Sessions

### Content Types ###
- Corporate Event
- Event Registration
- Event Speaker
- Event Session

### Sample Data ###
The initializer will create sample events in the Corporate Events list.

# PROGRAMMING WITH LOB OBJECTS #
The following describes how one develops an MVC application using LOB objects against the SharePoint data source.

## LOB ENTITIES ##
The following LoB entities are utilized in this sample:

- BaseListItem (all entities inherit from this class)
- Event
- Registration
- Session - not fully implemented
- Speaker - not fully implemented

### BaseListItem ###
The BaseListItem class is an abstract class that does all the hard work for LoB objects coming from SharePoint. Subclasses are implemented with the façade pattern meaning they decorates the abstract class with attributes (properties and methods). With the LoB objects above, each adds public properties which can be utilized by the MVC model.

#### Abstract Members ####
When implementing a subclass, there are a few members which need implemented from the BaseListItem class.

Member | Description
-------|------------
ContentTypeName | Property that gets the content type that is associated with the item. If null, the default library content type will be assigned to the item upon saving.
FieldInternalNames | Property containing a list of field names which can be cached to improve performance when used for checking field data prior to save.
ListTitle | Property that gets the title of the list (this is case sensitive) 
ReadProperties(ListItem) | Reads properties from the ListItem object using the BaseGet<T> methods and assigns them to properties on the subclass.
SetProperties(ListItem) | Set properties on the ListItem object using the BaseSet methods from the subclass.

#### Helper Methods ####
Blittable types are best for return parameters and there are separate methods for BaseGetEnum. Future methods will be defined as well.

Member | Description
-------|------------
BaseGet<T>(ListItem item, string internalName) | Gets the property defined by the internalName parameter from the ListItem and returns them of generic type T.
BaseSet(ListItem item, string internalName, object value) | Sets the property defined by the internalName parameter.

#### Saving Objects ####
Saving objects comes via the Save method. The process for saving an item is good for one item, but _not optimized for batch saves, yet._ Right now, the assumption is that the class will load the list, determine if the current item has a valid ID (greater than zero is valid) and if not creates a new list item. Properties are set on the list item via the SetProperties method and the item is updated, refreshed, and the properties on the subclass are set via the ReadProperties method. 

```C#
public void Save(Web web) {
    var context = web.Context;
    var list = web.GetListByTitle(ListTitle);
    if (!IsNew && Id > 0) {
        ListItem = list.GetItemById(Id);
    }
    else {
        var listItemCreationInfo = new ListItemCreationInformation();
        ListItem = list.AddItem(listItemCreationInfo);
    }

    // ensure that the fields have been loaded
    EnsureFieldsRetrieved(ListItem);

    // set the properties on the list item
    SetProperties(ListItem);
    BaseSet(ListItem, TITLE, Title);

    // use if you want to override the created/modified date
    //BaseSet(ListItem, CREATED, Created);
    //BaseSet(ListItem, MODIFIED, Modified);

    ListItem.Update();

    if (!string.IsNullOrEmpty(ContentTypeName)) {
        var contentType = list.GetContentTypeByName(ContentTypeName);
        if (contentType != null)
            BaseSet(ListItem, "ContentTypeId", contentType.Id.StringValue);
    }

    ListItem.Update();

    // Execute the batch
    context.ExecuteQuery();

    // reload the properties
    ListItem.RefreshLoad();
    UpdateBaseProperties(ListItem);
    ReadProperties(ListItem);
}
```

# WEB INTERFACE #
The user experience for the Corporate Events add-in uses both SharePoint to be the primary driving interface as well as the provider-hosted web interface. The web interface is MVC controlled using the OOB ASP.NET MVC interface with Bootstrap and jquery. 

## EVENTS HOME ##
![SharePoint - Events home](http://i.imgur.com/nwHBVn7.png)

## EVENT DETAILS ##
![Event Details](http://i.imgur.com/WAxDsCL.png)

## REGISTRATION ##
![Event Registration](http://i.imgur.com/dMAaZb6.png)

# DEPENDENCIES #


<img  src="https://telemetry.sharepointpnp.com/pnp/solutions/BusinessApps.CorporateEvents" />

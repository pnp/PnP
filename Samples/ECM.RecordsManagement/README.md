# In Place Records Management #

### Summary ###
This scenario shows how you can control the in place records management settings for a site and optionally list specific settings.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
ECM.RecordsManagement | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | August 5th 2015 | Nuget update
1.0  | September 2nd 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Scenario 1: Enable In Place Records Management + Site scoped settings #
This scenario will show you how to enable in place records management for a site collection. Once in place records management is enabled you can control the in place record management settings via this SharePoint add-in. The add-in UI is similar to the out of the box settings page for in place records management:

![App UI to enable settings](http://i.imgur.com/r34VIAj.png)

## Enable in place records management ##
If in place records management was not yet enabled the add-in will allow you to enable it. Enabling is done via an extension method from the OfficeDevPnP core library as shown in below code snippet:
Code snippet:
```C#
cc.Site.EnableSiteForInPlaceRecordsManagement();
```

## Setting the site specific in place records management settings ##
For all the site scoped in place records management settings there are extension methods that allow to set and get the setting. Below code sample shows how to use the setters:
```C#
// Set restrictions to default values after enablement (is also done at feature activation)
EcmSiteRecordRestrictions restrictions = EcmSiteRecordRestrictions.BlockDelete | EcmSiteRecordRestrictions.BlockEdit;
site.SetRecordRestrictions(restrictions);

// Set record declaration to default value
site.SetRecordDeclarationBy(EcmRecordDeclarationBy.AllListContributors);

// Set record undeclaration to default value
site.SetRecordUnDeclarationBy(EcmRecordDeclarationBy.OnlyAdmins);
```

## Code internals ##
As there's no CSOM API for records management the OfficeDevPnP core extension methods use web properties to define the site scoped list settings. The properties used are:
-  ecm_siterecorddeclarationdefault
-  ecm_siterecordrestrictions
-  ecm_siterecorddeclarationby
-  ecm_siterecordundeclarationby

Below method is used to set the restrictions for the defined records:

```C#
public static void SetRecordRestrictions(this Site site, EcmSiteRecordRestrictions restrictions)
{
    string restrictionsProperty = "";

    if (restrictions.Has(EcmSiteRecordRestrictions.None))
    {
        restrictionsProperty = EcmSiteRecordRestrictions.None.ToString();
    }
    else if (restrictions.Has(EcmSiteRecordRestrictions.BlockEdit))
    {
        // BlockEdit is always used in conjunction with BlockDelete
        restrictionsProperty = EcmSiteRecordRestrictions.BlockDelete.ToString() + ", " + EcmSiteRecordRestrictions.BlockEdit.ToString();
    }
    else if (restrictions.Has(EcmSiteRecordRestrictions.BlockDelete))
    {
        restrictionsProperty = EcmSiteRecordRestrictions.BlockDelete.ToString();
    }

    // Set property bag entry
    site.RootWeb.SetPropertyBagValue(ECM_SITE_RECORD_RESTRICTIONS, restrictionsProperty);
}
```

# Scenario 2: List scoped settings #
Once site scoped in place records management is active you do have the option to define list specific in place records management settings. This scenario shows you how to do this using an add-in UI that's similar to the out of the box SharePoint UI:

![App UI to enable settings](http://i.imgur.com/zLtHnAO.png)

## Settings list specific in place records management settings ##
Like for site scoped settings we've again implemented OfficeDevPnP core extension methods that will make this task very easy.

The code below shows enum EcmListManualRecordDeclaration is set based on the value entered in the UI and then used to set the manual record declaration for the list. For the auto declaration the same concept is used.

```C#
List ipr = cc.Web.GetListByTitle(IPR_LIBRARY);
EcmListManualRecordDeclaration listManual = (EcmListManualRecordDeclaration)Convert.ToInt32(rdListAvailability.SelectedValue);
ipr.SetListManualRecordDeclaration(listManual);
ipr.SetListAutoRecordDeclaration(chbAutoDeclare.Checked);
```

## Code internals ##
Just as for the site scoped settings there's no API for the list scoped in place records management settings. To implement these settings we've again used list properties (= properties on the rootfolder of the list) and event handlers. The used list properties are:
-  ecm_AllowManualDeclaration
-  ecm_IPRListUseListSpecific
-  ecm_AutoDeclareRecords

Below code snippet shows how to set the automatic record declaration for a list.

```C#
public static void SetListAutoRecordDeclaration(this List list, bool autoDeclareRecords)
{
    //Determine the SharePoint version based on the loaded CSOM library
    Assembly asm = Assembly.GetAssembly(typeof(Microsoft.SharePoint.Client.Site));
    int sharePointVersion = asm.GetName().Version.Major;

    if (autoDeclareRecords)
    {
        //Set the property that dictates custom list record settings to true
        list.SetPropertyBagValue(ECM_IPR_LIST_USE_LIST_SPECIFIC, true.ToString());
        //Prevent manual declaration
        list.SetPropertyBagValue(ECM_ALLOW_MANUAL_DECLARATION, false.ToString());

        //Hookup the needed event handlers
        list.Context.Load(list.EventReceivers);
        list.Context.ExecuteQuery();

        List<EventReceiverDefinition> currentEventReceivers = new List<EventReceiverDefinition>(list.EventReceivers.Count);
        currentEventReceivers.AddRange(list.EventReceivers);

        // Track changes to see if an list.Update is needed
        bool eventReceiverAdded = false;
        
        //ItemUpdating receiver
        EventReceiverDefinitionCreationInformation newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemUpdating, 1000, sharePointVersion);
        if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
        {
            list.EventReceivers.Add(newEventReceiver);
            eventReceiverAdded = true;
        }
        //ItemDeleting receiver
        newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemDeleting, 1000, sharePointVersion);
        if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
        {
            list.EventReceivers.Add(newEventReceiver);
            eventReceiverAdded = true;
        }
        //ItemFileMoving receiver
        newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemFileMoving, 1000, sharePointVersion);
        if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
        {
            list.EventReceivers.Add(newEventReceiver);
            eventReceiverAdded = true;
        }
        //ItemAdded receiver
        newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemAdded, 1005, sharePointVersion);
        if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
        {
            list.EventReceivers.Add(newEventReceiver);
            eventReceiverAdded = true;
        }
        //ItemUpdated receiver
        newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemUpdated, 1007, sharePointVersion);
        if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
        {
            list.EventReceivers.Add(newEventReceiver);
            eventReceiverAdded = true;
        }
        //ItemCheckedIn receiver
        newEventReceiver = CreateECMRecordEventReceiverDefinition(EventReceiverType.ItemCheckedIn, 1006, sharePointVersion);
        if (!ContainsECMRecordEventReceiver(newEventReceiver, currentEventReceivers))
        {
            list.EventReceivers.Add(newEventReceiver);
            eventReceiverAdded = true;
        }
                        
        if (eventReceiverAdded)
        {
            list.Update();
            list.Context.ExecuteQuery();
        }

        //Set the property that dictates the auto declaration
        list.SetPropertyBagValue(ECM_AUTO_DECLARE_RECORDS, autoDeclareRecords.ToString());
    }
    else
    {
        //Set the property that dictates the auto declaration
        list.SetPropertyBagValue(ECM_AUTO_DECLARE_RECORDS, autoDeclareRecords.ToString());
        //Note: existing list event handlers will just stay as they are, no need to remove them
    }
}
```


<img src="https://telemetry.sharepointpnp.com/pnp/samples/ECM.RecordsManagement" />
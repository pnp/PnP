# Ribbon Commands Sample #

### Summary ###
Demonstrates how to add/remove a ribbon to a SharePoint host web.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
Core.RibbonCommands | Suman Chakrabarti (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | 25-SEP-2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
The purpose of this scenario is to demonstrate how to add a ribbon command to the host web. Ribbon commands are housed in a Custom Action, so we leverage the Web.UserCustomActions property to add a new custom action, but we add XML for the CommandUIExtension.

_NOTE: It is easiest to build the CommandUIExtension by creating a SharePoint Elements.xml file and create a CustomAction element with ribbon XML to follow._

![Custom tab and buttons in ribbon](http://i.imgur.com/QLPFxHY.png)


## Adding the Ribbon ##
The following describes how the XML is loaded into the CommandUIExtension and a new custom action is created or updated.

_NOTE: During development, it is helpful to continually run this code to ensure that the ribbon looks the way you expect._ 

```C#
// get xml CommandUIExtension node from elements.xml file
... 

// see of the custom action already exists
var customAction = clientContext.Web.UserCustomActions
					.FirstOrDefault(uca => uca.Name == customActionName);

// if it does not exist, create it
if (customAction == null) {
    // create the ribbon
    customAction = clientContext.Web.UserCustomActions.Add();
    customAction.Name = customActionName;
}

// set custom action properties
customAction.Location = location;
customAction.CommandUIExtension = xmlContent; // CommandUIExtension node XML
customAction.RegistrationId = registrationId;
customAction.RegistrationType = registrationType;
customAction.Sequence = sequence;

customAction.Update();
clientContext.Load(customAction);
clientContext.ExecuteQuery();
```

# DEPENDENCIES #
-  Microsoft.SharePoint.Client.dll
-  Microsoft.SharePoint.Client.Runtime.dll

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.RibbonCommands" />
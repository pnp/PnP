# Provisioning.SiteModifier #

### Summary ###
This scenario demonstrates a pattern that allows users to add functionality to their sites without understanding the concept of 
features or any other technical information.

The idea behind the pattern is to provide users with an out of the box teamsite, and when in the future they need additional functionality
they can go to the 'Modify Site' menu and add additional features to the site. The add-in will then add the required functionality. 
That way sites can grow exactly to the level of functionality that the user requires without needed to use things like site definitions, 
web templates or even the SharePoint feature framework.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
A HTML5 compliant web browser
In order to test the add-in without deploying the SiteModifierWeb project to a publicly available URL, Azure ServiceBus with ACS authentication is required. See http://msdn.microsoft.com/en-us/library/office/dn275975(v=office.15).aspx for more information.


### Solution ###
Solution | Author(s)
---------|----------
Provisioning.SiteModifier | Erwin van Hunen (**Knowit Reaktor Stockholm AB**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | October 25th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Overview #
This scenario demonstrates how an add-in can be used to provide a dialog box in which users can make selections of artefacts to be added to the site the moment they need it.

This scenario demonstrates the following:

- How to add a custom action to the site settings menu in the host web (see Services\AppEventReceiver.svc)
- How to show a dialog in a custom action (see Services\AppEventReceiver.svc)
- How to hide a dialog that hosts a page from a remote add-in web (see Pages\Modify.aspx)
- How to create artefacts like lists and set the theme of a web (see Pages\Modify.aspx)

The add-in adds a menu option to the site settings pop up menu ("Modify Site"), which after clicking on it will show a dialogbox. In that dialogbox users can make selections to add certain artifacts to a site.
In this example 3 options are presented: a projects list (which is a generic/custom list), a contacts list, and the option to change the default theme.

## Permissions ##
Permissions used in this solution

- Web: 	FullControl  
 
## Use of OfficeDevPnP.Core ##
To set the theme and create lists the scenario leverages OfficeDevPnP.Core. See Pages\Modify.aspx.

## Custom Action added to host web ##
We add a custom action to the site settings pop up menu in the host web. The custom action calls the LaunchApp javascript function that is provided by the server on the host web.
Launching an add-in like this will show the page in a dialogbox and it will allow the dialog box to close itself if needed.

```C#
 UserCustomAction userCustomAction = web.UserCustomActions.Add();
 userCustomAction.Location = "Microsoft.SharePoint.StandardMenu";
 userCustomAction.Group = "SiteActions";
 BasePermissions perms = new BasePermissions();
 perms.Set(PermissionKind.ManageWeb);
 userCustomAction.Rights = perms;
 userCustomAction.Sequence = 100;
 userCustomAction.Title = "Modify Site";

 string realm = TokenHelper.GetRealmFromTargetUrl(new Uri(clientContext.Url));
 string issuerId = WebConfigurationManager.AppSettings.Get("ClientId");

 var modifyPageUrl = string.Format("https://{0}/Pages/Modify.aspx?{{StandardTokens}}", GetHostUrl());
 string url = "javascript:LaunchApp('{0}', 'i:0i.t|ms.sp.ext|{1}@{2}','{3}',{{width:300,height:200,title:'Modify Site'}});";
 url = string.Format(url, Guid.NewGuid().ToString(), issuerId, realm, modifyPageUrl);

 userCustomAction.Url = url;

 userCustomAction.Update();

 clientContext.ExecuteQuery();
```

## Closing the dialog box from backend code hosted in a remote site ##

In order to close a dialogbox from a page hosted outside of the SharePoint site, the following code is used:

```C#
ScriptManager.RegisterStartupScript(this, typeof(Page), "UpdateMsg", "window.parent.postMessage('CloseCustomActionDialogRefresh', '*');", true);
```

## Removing the add-in link from the recents menu ##

In order to only remove the add-in entry in the Recents menu, the following code is executed

```C#
 NavigationNodeCollection nodes = web.Navigation.QuickLaunch;
 clientContext.Load(nodes, n => n.IncludeWithDefaultProperties(c => c.Children));
 clientContext.ExecuteQuery();
 var recent = nodes.Where(x => x.Title == "Recent").FirstOrDefault();
 if (recent != null)
 {
     var appLink = recent.Children.Where(x => x.Title == "Site Modifier").FirstOrDefault();
     if (appLink != null) appLink.DeleteObject();
     clientContext.ExecuteQuery();
 }
```

## AppUninstalling event receiver ##
When the add-in is uninstalled we’re also removing the event receiver. In order to make this work during debugging you’ll need to ensure that you navigate to the “Apps in testing” library and use the remove option on the add-in. This remove will trigger the add-in uninstalling event with the proper permissions to remove the created remote event handler. If you just close the browser or uninstall the add-in from the “site contents” then either the event receiver never fires or the event receivers runs with unsufficient permissions to remove the list added event receiver. The reason for this behavior is differences in add-in deployment when the add-in gets side loaded which is what Visual Studio does when you press F5.

When a user uninstalls a deployed add-in this moves the add-in to the site's recycle bin and will NOT trigger the appuninstalling event handler. The add-in needs to be removed from all recycle bins in order to trigger the appuninstalled event.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.SiteModifier" />
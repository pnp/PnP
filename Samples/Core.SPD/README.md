# Disable Designer using CSOM #

### Summary ###
This sample shows how modify SharePoint Designer Settings using CSOM

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.SPD | Vesa Juvonen, Bert Jansen, Frank Marasco (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | July 26th, 2014 | Documentation
1.0  | March 5th, 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# INTRODUCTION #
Maybe you have been getting complaints lately about the ability to use SharePoint Designer (SPD) in an enterprise SharePoint environment or you governance policy prohibits the use of SPD in your SharePoint environment either on-premises or in the Cloud. This sample shows patterns on how to disable the SharePoint Designer Settings using CSOM You can then take this sample and implement a Timer Job like solution that would apply these settings. See Office AMS sample Core.SimpleTimerJob for additional information. 

# SCENARIO 1: CONSOLE APPLICATION #
This scenario is implemented as a console application that programmatically sets the SharePoint Designer settings using CSOM for your targeted site collection.

**NOTE**: Site Collection administrators, can still enable this setting. 

![Site Settings for enabling SharePoint Designer](http://i.imgur.com/in1qvog.png)

## DISABLE DESIGNER ##
The following code illustrates how to disable SharePoint Designer on the site:

```C#
/// <summary>
/// This method will disable SharePoint designer you must be a site collection administrator to perform this action
/// A UnauthorizedAccessException is thrown when attempting to set the property if either the user is not a Site Collection administrator or the setting is disabled at the 
/// web application level.
/// Site Collection Administrators will always be able to edit sites. 
/// </summary>
/// <param name="ctx"></param>
public static void DisableDesigner(ClientContext ctx)
{
try
       {
       	Site _site = ctx.Site;
              ctx.Load(_site);
              //Allow Site Owners and Designers to use SharePoint Designer in this Site Collection 
              _site.AllowDesigner = false;
              //Allow Site Owners and Designers to Customize Master Pages and Page Layouts 
              _site.AllowMasterPageEditing = false;
              //Allow Site Owners and Designers to Detach Pages from the Site Definition 
              _site.AllowRevertFromTemplate = false;
              //Allow Site Owners and Designers to See the Hidden URL structure of their Web Site 
              _site.ShowUrlStructure = false;
              ctx.ExecuteQuery();
            }
            catch 
            {
                throw;
            }
        }
}
```

## ENABLE DESIGNER ##
The following code illustrates how to enable SharePoint Designer on the site:

```C#
/// <summary>
/// This method will Enable SharePoint designer you must be a site collection administrator to perform this action
/// A UnauthorizedAccessException is thrown when attempting to set the property if either the user is not a Site Collection administrator or the setting is disabled at the 
/// web application level.
/// Site Collection Administrators will always be able to edit sites. 
/// </summary>
/// <param name="ctx"></param>
public static void EnableDesigner(ClientContext ctx)
{
try
   {
   	Site _site = ctx.Site;
  ctx.Load(_site);
  _site.AllowDesigner = true;
  _site.AllowMasterPageEditing = true;
  _site.AllowRevertFromTemplate = true;
  _site.ShowUrlStructure = true;
  ctx.ExecuteQuery();
   }
   catch
   {
   	throw;
   }
}
```

# SCENARIO 2: HIDE “DESIGNER SETTINGS” #
In the old days, we would use Full Trust code and the HideCustomAction element to remove the “SharePoint Designer Settings” on the setting page. In present times, how can you hide the element using the Add-In Model? We use the JavaScript injection pattern to hide the item. Just like using the HideCustomAction approach, a user with the appropriate permissions can still navigate to the page and change the settings.

**NOTE**: See AMS Sample, Core.JavaScriptInjection for additional information. 

Before | After
-------|------
![Before](http://i.imgur.com/8Cy2UWH.png) | ![After](http://i.imgur.com/4i2bBz6.png)

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SPD" />
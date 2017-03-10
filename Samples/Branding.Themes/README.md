# Setting a SharePoint Theme in an add-in for SharePoint #

### Summary ###
This scenario uses the Office PnP core library to set an existing theme to a site and for uploading and setting a custom theme. To learn more about the Office PnP core library please refer to its documentation.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Walkthrough Video ###

Visit the video on Channel 9  [http://channel9.msdn.com/Blogs/Office-365-Dev/Setting-a-SharePoint-Theme-in-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practices](http://channel9.msdn.com/Blogs/Office-365-Dev/Setting-a-SharePoint-Theme-in-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practices)

![http://channel9.msdn.com/Blogs/Office-365-Dev/Setting-a-SharePoint-Theme-in-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practices](http://i.imgur.com/6uloEpB.png)

### Applies to ###
- Office 365 Multi-Tenant (MT)
- Office 365 Dedicated (D)
- SharePoint 2013 on-premises


### Prerequisites ###
N/A 

### Solution ###
Solution | Author(s)
---------|----------
Branding.Themes | Vesa Juvonen (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.1  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.0  | May 11th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# APPLYING AN EXISTING THEME #

Applying an already existing theme is very simple using the Office AMS core extension methods. Below line of code show how you can call the SetThemeToWeb method and pass along the name of the theme to set:
```C#
web.SetThemeToWeb("Green");
```
# UPLOADING A CUSTOM THEME #
Before you can apply a custom theme you first need to upload that theme to the theme gallery in the root web of the site collection. Using the DeployThemeToWeb Office AMS Core method this is straightforward to do:
```C#
web.DeployThemeToWeb("SPC", 
                     HostingEnvironment.MapPath(string.Format("~/{0}","Resources/Themes/SPC/SPCTheme.spcolor")), 
                     null,
                     HostingEnvironment.MapPath(string.Format("~/{0}","Resources/Themes/SPC/SPCbg.png")),
                     string.Empty);
```

This method also supports a custom font file and master page linked to the theme, but in the above sample these are set.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Branding.Themes" />
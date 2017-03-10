# ALTERNATECSSURL & SITELOGOURL PROPERTIES IN WEB OBJECT #

### Summary ###
This scenario shows how to upload CSS and site image to the host web and how to set those to be used using CSOM. These are new properties released as part of the 2014 April CU for on-premises and they are also located in the cloud. MS online CSOM will be updated to expose these additional CSOM properties for the Web object during July 2014.
Notice that you can use 2014 April CU CSOM also with Office365 to set the properties accordingly to the host web.


### Walkthrough Video ###

Visit the video on Channel 9 [http://channel9.msdn.com/Blogs/Office-365-Dev/Alternate-CSS-and-set-site-logo-Office-365-Developer-Patterns-and-Practices](http://channel9.msdn.com/Blogs/Office-365-Dev/Alternate-CSS-and-set-site-logo-Office-365-Developer-Patterns-and-Practices)

![http://channel9.msdn.com/Blogs/Office-365-Dev/Alternate-CSS-and-set-site-logo-Office-365-Developer-Patterns-and-Practices](http://i.imgur.com/Sjxt6CX.png)

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
N/A

### Solution ###
Solution | Author(s)
---------|----------
Branding.AlternateCSSAndSiteLogo | Vesa Juvonen (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0  | June 30th 2014 | Initial release

### Disclaimer
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------


# SCENARIO: INJECT CUSTOM CSS FROM ADD-IN TO HOST WEB #
This scenario shows how to upload CSS and site image to the host web and how to set those to be used using CSOM. These are new properties released as part of the 2014 April CU for on-premises and they are also located in the cloud. MS online CSOM will be updated to expose these additional CSOM properties for the Web object during July 2014.
Notice that you can use 2014 April CU CSOM also with Office365 to set the properties accordingly to the host web.

![UI of the add-in](http://i.imgur.com/i1xq6Oq.png)

Once the modification has been injected, we can see the host web to have centralized alignment and we have relocated the page action bar using just CSS to different location. This demonstrates the capabilities on performing possible structural changes on the rendering without the need to use custom master page.
Custom master pages should be avoided to ensure that any updates or enhancements added to the out of the box master pages are automatically in use of the sites. BY combining this alternate CSS property with theming engine, you will have much more future friendlier approach on the customizations.

![Updated UI in host web](http://i.imgur.com/Hn8acco.png)

## UPLOADING ASSETS TO THE HOST WEB ##
Actual CSS and image files are uploaded using FileCreationInformation objet. In this case we are adding them to the Site Assets library, but they could be uploaded to any location in the host web or we could be referencing them also using absolute URLs.

```C#
// Instance to site assets
List assetLibrary = web.Lists.GetByTitle("Site Assets");
web.Context.Load(assetLibrary, l => l.RootFolder);

// Get the path to the file which we are about to deploy
string cssFile = System.Web.Hosting.HostingEnvironment.MapPath(
string.Format("~/{0}", "resources/contoso.css"));

// Use CSOM to uplaod the file in
FileCreationInformation newFile = new FileCreationInformation();
newFile.Content = System.IO.File.ReadAllBytes(cssFile);
newFile.Url = "contoso.css";
newFile.Overwrite = true;
Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
web.Context.Load(uploadFile);
web.Context.ExecuteQuery();

// Get the path to the file which we are about to deploy
string logoFile = System.Web.Hosting.HostingEnvironment.MapPath(
string.Format("~/{0}", "resources/99x.png"));

// Use CSOM to uplaod the file in
newFile = new FileCreationInformation();
newFile.Content = System.IO.File.ReadAllBytes(logoFile);
newFile.Url = "99x.png";
newFile.Overwrite = true;
uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
web.Context.Load(uploadFile);
web.Context.ExecuteQuery();
```
## CONTROLLING THE PROPERTIES OF THE HOST WEB ##
Adding the properties is pretty easy and straight forward. Both properties also support absolute addresses.

```C#
// Set the properties accordingly
// Notice that these are new properties in 2014 April CU of 15 hive CSOM and July release of MSO CSOM
web.AlternateCssUrl = web.ServerRelativeUrl + "/SiteAssets/contoso.css";
web.SiteLogoUrl = web.ServerRelativeUrl + "/SiteAssets/99x.png";
web.Update();
web.Context.ExecuteQuery();
```
    
## REMOVING THE CUSTOMIZATIONS FROM HOST WEB ##
Clearing the customizations is as easy as setting the properties to empty strings.
    
```C#
Web web = clientContext.Web;
// Clear the properties accordingly
// Notice that these are new properties in 2014 April CU of 15 hive CSOM and July release of MSO CSOM
web.AlternateCssUrl = "";
web.SiteLogoUrl = "";
web.Update();
web.Context.ExecuteQuery();
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Branding.AlternateCSSAndSiteLogo" />
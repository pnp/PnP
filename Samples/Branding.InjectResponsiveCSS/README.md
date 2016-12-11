# Making out of the box Seattle master responsive #

### Summary ###
Demonstrate how to update out of the box seattle.master user expirience responsive without a need to modify the master page as such, but rather to take advantage of the AlternateCssUrl property in Web level. Responsive CSS design has been done by Heather Solomon and you can read more information about the CSS details from following blog post.

* Heather Solomon (SharePoint Experts, Inc) - [Making Seattle master responsive](http://blog.sharepointexperience.com/2015/03/making-seattle-master-responsive/)

To make sure the css is rendered correctly on hardware devices a viewport html meta tag needs to be added to the master page. This can be accomplied by using the Search Engine Optimization Settings. Again the master page doesn't need to be edited. More details on this are covered in the following blog post.

* Stefan Bauer (n8d) - [How to add viewport meta without editing the master page](http://www.n8d.at/blog/how-to-add-viewport-meta-without-editing-the-master-page/)

*Notice* - There's more fine tuned and polished responsive UI CSS included in the PnP Partner Pack, which can be accessed from http://aka.ms/OfficeDevPnPPartnerPack. 

### Applies to ###
-  Office 365 Multi Tenant (MT) - Classic sites
-  Office 365 Dedicated (D)*
-  SharePoint 2013 on-premises*
-  SharePoint 2016 on-premises*

Experience might be slightly different, but the same thinking and process applies to on-premises as well.

### Solution ###
Solution | Author(s)
---------|----------
Branding.InjectResponsiveCSS | Heather Solomon (**SharePoint Experts, Inc**) 

Packaging and remote provisioning with AlternateCSSUrl approach done by Vesa Juvonen, Microsoft
Provisioning of viewport meta tag settings done by Stefan Bauer, n8d

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.3  | November 30th 2016 | Updated to use PnP CSOM Core to be consistent with other responsive samples from PnP
1.2  | November 19th 2015 | Small polishing on the code and adjustments in the documentation
1.1  | May 2nd 2015 | Viewport meta tag added
1.0  | April 26th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# November 2016 Update details #
Below screenshots and the process applies on classic approach. If you use the PnP CSOM Core component available as NuGet package for SharePoint Online, SharePoint 2016 and SharePoint 2013, you can simply run following command in the code. This will apply needed CSS and JS on the site.

```C#
// Get access to source site
using (var ctx = new ClientContext(siteUrl))
{
    ctx.AuthenticationMode = ClientAuthenticationMode.Default;
    ctx.Credentials = new SharePointOnlineCredentials(userName, pwd);

    // Enable Responsive UI in the site - Will apply needed CSS and JS to site
    ctx.Web.EnableResponsiveUI();

    // Disable Responsive UI in the site - Will remove CSS and JS from site. 
    // ctx.Web.DisableReponsiveUI();
}
```

# Responsive Experience #
Please check Heather's blog post for detailed information on the CSS design at [http://blog.sharepointexperience.com/2015/03/making-seattle-master-responsive](http://blog.sharepointexperience.com/2015/03/making-seattle-master-responsive/)

Here's three pictures which are showing how the responsive CSS will behave when screen size is adjusted.

![Normal sized team site](http://i.imgur.com/I0PR6Qj.png)

Notice how the left navigation is removed the search box has been relocated. 

![Tablet sized team site](http://i.imgur.com/iyAHWFh.png)

Notice how the top navigation is rendered completely differently

![Mobile sized team site](http://i.imgur.com/u9yYn8V.png)

If navigation control is clicked, user is presented the same list of navigation options

![Menu shown after click](http://i.imgur.com/BRtYm79.png)

# Attaching custom css to site #
Attaching the css to the site is two step process. We upload the CSS to some location where it can be used from and then we update the AlternateCssUrl property of the web to the right URL. 

Here's the code for uploading css to the site assets library using *FileCreationInformation* object.

```C#
/// <summary>
/// Uploads used CSS and site logo to host web
/// </summary>
/// <param name="web"></param>
    private static void UploadAssetsToHostWeb(Web web)
    {
        // Ensure site asset library exists in case of a publishing web site
        web.Lists.EnsureSiteAssetsLibrary();

        // Instance to site assets
        List assetLibrary = web.Lists.GetByTitle("Site Assets");
        web.Context.Load(assetLibrary, l => l.RootFolder);

        // Get the path to the file which we are about to deploy
        string cssFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/spe-seattle-responsive.css");

        // Use CSOM to upload the file in
        FileCreationInformation newFile = new FileCreationInformation
        {
            Content = System.IO.File.ReadAllBytes(cssFile),
            Url = "spe-seattle-responsive.css",
            Overwrite = true
        };
        Microsoft.SharePoint.Client.File uploadFile = assetLibrary.RootFolder.Files.Add(newFile);
        web.Context.Load(uploadFile);
        web.Context.ExecuteQuery();
    }

```

After the CSS is available, we can just set the AlternateCssUrl property, so that it's taken into use automatically by the site when browsers are accessing it. 

```C#
web.AlternateCssUrl = ctx.Web.ServerRelativeUrl + "/SiteAssets/spe-seattle-responsive.css";
web.Update();
web.Context.ExecuteQuery();
```

After setting the CSS Url, the viewport meta tag will be added to the "Search Engine Optimization Settings". This makes sure that the CSS values will be shown correctly on any hardware device.

```C#
    if (allProperties.FieldValues.ContainsKey("seoincludecustommetatagpropertyname")) {
        allProperties["seoincludecustommetatagpropertyname"] = true.ToString();
    }
    // Add value of custom meta tag
    if (allProperties.FieldValues.ContainsKey("seocustommetatagpropertyname"))
    {
        allProperties["seocustommetatagpropertyname"] = "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1\" />";
    }
```


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Branding.InjectResponsiveCSS" />
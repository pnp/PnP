# Making out of the box Seattle master responsive #

### Summary ###
Demonstrate how to update out of the box seattle.master user expirience responsive without a need to modify the mater page as such, but rather to take advantage of the AlternateCssUrl property in Web level. Responsive CSS design has been done by Heather Solomon and you can read more information about the CSS details from following blog post.

* Heather Solomon - [Making Seattle master responsive](http://blog.sharepointexperience.com/2015/03/making-seattle-master-responsive/)

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)*
-  SharePoint 2013 on-premises*

Experience might be slightly different, but the same thinking and process applies to on-premises as well.

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
Branding.InjectResponsiveCSS | Heather Solomon (CSS and responsive design), Vesa Juvonen (Only the provisioning part)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 26th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Responsive Experience #
Please check Heather's blog post for detailed information on the CSS design at [http://blog.sharepointexperience.com/2015/03/making-seattle-master-responsive](http://blog.sharepointexperience.com/2015/03/making-seattle-master-responsive/)

Here's three pictures which are showing how the responsive CSS will behave when screen size is adjusted.

![](http://i.imgur.com/I0PR6Qj.png)

Notice how the left navigation is removed the search box has been relocated. 

![](http://i.imgur.com/iyAHWFh.png)

Notice how the top navigation is rendered completely differently

![](http://i.imgur.com/u9yYn8V.png)

If navigation control is clicked, user is presented the same list of navigation options

![](http://i.imgur.com/BRtYm79.png)


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
    // Instance to site assets
    List assetLibrary = web.Lists.GetByTitle("Site Assets");
    web.Context.Load(assetLibrary, l => l.RootFolder);

    // Get the path to the file which we are about to deploy
    string cssFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/spe-seattle-responsive.css");

    // Use CSOM to upload the file in
    FileCreationInformation newFile = new FileCreationInformation();
    newFile.Content = System.IO.File.ReadAllBytes(cssFile);
    newFile.Url = "spe-seattle-responsive.css";
    newFile.Overwrite = true;
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
# Branding OneDrive for Business with an add-in for SharePoint #

> Note. This only works with Classic mode used in OneDrive for Business sites. 

### Summary ###
This scenario shows the pattern on how to access end user’s own OneDrive for Business and to apply custom branding to it automatically. Getting access to the personal my site will happen using Social CSOM which provides read access to user profile properties and also access to the Site object of the personal OneDrive for Business.
Actual branding is applied by uploading custom theme to the Site by using file upload mechanisms in the client side OM and then applying theme to the site. Notice that themes are not visible in the UI of the personal OneDrive for Business sites, but you can still use them.

In general it’s recommended to perform my site branding using themes and to avoid custom master page usage. If you’d start using custom master pages, you would have to ensure that any new changes on the oob master pages are reflected on custom master pages as well. On top of the themes, you can also inject custom CSS to the site to modify layout slightly without the need of changing actual master page. These would be preferred options with the branding.

Actual branding is applied from add-in part, which can be placed anywhere in the tenant, since it operates cross the site collections as needed. End user will only see gif animation indicating operations when add-in part is accessing the personal OneDrive for Business site. Typical locations for this customizer would be following locations.
- Intranet front page – When users arrive to Intranet, branding in OneDrive for Business is checked and applied if needed
- Public side of the my site – for example on the news feed page
Code also stores the version of the used branding, so that changes are only applied as needed, which will avoid performance issues with constantly deploying files without clear advantages.


### Walkthrough Video ###

Visit the video on Channel 9 - [http://channel9.msdn.com/Blogs/Office-365-Dev/Branding-OneDrive-for-Business-with-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practice](http://channel9.msdn.com/Blogs/Office-365-Dev/Branding-OneDrive-for-Business-with-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practice)

![Channel 9 video landing page](http://i.imgur.com/kWyLYet.png)


### Applies to ###
- Office 365 Dedicated (D)
- SharePoint 2013 on-premises

### Prerequisites ###
N/A

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.OneDrive | Vesa Juvonen (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0  | May 1st 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# SCENARIO: MODIFY PERSONAL ONEDRIVE SITE #
This scenario shows how to access user profile, create personal OneDrive for Business site and accessing created site for modification purposes

## ACCESSING USER PROFILE ##
Using social CSOM API you can easily access the user profile of particular user.

```C#
// Get user profile
ProfileLoader loader = Microsoft.SharePoint.Client.UserProfiles.ProfileLoader.GetProfileLoader(clientContext);
UserProfile profile = loader.GetUserProfile();
```

User profile then gives access to additional options, like accessing stored information in the user profile properties or to get access to the personal OneDrive for Business site, like in this case.

## STARTING ONEDRIVE FOR BUSINESS PROVISIONING IF IT DOESN’T EXIST ##
In this scenario we also schedule the OneDrive for Business creation if it has not yet been created for the particular user. This is done by using following lines of code.

```C#
Microsoft.SharePoint.Client.Site personalSite = profile.PersonalSite;

clientContext.Load(personalSite);
clientContext.ExecuteQuery();

// Let's check if the site already exists
if (personalSite.ServerObjectIsNull.Value)
{
    // Let's queue the personal site creation using oob timer job based approach
    // Using async mode, since end user could go away from browser, you could do this using oob web part as well
    profile.CreatePersonalSiteEnque(true);
    clientContext.ExecuteQuery();
    WriteDebugInformationIfNeeded("My site was not present, will be provisioned.");
}
```

This will schedule creation of the personal OneDrive for Business site using timer job based approach, like with the oob behavior.

## ACCESSING ONEDRIVE FOR BUSINESS SITE CROSS SITE COLLECTIONS ##
If personal OneDrive for Business site has been already created, we can access that simply using standard CSOM after getting instance to the site collection object from the user profile.

```C#
Microsoft.SharePoint.Client.Site personalSite = profile.PersonalSite;

clientContext.Load(personalSite);
clientContext.ExecuteQuery();

Web rootWeb = personalSite.RootWeb;
clientContext.Load(rootWeb);
clientContext.ExecuteQuery();
```

# SCENARIO: DEPLOY THEME AND APPLY THAT TO SITE #
This scenario shows how to deploy and apply custom theme to site.

## DEPLOYING THEME ##
Deploying of the theme can be achieve just by deploying theme files to right locations in the site using UploadThemeFile extension method. In this example case, we deploy three different files, which are then applied as “theme” to the site. You could actually deploy these files anywhere in the site, but for consistency sake, they are added to the same location as the oob files using following pattern.
Individual files are handled one-by-one by calling same extension method.

```C#
// Deploy files one by one to proper location
var colorFile = rootWeb.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/SPC/SPCTheme.spcolor")));
var backgroundFile = rootWeb.UploadThemeFile(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/Themes/SPC/SPCbg.jpg")));
```

Actual deployment is done in the method as follows.
```C#
rootWeb.CreateComposedLookByUrl("SPC", colorFile.ServerRelativeUrl, null, backgroundFile.ServerRelativeUrl, string.Empty);
```

The code for CreateComposedLookByUrl is the following:

```C#
public static void CreateComposedLookByUrl(this Web web, string lookName, string paletteServerRelativeUrl, string fontServerRelativeUrl, string backgroundServerRelativeUrl, string masterServerRelativeUrl, int displayOrder = 1, bool replaceContent = true)
{
    Utility.EnsureWeb(web.Context, web, "ServerRelativeUrl");
    var composedLooksList = web.GetCatalog((int)ListTemplateType.DesignCatalog);

    // Check for existing, by name
    CamlQuery query = new CamlQuery();
    query.ViewXml = string.Format(CAML_QUERY_FIND_BY_FILENAME, lookName);
    var existingCollection = composedLooksList.GetItems(query);
    web.Context.Load(existingCollection);
    web.Context.ExecuteQueryRetry();
    ListItem item = existingCollection.FirstOrDefault();

    if (item == null)
    {
        Log.Info(Constants.LOGGING_SOURCE, CoreResources.BrandingExtension_CreateComposedLook, lookName, web.ServerRelativeUrl);
        ListItemCreationInformation itemInfo = new ListItemCreationInformation();
        item = composedLooksList.AddItem(itemInfo);
        item["Name"] = lookName;
        item["Title"] = lookName;
    }
    else
    {
        if (!replaceContent)
        {
            throw new Exception("Composed look already exists, replace contents needs to be specified.");
        }
        Log.Info(Constants.LOGGING_SOURCE, CoreResources.BrandingExtension_UpdateComposedLook, lookName, web.ServerRelativeUrl);
    }

    if (!string.IsNullOrEmpty(paletteServerRelativeUrl))
    {
        item["ThemeUrl"] = paletteServerRelativeUrl;
    }
    if (!string.IsNullOrEmpty(fontServerRelativeUrl))
    {
        item["FontSchemeUrl"] = fontServerRelativeUrl;
    }
    if (!string.IsNullOrEmpty(backgroundServerRelativeUrl))
    {
        item["ImageUrl"] = backgroundServerRelativeUrl;
    }
    // we use seattle master if anything else is not set
    if (string.IsNullOrEmpty(masterServerRelativeUrl))
    {
        item["MasterPageUrl"] = UrlUtility.Combine(web.ServerRelativeUrl, Constants.MASTERPAGE_SEATTLE);
    }
    else
    {
        item["MasterPageUrl"] = masterServerRelativeUrl;
    }

    item["DisplayOrder"] = displayOrder;
    item.Update();
    web.Context.ExecuteQueryRetry();
}
```

Code adds also new composed look option to the composed looks list, which would not actually be needed and it not visible in the my sites, but you can use the same pattern when deploying theme to team sites and this would result new option in composed look selection.

```C#
public static void CreateComposedLookByUrl(this Web web, string lookName, string paletteServerRelativeUrl, string fontServerRelativeUrl, string backgroundServerRelativeUrl, string masterServerRelativeUrl, int displayOrder = 1, bool replaceContent = true)
{
    Utility.EnsureWeb(web.Context, web, "ServerRelativeUrl");
    var composedLooksList = web.GetCatalog((int)ListTemplateType.DesignCatalog);

    // Check for existing, by name
    CamlQuery query = new CamlQuery();
    query.ViewXml = string.Format(CAML_QUERY_FIND_BY_FILENAME, lookName);
    var existingCollection = composedLooksList.GetItems(query);
    web.Context.Load(existingCollection);
    web.Context.ExecuteQueryRetry();
    ListItem item = existingCollection.FirstOrDefault();

    if (item == null)
    {
        Log.Info(Constants.LOGGING_SOURCE, CoreResources.BrandingExtension_CreateComposedLook, lookName, web.ServerRelativeUrl);
        ListItemCreationInformation itemInfo = new ListItemCreationInformation();
        item = composedLooksList.AddItem(itemInfo);
        item["Name"] = lookName;
        item["Title"] = lookName;
    }
    else
    {
        if (!replaceContent)
        {
            throw new Exception("Composed look already exists, replace contents needs to be specified.");
        }
        Log.Info(Constants.LOGGING_SOURCE, CoreResources.BrandingExtension_UpdateComposedLook, lookName, web.ServerRelativeUrl);
    }

    if (!string.IsNullOrEmpty(paletteServerRelativeUrl))
    {
        item["ThemeUrl"] = paletteServerRelativeUrl;
    }
    if (!string.IsNullOrEmpty(fontServerRelativeUrl))
    {
        item["FontSchemeUrl"] = fontServerRelativeUrl;
    }
    if (!string.IsNullOrEmpty(backgroundServerRelativeUrl))
    {
        item["ImageUrl"] = backgroundServerRelativeUrl;
    }
    // we use seattle master if anything else is not set
    if (string.IsNullOrEmpty(masterServerRelativeUrl))
    {
        item["MasterPageUrl"] = UrlUtility.Combine(web.ServerRelativeUrl, Constants.MASTERPAGE_SEATTLE);
    }
    else
    {
        item["MasterPageUrl"] = masterServerRelativeUrl;
    }

    item["DisplayOrder"] = displayOrder;
    item.Update();
    web.Context.ExecuteQueryRetry();
}
```

##  APPLYING THEME ##
Actual applying of the theme is done with the SetComposedLookByUrl PnP Core extension method:
```C#
// Setting the Contoos theme to host web
rootWeb.SetComposedLookByUrl("SPC");
```

The code for SetComposedLookByUrl is the following:
```C#
 public static void SetComposedLookByUrl(this Web web, string lookName, string paletteServerRelativeUrl = null, string fontServerRelativeUrl = null, string backgroundServerRelativeUrl = null, string masterServerRelativeUrl = null, bool resetSubsitesToInherit = false, bool updateRootOnly = true)
 {
     var paletteUrl = default(string);
     var fontUrl = default(string);
     var backgroundUrl = default(string);
     var masterUrl = default(string);

     if (!string.IsNullOrWhiteSpace(lookName))
     {
         var composedLooksList = web.GetCatalog((int)ListTemplateType.DesignCatalog);

         // Check for existing, by name
         CamlQuery query = new CamlQuery();
         query.ViewXml = string.Format(CAML_QUERY_FIND_BY_FILENAME, lookName);
         var existingCollection = composedLooksList.GetItems(query);
         web.Context.Load(existingCollection);
         web.Context.ExecuteQueryRetry();
         var item = existingCollection.FirstOrDefault();

         if (item != null)
         {
             var lookPaletteUrl = item["ThemeUrl"] as FieldUrlValue;
             if (lookPaletteUrl != null)
             {
                 paletteUrl = new Uri(lookPaletteUrl.Url).AbsolutePath;
             }
             var lookFontUrl = item["FontSchemeUrl"] as FieldUrlValue;
             if (lookFontUrl != null)
             {
                 fontUrl = new Uri(lookFontUrl.Url).AbsolutePath;
             }
             var lookBackgroundUrl = item["ImageUrl"] as FieldUrlValue;
             if (lookBackgroundUrl != null)
             {
                 backgroundUrl = new Uri(lookBackgroundUrl.Url).AbsolutePath;
             }
             var lookMasterUrl = item["MasterPageUrl"] as FieldUrlValue;
             if (lookMasterUrl != null)
             {
                 masterUrl = new Uri(lookMasterUrl.Url).AbsolutePath;
             }
         }
         else
         {
             Log.Error(Constants.LOGGING_SOURCE, CoreResources.BrandingExtension_ComposedLookMissing, lookName);
             throw new Exception(string.Format("Composed look '{0}' can not be found; pass null or empty to set look directly (not based on an existing entry)", lookName));
         }
     }

     if (!string.IsNullOrEmpty(paletteServerRelativeUrl))
     {
         paletteUrl = paletteServerRelativeUrl;
     }
     if (!string.IsNullOrEmpty(fontServerRelativeUrl))
     {
         fontUrl = fontServerRelativeUrl;
     }
     if (!string.IsNullOrEmpty(backgroundServerRelativeUrl))
     {
         backgroundUrl = backgroundServerRelativeUrl;
     }
     if (!string.IsNullOrEmpty(masterServerRelativeUrl))
     {
         masterUrl = masterServerRelativeUrl;
     }

     web.SetMasterPageByUrl(masterUrl, resetSubsitesToInherit, updateRootOnly);
     web.SetCustomMasterPageByUrl(masterUrl, resetSubsitesToInherit, updateRootOnly);
     web.SetThemeByUrl(paletteUrl, fontUrl, backgroundUrl, resetSubsitesToInherit, updateRootOnly);
 }

```
<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Provisioning.OneDrive" />
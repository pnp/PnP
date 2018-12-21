# Create sub site or site collection #

### Summary ###
This sample shows how to use OfficeDevPnP core component to create sub sites or new site collections using MVC 5 and a Provider Hosted Add-In.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
-  You must register the application using appregnew.aspx
-  You need to deploy the app package to the app catalog and then install the add-in to your developer site.
-  You also need to approve the requested permissions (Tenant, Full Control, App Only) as a Tenant Admin

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.CreateSite.Mvc | Mike Morrison

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 20th 2018 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: Create a sub site or a site collection using core component and MVC 5 #
This sample demonstrates how to create sub sites or site collections using the extensions methods from the OfficeDevPnP core component. Extensions are available from normal client side object model objects after you have referenced the OfficeDevPnP core component

## Sub site creation ##
Sub site creation is actually a single line of code. Following calls are for applying small modifications to the newly created sub site

```C#
// Create the sub site
Web newWeb = cc.Web.CreateWeb(props.Title, props.Url, "", props.SelectedWebTemplate, 1033);

```

## Site collection creation ##
To be able to create site collections, you’ll need to associate to the admin site of the Office365 tenant and in this example we are also using the add-in only token so that end user does not have to have high permission to the tenant. In following lines we resolve the access token and then create site collection using extension methods.


```C#
public ActionResult CreateSite(NewSiteProperties props)
{
    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
    var baseUrl = $"{spContext.SPHostUrl.Scheme}://{spContext.SPHostUrl.Host}";

    // Create admin URL
    var adminUrl = new Uri(baseUrl.Insert(baseUrl.IndexOf("."), "-admin"));

    // Get the access token
    var accessToken = TokenHelper.GetAppOnlyAccessToken(
        TokenHelper.SharePointPrincipal,
        adminUrl.Authority,
        TokenHelper.GetRealmFromTargetUrl(adminUrl)).AccessToken;

    // Create the tenant ClientContext and create the site
    using (var ctx = TokenHelper.GetClientContextWithAccessToken(adminUrl.ToString(), accessToken))
    {
        Tenant tenant = new Tenant(ctx);
        // Pass the false parameter for wait so we do not hold the connection open
        // while waiting for the site to be created.  Instead we show a spinner.
        tenant.CreateSiteCollection($"{baseUrl}/sites/{props.Url}", props.Title, props.SiteOwnerEmail, props.SelectedWebTemplate, 1000, 800, 7, 10, 8, 1033, false, false, null);
    }

    // Change the leaf URL to the AbsoluteUri so we can provide a link to the newly created site.
    props.SPHostUrl = spContext.SPHostUrl.AbsoluteUri;
    //return View();
    return RedirectToAction("SiteStatus", props);
}
```

## Waiting on the Site Collection ##
In this sample, we do not wait on the site collection to be created while the HTTP connection is held open.  Instead we specify the wait parameter as false on on the call to ```tenant.CreateSiteCollection```.  We then redirect to a status page that refreshes every 10 seconds waiting on the site to be "Active".

Here is the code:

```C#
using (var ctx = TokenHelper.GetClientContextWithAccessToken(adminUrl.ToString(), accessToken))
{
    Tenant tenant = new Tenant(ctx);

    // Checks to see if site is created yet.
    var isSiteAvailable = tenant.CheckIfSiteExists($"{baseUrl}/sites/{props.Url}", "Active");


    if (!isSiteAvailable)
    {
        // This view uses JavaScript to refresh every 10 seconds to check if the site has been created
        return View("WaitingOnSite");
    }
    else
    {
        // Convert the URL to Absolute so we can provide a link to the new site collection
        props.Url = $"{baseUrl}/sites/{props.Url}";
        return View(props);
    }
}
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.CreateSite.Mvc" />
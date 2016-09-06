# Consume ASP.Net WebAPI 2 from provider hosted SharePoint apps #

### Summary ###
This scenario shows how you can call Web API 2 services from low trust provider hosted SharePoint apps. You'll have the option to authorize the Web API call and to "reinstantiate" the callers client context in the WebAPI service. We'll also address how to make cross domain calls to WebAPI services.

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
Core.Services.Authenticate | Bert Jansen (**Microsoft**) inspired by the [CloudTopia](http://blogs.technet.com/b/speschka/archive/2014/08/11/cloudtopia-connecting-o365-apps-azure-and-cortana-part-3.aspx) sample of Steve Peschka

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | October 23rd 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# What do we want to achieve #
The use case behind this sample is providing an easy way to pass along the calling user's context to the web API service when the service is called via JavaScript. A nice side effect of this is that we can do a basic authorization of Web API calls when done via JavaScript. 

Next chapters describe the steps taken to realize this. 

## Setup of your WebAPI service ##
This solution contains a provider hosted SharePoint add-in (Core.Services.Authenticate.SharePoint and Core.Services.Authenticate.SharePointWeb) and a WebAPI project (Core.Services.Authenticate.WebAPI). In both these projects a WebAPI service has been added by creating a folder named **"Controller"**, right clicking it, choosing **Add** and selecting **"Controller..."**:

![Add scaffold dialog from Visual Studio](http://i.imgur.com/M7oS3m8.png)

We then selected the WebAPI 2 controller to get an empty controller added to the project.

To make the WebAPI service work in our model you're required to add a "register" method to it:

```C#
[HttpPut]
public void Register(WebAPIContext sharePointServiceContext)
{
    WebAPIHelper.AddToCache(sharePointServiceContext);
}
```

This "register" method will be called once when the user launches the add-in:

```C#
protected void Page_Load(object sender, EventArgs e)
{
    // regular Page_Load code...

    //register the web API service in this SharePoint add-in
    Page.RegisterWebAPIService("api/demo/register");
}
```

What will happen when the "register" service is called is the following:

1. The cacheKey will be fetched. The cache key is an opaque string that is unique to the combination of user, user name issuer, add-in, and SharePoint farm or SharePoint Online tenant.
2. A cookie with name "servicesToken" and value = cacheKey will be added to the page response
3. The context token, client ID, client Secret, host web url and add-in web url will be fetched and used as input to call the "register" Web API service
4. The "register" Web API service will call the AddToCache method which will request an access token for provided input. The provided input, the requested access token and the accompanying refresh token are cached.


## Calling the Web API service ##
Calling Web API services from JavaScipt is very simple as shown below. As user you do not need to worry about passing additional parameters:

```JavaScript
function callWebAPIService() {
    var uri = '/api/demo';
    $.get(uri)
        .done(function (data) {
            // your code goes here
        })
        .fail(function (jqXHR, textStatus, err) {
            // your code goes here
        });
}
```

The above call will result in the Web API service being called. In the service we use below code to "reinstantiate" the caller's CleintContext:

```C#
[HttpGet]
public IEnumerable<Item> GetItems()
{
    using (var clientContext = WebAPIHelper.GetClientContext(ControllerContext))
    { 
        if (clientContext != null)
        {
            // your code goes here
        }
        else
        {
            // your code goes here
        }
```

What will happen in the GetClientContext method is the following:

1. The cacheKey will be retrieved from the servicesToken cookie
2. The cached access token will be retrieved using the cacheKey
3. If the access token has expired then the refresh token is used to obtain a new access token
4. A SharePoint ClientContext object will be created using the access token

To learn more about the different tokens used in a low trust setup check out this page http://msdn.microsoft.com/en-us/library/office/dn762763(v=office.15).aspx#CacheContextToken. 

## Authorize the Web API service calls ##
The fact that we issue a cookie when a Web API service gets registered means that we can also use this cookie to authorize a user: without the proper cookie the user is not authorized to call the service. This is done using a custom ActionFilterAttribute implementation:

```C#
public class WebAPIContextFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        if (actionContext == null)
        {
            throw new ArgumentNullException("actionContext");
        }

        if (WebAPIHelper.HasCacheEntry(actionContext.ControllerContext))
        {
            return;
        }
        else
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, CoreResources.Services_AccessDenied);
            return;
        }
    }
}
```

To make use you simply decorate the method calls you want to secure with the **WebAPIContextFilter** attribute:
```C#
[WebAPIContextFilter]
[HttpGet]
public IEnumerable<Item> GetItems()
{
    // your code goes here
}
```


# How to deal with cross domain calls #
When you want to call a Web API service from JavaScript and this service is running in a different domain then you'll need to deal with the cross domain issues. Below chapters describe how to enable cross domain calls and how to apply the above concepts in cross domain calls.

## Using CORS to allow cross domain calls ##
We've opted to shows how to deal with cross domain calls using [CORS](http://enable-cors.org/index.html) as this is the current standard solution. JSONP would be an, older, alternative approach. For WebAPI services there's CORS support from Microsoft via adding the "Microsoft ASP.Net Cross-Origin Support" NuGet package. http://msdn.microsoft.com/en-us/magazine/dn532203.aspx is an excellent article if you want to learn more about CORS support for the WebAPI. Once that's done enabling CORS is simple:

### Update WebApiConfig ###
CORS needs to be enabled as shown below:

```C#
public static void Register(HttpConfiguration config)
{
    // Web API configuration and services 
    config.EnableCors(); 

    config.MapHttpAttributeRoutes();

    config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
    );
}
```

### Decorate service methods to specify who can call them ###
Use the EnableCors attribute to specify who can call the service. Below example sets things wide open, but in real life you want to only allow certain origins and/or methods:

```C#
    [EnableCors(origins: "*", headers: "*", methods: "*")] 
    public class DemoController : ApiController
    {
        // service methods
    }
```

### Insert the cacheKey as URL parameter in the cross domain call ###
Final step is to insert the cacheKey as URL parameter in the WebAPI call: this is needed as cookies are not passed accross domain boundaries:

```JavaScript
function callWebAPIServiceCORS() {
    var uri = 'https://bjansencorswebapi.azurewebsites.net/api/demo?servicestoken=' + getCookie("servicesToken");

    $.get(uri)
        .done(function (data) {
            // your code goes here
        })
        .fail(function (jqXHR, textStatus, err) {
            // your code goes here
        });
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) != -1) return c.substring(name.length, c.length);
    }
    return "";
}
```

### Testing ###
This solution hardcodes https://bjansencorswebapi.azurewebsites.net as host for the cross domain call. If you want to test this then you'll need to deploy the Core.Services.Authenticate.WebAPI project to your host. Point is that you cannot test cross domain calls when testing on localhost.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.Services.Authenticate" />
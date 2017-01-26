# # AzureAD-WebAPI-SPOnline #

### Summary ###
In this sample, a Windows console Application calls a Web API secured by Azure AD and the API calls SharePoint Online on behalf the logged in user. This scenario is useful for situations where you need a protected API Proxy to interact with SharePoint Online using User's credentials.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
AzureAD.WebApi.SPOnline | Rodrigo Romano

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | July 9th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# AzureAD-WebAPI-SPOnline

In this sample, a Windows console Application calls a Web API secured by Azure AD and the API calls SharePoint Online on behalf the logged in user. This scenario is useful for situations where you need a protected API Proxy to interact with SharePoint Online using User's credentials.

The application uses the Active Directory Authentication Library (ADAL) to get a token from Azure AD using the OAuth 2.0 client credential flow, where the client credential is a password.

For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD.](http://go.microsoft.com/fwlink/?LinkId=394414)

If you just want to get an working demo, please go to [this link.](#How_to_run_this_sample)

## Console Application

### NuGet Packages
To create a solution from scratch, Open Visual Studio and create a new Windows Console Application Solution.

Then click on **Tools** menu, **NuGet Package Manager** and in **Package Manager Console** item.

Install below packages:

- Install-Package Microsoft.Net.Http
- Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory -Version 2.15.204151539

![Install Nuget Packages](https://cloud.githubusercontent.com/assets/12012898/7217636/cb27b382-e60f-11e4-90ad-b3c6338d3b1d.png)



## Web API

To create this project, Create a New ASP.NET Web Application and then choose Empty Template. We're going to create everything from the ground.

![New Project](https://cloud.githubusercontent.com/assets/12012898/7217678/a7caa63a-e612-11e4-9729-8d3b7c4e9f32.png)

![Choose Template](https://cloud.githubusercontent.com/assets/12012898/7217677/a7c60b16-e612-11e4-8603-b5cfaf261fea.png)

### NuGet Packages
To create a solution from scratch, Open Visual Studio and create a new Windows Console Application Solution.

Then click on **Tools** menu, **NuGet Package Manager** and in **Package Manager Console** item.

Install below packages:

- Install-Package Microsoft.AspNet.WebApi
- Install-Package Microsoft.AspNet.WebApi.Owin
- Install-Package Microsoft.Owin.Host.SystemWeb
- Install-Package Microsoft.Owin.Security.ActiveDirectory
- Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory -Version 2.15.204151539

![Install Nuget Packages](https://cloud.githubusercontent.com/assets/12012898/7217682/1323c2c2-e613-11e4-9b83-fec0f80fdad9.png)

### Add Owin Startup Class

The first thing we need to do is to tell Owin Framework what class is responsible for the Owin initialization.

![Add Owin Startup Class](https://cloud.githubusercontent.com/assets/12012898/7217698/4cc88178-e615-11e4-8166-1960558e911f.png)

Add below "annotation" right before class namespace

`[assembly: OwinStartup(typeof(AzureAD.WebApi.SPOnline.WebApi.Startup))]`

**Replace AzureAD.WebApi.SPOnline.WebApi for your own Startup class' namespace**

![Owin Startup Class Code](https://cloud.githubusercontent.com/assets/12012898/7217702/9b16731c-e615-11e4-9b6a-ad4138c0df6f.png)

Add this code:

```C#
using AzureAD.WebApi.SPOnline.WebApi.App_Start;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(AzureAD.WebApi.SPOnline.WebApi.Startup))]
namespace AzureAD.WebApi.SPOnline.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureAuth(app);
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Audience = ConfigurationManager.AppSettings["Audience"],
                    Tenant = ConfigurationManager.AppSettings["Tenant"]
                });
        }
    }
}
```

### Add WebApiConfig Class

Create a new Folder named **App_Start** and add a new class. The class name is *WebApiConfig*.

![Create class](https://cloud.githubusercontent.com/assets/12012898/7217709/d0106c20-e616-11e4-8e57-edac8dbee455.png)

Add the code below.

![WebApiConfig code](https://cloud.githubusercontent.com/assets/12012898/7217726/45eefda8-e617-11e4-9a3d-65a56a830aae.png)

```C#
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace AzureAD.WebApi.SPOnline.WebApi.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
			 config.Routes.MapHttpRoute(
             name: "DefaultApi",
             routeTemplate: "api/{controller}/{id}",
             defaults: new { id = RouteParameter.Optional });

            // Web API routes
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
```

### Add SharePoint Client References

Right-Click in **References** and select **Add Reference...**

Choose:

- Microsoft.SharePoint.Client
- Microsoft.SharePoint.Client.Runtime

![References](https://cloud.githubusercontent.com/assets/12012898/7217943/94eefaae-e624-11e4-9d20-953c133e5161.png)

### Add a Controller to Handle Requests

Create a new Folder named **Controllers**. Add a new Web API 2 Controller. Give it a name.

![Create new Controller](https://cloud.githubusercontent.com/assets/12012898/7217729/f66bd598-e617-11e4-91ae-e79bc3e6b475.png)

The *Test* method goes on SharePoint Online using a new User's AccessToken and returns the site title.

The important thing here is to Annotate your class with **[Authorize]**. With that annotation, your api will only accept authenticated request. Easy, isn't it?

```C#
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AzureAD.WebApi.SPOnline.WebApi.Controllers
{
    [Authorize]
    public class TestController : ApiController
    {
        [HttpGet]
        public string Test()
        {
            string sharePointUrl = ConfigurationManager.AppSettings["SharePointURL"];
            string newToken = GetSharePointAccessToken(sharePointUrl, this.Request.Headers.Authorization.Parameter);

            using (ClientContext cli = new ClientContext(sharePointUrl))
            {

                /// Adding authorization header 
                cli.ExecutingWebRequest += (s, e) => e.WebRequestExecutor.WebRequest.Headers.Add("Authorization", "Bearer " + newToken);
            
                var web = cli.Web;
                cli.Load(web);
                cli.ExecuteQuery();
                return web.Title;
            }
        }

        internal static string GetSharePointAccessToken(string url, string accessToken)
        {
            string clientID = ConfigurationManager.AppSettings["ClientID"];
            string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];

            var appCred = new ClientCredential(clientID, clientSecret);
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext("https://login.windows.net/common");

            AuthenticationResult authResult = authContext.AcquireToken(new Uri(url).GetLeftPart(UriPartial.Authority), appCred, new UserAssertion(accessToken));
            return authResult.AccessToken;
        }

     
    }
}
```





## How to run this sample

To run this sample you will need:
- Visual Studio 2013
- An Internet connection
- An Azure subscription (a free trial is sufficient)

Every Azure subscription has an associated Azure Active Directory tenant.  If you don't already have an Azure subscription, you can get a free subscription by signing up at [http://wwww.windowsazure.com](http://www.windowsazure.com).  All of the Azure AD features used by this sample are available free of charge.

### Step 1:  Clone or download this repository
Open the solution from your local PnP folder using Visual Studio. 

### Step 2:  Register the Web API in Azure Active Directory

To create your applications in Azure, please follow instructions provided in this link: [Create Azure AD Application.](http://bitoftech.net/2014/09/12/secure-asp-net-web-api-2-azure-active-directory-owin-middleware-adal/)

There are a lot of links that explains the same steps. If you will use the link I've provided, follow the steps: 3, 4, 7, 8 and 9.

In addition to that, open your WebAPI project in Azure management portal and click on Configure link.

Click on **Add Application** and Choose **Office 365 SharePoint Online** and grant **Have Full control of all site collections permission**.

### Step 3:  Update references in the Windows Console Application and WEB API project

In the **ConsoleApp** project, update values in **Program.cs** file.

```C#
/// Azure AD WebApi's APP ID URL
string resource = "";

/// Azure AD WebApi's Client ID 
string clientId = "";

/// Azure AD User's credentials
string userName = "";
string userPassword = "";

/// Web API's URL
string apiUrl = "http://localhost:3672/api/Test";
```

In the **WebApi** Project, update the **Web.Config** file.

```
 <appSettings>
    <add key="Audience" value="APPURI" />
    <add key="Tenant" value="TenantGUID" />
    <add key="ClientID" value="ClientID" />
    <add key="ClientSecret" value="ClientSecret" />
    <add key="SharePointURL" value="https://[yourtenant].SharePoint.com" />
  </appSettings> 
```


<img src="https://telemetry.sharepointpnp.com/pnp/samples/AzureAD.WebAPI.SPOnline" />
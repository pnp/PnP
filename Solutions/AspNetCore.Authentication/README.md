# OfficeDevPnP.Core.Framework.Authentication
An ASP.NET Core implementation of the TokenHelper and SharePointContext classes for use in SharePoint Add-ins that run on ASP.NET Core 2.x.

This library (and sample) demonstrates how to get ASP.NET Core provider-hosted apps authenticated through SharePoint.

### Summary ###
The PnP solution includes the following projects:
- OfficeDevPnP.Core.Framework.Authentication - The .NET Core assembly port with the necessary ASP.NET Core authentication middleware and authentication handler.
- AspNetCore.Mvc.StarterWeb - A sample ASP.NET Core web application demonstrating how to consume the above library and build a SharePoint provider-hosted app that can get SharePoint data
- AspNetCore.Mvc.SharePointApp - A sample SharePoint app to make deployment and testing easier

### Solution ###
Solution | Author(s)
---------|----------
AspNetCode.Authentication | Radi Atanassov, OneBit Software; Velin Georgiev, OneBit Software;

### Applies to ###
-  Office 365 Multi-Tenant (MT)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 20th 2016 | Initial version
1.0  | September 2016 | Updated to run on RTM bits
2.0  | September 2017 | Updated to run with .net core 2.0 authentication changes

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

## Design Decisions ##
This library/solution has the following goals in mind (some still not achieved!)

- We want it to be very easy to "Add" and "Use", based on the plug 'n' play model of ASP.NET Core
- We want it to achieve the least amount of change to the developer experience
- Based on the ASP.NET Core middleware design pattern
- We must compile to .NET Framework rather than Core due to Microsoft.IdentityModel.Extensions.dll. We will review this in future and see what our best options are.
- We've followed ASP.NET Core configuration and logging patterns
- Based on the ASP.NET Authorization & Authentication model
- We cannot add it to the current PnP Core assembly due to the old project format and the DNX format of this library
- Will be released through NuGet

## Further Reading ##
You will find more details on the decisions, challenges and implementations here:

[Developing the ASP.NET Core Authentication/Authorization Middleware for SharePoint Provider-Hosted Apps (Add-ins)](http://www.sharepoint.bg/radi/post/Developing-the-ASPNET-Core-AuthenticationAuthorization-Middleware-for-SharePoint-Provider-Hosted-Apps-(Add-ins))

[Getting Started with ASP.NET Core Add-ins for SharePoint Online](http://www.sharepoint.bg/radi/post/Getting-Started-with-ASPNET-Core-Add-ins-for-SharePoint-Online)


## Getting Started ##
1. Build the OfficeDevPnP.Core.Framework.Authentication project and add a reference to the output NuGet package.

2. The following must be added to the Startup.cs ConfigureServices method in your ASP.NET Core web application. Cookies and Session are also added here:
```C#
            services.AddSession();

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = SharePointAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = SharePointAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = SharePointAuthenticationDefaults.AuthenticationScheme;             
            })
            //OPTIONAL
            ////.AddCookie(options =>
            ////{
            ////    options.Cookie.HttpOnly = false; //set to false so we can read it from JavaScript
            ////    options.Cookie.Expiration = TimeSpan.FromDays(14);
            ////})
            .AddSharePoint(options =>
            {
                options.ClientId = Configuration["SharePointAuthentication:ClientId"];
                options.ClientSecret = Configuration["SharePointAuthentication:ClientSecret"];
                //OPTIONAL
                ////options.CookieAuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                //Handle events raised by the auth handler
                options.Events = new SharePointAuthenticationEvents()
                {
                    OnAuthenticationSucceeded = succeededContext => Task.FromResult<object>(null),
                    OnAuthenticationFailed = failedContext => Task.FromResult<object>(null)                    
                };
            });
```
3. The library needs Session in order to keep track of the client requests during redirects. Add the following to the Configure method:
```C#
	app.UseSession();
        
	app.UseAuthentication();
```
4. You might need to restore your Bower and Nuget packages if they are not present on your machine.

## Release Notes ##
- Works on RTM
- This only works with ACS, High Trust is still not supported/developed yet :(
- Remote Event Receivers not tested yet

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/AspNetCore.Authentication" />

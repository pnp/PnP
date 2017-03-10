# Office 365 Add-In With Groups Authorization #

### Summary ###
This sample shows how to use windows Azure Active directory and the groups to authorize
actions in your asp.net MVC app or Office 365 Add-In


### Applies to ###
-  Office 365 Multi Tenant (MT)

### Solution ###
Solution | Author(s)
---------|----------
Office365AddIn.GroupAuthorization | Luis Valencia (**Capatech**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 2th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Office 365 Add-In With Groups Authorization #

Creating Office 365 Apps or Add-Ins in ASP.NET MVC is pretty straightforward, you add a connected service to Azure AAD, setup a few things on the web.config and then you have authentication on your Add-in, this is very well explained in many other blogs, However out there its not explained how to use Authorization on those Add-Ins with Azure Active Directory Groups.   This post will just explain that.

Over the years authorization in ASP.NET Web forms or MVC has been done through the Authorize Attribute, this allows the developer to assign a user or role to a method or controller in a declarative way.

However what blogs don’t tell you is that even if you have groups in Azure AAD, this wont just work out of the box, you have to implement it. If you use the Authorize Attribute on an Office 365 Add-In it will try to use the ASP.NET User.IsInRole under the hood, which depends on how you have your app configured for authentication.  If you have ASP.NET Forms authentication with a sql database it will work without any problem.

Watch this [video](https://www.youtube.com/watch?v=sUC4kJ73Pns&feature=youtu.be) to see a demo.

Read entire documentation here:
[Luis Valencia Blog Post Office 365 Add-In With Groups Authorization](http://www.luisevalencia.com/2015/06/02/using-azure-aad-graph-office-365-add-in-with-groups-authorization/)


[Video](https://youtu.be/sUC4kJ73Pns)

<img src="https://telemetry.sharepointpnp.com/pnp/samples/AzureAD.GroupAuthorization" />
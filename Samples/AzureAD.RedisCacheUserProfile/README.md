# Use Azure RedisCache to Cache Office 365 User Profile Information in an Office 365 Add-in #

### Summary ###
This sample shows how to use windows Azure Active directory to get user profile information
and how to use Azure REDIS Cache to make performant Add-Ins 


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)

### Solution ###
Solution | Author(s)
---------|----------
AzureAD.RedisCacheUserProfile | Luis Valencia (Capatech)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 30th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Use Azure RedisCache to Cache Office 365 User Profile Information in an Office 365 Add-in #

This sample shows how to use Azure Redis Cache to put user profile information from Azure Active Directory.

In our testing we could see that doing a REST CALL to Azure AD to get one user information takes about 1400 milliseconds.
Uzing Azure Redis Cache it takes 88 milliseconds.

An average gain of 1590% to access data.
This same API could be used to save any kind of object into REDIS Cache, Just be sure to set some expiration on the objects depending in your needs.

Read entire documentation here:
[Luis Valencia Blog Post Use Azure RedisCache to Cache Office 365 User Profile Information in an Office 365 App](http://www.luisevalencia.com/2015/06/30/use-azure-rediscache-to-cache-office-365-user-profile-information-in-an-office-365-app/)


[Video](https://youtu.be/5O7uGB1KCRA)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/AzureAD.RedisCacheUserProfile" />
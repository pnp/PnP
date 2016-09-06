# User Profile Batch Update API #

### Summary ###
This sample demonstrates usage of new User Profile Batch Update API. This capability is designed to handle mass updates cross multiple user profiles in the SharePoint Online. API makes it faster and more efficient to synchronize custom attributes from miscellanious systems to user profile properties. 

Solution contains code implemented with console application and also a PowerShell script based implementation of API call.

See following resources for additional information
- API release announcement at dev.office.com - TBD
- [CSOM Nuget package updated - December 2015](https://dev.office.com/blogs/new-sharepoint-csom-version-released-for-Office-365)

*Notice* This API is released gradually to Office 365 production starting from mid-January 2016. 

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
Office 365 tenant updated to version which will support this capability. This will happen gradually cross the world starting from mid-January 2016.

### Solution ###
Solution | Author(s)
---------|----------
UserProfile.BatchUpdate.API | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 31st 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
As part of the new Client Side Object Model (CSOM) version (4622.1208 and newer), SharePoint has new capability for bulk importing user profile properties. Previously you could have taken advantage of the user profile CSOM operations for updating specific properties for user profiles, but this is not that performant and in case of thousands of profiles, operation is too time consuming.
Since many enterprises have however business requirements to replicate custom attributes to SharePoint user profile service, addition and more performant user profile bulk API has been released. This capability is available in newer version of the CSOM redistributable, which has been also released as Nuget package for developers. 

![Process picture with 6 steps](http://i.imgur.com/f0bUVto.png)

1. User attributes are synchronized from the corporate Active Directory to the Azure Active Directory. You can select which attributes are being replicated cross on-premises and Azure
2.	Standardized set of attributes are being replicated from the Azure Active Directory to SharePoint user profile store at Office 365. This cannot be controlled like in the on-premises.
3.	Custom synchronization tool taking advantage of the new build update APIs. Tool uploads a JSON formatted file to Office 365 tenant and queues the import process. Implemented as managed code (.NET) or as PowerShell script using the new CSOM APIs.
4.	LOB system or any external system, which is the actual source of the information in the JSON formatted file. This could be also combination of data from Active Directory and from any external system. Notice that from API perspective, LOB system could be also on-premises SharePoint 2013 or 2016 deployment from where you’d synchronize user profile attributes to SharePoint online.
5.	Out of the box server side timer job running in SharePoint online, which checks for queued import requests and will perform the actual import operation based on the API calls and information in provided file. 
6.	Extended user profile information is available in the user profile and can be used for any out of the box or custom functionality in the SharePoint online. 

*Notice.* Import only works for user profile properties, which has not been set to be editable for the end users. This is to avoid situation where the user profile import process would override any information which end user has already updated.

See additional references what mentioned in the start of the document for API reference.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/UserProfile.BatchUpdate.API" />
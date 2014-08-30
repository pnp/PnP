Office 365 Developer PnP Core Component
=======================================

### Summary ###
Office 365 Developer PnP Core Component is extension component which 
encapsulates commonly used remote CSOM/REST operations as reusable 
[extension methods](http://msdn.microsoft.com/en-us/library/bb383977.aspx) 
towards out of the box CSOM objects. It's targeted to be used with provider 
hosted apps and will help developers to be more efficient and productive by 
providing single line extension methods for commonly used operations from 
content type creation to uploading page layouts to publishing sites.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
Extension methods will be available from out of the box CSOM objects 
automatically when you add reference to this component.

### Solution ###
Solution | Author(s)
---------| ----------
OfficeDevPnP.Core | Office Developer PnP team (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
0.1  | May 6th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS 
OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR 
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


Introduction
============

Core component solution structure follows the generic structure like all PnP 
project. All documentation files are linked to the solution and actual code 
is in single Visual Studio project. This project contains different kind of 
capabilities, which are explained in following chapters.

![](http://i.imgur.com/jjEgRQk.png)


AppModelExtensions
------------------

Description:
Code snippet:
```C#
public static ContentType CreateContentType(this Web web, string name, 
	string id, string group)
{
  // Load the current collection of content types
  ContentTypeCollection contentTypes = web.ContentTypes;
  web.Context.Load(contentTypes);
  web.Context.ExecuteQuery();
  ContentTypeCreationInformation newCt = new ContentTypeCreationInformation();
  // Set the properties for the content type
  newCt.Name = name;
  newCt.Id = id;
  newCt.Group = group;
  ContentType myContentType = contentTypes.Add(newCt);
  web.Context.ExecuteQuery();

  //Return the content type object
  return myContentType;
}

```

To use this extension method you can do this:
```C#
cc.Web.CreateContentType("My CT","0x0101009189AB5D3D2647B580F011DA2F356FB2",
	"My custom content types group");
```

This is pretty typical example of the extension methods which are provided. 
In the provider hosted app side, we only need one line of code, which 
encapsulates all the other needed operations.


Entities
--------

Entities are simple classes used to provide and retrieve more complex objects 
from the extensions methods in AppModelExtensions. Currently following 
entities are defined:

![](http://i.imgur.com/xEmdKPU.png)


Enums
-----

Extension methods in AppModelExtensions can use enums and if so these enum 
classes are created in this folder

![](http://i.imgur.com/dTAJwY5.png)


Extensions
----------

This folder contains 
[extension methods](http://msdn.microsoft.com/en-us/library/bb383977.aspx) 
that are not SharePoint related such as extension methods to help with string 
manipulations.

![](http://i.imgur.com/yUWYgsr.png)


Utilities
---------

Utility classes (helper classes) are created in this folder.

![](http://i.imgur.com/lfDFMtT.png)


AuthenticationManager.cs
------------------------

AuthenticationManager is the class that you can use to obtain a client 
context in case you’re not having one available as part of the SharePoint App 
(e.g. in console projects) or when you want to create a client context using 
different credentials or using an AppOnly app. Following methods are available 
for this class:

```C#
public ClientContext GetSharePointOnlineAuthenticatedContextTenant(string siteUrl, 
	string tenantUser, string tenantUserPassword)
public ClientContext GetAppOnlyAuthenticatedContext(string siteUrl, string realm, 
	string appId, string appSecret)
public ClientContext GetNetworkCredentialAuthenticatedContext(string siteUrl, 
	string user, string password, string domain)

```

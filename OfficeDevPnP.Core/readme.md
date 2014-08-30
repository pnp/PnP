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


Coding Conventions
==================

Contributions are welcome, and the project regularly accepts pull requests.

Here are some guidelines for writing your code to ensure consistency across
the project.


Avoid using the ambiguous term "Site"
-------------------------------------

SharePoint has a mismatch between the terms used in the UI and the API, 
particularly with SPWeb = Site / SPSite = Site Collection, with the client
APIs dropping the SP prefix.

The term "Site" is ambiguous, both in the API and in documentation comments,
and should be avoided.

The terms *Web* and *SiteCollection* are preferred.

If the term Site is used, it should always refer to the Site Collection; never
use Site to refer to a Web.


App Model Extensions
--------------------

Classes derived from ClientObject have a Context property that can be used
for operations, such as ExecuteQuery.

The App Model Extensions API should extend from the relevant entity object, 
e.g. creating a new List should extended from the Web where it should be 
created, adding an exising Content Type to a List should extend from a List.

Do not generally extend from collections, as they are less discoverable, e.g.
do not extend from ContentTypeCollection.

Do not extend from an irrelevant type; if you do not use any properties or
methods (other than Context) then you are probably exteneding from the wrong
object. 

If there is no relevant object, then consider just extending from 
ClientRuntimeContext (although we want to avoid excessively overloading the 
context).

If the operation requires a specific type of context, consider extending from
something that makes this clear. E.g. tenant operations require a context of
the admin site, and won't work from all contexts, so extended from the Tenant
class instead (even though it requires one additional line of code from the
caller).


Clarify the type of URL
-----------------------

Clarify if a URL is a FullUrl, ServerRelativeUrl, SiteCollectionRelativeUrl, 
or LeafUrl.  This avoids ambiguity where users of the API have a different
default understanding of what 'url' means.

Prefer using the Uri class for clarity, but generally provide overloads that
use string.

Validate URL arguments conform to the format of the type expected, e.g. parse 
using the Uri class if necessary and check the IsAbsoluteUri property; for 
LeafUrl, throw an ArgumentException if the parameter has any path characters.

(Note that the Uri class unfortunately does not parse out the local path 
segments for relative URLs.)

Do not use string operations to combine URLs. Use the 
UrlUtility.Combine function, or Uri class functions.


Standard Verbs
--------------

The following standard verbs should be used for consistency in the API.

In particular note the difference between Create (new elements, even if 
created as the children of other elements) and Add (only associating existing
elements).

For reference to standard PowerShell verbs, see: 
[http://msdn.microsoft.com/en-us/library/ms714428(v=vs.85).aspx]


**Add:** The verb Add should not be used to create new elements (use Create 
instead). Add should be used for associating existing items with each other.
In PowerShell Cmdlets use Add-.

Examples: Add can be used for adding an existing site column to a content 
type, or adding a content type to a list. 

In some cases verbs may be used for consistency, e.g. use the same verb for
adding a list column as for adding a site column, even though one is actually
a create operation.


**Create:** Use the verb Create for operations that create new elements, even if 
they are children of other objects (do not use Add). In PowerShell Cmdlets 
use New-.

Examples: Creating a new site collection, creating a new site content type (in 
the context of a Web), adding a new child Web, adding a new List.

Note that the last two (new child Web, new List) are still Create operations
in the API, as primarily they still create new elements, even though in the UI
the term add may be used (e.g. 'add an app').


**Delete:** Used for destroying an object. In PowerShell Cmdlets use Remove- 
(the same verb is used for both meanings in PowerShell).

Examples: Deleting a site column from a Web (contrast with removing a site
column from a Content Type or List), deleting a child Web.


**Remove:** Use the verb Remove for operations that removing associations 
between objects, but does not destroy them. In PowerShell Cmdlets use Remove-.

Examples: Removing a site column from a content type or removing a user from a
group.


#### Less common verbs ####

**Deploy:** Uploads a file (or multiple files) as a resource (master page, 
theme, etc) and then (generally) makes it available for use (i.e. generally
includes Publish). In PowerShell Cmdlets use Install-.

**Ensure:** Checks if an element exists, creating it if necessary, and then
returning the instance. 

**Exists:** Used as a suffix to determine if an element exists. In PowerShell
Cmdlets use Test-.

**Upload:** Loads a file from a local resource into SharePoint. Generally this
also includes making it available for others (i.e. Publish). In PowerShell
Cmdlets use Import-.



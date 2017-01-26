# Using the $batch query option against the SharePoint REST APIs #

# Summary #
The purpose of this sample is to show how to use the $batch query option, as defined by the OData 3.0/4.0 standard, can be used with SharePoint Online's REST APIs and the Files and Folders APIs of Office 365.

The sample shows how to combine multiple OData operations against the SharePoint REST/OData service into a single HTTP Request/Response. The sample uses HTML, C#, and the managed code OData library in [Microsoft.Data.Odata](http://msdn.microsoft.com/en-us/office/microsoft.data.odata(v=vs.90)).

As of the initial release of this sample, Microsoft's support for $batch is not compliant with the OData 3.0/4.0 standard in one respect: The REST service does not support "all or nothing" transaction protection for the operations that are included in a ChangeSet, which is a set of operations that make changes on the OData source. (Purely "read" operations; that is, operations that use the GET verb, are outside of ChangeSets although they can be included in the operations of a batch request.)

### Applies to ###
-  SharePoint Online and the Files and Folders subset of the Office 365 REST APIs

### Prerequisites ###
Not required, but recommended: Install [Fiddler](http://www.telerik.com/fiddler).

### Solution ###
Solution | Author(s)
---------|----------
Core.ODataBatch | Ricky Kirkham (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 31st 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

# To use this sample #

12. Open the .sln file for the sample in **Visual Studio**.
13. In Solution Explorer, highlight the SharePoint add-in project and replace the **Site URL** property with the URL of your SharePoint developer site.
14. Turn on Fiddler if you have it installed.

You can now run the sample with F5. The first time you do, you are prompted to trust the add-in. The default page of the web application then opens. There are three different batch jobs you can try. They use lists that are present on every Developer Site out-of-the-box: The User list, the Composed Looks list, and the List of Lists on the website.

# Questions and comments

We'd love to get your feedback on this Android starter kit. You can send your questions and suggestions to us:

* In the [Issues](https://github.com/OfficeDev/SP-O365-REST-batch/issues) section of this repository.
* On [Stack Overflow](http://stackoverflow.com/questions/tagged/Office365+API). 
  Make sure that your questions or comments are tagged with either [Office365] or [SharePoint], and with [API].
  
<a name="resources"/>
# Additional resources

* [Make batch requests with the REST APIs](http://msdn.microsoft.com/library/office/dn903506.aspx)
* [Get started with the SharePoint 2013 REST service](http://msdn.microsoft.com/library/office/fp142380.aspx)
* [Office 365 APIs documentation](http://msdn.microsoft.com/office/office365/howto/platform-development-overview)
* [File REST operations reference](http://msdn.microsoft.com/office/office365/api/files-rest-operations)

### Copyright ###

Copyright (c) Microsoft. All rights reserved.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ODataBatch" />
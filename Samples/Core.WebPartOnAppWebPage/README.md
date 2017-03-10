# Declaratively embedding a Web Part on a page in the add-in web #

# Summary #
The purpose of this sample is to show the best way to declaratively include a Web Part on a page in the add-in web, because the best way is not the most obvious or natural way. Developers should *not* simply include WebPart markup in the ASPX page itself. Doing so can cause errors when the add-in is updated. 

Instead, the WebPart markup should be in the element manifest file (usually called elements.xml) for the page. 

### Applies to ###
-  SharePoint Online and SharePoint on-premise

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
Core.WebPartOnAppWebPage | Ricky Kirkham (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 31st 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

# To use this sample #

1. Open the .sln file for the sample in **Visual Studio**.
2. In **Solution Explorer**, highlight the SharePoint add-in project and replace the **Site URL** property with the URL of your SharePoint developer site.

You can now run the sample with <kbd>F5</kbd>. You will not be prompted to trust the add-in because this is a SharePoint-hosted add-in and it automatically has full rights to the add-in web. The add-in will launch immediately and you will see the start page with a list view WebPart on it. There are no items in the list.

![The start page of the add-in.](Images\Fig1AppPageWithWebPart.png)

# Questions and comments

We'd love to get your feedback on this sample. You can send your questions and suggestions to us:

* In the [Issues](https://github.com/OfficeDev/SP-WebPart-Page/issues) section of this repository.
* On [Stack Overflow](http://stackoverflow.com/questions/tagged/Office365+API). 
  Make sure that your questions or comments are tagged with either [Office365] or [SharePoint], and with [API].


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.WebPartOnAppWebPage" />
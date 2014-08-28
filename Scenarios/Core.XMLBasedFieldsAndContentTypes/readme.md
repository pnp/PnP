# Create fields and content types based on element XML #

### Summary ###
This sample shows how to provision site columns, content types and views using xml based structure.

### Walkthrough Video ###
Visit the video on Channel 9 - [http://channel9.msdn.com/Blogs/Office-365-Dev/Create-SharePoint-fields-and-content-types-based-on-element-XML-in-Apps-for-SharePoint-Office-365-De](http://channel9.msdn.com/Blogs/Office-365-Dev/Create-SharePoint-fields-and-content-types-based-on-element-XML-in-Apps-for-SharePoint-Office-365-De)

![](http://i.imgur.com/IBMsNa0.png)

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
Core.XMLBasedFieldsAndContentTypes | Sami Nieminen, Vesa Juvonen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 25th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: FEATURE FRAMEWORK XML BASED PROVISIONING #
This sample demonstrates how to use feature framework element xml files defining fields and content types to provision remotely structures to the host web. This is typical process when we provision site collections remotely using CSOM.

All implementation is based on the code located in the OfficeDevPnP core component, which encapsulates the processing completely. Developer will just need to provide the element xml file and call the right method. 


# Outlook Notifications REST API with ASP.NET Web API #

### Summary ###
This is a sample of an ASP.NET Web API project validating and responding to Outlook Notifications - created with the Outlook Notifications REST API.

As always, include a valid authorization header when making the request. You can learn more about the Outlook Notifications REST API and its operations at: <https://msdn.microsoft.com/en-us/office/office365/api/notify-rest-operations>

Using this event driven approach is a much more solid way of dealing with changes in the resources and entities in Outlook. As opposed to polling the Outlook REST APIs directly, this is much more lightweight (especially when the amount of items is large). With scale, this approach becomes essential for a sustainable service architecture.

Read more about this sample at: <http://simonjaeger.com/call-me-back-outlook-notifications-rest-api>

### Applies to ###
-  Exchange Online
-  Office 365
-  Hotmail.com
-  Live.com
-  MSN.com
-  Outlook.com
-  Passport.com

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
Office.TypeScriptAddin | Simon Jäger (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 12th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Doc scenario 1 #
Description
Image


## Sub level 1.1 ##
Description:
Code snippet:
```C#
string scenario1Page = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
string scenario1PageUrl = csomService.AddWikiPage("Site Pages", scenario1Page);
```

## Sub level 1.2 ##

# Doc scenario 2 #

## Sub level 2.1 ##

## Sub level 2.2 ##

### Note: ###

## Sub level 2.3 ##

# Doc scenario 3#


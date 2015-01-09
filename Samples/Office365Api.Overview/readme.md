# Office 365 API demo application #

### Summary ###
This WPF app show the output of various Office 365 API calls in a console alike output format. The goal of this app is to see the new API while keeping focus on the API calls themselves and less on the UI layer they're hosted in.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
This sample requires the Office 365 API version released on November 2014. See http://msdn.microsoft.com/en-us/office/office365/howto/platform-development-overview for more details.

### Solution ###
Solution | Author(s)
---------|----------
Office365Api.Overview | Bert Jansen (**Microsoft**), Paolo Pialorsi (**PiaSys.com**, @PaoloPia)

### Version history ###
Version  | Date | Comments
---------| -----| --------
3.0  | January 7th 2015 | Updated to Office 365 API RTM and ADAL 2.13 (Paolo Pialorsi)
2.0  | August 12th 2014 | Switched to WPF app and added documentation (Bert Jansen)
1.0  | July 29th 2014 | Initial release (Bert Jansen)

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Prepare the scenario for your environment #
This application will use the new Office 365 API's to perform the following list of tasks:
-  Discover the current user's OneDrive URL
-  Discover the current user's Mail URL
-  List the files and folders from the user's OneDrive
-  Upload a file to the "Shared with everyone" folder in the user's OneDrive
-  List all files and folders in the "Shared with everyone" folder of the user's OneDrive
-  Retrieve top 50 mails in the Inbox, just print the first 10
-  Send a mail with the sent mail ending up in the user's "Sent items" mailbox folder
-  Create a mail in the "Drafts" mailbox folder
-  Get all users from Azure AD, just print the first 10

For these tasks to succeed you need to provide some input before you run the application. This is done by changing the text fields in the UI, just after launching the WPF project + by registering an application in Azure AD. You can do this by right-clicking the **Office365Api.Demo** project -> **Add** -> **Connected Service**. Select **Office 365 APIs** and click on **Register your App**:

![](http://i.imgur.com/vksc2KD.png)

Click **Yes** to register an Azure AD App and then give it the permissions as shown below:

![](http://i.imgur.com/uhQpqHt.png)


## Run the sample ##
When you run the sample you'll see some text fields, a window with a big button named "Run demo" and a black output section. Fill out the text fields with proper values, select a file to upload by browsing the file system, and click on the "Run demo" button to trigger the demo. What will first happen is that you need to logon with an Office 365 user account.

![](http://i.imgur.com/852IH4o.png)


Once you've logged on the Office 365 API will ask you for permissions: you need to consent that the app access your data for the listed categories:

![](http://i.imgur.com/M9D343S.png)


After those 2 steps are done the app can run and use all the API's to do it's work. The output is shown in console style:

![](http://i.imgur.com/vLcdlrL.png)

## Some explanation about the API's themselves ##
The app is built by extending the default classes added when you hookup a connected service:
-  ActiveDirectoryApiSample.cs
-  CalendarApiSample.cs
-  ContactsApiSample.cs
-  MailApiSample.cs
-  MyFilesApiSample.cs
-  SitesApiSample.cs

The class DiscoveryAPISample.cs has been created manually, as well as the AuthenticationHelper.cs class. Those classes have been adopted in order to share and cache the AuthenticationContext created during the first logon. This is needed to avoid continues prompting for consent.

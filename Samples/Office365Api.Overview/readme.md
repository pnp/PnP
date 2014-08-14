# Office 365 API demo application (PREVIEW) #

### Summary ###
This WPF app show the output of various Office 365 API calls in a console alike output format. The goal of this app is the see the new API while keeping focus on the API calls themselves and less on the UI layer they're hosted in.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
This sample requires the Office 365 API **preview** version released on August 5th 2014. See http://blogs.office.com/2014/08/05/office-365-api-tool-visual-studio-2013-summer-update/ for more details.

### Solution ###
Solution | Author(s)
---------|----------
Office365Api.Overview | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.0  | August 12th 2014 | Switched to WPF app and added documentation
1.0  | July 29th 2014 | Initial release

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
-  List the total number of mails in the user's mailbox
-  Retrieve all mails in the Inbox, just print the first 10
-  Send a mail with the sent mail ending up in the user's "Sent items" mailbox folder
-  Create a mail in the "Drafts" mailbox folder
-  Get all users from Azure AD, just print the first 10

For these tasks to succeed you need to provide some input before you run the application. This is done by changing the below code snippet in the MainWindow.xaml.cs class:
```C#
//TODO: update these values to make them relevant for your environment
private string uploadFile = @"C:\temp\bulkadusers.xlsx";
private string serviceResourceId = "https://<tenant>.sharepoint.com";
private string siteUrl = "https://<tenant>.sharepoint.com/sites/<sitename>";
private string sendMailTo = "<email address>";
```
## Run the sample ##
When you run the sample you'll see a window with a big button named "Run demo" and a black output section. Click on the "Run demo" button to trigger the demo. What will first happen is that you need to logon with an Office 365 user account.

![](http://i.imgur.com/RIGgm7H.png)


Once you've logged on the Office 365 API will ask you for permissions: you need to consent that the app access your data for the listed categories:

![](http://i.imgur.com/6bDBl5w.png)


After those 2 steps are done the app can run and use all the API's to do it's work. The output is shown in console style:

![](http://i.imgur.com/LQnkq5W.png)

## Some explanation about the API's themselves ##
The app is built by extending the default classess added when you hookup a connected service:
-  ActiveDirectoryApiSample.cs
-  CalendarApiSample.cs
-  ContactsApiSample.cs
-  MailApiSample.cs
-  MyFilesApiSample.cs
-  SitesApiSample.cs

The class DiscoveryAPISample.cs has been created manually. The default classes have been adopted to so that they can pass along the DiscoveryContext created during the first use. This is needed to avoid continues prompting for consent.

```C#
//static DiscoveryContext _discoveryContext;
public static DiscoveryContext _discoveryContext
{
    get;
    set;
}

```

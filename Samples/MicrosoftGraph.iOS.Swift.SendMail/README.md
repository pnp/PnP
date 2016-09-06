# Microsoft Graph SDK for iOS using Swift #

### Summary ###
If you haven’t heard, there is an easy way to call a great amount of Microsoft APIs – using one single endpoint. This endpoint, so called the Microsoft Graph (<https://graph.microsoft.io/>) lets you access everything from data, to intelligence and insights powered by the Microsoft cloud.

No longer will you need to keep track of different endpoints and separate tokens in your solutions – how great is that? This post is an introductory part of getting started with the Microsoft Graph. For changes in the Microsoft Graph, head to: <https://graph.microsoft.io/changelog>

This sample showcases the Microsoft Graph SDK for iOS (<https://github.com/OfficeDev/Microsoft-Graph-SDK-iOS>) in a simple iOS application using the new Swift language (<https://developer.apple.com/swift/>). In the application we will send ourselves a mail. The objective is to get familiar with the Microsoft Graph and its possibilities.

![App UI in iPhone and email](http://simonjaeger.com/wp-content/uploads/2016/03/app.png)

Be aware, the Microsoft Graph SDK for iOS is still in preview. Read more about the conditions at: https://github.com/OfficeDev/Microsoft-Graph-SDK-iOS

Read more about this sample at: <http://simonjaeger.com/get-going-swiftly-with-the-microsoft-sdk-for-ios/>

### Applies to ###
-  Exchange Online
-  Office 365
-  Hotmail.com
-  Live.com
-  MSN.com
-  Outlook.com
-  Passport.com

### Prerequisites ###
You will need to register your application before you can make any calls towards the Microsoft Graph. Find more information at: <http://simonjaeger.com/get-going-swiftly-with-the-microsoft-sdk-for-ios/#step-11-register-the-application-in-azure-ad>

If you are building for Office 365 and you're missing an Office 365 tenant - get yourself a developer account at: <http://dev.office.com/devprogram>

You will need Xcode installed on your machine in order to run the sample. Get Xcode at: <https://developer.apple.com/xcode/>

### Project ###
Project | Author(s)
---------|----------
MSGraph.MailClient | Simon Jäger (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 9th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# How to Use? #

Your first step is to register your application in your Azure AD tenant (associated with your Office 365 tenant). You can find more details about registering you app in the Azure AD tenant here: <http://simonjaeger.com/get-going-swiftly-with-the-microsoft-sdk-for-ios/#step-11-register-the-application-in-azure-ad>

Since the application is calling back into the Microsoft Graph and sends a mail on behalf of the signed in user - it's important to grant it permissions to send mails.

When you have registered your application in Azure AD - you will have to configure the following settings in the **adal_settings.plist** file:
    
```xml
<plist version="1.0">
<dict>
	<key>ClientId</key>
	<string>[YOUR CLIENT ID]</string>
	<key>ResourceId</key>
	<string>https://graph.microsoft.com/</string>
	<key>RedirectUri</key>
	<string>[YOUR REDIRECT URI]</string>
	<key>AuthorityUrl</key>
	<string>[YOUR AUTHORITY]</string>
</dict>
</plist>
```

Launch the workspace file (**MSGraph.MailClient.xcworkspace**) in Xcode. Run the project using the **⌘R** shortcut, or by pressing the **Run** button in the **Product** menu.
    
# Source Code Files #
The key source code files in this project are the following:

- `MicrosoftGraph.iOS.Swift.SendMail\MSGraph.MailClient\MailClient.swift` - this class takes care of signing in the user, getting the user profile and finally sending the mail with a message.
- `MicrosoftGraph.iOS.Swift.SendMail\MSGraph.MailClient\ViewController.swift` - this is the single view controller for the iOS app, which triggers the MailClient.
- `MicrosoftGraph.iOS.Swift.SendMail\MSGraph.MailClient\adal_settings.plist` - this is the ADAL configuration property list file. Be sure to configure the required settings in this file before running this sample.

# More Resources #
- Discover Office development at: <https://msdn.microsoft.com/en-us/office/>
- Get started on Microsoft Azure at: <https://azure.microsoft.com/en-us/>
- Explore the Microsoft Graph and its operations at: <http://graph.microsoft.io/en-us/> 
- Read more about this sample at: <http://simonjaeger.com/get-going-swiftly-with-the-microsoft-sdk-for-ios/>


<img src="https://telemetry.sharepointpnp.com/pnp/samples/MicrosoftGraph.iOS.Swift.SendMail" />
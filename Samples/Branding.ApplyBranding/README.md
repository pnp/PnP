# Branding.ApplyBranding #

### Summary ###
This sample demonstrates common tasks related on the publishing features, like how to deploy page layouts and master pages. It also shows how you can use property bag entries in the Web object to control the available page layouts and available web templates settings for the publishing sites. 

### Walkthrough Video ###
Visit the video on Channel 9 - [http://channel9.msdn.com/Blogs/Office-365-Dev/Applying-Branding-to-SharePoint-Sites-with-an-App-for-SharePoint-Office-365-Developer-Patterns-and-P](http://channel9.msdn.com/Blogs/Office-365-Dev/Applying-Branding-to-SharePoint-Sites-with-an-App-for-SharePoint-Office-365-Developer-Patterns-and-P)

![Picture of the video in Channel 9](http://i.imgur.com/lvyDtQB.png)

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
none

### Solution ###
Solution | Author(s)
---------|----------
Branding.ApplyBranding  | Johan Skårman  (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 22th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SETUP #
To run this sample first set up 2 new site collections (/sites/pub1 & /sites/pub2) with publishing template. Also open settings.xml and change the url in the branding element. Please note the url's are case sensitive.

![XML configuration picture](http://i.imgur.com/jJxGEzc.png)

You will also be required to add in a command line argument. Goto Solution > Properties > Debug then enter any of the following options into the Start Options box. Please note you only need to enter "activate online" or whatever option suits your needs.

![Console application executed](http://i.imgur.com/ZZo3wto.png)

Now you should be able to run the console application with the result below. When prompted for SharePoint user name/Password enter credentials for a user with at least Full Control permission on the sites. 

![Console application executed](http://i.imgur.com/dJFm7Rp.png)

After the application is completed navigate to /sites/pub1 or /sites/pub2 and verify that the branding has been applied. You should also have two new page layouts available.

![Screen shot of the branded UI](http://i.imgur.com/ErHzlot.png)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Branding.ApplyBranding" />
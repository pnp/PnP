# BULK UPLOAD USER PROFILE PICTURES #

### Summary ###
This sample applications allows you to bulk upload user profile pictures to SharePoint Online, and edit the user profile properties to point to newly uploaded picture.

### Applies to ###
-  Office 365 Multi-Tenant (MT)


### Solution ###
Solution | Author(s)
---------|----------
Core.ProfilePictureUploader | Michael O’Donovan (Microsoft)
Multilingual support| Massimo Prota (Rapid Circle)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 3rd, 2014 | Initial release
1.1  | May 31st, 2016 | Added multilingual support

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# BULK UPLOAD USER PROFILE PICTURES TO SHAREPOINT ONLINE #
There are several scenarios when you would like to bulk upload user profile pictures into SharePoint Online (SPO). This utility allows you upload images to SPO, where the image file source location is either an HTTP URI or Windows File Share. There are several configuration options for the utility, which allow you to control aspects of the image uploads, for example, you can configure if the utility should create and upload three versions of a profile picture (small, medium and large). The utility will also edit the user’s profile, to ensure the profile picture (s) uploaded are referenced by the user the profile.

The documentation below shows show to run and configure the utility, and then covers some frequently asked questions.


## RUNNING THE UPLOADER ##

When running the uploader, you will need to specify a minimum of three command line parameters. That being; a username and password of an SPO tenant admin (Office 365 admin), and the path to the configuration file which the uploader will use to control the upload process. E.g.

    ProfilePictureUploader.exe –SPOAdmin admin@contoso.onmicrosot.com –SPOAdminPassword pass@word1 –Configuration configuration.xml

There are two optional parameters, which are a username and password, which should be used if your image source location requires authentication, and you don’t want to connect to the source as the user account which is executing the ProfilePictureUploader executable. Note: if the source is an HTTP(s) Uri, then this will only work if the authentication method is NTLM or basic authentication, not forms authentication. 
The screen shot below shows the two examples for running this command.

![Console output](http://i.imgur.com/5wx33eX.png)

## CONFIGURATION OPTIONS ##
The configuration.xml file allows you to control the upload process, and is required. Below is a sample configuration.

![Configuration options](http://imgur.com/6meHVRW.png)

1.	tenantName – required. Office 365 tenant name. Used by the utility to connect to the correct SPO web service endpoints during the upload of images, and editing of user profile properties.
2.	pictureSourceCSV – required. This is the path to a CSV file which contains a mapping of SPO user to source image location. More information and examples are detailed in the next section.
3.	thumbs – required. This element determines if source image files should be uploaded as is, or scaled to create 3 sizes of each image file.
	- upload3Thumbs – required. If set to “false”, the utility will take the source image for a user as is, and upload to SPO. A single image file per user profile will be uploaded. The value of “createSMLThumbs” is irrelevant if “Upload3Thumbs” is set to “false”. If “upload3Thumbs” is set to true, the utility will upload 3 image files for each user profile. The value of “createSMLThumbs” will control the size of each of those 3 images. Most often “upload3Thumbs” will be set to true, but there are cases where you might just want the source image uploaded as is.	
	- createSMLThumbs – required. If set to true, the utility will use the source image, and create 3 different sized variations of the image, and upload those 3 images for each user profile. The sizes are, small – width of 48px, medium – width of 72px, and large, width of 200px. If set to false, the utility will use the source image, do no resizing of it, and upload it 3 times. Most often this parameter will be set to true, but there are cases where you might want it to be false.
4. targetLibraryPath - required. This option allows to specify a different name / URL for picture library where profile pictures will be uploaded. Needs to be used to support where MySite is in a language different from English.  
   default value, for English, is */User Photos/Profile Pictures* . For instance for Dutch this should be set to */User Photos//Profielafbeeldingen* instead.
5.	additionalProfileProperties – required. This section allows you to specify additional user profile properties, and their values, to be set when the utility runs. For example, you may want to turn off Exchange Online picture sync to SharePoint Online for all users where you upload an image, or set any other custom or built-in SPO user profile property to a value. Note: the utility will automatically set 2 user profile properties for you i.e. PictureURL, will be set to the path of the uploaded image (if multiple image uploaded, it is always set to the path of the medium sized image), and SPS-PicturePlaceholderState will be set to 0, to indicate to SPO to show the upload picture for a user profile. 
	- Property – not required. Can have multiple property elements. 
		- Name – required. This is the name of SPO user profile property
		- Value – required. This is the value you would like to set the profile property to
6.	logFile – required. This is used to control the output of logging while the utility runs.
	- path – required. The full path to where the log file should be created. If the file doesn’t exist, the utility will create one. If it does exist, it will append to the existing file.
	- enablelogging – required. If set to “false”, the utility will not write any output to the logfile.
	- loggingLevel – not used. Support for this attribute be added to a future update.
7.	uploadDelay – required. If set greater than 0, the utility will pause this time (in milliseconds) between user profile uploads to SPO. This is to prevent the SPO service thinking the utility is issuing a denial of service attack. Recommended to leave at 500ms (half a second).

## SOURCE CSV FILE EXAMPLES ##
The source CSV file maps SPO user profile to source image path. A sample CSV file is shown below.

![Source file examples](http://i.imgur.com/gT53PGX.png)

The first column is required, and must be the SPO User Name (in Office 365 this is the user name found in the users and groups section in the Office 365 admin center). The second column is the path to the source picture to upload for the user. From the sample above, you will see that file shares, local files and web locations are supported. 

**Important:** the utility always ignores the first row in the CSV file. In this example the column header names are the first row.

**Tip:** You can get a list of usernames from Office 365 using Windows Azure AD PowerShell and export them straight into a CSV file. Then, you can open the CSV file in Microsoft Excel, and use autocomplete features in Excel to complete the source picture column on mass (assuming you have a repeatable naming convention for your source image files).
The sample PowerShell command below takes all licensed users in the Office 365 tenant and exports the UPN (user name) and display name to a CSV file. You could use this as the starting point to your source CSV file.
Get-MSOLUser | Where-Object { $_.isLicensed -eq "TRUE" } | select userprincipalname, displayname | Export-CSV c:\temp\userlist.csv
The article linked here, describes how to the Get-MSOLUser command - http://msdn.microsoft.com/en-us/library/windowsazure/dn194133.aspx

# FAQ #

## My company is running Azure AD Sync. Will it take user profile pictures into SPO from our local AD?
Answer: Maybe

Azure AD Synchronization (DirSync) will synchronize the value for the AD attribute “thumbnailphoto” from a user object in AD to Azure AD (Office 365). However, this doesn’t mean it ends up in SharePoint Online, and in the scenarios where it does, it doesn’t mean you have a good looking high resolution photo for a user profile. 
The way the thumbnailphoto sync works is as follows. AD syncs to Azure AD (as part of DirSync), then Exchange Online gets a copy of the picture from Azure AD, then SharePoint Online gets a copy from Exchange Online. As you can quickly see, there will be a few instances where the sync of the thumbnailphoto will not work all the way into a SPO user profile e.g:
- 	You don’t have Exchange Online in your Office 365 tenant. You may just have SharePoint Online.
- 	You have Exchange Online in your tenant, but the user profile in question has their Exchange mailbox  on-premises. For thumbnailphoto sync to work to SPO, the user’s mailbox must be in Exchange Online.
- 	You have everything in place, but the picture quality in AD is poor because it is a small thumbnail image. The large profile picture in SPO (the one shown on a user’s profile page) is 200px wide. The user’s profile picture will look terrible when seen on this page, using the standard picture sync process.
For these reasons, and others you may have, you want to use this utililty to upload user pictures to SPO.

**For Your Interest:** you will notice a SPO user profile property called SPS-PictureExchangeSyncState. It is there to toggle the syncing (pulling) of pictures from Exchange Online to SharePoint Online (pulled by SharePoint Online).

**Note: **If you have Exchange Online available in your Office 365 tenant, but user mailboxes are on-premises, the thumbnail attribute does end up in Exchange Online for a user, it just doesn’t sync to SharePoint Online. If you would like to use the Exchange Online picture as a source for SPO, you can get the source image from Exchange Online for any user using their Rest API, which is a simple URL. E.g. https://outlook.office365.com/ews/Exchange.asmx/s/GetUserPhoto?email=user1@contoso.com&size=HR648x648
Where email is the email address of the user profile in question. You could therefore, in the source CSV file use the URL above, together with the source authentication command line parameters to copy images from Exchange Online to SharePoint Online.

## Can I use SharePoint on-premises as the source for profile pictures?
Answer: Yes

It is very likely that you would want to do this e.g. you have SP2010 on-premises today. You could have the source URL field in the CSV file point straight to the HTTP URL for the large image file for each user profile. Why the large file? Because SPO has different profile picture size requrements compared to SP2010, and pointing to the large image, you could have the utility create the 3 correct sizes and upload to SPO.
E.g. your source URL could look something like this for SharePoint 2010 – http://<mysitehost>/User Photos/Profile Pictures/<domain>_<username>_LThumb.jpg
Note: the utility will connect to the source image location, as the user account which is running the utlity, or, you can specify a username and password to connect to the source as, on the command line when running the tool.

## Can I use this utility to upload pictures to an on-premises SharePoint 2013 farm?
Answer: Not without adjusting the source code.

It is a minor adjustment to allow this utility to upload images to SharePoint 2013 on-premises. The utlity currently only support authentication against Office 365. However with a minor change to the authenticaiton logic, and path to SP web services and my site host, the tool would work.
Perhaps in a future relase, we could add a configuration flag to support on-premises.

## Can I use this utility to mass update user profile properties, and not upload image files?
Answer: Not without adjusting the source code

You would need to comment out the code which fetches images from source location and uploads to SPO. The code already exists to read the “additionalProfileProperties” section in the configuration file and set the values in SPO user profiles, so this would be used to perform your mass update requirement.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ProfilePictureUploader" />
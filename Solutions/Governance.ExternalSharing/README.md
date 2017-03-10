# Site Collection External Sharing #

### Summary ###

SharePoint Online provides an ability to share contents to external users.  This is a great feature for collaboration with your partners. However, the External Sharing can be enabled/disabled only in SharePoint Admin Portal or PowerShell which requires the user to be part of SharePoint Online administrators group. This creates a dependency on Tenant Administrators every time a Site Owner/Site Collection Administrator wants to either allow or disallow external sharing in their site collection. This application gives the privilege for a Site Owner and Site Collection administrators to change the external sharing settings from a site collection itself. 

### Features ###
- Empowers Site Owner and Site Collection Administrators to change external sharing settings.
- Display banner if the site is externally shared
- File system logging


### Applies to ###
-  Office 365 Multi-tenant (MT)
-  Office 365 Dedicated (D)

### Solution ###
Solution | Author(s)
---------|----------
Governance.ExternalSharing | Chandrasekar Natarajan

### Version history ###
Version  | Date | Comments
---------| -----| --------
.1  | February 29, 2016 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

**NOTICE THIS SOLUTION IS UNDER ACTIVE DEVELOPMENT**

# Solution Components #
The solution consists of two parts;

1.	A link to access a page in remote web application to change external sharing settings
2.	The web application that hosts a page for changing external sharing settings. 

### External Sharing Link ###

Only users who have site collection manage permission will be able to use this application.  Users with manage site collection permission can go to site settings of a site collection and they will see a link called “External Sharing” under “Site Collection Administration”. 

![External sharing link in the site settings page](http://i.imgur.com/7MIROCT.png)

### Remote web application  ###

When the user clicks on the “External Sharing” link, they will be redirected to a page hosted in an IIS server On-Prem or Azure.  This page provides the users with the options to change external sharing settings.


![UI of the external application](http://i.imgur.com/F489vY5.png)

# Solution configuration and setup #

### External Sharing Link ###
The link to change External sharing will be available under site settings -> Site collection administration of each site collection.  This link will be added automatically when the site is provisioned using Site Provisioning App.

![XML definition on getting hte UI link available](http://i.imgur.com/jBEGuhK.png)

### Remote web application setup  ###
For the External Sharing Web Application to interact with SharePoint Online, we need to create App Principals which is registered with Microsoft Azure Access Control Service (ACS).

#### Add-in Registration and Permissions ####
External Sharing Application requests permissions at Tenant, Site Collection, and User Profile level.  Also, the application uses App-only Policy as all Site Owners or Site Collection Administrators will not have permission at Tenant and User Profile level.  

![Add-in permissions](http://i.imgur.com/w4u9pQv.png)

### Web configuration file ###
Enter the Client ID and Secret values in External Sharing Web Application’s web.cofig file.  For security purpose, the values will be encrypted using Add-in Security tool – See section "Securing Add-in Principals" on how to use the tool.  

The web.config of the application contains encrypted ClientID, encrypted ClientSecret, Tenant Admin URL, and JavaScript file that contains the code for displaying the banner.


#  Application walkthrough  #
### External Sharing page ###

External Sharing page is used for changing the settings of external sharing of a site collection. 

The page displays the following;

-	Top navigation with current logged in user name
-	Top level Site Settings navigation option to go back to the site collection’s site settings page
-	The URL of the site collection
-	Options to change the settings of external sharing
-	Ok button to submit the change or cancel button to go back to site settings page.

### External Sharing settings ###
There are 2 settings available;

1. Allow external sharing 
2. Don’t allow external sharing

By default, the current external sharing settings for the site collection will be selected.  Only when the user opts to change the settings, the Ok button will be enabled. 

#### Allow external sharing ####

To enable external sharing, users need to choose the option “Allow external users who accept sharing invitations and sign in as authenticated users”. 

 ![UI for end users to allow external sharing](http://i.imgur.com/F489vY5.png)

When the user chooses to allow external sharing, two things happen;

- The site collection will be shared externally At the tenant level, the option to allow external users who accept sharing invitations and sign in as authenticated users will be enabled.

- A banner will be displayed in the Site collection and sites informing the users that this site can be shared with people outside of Contoso.  We make use of SharePoint Online's OOB status banner to display custom message. Refer externalsharing.js located under scripts folder.  Since this js will be used across site collections, deploy this file in CDN or at any root site collection level.

 ![Custom action to present status on allowing external sharing](http://i.imgur.com/qUeCQGp.png)


#### Don’t allow external sharing ####
To disable external sharing, users need to choose the option “Don’t allow sharing outside your organization”.  When the user chooses not to share the site externally, a message will be displayed (see picture below) informing about the details if the stop external sharing of the site collection.

![UI when external sharing is disabled from site settings](http://i.imgur.com/0nNsGn3.png)

When the user clicks Ok, two things happen;

1.	The site collection will not be shared externally - At the tenant level, the option to disallow external sharing will be enabled.
2.	The banner will be removed from the site collection.
  

# Securing Add-in Principals #

To protect the Add-in principles, most importantly the Client ID and Client Secret, we encrypt their values and store the encrypted values in the respective configuration files.  The application handles the decryption of the values. DPAPI functions encrypt and decrypt data using the Triple-DES algorithm. In addition to encryption and decryption, DPAPI handles key generation and protection. DPAPI can generate encryption keys that are unique either for a user making the call or the computer on which the program making the call runs.  The encryption can be scoped at machine or user level.  Using the encrypted value generated in one machine we can't decrypted the value in another machine, it has to be decrypted in the same machine. When an application calls the DPAPI encryption routine, it can specify an optional secondary entropy ("secret" bytes) that will have to be provided by an application attempting to decrypt data. 

![UI of windows form tool for encrypting add-in principals](http://i.imgur.com/oBMuD3X.png)

**Please look at Governance.AddInSecurity application in PNP Github for the source code.**

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.ExternalSharing" />
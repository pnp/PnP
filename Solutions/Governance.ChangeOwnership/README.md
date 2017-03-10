# Change Site Collection Ownership #

### Summary ###

The Change Site Collection Owner application is used for changing the Site Owner of a site collection in SharePoint Online by the current site owner or any site collection administrator.
Out of the box in SharePoint Online, the Site Owner can only be changed in SharePoint Admin Center or PowerShell which requires the user to be part of SharePoint Online administrators group.  This application gives the privilege for a Site Owner and Site Collection administrators to change the site collection ownership from a site collection itself. 

### Features ###
- Empowers Site Owner and Site Collection Administrators to change current site owner
- Choose site owner from only supported certain domain (for e.g. disallow external users from being a site owner)
- Multiple option to choose a site owner.  Sample has the options to choose 
 * Assign to myself
 * Assign to my manager
 * Assign to another site collection administrators
- Automatically set the new site owner's email in Access Request Settings for the site.
- Email notification to Site Owner and Site Collection Administrators when any change happens
- File system logging


### Applies to ###
-  Office 365 Multi-tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises (Low Trust) 


### Solution ###
Solution | Author(s)
---------|----------
Governance.ChangeOwner | Chandrasekar Natarajan

### Version history ###
Version  | Date | Comments
---------| -----| --------
.1  | February 25, 2016 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

**NOTICE THIS SOLUTION IS UNDER ACTIVE DEVELOPMENT**

# Solution Components #
The solution consists of two parts;

1.	A link to access a page in remote web application to change site collection ownership
2.	The web application that hosts a page for changing the site ownership. 

### Change Site Collection Ownership Link ###

Only users who have site collection manage permission will be able to use this application.  Users with manage site collection permission can go to site settings of a site collection and they will see a link called “Change Site Collection Ownership” under “Site Collection Administration”. 

![Change Site Collection Ownership Link in site settings page](http://i.imgur.com/NRz4eGh.png)

### Remote web application  ###

When users click on the “Change Site Collection Ownership” link, they will be redirected to a page hosted in an IIS server On-Prem or Azure.  This page provides an interface with the options to choose a new site owner.


![UI for the operations](http://i.imgur.com/KKSWdpH.png)

# Solution configuration and setup #

### Change Site Collection Ownership Link ###
The link to change site collection ownership will be available under site settings -> Site collection administration of each site collection.  This link will be added automatically when the site is provisioned using Site Provisioning App. 

![Custom action definition for the UI link](http://i.imgur.com/I3LLSvH.png)

### Remote web application setup  ###
For the Change Owner Web Application to interact with SharePoint Online, we need to create App Principals which is registered with Microsoft Azure Access Control Service (ACS).

#### Add-in Registration and Permissions ####
Change Owner Application requests permissions at Tenant, Site Collection, and User Profile level.  Also, the application uses App-only Policy as all Site Owners or Site Collection Administrators will not have permission at Tenant and User Profile level.  

![Permissions requested for the add-in](http://i.imgur.com/w4u9pQv.png)

### Web configuration file ###
Enter the Client ID and Secret values in Change Owner Web Application’s web.cofig file.  For security purpose, the values will be encrypted using Add-in Security tool – See section "Securing Add-in Principals" on how to use the tool.  

The web.config of the application contains encrypted ClientID, encrypted ClientSecret, Email template file location, Supported Domains and email settings.  You can add multiple supported domains separated by comma (for e.g. value="contoso.com,contosotest.onmicrosoft.com”)


#  Application walkthrough  #
### Change Site Collection Ownership Page ###

Change Site Collection Ownership page is used for choosing a new site owner.

The page displays the following;

-	Top navigation with current logged in user name
-	Top level Site Settings navigation option to go back to the site collection’s site settings page
-	Left navigation to browse through site collection giving great end user experience.
-	The URL of the site collection
-	Current site owner
-	Options to choose new site owner
-	Add site collection administrators
-	Ok button to submit the change or cancel button to go back to site settings page.

### Choosing a new site owner ###
There are 3 different options to choose a new site owner.  By default none of the options is choose and the “Ok” button will be disabled.  Only when the user chooses a new site owner, the user will be able to submit. 

#### Assign to myself ####
In this option, the user can choose themselves as a site owner.  When this option is chosen, it will display current user’s name which will be grayed out.   In case, the current owner is the same person as the logged in user, this option will be grayed out. 

![UI for the my self assigning](http://i.imgur.com/KKSWdpH.png)

#### Assign to my manager ####
This provides an option to choose user’s manager as a site owner.  If the user does not have any manager assigned in user profile, it will be grayed out – The user can’t select this option. 

![UI for the assigning to manager](http://i.imgur.com/eDdfikJ.png)

#### Assign to another site collection administrator ####
You can opt to choose one of the existing site collection administrators as a new site owner.  Choosing this option, displays a dropdown box that shows all current site collection administrators.  It doesn’t display any service accounts though.  The users will be shown only if their accounts belong to supported domains configured in web.config file, for e.g. you can restrict external/partners from being a site owner. 

If there aren’t any site collection administrators available, then the page displays a message asking users to add site collection administrators using Add Site Collection Administrators link.  

![UI for assigning to another site collection admin](http://i.imgur.com/vCMJgT1.png)

### Site Collection Administrators ###
The page also provides an option to add site collection administrators. 

## Submission ##
When a new owner is selected and submitted for change, few things takes place;

- The selected person will be assigned as a new site owner for this site collection
- In the Access Request settings section of the site collection, the newly selected owner's email address will be updated.
- An Email notification will be sent to new owner, old owner, and all Site Collection administrators informing them about the change in ownership.

![Email received by end users](http://i.imgur.com/zMAWksp.png)


# Securing Add-in Principals #

To protect the Add-in principles, most importantly the Client ID and Client Secret, we encrypt their values and store the encrypted values in the respective configuration files.  The application handles the decryption of the values. DPAPI functions encrypt and decrypt data using the Triple-DES algorithm. In addition to encryption and decryption, DPAPI handles key generation and protection. DPAPI can generate encryption keys that are unique either for a user making the call or the computer on which the program making the call runs.  The encryption can be scoped at machine or user level.  Using the encrypted value generated in one machine we can't decrypted the value in another machine, it has to be decrypted in the same machine. When an application calls the DPAPI encryption routine, it can specify an optional secondary entropy ("secret" bytes) that will have to be provided by an application attempting to decrypt data. 

![Windows form application for encrypting add-in principals](http://i.imgur.com/oBMuD3X.png)

**Please look at Governance.AddInSecurity application in PNP Github for the source code.**

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.ChangeOwnership" />
# Readme file for UserProfile.MIMSync toolset #

### Summary ###
This solution provides a set of powershell commandlets to set-up Microsoft Identity Manager sync engine with SharePoint and to kick off sync on-demand. Commandlets included in this tool should help two scenarios:
1.  Customers setting up SharePoint 2016 on-prem farm from scratch can use the tool to set-up initial mapping for default user profile properties to corresponding properties in Active Directory. 
2.  Customer upgrading from SharePoint 2010 or 2013 on-prem, where they used SharePoint's in-product sync solution for syncing data from Active Directory to user profile properties can use the tool to replicate the same mappings in a Microsoft Identity Manager based sync.

Detailed step-wise documentation is provided in the repository.    

### Applies to ###
-  SharePoint 2016 on-premises - new deployments or upgrades from SharePoint 2010 or SharePoint 2013

### Prerequisites ###
Documentation in the repository calls out prerequisites 

### Solution ###
Solution | Author(s)
---------|----------
MIM-Sync Tools | Craig Martin 

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.0  | March 21st 2014 (to update/remove)| comment
1.0  | November 6th 2013 (to update) | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# For detailed description of scenarios and steps, please refer to following documentation in the repository.#

## Installing Microsoft Identity Manager components needed for User Profile Sync in SharePoint 2016 ##
Use document "Install Microsoft Identity Manager for User Profiles in SharePoint Server 2016"

## Deploying a new SharePoint 2016 farm and setting up sync using Microsoft Identity Manager from scratch ##
Use document "Set up User Profile Sync in a new SharePoint Server 2016 farm using Microsoft Identity Manager"

## Setting up User Profile Sync in an upgraded SharePoint 2016 farm using sync config from previous version ##
Use document "Use Microsoft Identity Manager to set-up User Profile Sync in an upgraded SharePoint  Server 2016 farm using sync configuration from previous version"





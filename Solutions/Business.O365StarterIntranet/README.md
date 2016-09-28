# PnP Starter Intranet for Office 365/SharePoint Online  #

### Summary ###

Intranet projects shouldn’t have to reinvent the wheel every time for basic features (like navigation or multilingualism).
This solution aims to provide the fundamental building blocks of a common intranet solution with SharePoint Online/Office 365 through a lightweight client side solution using the latest web stack development tools and frameworks.

Here is what you get with this sample:
- A basic page creation experience with common layouts for static page, home page and news.
- Common intranet navigation menus like main menu, header links, footer, contextual menu and breadcrumb based on taxonomy.
- A basic translation system for multilingual sites (pages and UI).
- A search experience including results with preview.
- A mobile intranet using SharePoint Online.

<p align="center">Home Page</p>
<p align="center">
  <img width="600" 
  src="http://thecollaborationcorner.com/wp-content/uploads/2016/09/homepage.png"/>
  
</p>

<p align="center">News page (Desktop)</p>
<p align="center">
  <img width="600" 
  src="http://thecollaborationcorner.com/wp-content/uploads/2016/08/o365_starterintranet_news.png"/>
</p>

<p align="center">News page (Mobile)</p>
<p align="center">
  <img width="300" src="http://thecollaborationcorner.com/wp-content/uploads/2016/08/o365_starterintranet_news_mobile.png">
</p>

This solution is implemented using:

- TypeScript (for the code structure and definitions)
- Webpack (for application bundling and packaging)
- PnP JS Core library (for REST communications with SharePoint Online)
- PnP Remote Provisioning engine and PnP PowerShell cmdlets (for SharePoint site configuration and artefacts provisioning)
- Knockout JS (for application behavior and UI components)
- Bootstrap (for mobile support)
- Office UI Fabric (for icons, fonts and styles)
- Node JS (for dependencies management with npm)

The entire solution is "site collection self-contained" to not interfer with the global tenant configuration (especially taxonomy and search configuration). It allows you to deploy this solution safely in your tenant.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)

### Documentation #

A blog series will come soon to explain how we did this solution in details. Here is the provided plan:

* [Part 1: Functional overview (How to use the solution?)](http://thecollaborationcorner.com/2016/08/22/part-1-functional-overview-how-to-use-the-solution/)
* [Part 2: Frameworks and libraries used (How it is implemented?)](http://thecollaborationcorner.com/2016/08/25/part-2-frameworks-and-libraries-used-how-it-is-implemented)
* [Part 3: Design and mobile implementation](http://thecollaborationcorner.com/2016/08/29/part-3-design-and-mobile-implementation)
* [Part 4: The navigation implementation](http://thecollaborationcorner.com/2016/08/31/part-4-the-navigation-implementation)
* [Part 5: Localization](http://thecollaborationcorner.com/2016/09/02/part-5-localization)
* [Part 6: The search implementation](http://thecollaborationcorner.com/2016/09/08/part-6-the-search-implementation)

### Set up your environment ###

Before starting, you'll need to install some prerequisites:

- Install the latest release of [PnP PowerShell cmdlets SharePointPnPPowerShellOnline](https://github.com/OfficeDev/PnP-PowerShell/releases) or a version compatible with the 201605 PnP schema version (the solution uses the september 2016 release).
- Install Node.js on your machine https://nodejs.org/en/
- Install the 'typings' Node JS client (`npm install typings --global`)
- Install the 'webpack' Node JS client (`npm install webpack --global`)
- Go to the ".\App" folder and install all dependencies listed in the package.json file by running the `npm install` cmd 
- Install TypeScript typings by running the "`typings install`" cmd from the ".\App" folder.
- Check if everything is OK by running the "`webpack`" cmd from the ".\App" folder. We shouldn't any errors here (just warnings)
- Create a site collection with the publishing template. We don't manage the site collection creation process in the deployment procedure because it takes too much time with SharePoint Online.

<p align="center">
  <img width="600" src="http://thecollaborationcorner.com/wp-content/uploads/2016/08/create-new-site-collection.png">
</p>

- Ensure your taxonomy term store has both "French" and "English" working languages selected (you need to be a term store administrator to do this).

<p align="center">
  <img width="600" src="http://thecollaborationcorner.com/wp-content/uploads/2016/09/workinglanguages.png">
</p>

### Solution ###
Solution                | Author(s)
------------------------|----------
Business.O365StarterIntranet | Franck Cornu

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0 | August 19th 2016 | Initial release
1.1 | Sept 21st 2016 | Added carousel component + small fixes

### Disclaimer ###

THIS CODE IS PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

----------

# Installation #

- Download the PnP source code as ZIP from GitHub and extract it to your destination folder
- Set up your environment as described above
- On a remote machine (basically, where PnP cmdlets are installed), start new PowerShell session as an **administrator** an call the `Deploy-Solution.ps1` script with your parameters like this:

```csharp
$UserName = "username@<your_tenant>.onmicrosoft.com"
$Password = "<your_password>"
$SiteUrl = "https://<your_tenant>.sharepoint.com/sites/<your_site_collection>"

Set-Location "<your_pnp_installation_folder>\Solutions\Business.O365StarterIntranet"

$Script = ".\Deploy-Solution.ps1" 
& $Script -SiteUrl $SiteUrl -UserName $UserName -Password $Password -IncludeData

```
- Use the "`-Prod`" switch parameter for the `Deploy-Solution.ps1` script to use a production bundled version of the code.
- Use the "`-IncludeData`" switch parameter to provision sample data.

# Troubleshooting #

- __Issue with the extensibility provider__: If you encounter some troubles with the provider assembly during the deployment (or the default column values are not applied in the "Pages" library), just rebuild the Visual Studio solution (in release mode) by opening the **Intranet.Providers.sln** file. Also, make sure the PnP PowerShell cmdlets use the same version of the PnP Core assembly as the extensibility provider (check the nuget package reference).

----------
<img  src="https://telemetry.sharepointpnp.com/pnp/solutions/Business.O365StarterIntranet" />
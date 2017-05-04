# PnP Starter Intranet for SharePoint 2013/2016 and SharePoint Online  #

### Summary ###

Intranet projects shouldn’t have to reinvent the wheel every time for basic features (like navigation or multilingualism).
This solution aims to provide the fundamental building blocks of a common intranet solution with SharePoint 2013/2016 on-premises and SharePoint Online through a lightweight client side solution using the latest web stack development tools and frameworks.
This solution leverages the SharePoint classic publishing infrastructure feature.

Here is what you get with this sample:
- A basic page creation experience with common layouts for static page, home page and news.
- Common intranet navigation menus like main menu, header links, footer, contextual menu and breadcrumb based on taxonomy.
- A basic translation system for multilingual sites (pages and UI).
- A search experience including results with preview.
- A mobile intranet using SharePoint 2013/2016 on-premises or SharePoint Online.

This solution is implemented using:

- TypeScript (for the code structure and definitions)
- Webpack (for application bundling and packaging)
- PnP JS Core library (for REST communications with SharePoint)
- PnP Remote Provisioning engine and PnP PowerShell cmdlets (for SharePoint site configuration and artefacts provisioning)
- Knockout JS (for application behavior and UI components)
- Bootstrap (for mobile support)
- Node JS (for dependencies management with npm)
- Bootstrap & Font Awesome(for overall branding)

The entire solution is "site collection self-contained" to not conflict with the global tenant/farm configuration (especially taxonomy and search configuration). It allows you to deploy this solution safely in your farm.

### Applies to ###
- SharePoint 2013/2016 on-premises
- SharePoint Online (Office 365)

### Set up your environment ###

Before starting, you'll need to setup tour environment:

- Install at latest release of [PnP PowerShell cmdlets SharePointPnPPowerShellXXX](https://github.com/OfficeDev/PnP-PowerShell/releases) according to your SharePoint version.
- Install Node.js on your machine https://nodejs.org/en/ (v6.10.1)
- Install the 'webpack' Node JS client (`npm i webpack -g`). This sample uses webpack v2.
- Go to the ".\app" folder and install all dependencies listed in the package.json file by running the `npm i` cmd 
- Check if everything is OK by running the "`webpack`" cmd from the ".\app" folder. You shouldn't see any errors here.

<table style="margin: 0px auto;">
  <tr>
    <th>
        <p align="center">
            <img src="./images/pnp-powershell-version.png"/>
        </p>
    </th>
    <th>
        <p align="center">
            <img src="./images/pnp-nuget-version.png"/>
        </p>
    </th>
  </tr>
</table>

- Create a site collection with the **publishing template**.

<p align="center">
  <img width="400" src="./images/new-sitecollection.png">
</p>

- Ensure your taxonomy term store has both "French" and "English" working languages selected (you need to be a term store administrator to do this).

<p align="center">
  <img width="600" src="./images/taxonomy-languages.png">
</p>

- For on-premises deployments, make sure the managed metadata service application is the default storage location for column specific term sets.

<p align="center">
  <img width="600" src="./images/mms-proxy-setting.png">
</p>

### Solution ###
Solution                | Author(s)
------------------------|----------
Business.StarterIntranet | Franck Cornu (MVP Office Development at aequos) - Twitter @FranckCornu 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0 | August 19th 2016 | <ul style="list-style: none"><li>Initial release</li></ul>
1.1 | September 21st 2016 | <ul style="list-style: none"><li>Added carousel component + miscellaneous fixes</li></ul>
1.2 | January 31st 2016 |  <ul style="list-style: none"><li>Added the support of SharePoint 2013 and 2016 on-premises</li><li>Updated to TypeScript 2.1.5 and PnP Js Core 1.0.6</li></ul>
1.3 | April 9th 2017 | <ul style="list-style: none"><li>Added the support of event pages</li><li>News display templates for news</li><li>Added the support for a bot web chat connected to the Bot Framework</li><li>Bug fixes + migration to webpack 2.0 and sp-pnp-js 2.0</li></ul>

# Installation #

- Download the PnP source code as ZIP from GitHub and extract it to your destination folder
- Set up your environment as described above
- On the newly created site collection, upload and activate the Bind Tuning sandbox solution to deploy the master pages and page layouts
- On a remote machine (basically, where PnP cmdlets are installed), start new PowerShell session as an **administrator** an call the `Deploy-Solution.ps1` script with your parameters like this:

```csharp
$UserName = "<your_username>"
$Password = "<your_password>"
$SiteUrl = "https://<your_site_collection>"

Set-Location "<your_installation_folder>"

$Script = ".\Deploy-Solution.ps1" 
& $Script -SiteUrl $SiteUrl -UserName $UserName -Password $Password -IncludeData

```
- Use the "`-Prod`" switch parameter for the `Deploy-Solution.ps1` script to use a production bundled version for the JavaScript code.
- Use the "`-IncludeData`" switch parameter to provision sample data (carousel and links).

# Post-installation steps #

## Configure column default values in the Pages library ##
Right after the deployment, you have to complete some manual steps to set up default column value settings as follow (the extensibilty provider has been removed). These information are used for the news and event webparts on the home page to filter archive page (the "See all news/events" links).

Library/Folder | Column | Value
---------| -----| --------
Pages/ | Content Type | Page 
Pages/News | Content Type | News 
Pages/News | Site Map Position | News 
Pages/Events | Content Type | Event 
Pages/Events | Site Map Position | Events 
Documents/ | Content Type | Document 

## Configre the QnA bot ##



### Disclaimer ###

THIS CODE IS PROVIDED AS IS WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

----------
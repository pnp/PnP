<p align="center">
  <img width="400" src="./images/logo_intranet.png">
</p>

# International Development Research Centre (IDRC) - Intranet solution for SharePoint 2013 #

### Summary ###

The solution implements the publishing intranet for IDRC based on the [PnP starter intranet](https://github.com/SharePoint/PnP/tree/master/Solutions/Business.StarterIntranet) for SharePoint 2013 and the [SPC BindTuning theme](http://bindtuning.com/cms/sharepoint/sharepoint-2013/theme/SPC/page/Home/customize).

This solution is implemented using:

- TypeScript (for the code structure and definitions)
- Webpack (for application bundling and packaging)
- PnP JS Core library (for REST communications with SharePoint)
- PnP Remote Provisioning engine and PnP PowerShell cmdlets (for SharePoint site configuration and artefacts provisioning)
- Knockout JS (for application behavior and UI components)
- Bootstrap (for mobile support)
- Node JS (for dependencies management with npm)
- Bind Tuning SPC theme for SharePoint 2013 (for overall branding)

The entire solution is "site collection self-contained" to not conflict with the global tenant/farm configuration (especially taxonomy and search configuration). It allows you to deploy this solution safely in your farm.

### Applies to ###
- SharePoint 2013 on-premises

### Tested versions ###

Here are the following versions of PnP and SharePoint used for this solution:

PnP PowerShell cmdlets version (All SharePoint versions)| PnP NuGet package version (All SharePoint versions) |SharePoint 2013 tested version(s) 
---------|---------|---------
<ul style="list-style: none"><li>2.12.1702.0 (February 2017)</li></ul> | <ul style="list-style: none"><li>2.12.1702.0 (February 2017)</li></ul> | <ul style="list-style: none"><li>15.0.4893.1000 (January 2017 CU)</li><li>15.0.4867.1000 (October 2016 CU)</li></ul>

### Set up your environment ###

Before starting, you'll need to setup tour environment:

- Install at least the February 2017 release of [PnP PowerShell cmdlets SharePointPnPPowerShellXXX](https://github.com/OfficeDev/PnP-PowerShell/releases) for SharePoint 2013
- Install Node.js on your machine https://nodejs.org/en/ (v6.10.1)
- Install the 'webpack' Node JS client version 1.14.0 (`npm i webpack@1.14.0 -g`)
- Go to the ".\app" folder and install all dependencies listed in the package.json file by running the `npm i` cmd 
- Check if everything is OK by running the "`webpack`" cmd from the ".\app" folder. You shouldn't see any errors here.
- According to the targeted SharePoint version, build the extensibility provider Visual Studio solution with the corresponding PnP NuGet package (the deployment script uses the *Debug* bin folder by default). Be careful, the PnP NuGet package version **must be the same** as the PnP PowerShell one. Before adding a new NuGet package, make sure your removed older references (remove the old *Debug* folder as well).

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
IDRC.Intranet | Franck Cornu (Aequos)

### Version history ###
Version  | Date | Comments
---------| -----| --------
0.1 | 4th March 2017 | <ul style="list-style: none"><li>Initial release with adapated BindTuning theme</li></ul>

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

Right after the deployment, you have to complete some manual steps to set up default column value settings as follow. These information are used for the news and event webparts on the home page to filter archive page (the "See all news/events" links).

Library/Folder | Column | Value
---------| -----| --------
Pages/ | Content Type | Page 
Pages/News | Content Type | News 
Pages/News | Site Map Position | News 
Pages/Events | Content Type | Event 
Pages/Events | Site Map Position | Events 
Documents/ | Content Type | Document 


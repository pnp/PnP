# PowerShell Installer #

This PowerShell script will install the application to a SharePoint 2013 or SharePoint Online site.

## Prerequisite ##

The installer requires the OfficeDev PnP PowerShell Commands. These can be found in the
[PnP Github repo](https://github.com/OfficeDev/PnP). The related wiki is [here](https://github.com/OfficeDev/PnP/wiki).

1. Ensure you have the following installed:
  * Visual Studio 2013
  * [WiX Toolset](http://wix.codeplex.com/)
  * [Windows Management Framework 4.0](http://www.microsoft.com/en-us/download/details.aspx?id=40855)
		

2. Fork the Github repo following [these instructions](https://github.com/OfficeDev/PnP/wiki/Setting-up-your-environment)
	
3. Open the project ...\Repos\PnP\Solutions\PowerShell.Commands\OfficeDevPnP.PowerShell.sln and build it. It should open the dist folder in Windows Explorer; run the Install.ps1 script and restart PowerShell to enable the commands.

## Usage ##



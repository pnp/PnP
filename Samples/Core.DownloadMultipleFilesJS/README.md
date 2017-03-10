# Download Multiple Files using JavaScript #

### Summary ###
This sample shows how to add download multiple files functionality to SharePoint document libraries.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
Core.DownloadMultipleFilesJS | Antons Mislevics (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 2nd 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


# Overview #
This sample demonstrates how to add download multiple files functionality to SharePoint document libraries using JavaScript.
Two new buttons are added to the ribbon:

- Download All – initiates separate downloads for selected files (including files in subfolders);
- Download All as Zip – generates ZIP file containing all files and subfolders, and initiates download.

![Document library view with two custom buttons](http://i.imgur.com/xAYoQ6F.png)

This functionality is built using JavaScript only approach. Obviously, this brings some limitations, as ZIP files are generated in memory on the client machine. However, it allows the customers having no app hosting environment to introduce this functionality. In addition, comparing to provider-hosted model (for example, on Azure), it allows to avoid additional charges for outgoing traffic.

The sample relies on the following 3rd party libraries:

- [FileSaver.js](https://github.com/eligrey/FileSaver.js/)
- [jQuery](http://jquery.com/)
- [multi-download](https://github.com/sindresorhus/multi-download)
- [zip.js](http://gildas-lormeau.github.io/zip.js/)

# Running the sample #
The sample is implemented as a console application that automates deployment. The following steps must be completed in order to run the sample:

1. Create new site collection based on standard Team site template.
2. Open Visual Studio project and update values for `siteUrl`, `username` and `password` variables in `Program.cs`.
3. Run the project.
4. Check new ribbon buttons in document libraries.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.DownloadMultipleFilesJS" />
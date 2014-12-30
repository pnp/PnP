# Configure SharePoint Provider-Hosted Apps for Distribution

### Summary ###

This page explains issues that may arise when sharing a SharePoint Provider-Hosted application with other developers or when obtaining a copy from a source control system such as Team Foundation Server, Git or Visual Studio Online.


# Configure SharePoint Provider-Hosted Apps for Distribution

All SharePoint Provider-Hosted apps created using Visual Studio 2013 include a NuGet package that adds SharePoint-specific code and references to the web application that serves as the RemoteWeb for the SharePoint app. 

The NuGet package added to the web application project by the Office Developer Tools in Visual Studio is not present in the NuGet package registry and therefore attempts to perform a NuGet package restore will fail because it cannot find a matching package.

## Understanding the Problem ##

The **Office Developer Tools for Visual Studio 2013**, version 12.0.31105, adds a NuGet package to web applications created as the RemoteWeb for SharePoint Provider-Hosted apps. This package, the **App for SharePoint Web Toolkit**, adds the following things to the web project:

- Assemblies & references to the SharePoint Client-Side Object Model (CSOM) assemblies
- A code file `TokenHelper.cs` that assists in the authentication process for apps.
- A code file `SharePointContext.cs` that helps in creating and maintaining a SharePoint context within the web application.

The way Visual Studio works is that it, or addins, typically contain a local copy of the NuGet package so developers do not always have to be connected to the internet to download the NuGet packages. The package that the tools include has an ID of **AppForSharePoint16WebToolkit**.

When projects are committed to source control, typically the packages are not included as part of the commit because they can add a lot of extra storage space demands and unnecessarily increase the size of a package when sharing it with other developers. Therefore one of the first tasks developers do after getting a copy of the project from source control is to run [NuGet package restore](http://docs.nuget.org/docs/reference/package-restore).

The challenge is that a package with the same ID does not exist in the NuGet package registry; there is no package with an ID of **AppForSharePoint16WebToolkit**. Instead the exact same package was added to the NuGet package registry as **[AppForSharePointWebToolkit](www.nuget.org/packages/AppForSharePointWebToolkit)** (*notice the lack of the '16' in the ID*).

## Preparing a SharePoint Provider-Hosted App Project for Source Control / Distribution ##

Until the Office Developer Tools for Visual Studio 2013 are updated to fix this issue, it is recommended to alter the project prior to committing to your source control system, regardless if you are using Team Foundation Server, Visual Studio Online, Git or any other solution.

After creating the project, look within the project's `packages.config` file and search for a package with an ID of **AppForSharePoint16WebToolkit**. The safest way to update the project is to uninstall & then reinstall the package.

Open the **Package Manager Console** in Visual Studio and enter the following to uninstall the package:

  ````powershell
  PM> Uninstall-Package -Id AppForSharePoint16WebToolkit
  ````

  > If the uninstall throws an error about not finding the package, simply remove the package reference from the `packages.config` file manually & save your changes.

Now, install the public version of the same NuGet package from the public registry:

  ````powershell
  PM> Install-Package -Id AppForSharePointWebToolkit
  ````

----------

### Related links ###
- [NuGet: App for SharePoint Web Toolkit](http://www.nuget.org/packages/AppForSharePointWebToolkit)
- [NuGet: Package Restore](http://docs.nuget.org/docs/reference/package-restore)


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Version history ###
Version  | Date | Comments
---------| -----| --------
0.1  | December 31, 2014 | First draft



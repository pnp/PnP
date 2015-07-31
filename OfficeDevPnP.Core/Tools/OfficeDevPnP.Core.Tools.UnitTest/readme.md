# PnP Build and Test automation #

### Summary ###
This project contains the build scripts and build and test extensions used to automate the building and testing of PnP

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
Git needs to be installed, see documentation for details

### Solution ###
Solution | Author(s)
---------|----------
OfficeDevPnP.Core.Tools.UnitTest | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | July 4th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Purpose #
The purpose of this project is to automate testing of PnP against multiple environments using multiple configurations. In the current setup this tool is used to execute the PnP unit tests against SharePoint Online (Office 365 MT) and SharePoint 2013 on-premises. For each environment the unit test execution in running for 2 configurations: username + password and app-only.

# How to use #
Below are the steps needed to get this solution working.

## Copy needed files to the build server ##
Following files are needed:
- Copy the output from the release build to a location (e.g. folder c:\pnpunittestrunner) on the server that's running the build automation. This should give you OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions.dll and Microsoft.VisualStudio.TestPlatform.ObjectModel.dll
- Copy the mastertestconfiguration sample.xml file and rename it to mastertestconfiguration.xml. Update this file to match your environment
- Copy the PnPCore.targets file 
- Copy the supporting nuget.exe which is needed to pull down the needed nuget packages before we do the build

## Create a .bat file that runs msbuild ##
A possible bat file can be the following (is also copied as part of the build output)

```batch
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnPremCred /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnPremAppOnly /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnlineCred /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnlineAppOnly /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /target:PushResultsToGitHub
```

## Copy the OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions.dll to the VS extensions folder ##
We're using a custom VS test log writer that writes output to MD. This log writer needs to be copied to `C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions`. Replace the Visual Studio folder with the version you're using.

## Update the PnPCore.Targets file to suit your needs ##
The below sections need to be adjusted to match your environment:

```XML
<!-- PnP Repo information -->
<PropertyGroup Label="PnP">
  <PnPRepo>c:\pnpbuild</PnPRepo>
  <PnPRepoUrl>https://github.com/OfficeDev/PnP.git</PnPRepoUrl>
</PropertyGroup>

<!-- Unit test information-->
<PropertyGroup Label="Test information">
  <ConfigurationPath>C:\pnpunittestrunner</ConfigurationPath>
  <PnPExtensionsAssembly>$(ConfigurationPath)\OfficeDevPnP.Core.Tools.UnitTest.PnPBuildExtensions.dll</PnPExtensionsAssembly>
  <ConfigurationFile>mastertestconfiguration.xml</ConfigurationFile>
  <TestResultsPath>$(PnPRepo)temp</TestResultsPath>
  <VSTestExe>C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe</VSTestExe>
  <VSTestExtensionPath>C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions</VSTestExtensionPath>
</PropertyGroup>
```

These are the important parameters to change:
- **PnPRepo**: this defines where the PnP repo will be pulled down. The build scripts will use a separate copy, **so please do not put your working PnP fork/clone here**
- **ConfigurationPath**: this is the folder in which you've copied all the files needed for the test automation

## Setup git ##
The build script requires git to be present, hence git needs to be installed. The tested version is git for windows which can be fetched from here: http://msysgit.github.io/. If you want to push back changes to the PnP repo than ensure git is properly configured.

## Create a scheduled task ##
Final step is creating a scheduled task that executes the created bat file on a regular basis.


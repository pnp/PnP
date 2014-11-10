@echo off
echo Packaging SharePoint Client Components SDK files
if not exist .\Packages\NUL mkdir Packages
.\Nuget.exe pack SharePoint.ClientComponents.15.nuspec -OutputDirectory .\Packages 
.\Nuget.exe pack SharePoint.ClientComponents.16.nuspec -OutputDirectory .\Packages
.\NuGet.exe pack OfficeDevPnP.Core.nuspec -OutputDirectory .\Packages 
echo.
echo To upload to Nuget:
echo   .\Nuget.exe SetApiKey 'API-Key'
echo   .\Nuget.exe Push .\Packages\SharePoint.ClientComponents.15.0.4641.1011.nupkg
echo   .\Nuget.exe Push .\Packages\SharePoint.ClientComponents.16.0.3104.1200.nupkg
echo   .\Nuget.exe Push .\Packages\OfficeDevPnP.Core.0.5.1110.0.nupkg

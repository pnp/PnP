@echo off
echo Packaging SharePoint Client Components SDK files
if not exist ..\Packages\NUL mkdir ..\Packages
REM ..\Assemblies\Nuget.exe pack SharePoint.ClientComponents.v15.nuspec -OutputDirectory ..\Packages 
REM ..\Assemblies\Nuget.exe pack SharePoint.ClientComponents.v16.nuspec -OutputDirectory ..\Packages
..\Assemblies\NuGet.exe pack OfficeDevPnP.Core.v15.nuspec -OutputDirectory ..\Packages 
..\Assemblies\NuGet.exe pack OfficeDevPnP.Core.v16.nuspec -OutputDirectory ..\Packages 
echo.
echo NOTE: Nuspec files are not automatically updated.
echo       Make sure they have correct version numbers, etc.
echo.
echo To upload to Nuget:
echo   Nuget.exe SetApiKey 'API-Key'
REM echo   Nuget.exe Push .\Packages\SharePoint.ClientComponents.15.0.4641.1011.nupkg
REM echo   Nuget.exe Push .\Packages\SharePoint.ClientComponents.16.0.3104.1200.nupkg
echo   Nuget.exe Push .\Packages\OfficeDevPnP.Core.0.5.1110.0.nupkg
echo   Nuget.exe Push .\Packages\OfficeDevPnP.Core.0.6.1110.0.nupkg

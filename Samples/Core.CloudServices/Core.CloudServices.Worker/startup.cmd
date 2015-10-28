@ECHO off

if "%EMULATED%"=="true" goto :EOF
 
ECHO "Starting SharePoint CSOM  + SIA Installation" >> log.txt
ECHO "Installing the SharePoint CSOM library" >> log.txt
msiexec.exe /I "sharepointclientcomponents_x64.msi" /qn
"Installing Microsoft Online Services Sign In Assistant" >> log.txt
msiexec.exe /I "msoidcli_64bit.msi" /qn
ECHO "Completed SharePoint CSOM + SIA Installation" >> log.txt

ECHO "Execute additional PowerShell based tasks" >> log.txt
powershell -command "Set-ExecutionPolicy Unrestricted" >> log.txt
powershell .\startuptasks.ps1 >> log.txt

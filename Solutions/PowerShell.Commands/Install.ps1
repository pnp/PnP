$source = $PSScriptRoot;

$programFilesFolder = [environment]::getfolderpath("ProgramFilesX86");

$PnPRoot = "$programFilesFolder\OfficeDevPnP";

$ModuleHome = "$PnPRoot\PowerShell\Modules\OfficeDevPnP.PowerShell.Commands"

New-Item -Path $ModuleHome -ItemType Directory -Force

Write-Host "Copying files from $source to $ModuleHome" 
Copy-Item "$source\*.dll" -Destination "$ModuleHome"
Copy-Item "$source\*help.xml" -Destination "$ModuleHome"
Copy-Item "$source\*.psd1" -Destination "$ModuleHome"
Copy-Item "$source\*.ps1xml" -Destination "$ModuleHome"

$CurrentValue = [Environment]::GetEnvironmentVariable("PSModulePath", "Machine")
if($CurrentValue.Contains($PnPRoot) -ne $true)
{
	[Environment]::SetEnvironmentVariable("PSModulePath", $CurrentValue + ";$PnPRoot", "Machine")
}
Write-Host "Restart PowerShell to make the commands available."
param ([string]$config, [string]$target)

$PSModuleHome = "$Home\Documents\WindowsPowerShell\Modules\OfficeDevPnP.PowerShell.Commands"
New-Item -Path $PSModuleHome -ItemType Directory -Force

Write-Host "Copying files from $target to $PSModuleHome"
Copy-Item "$target\*.dll" -Destination "$PSModuleHome"
Copy-Item "$target\*help.xml" -Destination "$PSModuleHome"
Copy-Item "$target\ModuleFiles\*.psd1" -Destination  "$PSModuleHome"
Copy-Item "$target\ModuleFiles\*.ps1xml" -Destination "$PSModuleHome"
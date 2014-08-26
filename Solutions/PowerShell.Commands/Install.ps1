$ModuleHome = "$Home\Documents\WindowsPowerShell\Modules\OfficeDevPnP.PowerShell"
New-Item -Path $ModuleHome -ItemType Directory -Force

Write-Host "Copying files from $target to $PSModuleHome"
Copy-Item "$target\*.dll" -Destination "$PSModuleHome"
Copy-Item "$target\*help.xml" -Destination "$PSModuleHome"
Copy-Item "$target\*.psd1" -Destination  "$PSModuleHome"

Write-Host "Restart PowerShell to make the commands available."
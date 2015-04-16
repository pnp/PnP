# Sample tasks that can be additionally configured on the Azure host

#Disable IE IEC
#$AdminKey = "HKLM:\SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}" 
#$UserKey = "HKLM:\SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A8-37EF-4b3f-8CFC-4F3A74704073}"
#Set-ItemProperty -Path $AdminKey -Name "IsInstalled" -Value 0     
#Set-ItemProperty -Path $UserKey -Name "IsInstalled" -Value 0     

#Add sharepoint.com to local intranet sites
#New-Item -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\sharepoint.com"
#New-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\sharepoint.com" -Name https -PropertyType DWord -Value 1

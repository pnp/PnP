
 #
 # By default time out setting is 90 seconds, which might not be enough
 # for site collection creation. This setting can be controlled from the  
 # SPWebApplication.ClientCallableSettings. 
 #
 # This script will increase the setting to 5 minutes to ensure that
 # site collection creation is successful. 
 #
 $webAppUrl = "http://dev.contoso.com"
 $timeoutInMinutes = 5;
  
 $snapin = Get-PSSnapin | Where-Object {$_.Name -eq 'Microsoft.SharePoint.Powershell'}
 if ($snapin -eq $null) 
 {
     Write-Host "Loading SharePoint Powershell Snapin"
     Add-PSSnapin "Microsoft.SharePoint.Powershell"
 }

# Get web application
$wa = Get-SPWebApplication -Identity $webAppUrl
# Increase time out for CSOM calls - by default this is 90 seconds
$wa.ClientCallableSettings.ExecutionTimeout = [System.Timespan]::FromMinutes($timeoutInMinutes);
$wa.Update();

# Output current setting
$wa = Get-SPWebApplication -Identity $webAppUrl
$wa.ClientCallableSettings


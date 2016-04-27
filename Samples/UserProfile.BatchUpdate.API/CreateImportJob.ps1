<# Disclaimer

 Microsoft provides programming examples for illustration only, without warranty either expressed or
 implied, including, but not limited to, the implied warranties of merchantability and/or fitness 
 for a particular purpose. 
 
 This sample assumes that you are familiar with the programming language being demonstrated and the 
 tools used to create and debug procedures. Microsoft support professionals can help explain the 
 functionality of a particular procedure, but they will not modify these examples to provide added 
 functionality or construct procedures to meet your specific needs. if you have limited programming 
 experience, you may want to contact a Microsoft Certified Partner or the Microsoft fee-based consulting 
 line at (800) 936-5200. 

# User profile bulk API - Create Import Job # 
# 4th of Jan, 2016 - Release v1.0

# Author(s)
Vesa Juvonen (Microsoft)

# Notes
Support for this script requires that you have installed update SharePoint Online SDK/CSOM redistributable
with minimum version of 4622.1208 to the computer where the script is executed. 
Download redistributable package from https://www.microsoft.com/en-us/download/details.aspx?id=35585

#>

# Load assemblies to PowerShell session - Will try to resolve tenant dll status automatically, if possible
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint.Client")
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint.Client.Runtime")
$a = [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.Online.SharePoint.Client.Tenant")
if( !$a ){
    # Let's try to load that from default location.
    $defaultPath = "C:\Program Files\SharePoint Client Components\16.0\Assemblies\Microsoft.Online.SharePoint.Client.Tenant.dll"
    $a = [System.Reflection.Assembly]::LoadFile($defaultPath)
}

# Get needed information from end user
$adminUrl = Read-Host -Prompt 'Enter the admin URL of your tenant'
$userName = Read-Host -Prompt 'Enter your user name'
$pwd = Read-Host -Prompt 'Enter your password' -AsSecureString
$importFileUrl = Read-Host -Prompt 'Enter the URL to the file located in your tenant'

# Get instances to the Office 365 tenant using CSOM
$uri = New-Object System.Uri -ArgumentList $adminUrl
$context = New-Object Microsoft.SharePoint.Client.ClientContext($uri)

$context.Credentials = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($userName, $pwd)
$o365 = New-Object Microsoft.Online.SharePoint.TenantManagement.Office365Tenant($context)
$context.Load($o365)

# Type of user identifier ["Email", "CloudId", "PrincipalName"] in the User Profile Service
$userIdType=[Microsoft.Online.SharePoint.TenantManagement.ImportProfilePropertiesUserIdType]::Email

# Name of user identifier property in the JSON
$userLookupKey="IdName"

# Create property mapping between on-premises name and O365 property name
# Notice that we have here 2 custom properties in UPA called 'City' and 'OfficeCode'
$propertyMap = New-Object -type 'System.Collections.Generic.Dictionary[String,String]'
$propertyMap.Add("Property1", "City")
$propertyMap.Add("Property2", "Office")

# Call to queue UPA property import 
$workItemId = $o365.QueueImportProfileProperties($userIdType, $userLookupKey, $propertyMap, $importFileUrl);

# Execute the CSOM command for queuing the import job
$context.ExecuteQuery();

# Output unique identifier of the job
Write-Host "Import job created with following identifier:" $workItemId.Value

<#
 SCRIPT TO PROVISION FIELDS, CONTENT TYPES AND LISTS FOR USE SHAREPOINT HAS THE REPOSITORY 
 FOR PNP SITE PROVISONING SOLUTION


 Microsoft provides programming examples for illustration only, without warranty either expressed or
 implied, including, but not limited to, the implied warranties of merchantability and/or fitness 
 for a particular purpose. 


 ----------------------------------------------------------
 History
 ----------------------------------------------------------
7-27-2016 - Created

==============================================================#>


#########VARIABLES############
$siteUrl = Read-Host 'What is your Site Url'
$provisioningTemplate = "PnPSiteProvisioning.xml"
##############################


#########MAIN##########################################
Connect-SPOnline -Url $siteUrl –Credentials (Get-Credential)
Write-Host "Connected to Site " $siteUrl

Apply-SPOProvisioningTemplate -Path $provisioningTemplate
Write-Host "Completed Provisioning Site Assets"

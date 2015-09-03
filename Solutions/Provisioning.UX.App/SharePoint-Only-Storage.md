# Setup Provisioning.UX.App with SharePoint Only storage
How to set up the Provisioning.UX.App using SharePoint only data storage.

 **Last modified:** September 2, 2015

 _**Verified with:** PnP Provisioning.UX.App, SharePoint Online_

 
## Configure the provisioningSettings.config file
The provisioningSettings.config file should look as follows:
```XML
<modulesSection>
  <Modules>
    <!-- RepositoryManager-->

    <!-- HOSTING ALL DATA IN SHAREPOINT-->
    <!-- NOTE: connectionString is empty, use appsettings RepositoryManager_connectionString to store URL -->
    <Module name="RepositoryManager"         
            type="Provisioning.Common.Data.SiteRequests.Impl.SPSiteRequestManager, Provisioning.Common"
            connectionString=""
            container="" />

    <!-- NOTE: connectionString is empty, use appsettings MasterTemplateProvider_connectionString to store URL -->
    <Module name="MasterTemplateProvider"
            type="Provisioning.Common.Data.Templates.Impl.SPSiteTemplateManager, Provisioning.Common"
            connectionString=""
            container="" />


    <!-- NOTE: connectionString is empty, use appsettings ProvisioningProviders_connectionString to store URL -->
    <Module name="ProvisioningProviders"
                  type="OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml.XMLSharePointTemplateProvider, OfficeDevPnP.Core"
                  connectionString=""
                  container="SiteTemplatesData"/>

    <!-- NOTE: connectionString is empty, use appsettings ProvisioningConnectors_connectionString to store URL -->
    <Module name="ProvisioningConnectors"
            type="OfficeDevPnP.Core.Framework.Provisioning.Connectors.SharePointConnector, OfficeDevPnP.Core"
            connectionString=""
            container="SiteTemplatesData"/>
    
    
  </Modules>
</modulesSection>
```
** Note that we do not have any references to tenant names in the connectionString attributes. Instead it is recommended to use the Azure Web Sites or IIS configuration settings with for the following keys all with the same value of the URL to the site where the templates is going to be hosted (see below). For an example see the [Azure App Settings document](Azure-App-Settings.md)

 - ProvisioningProviders_connectionString
 - ProvisioningConnectors_connectionString
 - MasterTemplateProvider_connectionString
 - RepositoryManager_connectionString


## Install the Add-in
Install and register the Add-in and Timer job according to the [Read me file](readme.md). 

## Create lists and libraries
Start a new PowerShell session and use the [setup PowerShell script](Setup/SetupSPRepository.ps1) to create the required lists and libraries on the SharePoint site.

## Create a template
To create a template you need to first create an entry in Templates list. The *Provisioning Template* column should contain the name of the PnP Provisioning template file you would like to use. The PnP Provisioining template files are uploaded to the "Site Templates Data" library.





# PnP Governance - SharePoint Online governance reference implementation #

*In progress version of the solution*

### Summary ###
Once [self-service site provisioning](https://github.com/OfficeDev/PnP/tree/master/Solutions/Provisioning.UX.App) is enabled in your SharePoint Online tenant, this solution could further help to enforce customized SharePoint governance policies and site lifecycle management policies of your organization. This process would benefit your SPO tenant in the ways below:

- Reduced the number of inactive and abandoned sites
- Increased information security
- Improved search performance and relevance

### Features ###
- Site lifecycle management
- Site owners policy 
- Site classification policy
- External users membership review policy
- Broadly accessible HBI sites detection
- Tenant management reports by SQL Server Reporting Service

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)

### Solution ###
Solution | Author(s)
---------|----------
Governance.TimerJobs | Eric Xu, Sinan Pan

### Version history ###
Version  | Date | Comments
---------| -----| --------
0.1  | June 17th 2015 | Initial draft
0.5  | June 29th 2015 | Add notes for required UX components
0.6  | August 13th 2015 | Updated NuGet Packages added PNP Core NuGet Package 

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

### Prerequisites ###
- SharePoint Online subscription 
- Azure subscription and existing SQL Azure database to store site information records

## Conceptual design ##
![Conceptual design and processes cross elements](https://raw.githubusercontent.com/LiyeXu/PnP-Governance-Assets/master/Governance.png)

## Solution Description ##
Projects what are included in the solution.

### Governance.TimerJobs.Data ###
The data access layer for governance information records powered by Entity Framework 6.

The [SiteInformation](https://github.com/OfficeDev/PnP/blob/dev/Solutions/Governance.TimerJobs/Governance.TimerJobs.Data/SiteInformation.cs) entity class contains both SharePoint site status and additional governance status.  

```C#
DateTime time = DateTime.UtcNow;
string url = "https://constoso.sharepoint.com/teams/EnterpriseSiteCollection";

var site = new SiteInformation()
{
    // DB record properties
    CreatedBy = "i:0#.f|membership|admin@contoso.onmicrosoft.com",
    CreatedDate = time,
    ModifiedBy = "i:0#.f|membership|admin@contoso.onmicrosoft.com",
    ModifiedDate = time,
    // SharePoint site properties
    Url = url,
    Title = "Enterprise Site Collection",
    Description = "Enterprise Site Collection",
    Administrators = new List<SiteUser>() {
        new SiteUser()
        {
            Email = "admin@constoso.onmicrosoft.com",
            LoginName = "i:0#.f|membership|admin@constoso.onmicrosoft.com",
        }
    },
    Guid = Guid.NewGuid(),
    Lcid = 1099,                
    StorageMaximumLevel = 500,
    StorageWarningLevel = 400,
    Template = "STS#0",
    TimeZoneId = 13,
    SharingStatus = 0,
    // Governance properties
    AudienceScope = "Enterprise",
    BusinessImpact = "MBI",
    LastBusinessImpact = "LBI",
    ComplianceState = new ComplianceState(),
    SiteMetadata = new SiteMetadata[] {
        new SiteMetadata()
        {
            MetadataKey = "TargetAudience",
            MetadataValue = "Developer",
        },
        new SiteMetadata()
        {
            MetadataKey = "TargetAudience",
            MetadataValue = "Information Worker",
        }
    },                
};
```


The [GovernanceDbRepository](https://github.com/OfficeDev/PnP/blob/dev/Solutions/Governance.TimerJobs/Governance.TimerJobs.Data/GovernanceDbRepository.cs) class provides methods to access SiteInformation records. 

```C#
string connetionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
var repository = new GovernanceDbRepository(connectionString);
repository.UsingContext(context => { 
    // Load site information from DB
    var existed = context.GetSite(url);
    // Add or Update site information to DB
    context.SaveSite(site);
});
```

To step thru each site information record of the DB repository:
```c#
string connetionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
var repository = new GovernanceDbRepository(connectionString);
repository.UsingContext(context => { 
	int maxPage;
	int page = 1;
	do
	{
		var sites = context.GetAllSites(page, PageSize, out maxPage);
    	foreach (var site in sites)
    	{
    		// ... ...
		}
	}
	while (page++ < maxPage);
});
```

### Governance.TimerJobs.Policy ###
A set of reusable abstract or concrete site management policies.

![Policy classes](https://raw.githubusercontent.com/LiyeXu/PnP-Governance-Assets/master/policy.png)

- [AdministratorPolicy](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs.Policy/Samples/AdministratorsPolicy.cs): marks site information record as in-compliant if there is less than 2 site collection administrators
- [MembershipReviewPolicy](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs.Policy/Samples/MembershipReviewPolicy.cs): marks site information record as in-compliant if the latest external user membership review date in DB repository is earlier than 1 month ago
- [HbiBroadAccessPolicy](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs.Policy/Samples/HbiBroadAccessPolicy.cs): marks site information record as in-compliant if there is any permission assignment at site collection or sub web level for a large security group, which is predefined in the app.config
- [LifeCyclePolicy](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs.Policy/Samples/LifeCyclePolicy.cs): marks site information record as expired if the site collection is about to be existing for longer than expected time of period. (6 months for team sites / 1 year for enterprise sites)  

### Governance.TimerJobs ###
This project contains a set of PnP timer jobs. It is responsible of the SPO site status synchronization and site policy enforcement / notification works.

![Time job classes](https://raw.githubusercontent.com/LiyeXu/PnP-Governance-Assets/master/TimerJobs.png)

- [TenantManagementTimerJob](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs/TenantManagementTimerJob%20.cs) is an abstract class inherits [OfficeDevPnP.Core.Framework.TimerJobs](https://github.com/OfficeDev/PnP-Sites-Core/blob/dev/Core/OfficeDevPnP.Core/Framework/TimerJobs/TimerJob.cs), which is designed to be used as the base class of all tenant management related timer jobs. It outputs the current job progress to the console.
- [SynchronizationJob](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs/SynchronizationJob.cs) is a concrete TenantManagementTimerJob which iterates thru all SharePoint site collections to keep the DB repository being up to date.
- [DatabaseTimerJob](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs/DatabaseTimerJob.cs) is an abstract class derived from TenantManagementTimerJob, it replaces the default SharePoint site resolving logic by providing a GovernanceDdContext for concrete classes to query the site information records from DB repository.
- [CleanUpJob](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs/CleanUpJob.cs) is a concrete DatabaseTimerJob. It steps thru each DB site record and deletes all out-dated ones of which the corresponding site collection has been deleted manually from SPO.
- [GovernanceJob](https://github.com/OfficeDev/PnP/blob/master/Solutions/Governance.TimerJobs/Governance.TimerJobs/GovernanceJob.cs) is a concrete DatabaseTimerJob. It queries all incompliant site collections from DB by using the NoncompliancePredictor property of all registered site policies and then run governance workflow. 
- [GovernancePreprocessJob](https://github.com/OfficeDev/PnP/blob/dev/Solutions/Governance.TimerJobs/Governance.TimerJobs/GovernancePreprocessJob.cs) is designed to support some special site policies (like [HbiBroadAccessPolicy](https://github.com/OfficeDev/PnP/tree/dev/Solutions/Governance.TimerJobs/Governance.TimerJobs.Policy)) which requires a customizable site scope query (check all HBI webs) as well as a complex DB status updates process. 

### Governance.TimerJobs.ConsoleHost ###
This is a sample console project to host the governance timer jobs, which maybe deployed to Azure or on-premises.

Before this console application can work with your SharePoint tenant and SQL Azure database repository, some app settings must be provided in app.config file:

```XML
 <appSettings>
    <add key="TenantUrl" value="[TENANT_URL]" />
    <add key="User" value="[TENANT_ADMIN]" />
    <add key="Password" value="[TENANT_ADMIN_PASSWORD]" />
    <add key="ClientId" value="[CLIENT_ID]" />
    <add key="ClientSecret" value="[CLIENT_SECRET]" />
    <add key="DefaultFirstLockNotificationDays" value="30" />
    <add key="DefaultSecondLockNotificationDays" value="15" />
    <add key="DefaultDeleteNotificationDays" value="30" />
    <add key="DefaultDeleteDays" value="90" />
    <add key="BroadAccessGroups" value="{
                  'c:0(.s|true':'Everyone',
                  'c:0-.f|rolemanager|spo-grid-all-users/ec63b09b-9748-47ba-9018-beeadd405204':'Everyone except external users'
                  }" />
  </appSettings>
```

- **TenantUrl**: The SharePoint Online tenant URL, like "https://contoso.sharepoint.com"
- **User**: Tenant Administrator's user name used by PnP core to iterate thru site collections.
- **Password**: Tenant Administrator's password
- **ClientId**: A SharePoint App client id with tenant full control permission. Please reference the instruction at [here](https://github.com/OfficeDev/PnP/tree/master/Solutions/Provisioning.UX.App#app-registration-and-permissions) to install the SPApp.
- **ClientSecret**: The cliet secret of the SPApp
- **DefaultFirstLockNotificationDays**: Days before the lock up date to send the first lock notification email
- **DefaultSecondLockNotificationDays**: Days before the lock up date to send the second lock notification email
- **DefaultDeleteNotificationDays**: Days after the lock up date to send the site collection delete notification email
- **DefaultDeleteDays**: Days after the lock up date to delete the site collection
- **BroadAccessGroups**: a JSON dictionary which contains the login_name:display_name of the large security groups for HBI broad access detection

The sample below shows how to start a governance related timer job: 

```C#
var syncJob = new SynchronizationJob(dbRepository, TenantUrl);
syncJob.UseThreading = true;
syncJob.SetEnumerationCredentials(User, Password);
syncJob.UseAppOnlyAuthentication(ClientId, ClientSecret);
syncJob.Run();
```

### Governance.TimerJobs.UnitTest ###
The unit test project

## Additional governance related UX components ##

For now, this governance solution only implements the backend part of works, which update the DB repository status and carry out the site policy enforcement workflow. However the full site lifecycle management also requires some UX components to be setup at site collection provisioning time, so that administrators could complete the remediation process before a site collection got locked up or deleted. 

#### UX for classification policy ####
Provisioning UX App provides an [edit page](https://github.com/OfficeDev/PnP/tree/master/Solutions/Provisioning.UX.App/Provisioning.UX.AppWeb/Pages/SiteClassification) for site administrator to update the site classification. In addition one should leverage the [JS link sample](https://github.com/OfficeDev/PnP/tree/master/Samples/Branding.JSLink#how-to-use-jslink) to inject a site collection scope javascript at site provisioning time so that all new sub web can be add into governance DB repository from client side.

```javascript
var hosturl = _spPageContextInfo.webAbsoluteUrl;
var headID = document.getElementsByTagName('body')[0];
iframe = document.createElement('iframe');
iframe.width = '0px';
iframe.height = '0px';
iframe.style.display = "none";
iframe.src = GovernanceAzureHost + '/SyncUpData/AddSubweb?bizImpact=' + rootBizImpact + '&SPHostUrl=' + hosturl + '&ticks=' + (new Date()).getTime();
headID.appendChild(iframe);
```

#### UX for site life cycle policy ####
Governance Solution should provide a page for administrators to either extend or decommission the site collection at the end of a default site life cycle.

![UI for extending a site life time](https://raw.githubusercontent.com/LiyeXu/PnP-Governance-Assets/master/ExtendSite.png)

#### UX for membership review policy ####
Governance Solution should provide a page for administrators to review each external users membership and update the last review date in DB repository once it's confirmed the review process is done.

![UI for seeing shares done externally](https://raw.githubusercontent.com/LiyeXu/PnP-Governance-Assets/master/MembershipReview.png)

#### UX for HBI broad access detection policy ####
Governance Solution should provide a page for administrators to reclassify or update permissions for in-compliant sites.

![UI for fixing issues on HBI sites](https://raw.githubusercontent.com/LiyeXu/PnP-Governance-Assets/master/hbi_access.png)
 
#### UX for site collection lockup ####
Governance Solution should provide a page for administrators to restore a locked up site collection back to its active status.

The unlock page url should be assigned to the property [Tenant.NoAccessRedirectUrl](https://msdn.microsoft.com/en-us/library/office/microsoft.online.sharepoint.tenantadministration.tenant.noaccessredirecturl(v=office.15).aspx) with related tenant CSOM API.

![UI when site is locked](https://raw.githubusercontent.com/LiyeXu/PnP-Governance-Assets/master/UnlockSite.png)

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.TimerJobs" />
# OneDrive for Business customization (async) #

*Notice.* This solution **ONLY** works when you use so called "Classic" option for OneDrive for Business. New experiences do **NOT** support any level of branding in OneDrive for Business sites.

### Summary ###
This is an enhanced add-in part based customization for applying and managing needed customizations to the OD4B sites. 

Actual logical design follows the hidden add-in part approach, which was demonstrated in the older PnP samle called [Provisioning.OneDrive](https://github.com/OfficeDev/PnP/tree/master/Solutions/Provisioning.OneDrive). This means that the assumption is that you have centralized Intranet in the office 365 environment where you can put the needed add-in part and that the end users will be landing to this welcome page when they open up their browser. It is common that each company browser will have same home page set using group policies, so that end users will always start from one centralized location when they open up their browser. This is the location where you’d put add-in part, which can be set to be sized as 0 pixel width and height. Key point here is that you use the end user context to execute the add-in part, which contains page from the provider hosted add-in.

Solution and approach is explained in detail from following blog post: [Customizing OneDrive for Business sites with add-in model](http://blogs.msdn.com/b/vesku/archive/2015/01/01/customizing-onedrive-for-business-sites-with-app-model.aspx).

Here's also a video recording demonstrating the solution in detail from [Office 365 Developer Patterns and Practices Channel 9 section](http://aka.ms/officedevpnpvideos).

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
Solution uses Azure storage queues and web jobs, so you will need to have storage name space in place for setting things up and update that to web.config and app.config files in the project. When you also initially deploy the add-in to the your environment and grant permissions for the add-in, you should copy the add-in id and add-in secret from the web site to other projects to ensure that they can use the granted add-in only token access for site modifications.

### Solution ###
Solution | Author(s)
---------|----------
OD4B.Configuration.Async | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 2nd 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction  #
Solution demonstrates how to apply customizations to Office 365 OneDrive for Business sites using asynchronous method using Azure Storage queues and WebJobs.

Here’s an example of OD4B site, which has been customized using this solution. In this case the end result has been achieved with combination of Office 365 themes, site theme and usage of so called JavaScript injection pattern. You can obviously add and modify the applied customizations as needed.

![Picture of OneDrive for Business site with custom branding](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/3286.image_5F00_thumb_5F00_20BE5FEA.png)

# Solution structure  #
This Visual Studio solution consists from quite a few solutions, but each of them have pretty reasonable reason to be there. Here’s introduction to each of the projects in the solution and why they exists or what they are for.

![List of projects in Visual Studio solution](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/0246.image_5F00_thumb_5F00_192C9789.png)

## OD4B.Configuration.Async ##

This is the actual SharePoint add-in project, which will introduce the provider hosted add-in to SharePoitn and will ask the needed permissions. Notice that even though we do not actually perform tenant level operations from the add-in part it self, we are asking pretty high permissions for the add-in. This is because we will use the same  client ID and secret from this add-in file in our WebJob execution. Using this approach, you do not have to manually register add-in id and secret to the SharePoint, we rather just use the same identifier and secret cross solution. 

![Permission configuration for the add-in](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/3302.image_5F00_thumb_5F00_2726198A.png)

This project also contains the add-in part definition which will be then deployed to the host web. 

## OD4B.Configuration.Async.Common ##

This project contains all the actual business logic and shared code cross projects, like the definition for the data object which is placed to the storage queue and the actual business logic to customize OD4B sites. Reason to place code in here is simply for giving us easier way to develop and test the operations when the project is created. Like with general development, you should not really place your business logic code directly to the WebJob or to add-in part, rather to locate that in business logic layer for easier testing and code reuse. 

All the actual operations towards the OD4B sites are located in *OD4B.Configuration.Async.Common.SiteModificationManager* class.

## OD4B.Configuration.Async.Console.Reset ##

This project is our test and debugging project for the actual customizations. It can be used to manually apply the wanted customizations to any OD4B site. During development time this project was our testing project to test the customization process before it was hooked to the WebJob. Project can be also used to reset customizations from any OD4B site for demonstration or testing purposes. Since actual business logic is located in the common project, this project will use the same *SiteModificationManager* class as the others to apply or reset customizations from the sites.

When you test the customizations, you can simply change the code in the Main method between Apply and Reset to change the wanted operation. 

```C#
static void Main(string[] args)
{

    Uri url = 
        new Uri("https://vesaj-my.sharepoint.com/personal/vesaj_veskuonline_com");

        //get the new site collection
    string realm = TokenHelper.GetRealmFromTargetUrl(url);
    var token = TokenHelper.GetAppOnlyAccessToken(
                    TokenHelper.SharePointPrincipal, 
                    url.Authority, realm).AccessToken;
    using (var ctx = 
        TokenHelper.GetClientContextWithAccessToken(url.ToString(), 
        token))
    {
        // Uncomment the one you need for testing/reset
        // Apply(ctx, url);
        Reset(ctx);
    }
}
```

Notice that you will need to ensure that add-in id and secret for this project in the app.config are matching the ones you gave needed permissions to your tenant. You can easily execute the project by right clicking the project and choosing Debug – Start New Instance, so that you can walk the actual code which is executed line by line.

## OD4B.Configuration.Async.Console.SendMessage ##

This project was added to the solution to test the storage queue mechanism before it was hooked to the add-in part. Project can be used to by pass the add-in part process for adding new messages to the storage queue. Notice that you will need to update the storage queue connection string accordingly in the app.config to make the project work properly. 

You can easily execute the project by right clicking the project and choosing Debug – Start New Instance, so that you can walk the actual code which is executed line by line.

## OD4B.Configuration.Async.WebJob ##

This is the actual WebJob project, which was created using WebJob project template, introduced in the Visual Studio 2013 Update 4. This template makes it easier to create WebJob projects by adding right references in place and it also provides nice deployment automation with right click support for the project. You can simply deploy initial version or new version of the project to the Azure by right clicking and selecting *Publish as Azure Web Job…* which will open up the publishing wizard.

![Publish web option in Visual Studio wizards](http://blogs.msdn.com/cfs-file.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-00-81-08-metablogapi/6406.SNAGHTML1f4a2e81_5F00_thumb_5F00_382CD81E.png)

This WebJob is created as a continuous WebJob, which is needed for the queue based processing. This means that in the Main method, we only set the process to be executing continuous like follows.

```C#
// To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
class Program
{
    // Please set the following connection strings in app.config for this 
    // WebJob to run: AzureWebJobsDashboard and AzureWebJobsStorage
    static void Main()
    {
        var host = new JobHost();
        // The following code ensures that the WebJob will be 
        // running continuously
        host.RunAndBlock();
    }
}
```

Actual queue processing is really easy with WebJobs. Only thing we need to do is to set the right attributes for the method and to ensure that the Azure storage connection strings in the add-in config are updated accordingly and matching the storage queue’s you have created to Microsoft Azure. Following is the ProcessQueueMesasge method from the functions.cs class. Notice how we use the add-in Only token model to access the SharePoint from the WebJob. To make this work, you will need to ensure that you have copied the right add-in id and secret to the app.config of the project. Actual business logic is located in the SiteModificationManager class, so we just call that with the right client context and parameters. 

```C#
// This function will get triggered/executed when a new message is written 
// on an Azure Queue called queue.
public static void ProcessQueueMessage(
    [QueueTrigger(SiteModificationManager.StorageQueueName)] 
    SiteModificationData request, TextWriter log)
{
    Uri url = new Uri(request.SiteUrl);

    //Connect to the OD4B site using add-in Only token
    string realm = TokenHelper.GetRealmFromTargetUrl(url);
    var token = TokenHelper.GetAppOnlyAccessToken(
        TokenHelper.SharePointPrincipal, url.Authority, realm).AccessToken;

    using (var ctx = TokenHelper.GetClientContextWithAccessToken(
        url.ToString(), token))
    {
        // Set configuration object properly for setting the config
        SiteModificationConfig config = new SiteModificationConfig()
        {
            SiteUrl = url.ToString(),
            JSFile = Path.Combine(Environment.GetEnvironmentVariable
                ("WEBROOT_PATH"), "Resources\\OneDriveConfiguration.js"),
            ThemeName = "Garage",
            ThemeColorFile = 
                Path.Combine(Environment.GetEnvironmentVariable
                ("WEBROOT_PATH"), "Resources\\Themes\\Garage\\garagewhite.spcolor"),
            ThemeBGFile = 
                Path.Combine(Environment.GetEnvironmentVariable
                ("WEBROOT_PATH"), "Resources\\Themes\\Garage\\garagebg.jpg"),
            ThemeFontFile = "" // Ignored in this case, but could be also set
        };

        new SiteModificationManager().ApplySiteConfiguration(ctx, config);
    }
}
```

Other thing worth noticing is that you will need to ensure that you have set the Copy Local property for the SharePoint CSOM assembly references property for the project, so that all dependent assemblies are properly copied to Azure when you deploy the web job. This is simply because these assemblies are not located in normal Azure web site by default, so by setting this property True, you will ensure that the referenced assemblies are copied to cloud as well.

## OD4B.Configuration.AsyncWeb ##

This is the actual provider hosted add-in which is hosted in Microsoft Azure. It contains the page laded to the add-in part, which is placed on the front page of the intranet. Default.aspx page of this add-in does not actually contain any operations, it shows provides details on how to use the add-in.

Notice. If you will face permission denied issues with the WebJob or add-in only access in general, make sure that you have updated add-in client id and secret in the app.config to match the values in the web.config from this project. Visual Studio can change these values in certain scenarios. 

 
<img src="https://telemetry.sharepointpnp.com/pnp/solutions/OD4B.Configuration.Async" />
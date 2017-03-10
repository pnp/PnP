# Asynchronous operations with Azure storage queues and WebJobs #

### Summary ###
This sample shows how to perform operations on-demand with Azure WebJobs as continuously running operation, which will handle incoming requests from message queue. This can be really easily achieved with only few lines of code and is extremely powerful technique for providing asynchronous operations started by end user. Comparing this to classic server side timer jobs, this equals to the model where you use SPOneTImeSchedule class for your timer job scheduling based on end user input. 

Typical use cases for long lasting asynchronous operations would be for example following. 
- Complex configurations installed from the add-in to the given host web  
- Complex add-in Installed operations due 30 sec time out
- Self service operations for the end users, like site collection provisioning for cloud or for on-premises with service bus usage
- Route entries or information from Office 365 to on-premises systems
- Start perform complex usage calculations or other long lasting business logic cross tenant

See exact details on the model from blog post from Vesa Juvonen

- [Using Azure storage queues and WebJobs for async actions in Office 365](http://blogs.msdn.com/b/vesku/archive/2015/03/02/using-azure-storage-queues-and-webjobs-for-async-actions-in-office-365.aspx)


### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
You will need to have or create Azure storage account for the communications between provider hosted add-in and the WebJob, which is executed dynamically based on messages added to the storage queue. 

### Solution ###
Solution | Author(s)
---------|----------
Core.QueueWebJobUsage | Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 2nd 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
Following picture explains the logical architecture for this pattern. 

![Logical design of the solution with Azure storage queues](http://i.imgur.com/EGLm9Tw.png)

1. User operates in the SharePoint and starts provider hosted add-in UI one way or another (full page, pop up, add-in part etc.)
1. Actual operations are performed in the provider hosted add-in side which is collecting needed parameters and other settings for the processing
1. Operation request is stored to Azure storage queue or Service bus for processing
1. Task is picked up automatically by continuously running WebJob with needed details and requested operation is applied

Notice that operation could be also targeted not only to Office 365, but also to any other system. You can for example use this same pattern to feed LOB system from on-premises with service bus connection. 

Typical use cases for long lasting asynchronous operations would be for example following. 
- Complex configurations installed from the add-in to host web 
- Complex add-in Installed operations due 30 sec time out
- Self service operations for the end users, like site collection provisioning for cloud or for on-premises with service bus usage
- Route entries or information from Office 365 to on-premises systems
- Start perform complex usage calculations or other long lasting business logic cross tenant

# Solution structure #
Here's the solution structure in Visual Studio solution

![Visual Studio solutions structure](http://i.imgur.com/qVIdv3s.png)

## Core.QueueWebJobUsage.Console.SendMessage ##
This is helper project, which can be used to test the storage queue behavior and send messages to the queue for processing without the need for user interface operations. Purpose is simply to ensure that the storage account information and queue creation works as expected. 

app.config file contains StorageConnetionString key, which should be updated accordingly to match the storage queue for used Azure environment. 

```XML
<appSettings>
  <add key="StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName=officedevpnpsample2;AccountKey=nplj6ZOHI4JZJnPXNYOdMMZVNU0KPV8LGbhn8abCsXND5blxNHXa2B1DnMQCYEyk/l/M6alpTTh2rcAgstIO/Q==" />
</appSettings>
```


## Core.QueueWebJobUsage ##
This is the actual SharePoint add-in project, which is used to introduce the add-in for SharePoint and contains also the requested permission, which are needed for the actual provider hosted add-in. In this case we are requesting following permissions, which are needed for the synchronous operation demonstration in the reference provider hosted code side. Technically these are not needed for the WebJob based implementation, since you could request or register needed permission directly by using appinv.aspx page explained for example in this[ great blog post from Kirk Evans](http://blogs.msdn.com/b/kaevans/archive/2014/03/02/building-a-sharepoint-app-as-a-timer-job.aspx) related on the remote timer jobs.

- Allow the add-in to make add-in-only calls to SharePoint
- FullControl permission to Web 


## Core.QueueWebJobUsage.Common ##
This is business logic and entity project, so that we can use needed code from numerous projects. It also contains the data object or entity for message serialization.

Notice that business logic (Core.QueueWebJobUsage.Common.SiteManager::PerformSiteModification()) has intentional 20 second sleep for the thread to demonstrate long lasting operation. 

## Core.QueueWebJobUsage.Job ##
This is the actual Azure WebJob created using WebJob template which was introduced by the [Azure SDK for .NET 2.5](https://msdn.microsoft.com/en-us/library/azure/dn873976.aspx). All the actual business logic is located in the Common component, but the logic to hook up the queues and initial creation of add-in only client context is located in this project. 

You will need to update right add-in Id and add-in secret to the app.config for this one like follows.

```XML
  <appSettings>
    <add key="ClientId" value="[your add-in id]" />
    <add key="ClientSecret" value="[your add-in secret]" />
  </appSettings>
```

You will also need to update connection strings to match your storage connection strings. 

```XML
<connectionStrings>
  <!-- The format of the connection string is "DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY" -->
  <!-- For local execution, the value can be set either in this config file or through environment variables -->
  <add name="AzureWebJobsDashboard" connectionString="DefaultEndpointsProtocol=https;AccountName=[YourAccount];AccountKey=[YourKey]" />
  <add name="AzureWebJobsStorage" connectionString="DefaultEndpointsProtocol=https;AccountName=[YourAccount];AccountKey=[YourKey]" />
</connectionStrings>
```

## Core.QueueWebJobUsageWeb ##
This is the user interface for the provider hosted add-in. Only thing you need to configure is the storage configuration string based on your environment. add-in ID and Secret information is managed automatically by Visual Studio when you deploy the solution for debugging with F5, but obviously those would need to be properly configured for actual deployment. 

Notice also that you need to update the storage account information accordingly, so that UI can add requests to queue.

```XML
<appSettings>
  <add key="ClientId" value="7b4d315d-e00a-46a1-b644-67e42ea37b79" />
  <add key="ClientSecret" value="U/2uECA7fAT/IhIU2O2T8KYcUwvCcI1QLCOzHMtSOcM=" />
  <add key="StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName=[YourAccountName];AccountKey=[YourAccountKey]" />
</appSettings>
```

This is typical Office 365 Developer Patterns and Practices sample, which is concentrating on demonstrating the pattern or functional model, but does not concentrate on anything else. This way you can easily learn or adapt only on the key functionality without any additional distractors. 

![Add-in UI](http://i.imgur.com/Bx2oFGA.png)

This add-in has two different buttons, one for synchronous and one for asynchronous operation. Both buttons will create new document library to the host web, but asynchronous operation will take advantage of the Azure WebJob based execution. Here’s example of the library created by using this code. Notice that the description for the library is also dynamic and the requestor name is added there for demonstration purposes. This was just done to show how to provide complex data types cross storage queues. 

![List settings](http://i.imgur.com/CAnZ5Ac.png)

# Key areas of the code #

## Adding message to storage queue ##
Adding new items to queue is really easy and does not require that many lines of code. In our reference implementation case we have created a data entity called SiteModifyRequest, which contains the needed information for site changes. 

```C#
public class SiteModifyRequest
{
    public string SiteUrl { get; set; }
    public string RequestorName { get; set; }
}
```

This object in then serialized to the queue using following lines of code. Notice that code is also creating the storage queue if it does not exist. Notice that we use constant called SiteManager.StorageQueueName to define the storage queue and we will reference this constant also in the WebJob side to avoid any typos on the queue name. 

```C#
/// <summary>
/// Used to add new message to storage queue for processing
/// </summary>
/// <param name="modifyRequest">Request object with needed details</param>
/// <param name="storageConnectionString">Storage connection string</param>
public void AddAsyncOperationRequestToQueue(SiteModifyRequest modifyRequest, 
                                            string storageConnectionString)
{
    CloudStorageAccount storageAccount =
                        CloudStorageAccount.Parse(storageConnectionString);

    // Get queue... create if does not exist.
    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
    CloudQueue queue = queueClient.GetQueueReference(SiteManager.StorageQueueName);
    queue.CreateIfNotExists();

    // Add entry to queue
    queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(modifyRequest)));
}
```

## Hooking up the WebJob to the storage queue ##
Hooking up WebJob to new storage queue messages is really simple due and does not require that much of code. Our WebJob has to be setup to be executing continuously, which we can implement as follows in our Main method. Azure SDK 2.5 templates have this code in place by default. 

```C#
// Please set the following connection strings in app.config for this WebJob to run:
// AzureWebJobsDashboard and AzureWebJobsStorage
static void Main()
{
    var host = new JobHost();
    // The following code ensures that the WebJob will be running continuously
    host.RunAndBlock();
}
```

Actual connection then to the Azure Storage queues is combination of correct connection string configuration in the app.config and proper signature for our method or function. In app.config side we need to ensure that AzureWebJobsStorage connection string is pointing to the right storage account. Notice that you definitely also want to ensure that the AzureWebJobsDashboard connection string has valid entry, so that your logging will work properly. 

```C#
<connectionStrings>
  <!-- The format of the connection string is "DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY" -->
  <!-- For local execution, the value can be set either in this config file or through environment variables -->
  <add name="AzureWebJobsDashboard" connectionString="DefaultEndpointsProtocol=https;AccountName=[YourAccount];AccountKey=[YourKey]" />
  <add name="AzureWebJobsStorage" connectionString="DefaultEndpointsProtocol=https;AccountName=[YourAccount];AccountKey=[YourKey]" />
</connectionStrings>
```

In our code side we only need to decorate or add right signature with right attributes are our method is called automatically. Here’s the code from our reference implementation. Notice that we use same SiteManager.StorageQueueName constant as the queue name to link the WebJob.

```C#
namespace Core.QueueWebJobUsage.Job
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage(
            [QueueTrigger(SiteManager.StorageQueueName)] 
            SiteModifyRequest modifyRequest, TextWriter log)
        {
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.QueueWebJobUsage" />
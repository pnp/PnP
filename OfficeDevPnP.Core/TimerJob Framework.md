# Guide to using the Timer Job Framework #

The PnP timer job framework is set of classes designed to ease the creation of background processes that operate against SharePoint sites, kind of similar to what full trust code timer jobs (`SPJobDefinition`) are for an on-premises SharePoint deployment. The big difference with between this timer job framework and the out of the box one is that this one only uses client side API's and as such can (and should) be run outside of SharePoint. This makes it possible to build timer jobs that operate against SharePoint Online. Once a timer job has been created it needs to be scheduled and executed and for that the two most common options are:

- When working with **Microsoft Azure** as hosting platform deploying and running timer jobs as **Azure WebJobs** is super powerful and really easy
- When working with **Windows Server** as hosting platform (e.g. for on-premises SharePoint) the easiest option is to use the built in **Windows scheduler**

Further in this article you will find more details around timer job deployment together with all the other timer job details you might be interested in.

## Simple timer job example ##
In this chapter you'll see how to create a very simple timer job: the goal of this sample is to provide the reader a quick view, later on we'll provide a more detailed explanation of the timer job framework. 

**Note:**
- There a [PnP video](http://channel9.msdn.com/blogs/OfficeDevPnP/Introduction-to-the-PnP-timer-job-framework) that provides an introduction to timer jobs and shows a demo of the below simple timer job sample. 
- There's a PnP solution that shows 10 individual timer job samples. See https://github.com/OfficeDev/PnP/tree/dev/Solutions/Core.TimerJobs.Samples to learn more about these 10 timer job samples. Samples range from "Hello world" type samples up to real life content expiration jobs.

### Step 1: Create a Console project and reference PnP Core ###
In this first step you create a new project of the type "console" and reference the PnP core library. You can do this by:

- Adding the Office 365 Developer Patterns and Practices Core Nuget package to your project. There's a [nuget package for v15 (on-premises) and for v16 (Office 365)](https://www.nuget.org/packages?q=pnp). This is the easiest and preferred option.
- Add the existing PnP Core source project to your project. This will allow you to step into the PnP core code when you're debugging, but keep in mind that you're responsible for keeping this code updated with the latest changes added to PnP.

### Step 2: Create a timer job class and add your timer job logic ###
Add a class for your timer job (`SimpleJob`) in below sample and take the following three simple steps:

1. Have the class inherit the `TimerJob` abstract base class
2. In the constructor give the timer job a name (`base("SimpleJob")`) and connect the `TimerJobRun` event handler
3. Add your timer job logic to the `TimerJobRun` event handler

The result will be similar to below sample code:
```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.TimerJobs;

namespace Core.TimerJobs.Samples.SimpleJob
{
    public class SimpleJob: TimerJob
    {
        public SimpleJob() : base("SimpleJob")
        {
            TimerJobRun += SimpleJob_TimerJobRun;
        }

        void SimpleJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
        {
            e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
            e.WebClientContext.ExecuteQueryRetry();
            Console.WriteLine("Site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
        }
    }
}
```

### Step 3: Update the console program.cs to use the timer job ###
The timer job we created in the previous step still needs to be executed. To do so you'll need to update the `program` of the console application using the four steps explained below:

1. Instantiate your timer job class
2. Provide the authentication details for the timer job. Here we're using user name and password to authenticate against SharePoint Online
3. Add one ore more sites the timer job code should be run against. This sample shows a wild card url: the timer job code will be fired for all sites that match this wild card url
4. Trigger the job execution by calling `Run`

```C#
static void Main(string[] args)
{
    // Instantiate the timer job class
    SimpleJob simpleJob = new SimpleJob();
    
    // The provided credentials need access to the site collections you want to use
    simpleJob.UseOffice365Authentication("user@tenant.onmicrosoft.com", "pwd");

    // Add one or more sites to operate on
    simpleJob.AddSite("https://<tenant>.sharepoint.com/sites/d*");
    
    // Run the job
    simpleJob.Run();
}
```

## Timer job deployment options ##
In the previous step you've see a simple timer job in action, next step is to deploy this timer job: a timer job is a .exe that needs to be scheduled on a hosting platform. Depending on the chosen hosting platform the deployment differs. Below chapters describe the two most common options:
- Using Microsoft Azure as hosting platform
- Using Windows Server as a hosting platform

### Deploying timer jobs to Microsoft Azure using Azure WebJobs ###
Before you can deploy a timer job you'll need to ensure that the job can run without user interaction. The used samples always prompt you to provide a password or a clientsecret (see more in latter **Authentication** chapter) which is fine while testing but will obviously not work when deployed. The existing samples all allow to provide password/clientsecret via the app.config file:

```XML
  <appSettings>
    <add key="user" value="user@tenant.onmicrosoft.com"/>
    <add key="password" value="your password goes here!"/>
    <add key="domain" value="Contoso"/>
    <add key="clientid" value="a4cdf20c-3385-4664-8302-5eab57ee6f14"/>
    <add key="clientsecret" value="your clientsecret goes here!"/>
  </appSettings>
```

Once that's done test by running your timer job from Visual Studio: it should now run and end without any user interaction. 

The actual deployment to Azure is based on Azure Web Jobs. We've an [excellent guidance article](https://github.com/OfficeDev/PnP-Guidance/blob/master/articles/Getting-Started-with-building-Azure-WebJobs-for-your-Office365-sites.md) that describes all the needed steps in great detail, but a short summary is added here as well.

1. Right click your project in Visual Studio and choose **Publish as Azure WebJob...**
2. Provide a schedule for your timer job and click **OK**
3. Select **Microsoft Azure Websites** as a publish target. You'll be asked to login to Azure and select the Azure Web Site that will host your timer job (you can also create a new one if that would be needed)
4. Press **Publish** to push the WebJob to Azure
5. Once it has been published you can trigger the job and check the job execution from either Visual Studio or from the [Azure management portal](https://manage.windowsazure.com).

![](http://i.imgur.com/4xDUvXv.png)

### Deploying timer jobs to Windows Server using the Windows Schedular ###
Just like in the Azure WebJobs guidance you'll first need to ensure your timer job can run without user interaction. Once that's done you copy the release version of your job to the server you want it to run on. **Important:** copy all the relevant assemblies, the .exe and the .config file to ensure the job can run on the server without installing additional bits on the server. Final step is scheduling the execution of your timer job and for this we recommend to rely on the built in [Windows Schedular functionality](https://technet.microsoft.com/en-us/library/cc721871.aspx). In short the steps are:

1. Open the task schedular (Control Panel -> Task Schedular)
2. Click on **Create Task** and specify a name and an account that will execute the task
3. Click on **Triggers** and add a new trigger. Specify the schedule you want for your timer job
4. Click on **Actions** and choose action "Start a program", select your timer job .exe and set the start in folder
5. Click on **OK** to save the task

![](http://i.imgur.com/hkRc0Bo.png)

## Timer job internals ##
After reading this chapter you'll have a detailed understanding of how the timer job framework works and how to use each and every feature of it. 

### Structure ###
The PnP structure is rather simple: there's an abstract base class called `TimerJob` that will be the base class for your timer jobs. This base class contains the below public properties, methods and events:

![](http://i.imgur.com/rbXcZQq.png)

Most properties and methods will be explained in more detail in the coming chapters, for the ones which aren't you'll find a description below:

- **IsRunning** property: Indicates if the timer job is already executing or not
- **Name** property: Gives you the name of the timer job. The name is initially set in the timer job constructor
- **SharePointVersion** property: this property is automatically set based on the version of the loaded Microsoft.SharePoint.Client.dll and in general should not change. You however can change this property in case you for example want to use the v16 CSOM libraries in a v15 (on-premises) deployment
- **Version** property: get you the version of the timer job. The version is initially set in the timer job constructor or defaults to 1.0 when not set via the constructor

To prepare for a timer job run you need to first **configure** it:

1. Provide **authentication** settings
2. Provide a **scope** (= list of sites)
3. Optionally set **timer job properties** 

From an execution perspective the following big steps are taken when a timer job run is started:

1. **Resolve sites**: wild card site urls (e.g. https://tenant.sharepoint.com/sites/d*) are resolved into an actual list of existing sites. If sub site expanding was requested then the resolved sites list is expanded with all sub sites
2. **Create batches of work** based on the current treading settings and create a thread per batch
3. The **threads execute work batches** and call the `TimerJobRun` event for each site in the list

All of the above prepare and run steps will be more detailed in this article.

### Authentication ###
Before a timer job can be used the timer job needs to know how it needs to authenticate back to SharePoint. The framework currently supports the following approaches. Using these methods also automatically set the **AuthenticationType** property to either Office365, NetworkCredentials or AppOnly. The below flowchart shows the steps you need to take, detailed explanation is following in the next chapters.

![](http://i.imgur.com/ccIBRmk.png)

#### User credentials ####
To specify user credentials for running against **Office 365** you can use these 2 methods:
```C#
public void UseOffice365Authentication(string userUPN, string password)
public void UseOffice365Authentication(string credentialName)
```

The first method simply accepts a user name and password. The second one allows you to specify a generic credential stored in the Windows Credential Manager. Below screen shot shows the `bertonline` generic credential. If you want to use that in for timer job authentication you simply provide "bertonline" as input to the second method.

![](http://i.imgur.com/HdqvsHy.png)

There are similar methods for running against **SharePoint on-premises**:
```C#
public void UseNetworkCredentialsAuthentication(string samAccountName, string password, string domain)
public void UseNetworkCredentialsAuthentication(string credentialName)
```

#### App Only ####
App only is the **preferred method** as you can grant tenant scoped permissions to it whereas for user credentials you'll need to hope that the used user account has the needed permissions. The downside with app-only is that certain site resolving logic wont work, but more about that in the next chapter. 

To configure the job for app-only authentication the following method needs to be used:
```C#
public void UseAppOnlyAuthentication(string clientId, string clientSecret)
```

As you can see the same method can be used for either Office 365 as SharePoint on-premises which makes timer jobs using app-only better transportable between environments. 

**Note:**
When you use app-only your timer job logic will fail when API's are used that do not work with App-Only. Typical samples are the Search API, writing to the taxonomy store and using the user profile API.

### Sites to operate on ###
When a timer job runs it needs one or more sites to run against. To add sites to a timer job you can use the below set of methods.

```C#
public void AddSite(string site)
public void ClearAddedSites()
```

When you add a site you can either specify a correct fully qualified url to the site (e.g. https://tenant.sharepoint.com/sites/dev) or a wild card url. This wild card url is a url that ends on a * (only one single * is allowed and it must be the last character of the url). A sample wild card url is https://tenant.sharepoint.com/sites/* which will give you **all** the site collections that underneath the sites managed path. Similar you can for example get all the site collections where the url contains dev in the name via https://tenant.sharepoint.com/sites/dev*.

Typically the sites are added by the program that instantiates the timer job object, but if needed the timer job can take control over the passed list of sites. You can do this by adding a method override for the `UpdateAddedSites`virtual method as shown in below sample:

```C#
public override List<string> UpdateAddedSites(List<string> addedSites)
{
    // Let's assume we're not happy with the provided list of sites, so first clear it
    addedSites.Clear();

    // Manually adding a new wildcard Url, without an added URL the timer job will do...nothing
    addedSites.Add("https://bertonline.sharepoint.com/sites/d*");

    // Return the updated list of sites
    return addedSites;
}
```

When you've added a wild card url and you've set authentication to app-only you'll also need to specify so called enumeration credentials. These enumeration credentials are used to fetch a list of site collections which are then used in the site matching algorithm to come up with a real list of sites. To acquire a list of site collections the timer framework behave different between Office 365 (v16) and on-premises (v15):
- Office 365: the `Tenant.GetSiteProperties` method is used to read the 'regular' site collections, the search API is used to read the OneDrive for Business site collections
- On-Premises: the search API is use to read all site collections

Given that the search API doesn't work with a user context the timer job falls back to the specified enumeration credentials. 

To specify user credentials for running against **Office 365** you can use these 2 methods:
```C#
public void SetEnumerationCredentials(string userUPN, string password)
public void SetEnumerationCredentials(string credentialName)
```

There are similar methods for running against **SharePoint on-premises**:
```C#
public void SetEnumerationCredentials(string samAccountName, string password, string domain)
public void SetEnumerationCredentials(string credentialName)
```

The first method simply accepts a user name, password and optionally domain (when in on-premises). The second one allows you to specify a generic credential stored in the Windows Credential Manager. See the **Authentication** chapter to learn more about the Credential Manager.

#### Sub site expanding ####
Often you want your timer job code to be executed against the root site of the site collection but also against all the sub sites of that site collection. To realize this you can set the **ExpandSubSites** property to true. When you do so the timer job will also expand the sub sites as part of the site resolving step.

#### Override resolved and/or expanded sites ####
Once the timer framework has resolved the wild card sites and optionally expanded their sub sites the next step is to process this list of sites. You however might want to override this behavior and manipulate the created list of sites (e.g. exclude some sites, retrieve all sites from a database,...). This is possible by adding a method override for the `ResolveAddedSites`virtual method. Below sample shows how to do so. 

```C#
public override List<string> ResolveAddedSites(List<string> addedSites)
{
    // Use default TimerJob base class site resolving
    addedSites = base.ResolveAddedSites(addedSites);

    //Delete the first one from the list...simple change. A real life case could be reading the site scope 
    //from a SQL (Azure) DB to prevent the whole site resolving. 
    addedSites.RemoveAt(0);

    // return the updated list of resolved sites...this list will be processed by the timer job
    return addedSites;
}
```

### TimerJobRun event ###
Now that we've setup authentication and added sites to operate on the timer job framework will split the sites in work batches: by default the framework will create 5 batches and as such 5 threads will be used to run these batches in parallel. See the **Threading** chapter to learn more about the threading options. When a thread processes a batch the `TimerJobRun` event is triggered by the timer framework and will provide you with all the necessary information to easily write your timer job code. To make your timer job actually work you need to have connected an event handler to this `TimerJobRun` event:

```C#
public SimpleJob() : base("SimpleJob")
{
    TimerJobRun += SimpleJob_TimerJobRun;
}

void SimpleJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
{
    // your timer job logic goes here
}
```

An alternative approach is using an inline delegate as shown here:

```C#
public SimpleJob() : base("SimpleJob")
{
    // Inline delegate
    TimerJobRun += delegate(object sender, TimerJobRunEventArgs e)
    {
        // your timer job logic goes here
    };
}
```

When the `TimerJobRun` event fires you receive a `TimerJobRunEventArgs`object which provides you with the information you need to easily write your timer job logic. Following attributes and methods are available in this class:

![](http://i.imgur.com/OVSYF8b.png)

Several of the properties and all of the methods are used in the optional state management feature which will be discussed in the next chapter. However the following properties will always be available in each and every event, regardless of the used configuration:
- **Url** property: this holds the site the event is fired for. This can be the root site of the site collection, but it can also be a sub site in case site expanding was done
- **WebClientContext** property: this property contains a `ClientContext` object for the site defined in the Url property. This is typically the `ClientContext` object that you would use in your timer job code
- **SiteClientContext** property: When you have expanded sub sites your timer job logic might need to do something with the root site (e.g. add page lay-out to the master page gallery). To make that tasks easy you can use the SiteClientContext property as this contains a `ClientContext`object for the root site of the currently processed url

All `ClientContext`objects do use the authentication information like setup in the **Authentication** chapter. If you've opted for user credentials please ensure that the used account has the needed permissions to operate against the specified sites. When using app-only is best to set tenant-scoped permissions to the app-only principal.

### State management ###
When you write timer job logic you often need to persist state (e.g. simply knowing when this site was last processed, storing data to support your timer job business logic). You can build all of this as part of your timer job logic, but the timer job framework can make things super easy via it's built in state management capabilities. What state management does is storing and retrieving a set of standard and custom properties as JSON serialized string in the web property bag of the processed site (name = timer job name + "_Properties"). Out of the box you'll get the following properties as part of the `TimerJobRunEventArgs`object:
- **PreviousRun** property: this one contains the date time of the previous run
- **PreviousRunSuccessful** property: contains a boolean indicating whether the previous run went fine. Note that the timer job author is responsible for flagging a job run as successful by setting the **CurrentRunSuccessful** property as part of your timer job implementation
- **PreviousRunVersion** property: The timer job version of the previous run.

Next to these standard properties you also have the option to specify your own properties by adding keyword - value pairs to the `Properties` collection of the `TimerJobRunEventArgs`object. To make this easier there are three methods to help you:
- **SetProperty** can be used to add/update a property
- **GetProperty** returns the value of a property
- **DeleteProperty** removes a property from the property collection

Below timer job implementation shows how state management can be used:

```C#
void SiteGovernanceJob_TimerJobRun(object o, TimerJobRunEventArgs e)
{
    try
    {
        string library = "";

        // Get the number of admins
        var admins = e.WebClientContext.Web.GetAdministrators();

        Log.Info("SiteGovernanceJob", "ThreadID = {2} | Site {0} has {1} administrators.", e.Url, admins.Count, Thread.CurrentThread.ManagedThreadId);

        // grab reference to list
        library = "SiteAssets";
        List list = e.WebClientContext.Web.GetListByUrl(library);

        if (!e.GetProperty("ScriptFileVersion").Equals("1.0", StringComparison.InvariantCultureIgnoreCase))
        {
            if (list == null)
            {
                // grab reference to list
                library = "Style%20Library";
                list = e.WebClientContext.Web.GetListByUrl(library);
            }

            if (list != null)
            {
                // upload js file to list
                list.RootFolder.UploadFile("sitegovernance.js", "sitegovernance.js", true);

                e.SetProperty("ScriptFileVersion", "1.0");
            }
        }

        if (admins.Count < 2)
        {
            // Oops, we need at least 2 site collection administrators
            e.WebClientContext.Site.AddJsLink(SiteGovernanceJobKey, BuildJavaScriptUrl(e.Url, library));
            Console.WriteLine("Site {0} marked as incompliant!", e.Url);
            e.SetProperty("SiteCompliant", "false");
        }
        else
        {
            // We're all good...let's remove the notification
            e.WebClientContext.Site.DeleteJsLink(SiteGovernanceJobKey);
            Console.WriteLine("Site {0} is compliant", e.Url);
            e.SetProperty("SiteCompliant", "true");
        }

        e.CurrentRunSuccessful = true;
        e.DeleteProperty("LastError");
    }
    catch(Exception ex)
    {
        e.CurrentRunSuccessful = false;
        e.SetProperty("LastError", ex.Message);
    }
}
```

Given the state is stored as a single JSON serialized property means it can be used by other customizations as well: e.g. you could use JavaScript to prompt the user to act when the timer job has set a site to be incompliant and wrote a "SiteCompliant=false" custom property.

### Threading ###
The timer job framework by default is using threading to parallelize the work. Threading is used for both the sub site expanding (when requested) and for the running the actual timer job logic (`TimerJobRun` event) for each site. Following properties can be used to control the threading implementation:
- **UseThreading** property: defaults to true, but can be set to false to perform all actions using the main application thread
- **MaximumThreads** property: default to 5. Can be set anywhere between 2 and 100. Having lots of threads is not necessarily faster then having just few threads...the optimal number should be acquired via testing using various number of threads. Based on initial testing we've set 5 as the default as having 5 threads significantly boosts performance in most scenarios

#### Throttling ####
The fact that the timer job does threading combined with the typical resource intensive operations that are used in timer jobs means that a timer job run could be throttled. In order to correctly deal with throttling the timer job framework and the whole of PnP Core uses the `ExecuteQueryRetry` method instead of the default `ExecuteQuery`method. **It's important that you also use `ExecuteQueryRetry` in your actual timer job implementation code.**

#### Concurrency issues - process all sub sites of a site collection in the same thread####
When you've opted to expand sub sites and you use multi-threading the timer framework will have built up a list of site and sub sites which will be evenly split in work batches (one per thread). This means that thread A can process the first set of sub sites of site collection 1 and thread B will process the remaining. If the timer job logic is dealing with sub site settings only that's fine, but if the timer job logic is also working with the root web (using the `SiteClientContext`) then there might be a potential concurrency issue given that both thread A and B will be updating the same root web. To avoid this you can perform the sub site expanding in your timer job implementation instead of having the framework do it for you. To make this easy the timer job framework exposes the **GetAllSubSites** method. Below code snippet shows how you can use this:

```C#
public class SiteCollectionScopedJob: TimerJob
{
    public SiteCollectionScopedJob() : base("SiteCollectionScopedJob")
    {
        // ExpandSites *must* be false as we'll deal with that at TimerJobEvent level
        ExpandSubSites = false;
        TimerJobRun += SiteCollectionScopedJob_TimerJobRun;
    }

    void SiteCollectionScopedJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
    {
        // Get all the sub sites in the site we're processing
        IEnumerable<string> expandedSites = GetAllSubSites(e.SiteClientContext.Site);

        // Manually iterate over the content
        foreach (string site in expandedSites)
        {
            // Clone the existing ClientContext for the sub web
            using (ClientContext ccWeb = e.SiteClientContext.Clone(site))
            {
                // Here's the timer job logic, but now a single site collection is handled in a single thread which 
                // allows for further optimization or prevents race conditions
                ccWeb.Load(ccWeb.Web, s => s.Title);
                ccWeb.ExecuteQueryRetry();
                Console.WriteLine("Here: {0} - {1}", site, ccWeb.Web.Title);
            }
        }
    }
}
```

### Logging ###
The timer job framework uses the PnP Core logging components as it's part of the PnP Core library. To activate the built-in PnP Core logging you simply need to configure it using your config file (app.config / web.config). Below sample shows the needed syntax:

```XML
  <system.diagnostics>
    <trace autoflush="true" indentsize="4">
      <listeners>
        <add name="DebugListenter" type="System.Diagnostics.TextWriterTraceListener" initializeData="trace.log" />
        <!--<add name="consoleListener" type="System.Diagnostics.ConsoleTraceListener" />-->
      </listeners>
    </trace>
  </system.diagnostics>
```

Using the above configuration file the timer job framework will use the out of the boc tracelistener `System.Diagnostics.TextWriterTraceListener` to write logs to a file called trace.log in the same folder as the timer job .exe. Obviously you can also use other tracelisteners like there are:
- **ConsoleTraceListener** writes logs to the console (= out of the box)
- See https://msdn.microsoft.com/en-us/magazine/ff714589.aspx (uses Microsoft.WindowsAzure.Diagnostics.**DiagnosticMonitorTraceListener**) for more information about logging and tracing in Azure. Additional Azure resources can be found here:
    - [Enable diagnostic logging for Azure Websites](http://azure.microsoft.com/en-us/documentation/articles/web-sites-enable-diagnostic-log/)
    - [Troubleshooting Azure Websites in Visual Studio](http://azure.microsoft.com/en-us/documentation/articles/web-sites-dotnet-troubleshoot-visual-studio/)

We explained how to get the timer job framework to log data but it's strongly advised that can also use the same logging approach for your custom timer job code. In your timer job code you can use the PnP Core `Log` class:

```C#
void SiteGovernanceJob_TimerJobRun(object o, TimerJobRunEventArgs e)
{
    try
    {
        string library = "";

        // Get the number of admins
        var admins = e.WebClientContext.Web.GetAdministrators();

        Log.Info("SiteGovernanceJob", "ThreadID = {2} | Site {0} has {1} administrators.", e.Url, admins.Count, Thread.CurrentThread.ManagedThreadId);

        // Additional timer job logic...

        e.CurrentRunSuccessful = true;
        e.DeleteProperty("LastError");
    }
    catch(Exception ex)
    {
        Log.Error("SiteGovernanceJob", "Error while processing site {0}. Error = {1}", e.Url, ex.Message);
        e.CurrentRunSuccessful = false;
        e.SetProperty("LastError", ex.Message);
    }
}
```


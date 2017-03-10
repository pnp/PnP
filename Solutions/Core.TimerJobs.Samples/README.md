# Timer job framework #

### Summary ###
This solution shows how you can use the PnP Core timer job framework to create your own timer jobs. 10 different timer job samples will explain you the ins and outs of the PnP Core timer job framework.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.TimerJobs.Samples | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | June 23rd 2015 | Additional sample showing how to use the tenant API in a timer job
1.0  | February 13th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
The PnP timer job framework is set of classes designed to ease the creation of background processes that operate against SharePoint sites, kind of similar to what full trust code timer jobs (`SPJobDefinition`) are for an on-premises SharePoint deployment. The big difference with between this timer job framework and the out of the box one is that this one only uses client side API's and as such can (and should) be run outside of SharePoint. This makes it possible to build timer jobs that operate against SharePoint Online. 

For a detailed view on how the timer job framework can be used, how to deploy timer jobs and learn everything about the internals of the timer job framework consult the [PnP Core documentation](https://github.com/OfficeDev/PnP-Sites-Core/blob/dev/Core/TimerJob%20Framework.md). 

Below documentation describes each sample and focuses on the unique elements in the sample. You'll notice that the samples below slowly build up in complexity, so if you want to learn about the timer framework you should read this sequentially. If you already understand how things work you better jump to the sample you need.

Each sample documentation first explain the timer job itself and next the hosting and running of the timer job is discussed.

## Videos on the timer job framework ##
On the Office 365 Developer Patterns & Practices video channel (aka.ms/officedevpnpvideos) there are the following videos available:
- [Introduction to the timer job framework](http://channel9.msdn.com/blogs/OfficeDevPnP/Introduction-to-the-PnP-timer-job-framework): this video explains about the "why" and "what" of the timer job framework and shows a simple demo.


# Sample 1: SimpleJob #
## Goal ##
Hello world type sample...it can't really be any simpler :-)

## Timer job implementation ##
This job will simply request the site title and display it. When you create your timer job you'll need to follow 3 simple steps:

1. Have your timer job class inherit from the `TimerJob` abstract base class
2. Provide a name for your timer job in the constructor
2. Create an event handler for the `TimerJobRun` event and add your timer logic in that event handler

```C#
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
```

## Timer job host implementation ##
For this first sample all the steps are shown and explained:

1. Instantiate your timer job
2. A timer job needs credentials: this sample uses Office 365 as target platform and a user and password is provided
3. You need to tell the job on which sites it needs to work: in this simple sample it's just on one single site added via the `AddSite` method
4. Final step is running the job: `PrintJobSettingsAndRunJob` prints the current settings and then calls `job.Run()` to invoke the job execution.

```C#
static void Main(string[] args)
{
    // Instantiate the timer job class
    SimpleJob simpleJob = new SimpleJob();
    
    // The provided credentials need access to the site collections you want to use
    simpleJob.UseOffice365Authentication(User, Password);

    // In case of SharePoint on-premises use
    //simpleJob.UseNetworkCredentialsAuthentication(User, Password, Domain);
    
    // Add one or more sites to operate on
    simpleJob.AddSite("https://bertonline.sharepoint.com/sites/dev");
    
    // Print timer job information and then call Run() on the job
    PrintJobSettingsAndRunJob(simpleJob);
}
```

# Sample 2: ExpandJob #
## Goal ##
Shows how you have the timer job framework expand sub sites for you. This is useful when your timer job logic needs to apply to all sub sites.

## Timer job implementation ##
To make the timer job framework expand sub sites you'll just need to set the **ExpandSubSites** property to true (default = false). Sub site expanding follows the same threading settings as the actual job execution: if threading is enabled (=default) the sub site expanding also is done in a multi-threaded manner. In the below sample the amount of threads is reduced to 3 (5 is the default value).

The actual job implementation simply shows the title from the root site and the actual (sub) site being processed. Also note that in this sample the timer job version is set as part of the timer job constructor.

```C#
public ExpandJob() : base("ExpandJob", "2.0") 
{
    // We want to operate at sub site level, so let's have the timer framework expand all sub sites
    ExpandSubSites = true;
    // Only use 3 threads instead of the default of 5
    MaximumThreads = 3;
    TimerJobRun += ExpandJob_TimerJobRun;
}

void ExpandJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
{
    // Read the title from the site being processed
    e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
    e.WebClientContext.ExecuteQueryRetry();

    // Read the title from the root site of the site being processed
    e.SiteClientContext.Load(e.SiteClientContext.Web, p => p.Title);
    e.SiteClientContext.ExecuteQueryRetry();

    Console.WriteLine("Root site of site {0} has title {1}", e.Url, e.SiteClientContext.Web.Title);
    Console.WriteLine("Sub site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
}
```

## Timer job host implementation ##
The timer job host code is similar to sample 1, the only difference is that now a **wild card url** is specified:

```C#
// Add one or more sites to operate on. Sites can contain a * wildcard as last character
expandJob.AddSite("https://bertonline.sharepoint.com/sites/d*");
```

The net result of this is that the timer framework will enumerate the existing sites and match the resulting list with the wild card.

# Sample 3: ExpandJobAppOnly #
## Goal ##
Similar to sample 2, but now using add-in only authentication

## Timer job implementation ##
Identical to sample 2.

## Timer job host implementation ##
This job uses add-in only authentication and as such the `UseAppOnlyAuthentication` call is used to provide the job with a clientId and clientSecret. Once you use add-in only authentication and you specify one or more wild card url's (like below) you'll also need to set enumeration credentials: these credentials will be used by the search API to acquire a list of sites which will then be matched with the wild card urls

```C#
// specify the needed information to work add-in only
expandJobAppOnly.UseAppOnlyAuthentication(ClientId, ClientSecret);

// set enumeration credentials to allow using search API to find the OD4B sites
expandJobAppOnly.SetEnumerationCredentials(User, Password);

// In case of SharePoint on-premises use
//expandJobAppOnly.SetEnumerationCredentials(User, Password, Domain);

// Add one or more sites to operate on            
expandJobAppOnly.AddSite("https://bertonline.sharepoint.com/sites/2014*");
expandJobAppOnly.AddSite("https://bertonline-my.sharepoint.com/personal/*");
```

# Sample 4: CredentialManagerJob #
## Goal ##
Providing either user credentials or a clientId/clientSecret combination means that you would need to secure this data. Simply adding it the app.config works, but if the app.config gets compromised so are your credentials. The Windows Credential Manager can be a solution here assuming your timer job runs on a Windows Server. This method will not work for an Azure WebJob.


## Timer job implementation ##
Identical to sample 1.

## Timer job host implementation ##
Below code will read the **bertonline** generic credential from the Windows Credential Manager and use the user name and password information obtained from the credential.

```C#
// The provided credentials need access to the site collections you want to use
// Use generic credential stored in the windows credential manager (control panel -> credential manager)
// This makes is possible to safely store the username + password when running on Windows Server
simpleJob.UseOffice365Authentication("bertonline");
```

# Sample 5: GovernanceJob #
## Goal ##
This shows a real life scenario where the company wants to enforce that all site collections have 2 admins. The timer job will iterate the site collections and if not compliant it will add a custom action that triggers a JavaScript which on it's turn shows a "your site is not compliant" warning to the user. 

Besides the real life business scenario this sample also shows how to use the timer job state management features.

## Timer job implementation ##
The timer job logic in this sample is using the PnP Core extension methods which makes things so much easier. Methods like `GetAdministrators`, `GetListByUrl`, `UploadFile`, `AddJsLink` and `DeleteJsLink` all are coming from PnP Core.

What's specific in this timer job sample is the state management: the property **ManageState** has been set to true which makes that the timer job framework stores information about the timer job run (=state). This state data is stored as a JSON serialized text in a single web property named *<timerjobname>_properties*. For this sample that means that the property is called *SiteGovernanceJob_properties*. To learn everything about managing state using the timer job framework check out the PnP core documentation over [here](https://github.com/OfficeDev/PnP-Sites-Core/blob/dev/Core/TimerJob%20Framework.md#state-management), but in a nutshell you'll need to know that:
- The **last run (DateTime), timer job version (string) and successful run (boolean)** are always stored after the run. If your code processes the same site the next time these properties are automatically available. 
- As a timer job author you are responsible for setting **CurrentRunSuccessful** to true when the timer job did run successful
- As a timer job author you can store your own property/value pairs in the state and easily retrieve them afterwards. Simply use the `GetProperty`, `SetProperty` and `DeleteProperty` methods on the `TimerJobRunEventArgs` object like shown in below sample

```C#
public SiteGovernanceJob () : base ("SiteGovernanceJob")
{
    TimerJobRun += SiteGovernanceJob_TimerJobRun;
    ManageState = true;
    UseThreading = true;
}

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
        Log.Error("SiteGovernanceJob", "Error while processing site {0}. Error = {1}", e.Url, ex.Message);
        e.CurrentRunSuccessful = false;
        e.SetProperty("LastError", ex.Message);
    }
}
```

## Timer job host implementation ##
Nothing specific here, it's using add-in only like the previous sample.

# Sample 6: ContentTypeRetentionEnforcementJob #
## Goal ##
This sample implements the [Governance.ContentTypeEnforceRetention](https://github.com/OfficeDev/PnP/tree/master/Solutions/Governance.ContentTypeEnforceRetention) solution using the timer job model. When you compare this exising solution with the timer job equivalent you'll notice how easy it is to port existing code into the new model.

## Timer job implementation ##
Please check out the documentation of the [Governance.ContentTypeEnforceRetention](https://github.com/OfficeDev/PnP/tree/master/Solutions/Governance.ContentTypeEnforceRetention) solution.

## Timer job host implementation ##
The timer job host is fairly standard, the only new thing is the timing code. You can use this timing code (`StopWatch` class) to experimentally determine the ideal number of threads.

```C#
Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();

// Enable logging in app.config by uncommenting the debugListener
PrintJobSettingsAndRunJob(contentTypeRetentionEnforcementJob);

stopWatch.Stop();
Console.WriteLine("Total elapsed time = {0}", stopWatch.Elapsed); 
```

# Sample 7: OverrideJob #
## Goal ##
This sample explains how the timer job implementation can fully control the sites that are typically being fed by the timer job host. This is useful when the timer job gets the sites from another location (e.g. SQL database, config file,...) or when you want to *update* the provided list of sites.

## Timer job implementation ##
Below code snippet shows the 2 methods that can be used to override the sites provided by the timer job host. A first level is overriding the provided list of sites (regular sites urls or wildcard urls) by overriding the `UpdateAddedSites` virtual method. 

A second override is doing an override on the site resolving: site resolving will transform a list of (wild card) site urls into a list of sites and sub sites (when site expanding was selected). Using the `ResolveAddedSites` virtual method you intervene here: in the sample we'll initially use the out of the box resolving but then the resulting list of sites is manipulated. You can also use this method to read sites from another location (e.g. SQL database, config file,...).

```C#
/// <summary>
/// This virtual method is executed by the timerjob framework before it starts with site resolving. 
/// The idea here is add your own sites at this point instead of the one provided by the TimerJob caller
/// </summary>
/// <param name="addedSites">Current list of added sites</param>
/// <returns>New list of added sites</returns>
public override List<string> UpdateAddedSites(List<string> addedSites)
{
    // Let's assume we're not happy with the provided list of sites, so first clear it
    addedSites.Clear();
    // Manually adding a new wildcard Url, without an added URL the timer job will do...nothing 
    addedSites.Add("https://bertonline.sharepoint.com/sites/d*");

    // Return the updated list of sites
    return addedSites;
}

/// <summary>
/// This virtual method is used for resolving sites (= going from wildcard to actual list of sites and/or enumerating 
/// the sub sites). Use this method to either provide your own list of sites and/or sub sites or for manipulating 
/// the default generated list (e.g. adding or removing some sites)
/// </summary>
/// <param name="addedSites">List of sites to resolve</param>
/// <returns>Resolved set of sites</returns>
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

## Timer job host implementation ##
Identical to sample 1.


# Sample 8: NoThreadingJob #
## Goal ##
Simple sample to show how to not use multi-threading

## Timer job implementation ##
Only important new thing here is setting the **UseThreading** property to false.

```C#
public NoThreadingJob(): base("NoThreadingJob")
{
    // Default is to use threading, so explicitely set it to false
    UseThreading = false;
    ExpandSubSites = true;
    
    // Inline delegate sample
    TimerJobRun += delegate(object sender, TimerJobRunEventArgs e)
    {
        e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
        e.WebClientContext.ExecuteQueryRetry();
        ThreadingDebugInformation();
        Console.WriteLine("NoThreadingJob: Site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);
    };
}
```

## Timer job host implementation ##
Identical to sample 1.

# Sample 9: SiteCollectionScopedJob #
## Goal ##
When you've opted to expand sub sites and you use multi-threading the timer framework will have built up a list of site and sub sites which will be evenly split in work batches (one per thread). This means that thread A can process the first set of sub sites of site collection 1 and thread B will process the remaining. If the timer job logic is dealing with sub site settings only that's fine, but if the timer job logic is also working with the root web (using the `SiteClientContext`) then there might be a potential concurrency issue given that both thread A and B will be updating the same root web.

## Timer job implementation ##
To avoid these concurrency issues you can perform the sub site expanding in your timer job implementation instead of having the framework do it for you. To make this easy the timer job framework exposes the **GetAllSubSites** method. 

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

## Timer job host implementation ##
Identical to sample 1.

# Sample 10: TenantAPIJob #
## Goal ##
This sample's purpose is to show you how the SharePoint Tenant API can be used in a timer job. 
## Timer job implementation ##
In the `TimerJobRun` event handler you can construct a `Tenant` class via providing it the correct `ClientContext` object via the `TimerJobRunEventArgs.TenantClientContext` property. Below code shows this:

```C#
public class TenantAPIJob: TimerJob
{
    public TenantAPIJob()
        : base("TenantAPIJob", "1.0")
    {
        TimerJobRun += TenantAPIJob_TimerJobRun;
    }

    void TenantAPIJob_TimerJobRun(object sender, TimerJobRunEventArgs e)
    {
        Tenant t = new Tenant(e.TenantClientContext);
        var sites = t.GetSiteProperties(0, true);
        e.TenantClientContext.Load(sites);
        e.TenantClientContext.ExecuteQueryRetry();

        foreach(var site in sites)
        {
            Console.WriteLine(site.Template);
        }

    }
}
```

## Timer job host implementation ##
Identical to sample 3.

# Sample 11: ChainingJob #
## Goal ##
A more theoretical example, but still might be valuable...showing how you can call another timer job from an existig one.

## Timer job implementation ##
In the `TimerJobRun` event handler you can call another timer job. To do this you'll need to take in account the following 3 steps:

1. Set the **UseThreading** of the job being called to false. Threaded jobs spawning again threaded jobs is not working well and also doesn't make any sense as you're typically processing a single site anyway
2. Use the **Clone** method to copy the existing jobs authentication settings to the timer job you want to call
3. Add a site to the called timer job. Typically this will be the site the current timer job is processing

```C#
public ChainingJob(): base("ChainingJob")
{
    TimerJobRun += delegate(object sender, TimerJobRunEventArgs e)
    {
        e.WebClientContext.Load(e.WebClientContext.Web, p => p.Title);
        e.WebClientContext.ExecuteQueryRetry();
        Console.WriteLine("Site {0} has title {1}", e.Url, e.WebClientContext.Web.Title);

        // Chain another job in this job
        NoThreadingJob noThreadingJob = new NoThreadingJob();
        // Threading inside threaded executions is not supported...override the value set in the original job constructor
        noThreadingJob.UseThreading = false;
        // Take over authentication settings from calling job
        noThreadingJob.Clone(this);
        // Add the site Url we're currently processing in this task
        noThreadingJob.AddSite(e.Url);
        // Run...
        noThreadingJob.Run();

    };
}
```

## Timer job host implementation ##
Identical to sample 1.

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Core.TimerJobs.Samples" />
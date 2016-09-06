# # Logging with OfficeDevPnP.Core #

### Summary ###
In this sample, a SharePoint provider hosted add-in and an Azure Web Job are used to demonstrate the flexibility and rich output of OfficeDevPnP.Core trace logging functionality.  The PHA web site has a single page with a few buttons that write trace output, the Web Job calls out looking for the "Site Pages" list and uses monitored scope to keep track of the time it takes to make the calls to SharePoint.   

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###

Solution | Author(s)
---------|----------
Diagnostics.Logging | Daniel Budimir

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 30th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# Guide to using Logging with OfficeDevPnP.Core #

The officeDevPnP Core framework uses its built-in tracing framework, which defaults to the standard Event Tracing framework of .NET.  Using PnP tracing is as simple as adding a reference to the OfficeDevPnP.Core assembly and calling the Logging functions.  In the future many core methods in OfficeDevPnP.Core will use this trace    output debug messages to the logging framework.  We'll start here and work our way up to some of the  more robust logging offerings later.  To get started, add the following text to your config file to add a Trace listener for the console.
```xml
<configuration>
	<system.diagnostics>
	    <sharedListeners>
	      	<add name="console" type="System.Diagnostics.ConsoleTraceListener" />
	    </sharedListeners>
	    <trace indentsize="0" autoflush="true">
	      	<listeners>
	        	<add name="console" />
	      	</listeners>
	    </trace>
	</system.diagnostics>
</configuration>
```

Okay now we're ready to hava a look at some trace output.  Consider the default output of the .Net Trace
```csharp
System.Diagnostics.Trace.TraceInformation("test message"); 
```

Output:

	SPLoggerDemo.vshost.exe Information: 0 : test message 

#### Enter PnP Logging ####

Now have a look at the information provided by PnP Logging 
```csharp
OfficeDevPnP.Core.Diagnostics.Log.Info("MyFunction", "test message");
```

Output:

	SPLoggerDemo.vshost.exe Information: 0 : 2015-09-28 17:55:04.0211       [MyFunction]    [0]     [Information]   test message    0ms

Notice the addition of a Source field, time-stamp and also milliseconds count that we'll have a look at in a bit.  One cool thing is that everything  is tab delimited so you can pull your file up in Excel and easily plow though the data.

Now lets add a little more to our config file so if you want a more or less detailed log you can set the level, the logLevel attribute accepts "Debug", "Error", "Warning" or "Information", be sure to add at the top of the app.config or web.config file straight after the ```<Configuration>``` tag. 

```xml
<configuration>
	<configSections>
	    <sectionGroup name="pnp">
	      	<section
			    name="tracing"
			    type="OfficeDevPnP.Core.Diagnostics.LogConfigurationTracingSection, OfficeDevPnP.Core"
			    allowLocation="true"
			    allowDefinition="Everywhere"
			    />
		</sectionGroup>
	<!-- Other <section> and <sectionGroup> elements. -->
	</configSections>
	<pnp>
		<tracing logLevel="Debug">
	      	<logger type="OfficeDevPnP.Core.Diagnostics.TraceLogger, OfficeDevPnP.Core, Culture=neutral, PublicKeyToken=null" />
	    </tracing>
	</pnp>
	<system.diagnostics>
	    <sharedListeners>
	      	<add name="console" type="System.Diagnostics.ConsoleTraceListener" />
	    </sharedListeners>
	    <trace indentsize="0" autoflush="true">
	      	<listeners>
	        	<add name="console" />
	      	</listeners>
	    </trace>
	</system.diagnostics>
</configuration>
```


##Using the PnP Logging Framework Monitored Scope

One of the really great things about PnP logging is monitored scope, the monitored scope will keep a running list of the time elapsed between each call to the scopes log function.   At the start and end of the scope a debug message will be written to the log stating the name and time it took to execute the scope.  This will enable you to easily see how long your calls to retrieve data are taking and find bottlenecks in your code.

```csharp
using (var scope = new PnPMonitoredScope("My Scope"))
{
	scope.LogInfo("Starting sleep");
	System.Threading.Thread.Sleep(2000);
	scope.LogInfo("Ending sleep");
}
```

Output:

	SPLoggerDemo.vshost.exe Information: 0 : 2015-10-01 15:59:51.8878       [My Scope]      [9]     [Debug] Code execution scope started    1ms     eba94e84-5891-4939-b90f-075fad1c76e0
	SPLoggerDemo.vshost.exe Information: 0 : 2015-10-01 15:59:53.7943       [My Scope]      [9]     [Information]   Starting sleep  1926ms  eba94e84-5891-4939-b90f-075fad1c76e0
	SPLoggerDemo.vshost.exe Information: 0 : 2015-10-01 15:59:56.9868       [My Scope]      [9]     [Information]   Ending sleep    5119ms  eba94e84-5891-4939-b90f-075fad1c76e0
	SPLoggerDemo.vshost.exe Information: 0 : 2015-10-01 15:59:58.1284       [My Scope]      [9]     [Debug] Code execution scope ended      6260ms  eba94e84-5891-4939-b90f-075fad1c76e0


Now let's take the next step a publish the project out to o365 and Azure and have a look at our output in the cloud.  Follow the instructions here to configure your Azure web site to catch the logging output and save it to a file located where you can access it through ftp.

[Enable diagnostics logging for web apps in Azure App Service](https://azure.microsoft.com/en-us/documentation/articles/web-sites-enable-diagnostic-log/ "Enable diagnostics logging for web apps in Azure App Service")

Setup the demo: 

In Diagnostics.LoggingWebJob be sure to update these values with you SharePoint information and the storage keys you got from following the article above.  

```xml
<appSettings>
	<add key="SpUrl" value=""/>
	<add key="SpUsername" value=""/>
	<add key="SpPassword" value=""/>
</appSettings>
<connectionStrings>
	<!-- The format of the connection string is "DefaultEndpointsProtocol=https;AccountName=NAME;AccountKey=KEY" -->
	<!-- For local execution, the value can be set either in this config file or through environment variables -->
	<add name="AzureWebJobsDashboard" connectionString=""/>
	<add name="AzureWebJobsStorage" connectionString=""/>
</connectionStrings>
```

In Diagnostics.LoggingWeb be sure to update the ClientId and ClientSecret with the values you got from registering your provider hosted add-in with SharePoint.

After publishing and navigating to the add-in click the buttons a few time to write some tracing output then connect using your favorite ftp client to view the output.

![FTP client with view to log files](http://i.imgur.com/EdpxQVH.png)

In the next update we'll demonstrate how to send and retrieve your logging data to Azure Table and Blob Storage.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Diagnostics.Logging" />
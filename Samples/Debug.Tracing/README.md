# Tracing and error logging for provider hosted apps #

### Summary ###
This sample shows how to use tracing and error logging in Sharepoint provider hosted apps. Tracing can give you additional information about errors that occur on a production environment (where debugging usually is not a option). This sample also shows how you can log a error to sharepoint, that shows up at the add-in detail page.

### Solution ###
Solution | Author(s)
---------|----------
Debug.Tracing | Stijn Neirinckx 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 20th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# How to setup error logging #
The errorlogger class is included in the solution and can be used to log a errormessage or exception to the Sharepoint add-in details page.

## STEP 1: setup productid for errorlogger ##
The errorlogger needs to pass the productid of the add-in to sharepoint. You can configure this in the web.config file. Just add the ErrorLoggerAppProductId appsetting to the appsettings section. The value of this parameter has to be the productid of your add-in. You can find the productId in the appmanifest.xml file (open this in xml view).

```XML
<appSettings>
    <add key="ClientId" value="d65c295e-9c43-48d3-9c37-499fdbbdca19" />
    <add key="ClientSecret" value="0aCClZqOC3yEWzp52l3Xxsi0YvLHZVZksULu8xieANY=" />
    <add key="ErrorLoggerAppProductId" value="{b4351824-86ea-41f0-b29c-1605b159e4f0}" /> <!-- This is needed to help the errorlogger log the error to Sharepoint -->
  </appSettings>
```

## STEP 2: add a try catch block with errorlogger ##
Add a try catch block to the eventhandler of your page, where you want to catch exceptions. This way, if a exception is thrown in the eventhandler, or in any of the classes or methods used by the eventhandler, you can log it, and are able to show a decent errormessage or page to the user.


```JavaScript
protected void LogError_Click(object sender, EventArgs e)
        {
            try
            {
               //do some logic here

                throw new Exception("Something bad happened here");
            }
            catch (Exception eX)
            {
                ErrorLogger.LogException(eX); 
                //It can take a couple of minutes for the error to show up in the sharepoint screen

                //show some error message to the user...
            }
        }
``` 

It is also possible to handle all unexpected exceptions in your application. You can do this by adding the following code to the global.asax file.

```JavaScript
protected void Application_Error(object sender, EventArgs e)
        {
            Exception eX = Server.GetLastError().GetBaseException(); //get exception
            ErrorLogger.LogException(eX); //log it to trace.axd and Sharepoint
        }
``` 

## STEP 3: where can I find the error ##
The error is logged in the add-in details page. You can get there by the following steps:

![Details page for add-in](http://i.imgur.com/SnMcwfw.png)

Go to site contents. Click on the ... next to your add-in. Click details

![Add-in details page](http://i.imgur.com/n6lshti.png)

Here you can see the number of runtime errors. Click on the link to see the error.

![Example runtime error](http://i.imgur.com/vFrJiYx.png)

The error will also show up in the add-in tracing. More info about how to get to this tracing is in the next section.

![Trace information](http://i.imgur.com/x80isGy.png)

# How to use tracing #

## How to enable tracing ##
Add the following to the system.web section of your web.config file:

```XML
<trace enabled="true" localOnly="false" requestLimit="100" /> <!-- Configure tracing to be active -->  
```

You can also enable or disable tracing from code:

```JavaScript
public static void EnableTracing()
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
            section.Enabled = true;
            section.LocalOnly = false;
            configuration.Save();
        }

        public static void DisableTracing()
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            TraceSection section = (TraceSection)configuration.GetSection("system.web/trace");
            section.Enabled = false;
            section.LocalOnly = false;
            configuration.Save();
        }
```

Keep in mind that there is a small performance penalty by using tracing. Also watch out when you enable/disable tracing from code, because this will trigger your web application to restart.

## How to trace messages ##
By default all the out of the box methods in a aspx page are traced. But you can add additional messages, for example to trace certain info, or progress. You can trace the messages by using HttpContext.Current.Trace, or use the wrapper included in this solution:

```JavaScript
protected void TraceMessage_Click(object sender, EventArgs e)
        {
            TraceUtil.TraceMessage("Starting some work");  //log message to trace.axd
            
            //Do some work here (step 1)

            TraceUtil.TraceMessage("work step 1 complete");  //log your work progress

            //Do some work here (step 2)

            TraceUtil.TraceMessage("work complete");  //log that your work completed
        }
``` 


## How to trace methods ##
If an error occurs you want as much information as possible. If you add the method trace, the traceUtil will log every start and end of a method. This way you can see what happened prior to the error. The traceUtil can also log parameters, if you specify those. This way you have all the information to reproduce the error at your development environment. 

Since the start and end time of every method is logged, you can also use tracing to see what parts of your code are running slow on production in case of performance problems.

U can use the methodtracer like this:

```JavaScript
protected void TraceMethods_Click(object sender, EventArgs e)
        {
            using (new TraceUtil().TraceMethod(sender, e))  //this logs the beginning and end of the method to trace.axd
            {
                //do some logic here

                //use class to execute logic
                new SomeClass().SomeMethod1("parameter1", "param2");
            }
        }
``` 

## How to view tracing ##
Go to pages/trace.axd. If tracing is enabled, you will get the following screen:

![Application Trace view](http://i.imgur.com/ekp1vgH.png)

This screen shows all the requests to your web application, with their status. If you click on View Details, you can see detailed information about the request, including the tracing.

This screen shows custom trace messages.

![Request details](http://i.imgur.com/QVGTOS1.png)

This screen shows method tracing.

![Method tracing](http://i.imgur.com/7cb3795.png)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Debug.Tracing" />
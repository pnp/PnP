# SharePoint Online Throttling #

### Summary ###
This sample shows pattern on how to handle possible SharePoint Online throttling which could be applied for CSOM, REST and web service operations in the SharePoint Online. 

*Notice that sample contains code which could cause performance issues for your tenant, so it is not really intended to be executed as such, rather to be used as a reference code.*

### Applies to ###
-  Office 365 Multi Tenant (MT)

*Similar throttling does exist in on-premises as well, but this code is using SPO credentials for authentication, so it is not suitable for on-premises as such.*

### Prerequisites ###
none

### Solution ###
Solution | Author(s)
---------|----------
Core.Throttling | Shyam Narayan (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 1st 2014 | Initial release with documentation by Vesa Juvonen

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
Following picture explains the key elements in the throttling. 

![Throttling elements](http://i.imgur.com/mlxg5wx.png)

1. All CSOM, REST and web services calls are under the monitoring
2. If there are too many requests coming from one account in certain time period, request could get throttled. During this time all requests will fail with http status 429. 

Please check more information from following MSDN article:
- [How to avoid getting throttled on SharePoint Online](https://msdn.microsoft.com/en-us/library/dn878981.aspx)


# Handling the throttling in the code #
Following code example shows how to mimic throttling. In this case we are creating new folder to given document library in loop for 1000 times. This will hit the throttling limits and without any additional code considerations, request would fail. 

Code does however take advantage of new **ExecuteQueryWithIncrementalRetry** extension method, which is reference implementation on how to handle the throttling in your CSOM code.  

```C#
static void Main(string[] args)
{
    string serverUrl = "<URL>";
    String login = "<USERNAME>";
    String password = "<PASSWORD>";
    string listUrlName = "Shared%20Documents";

    using (var ctx = new ClientContext(serverUrl))
    {
        //Provide account and pwd for connecting to the source
        var passWord = new SecureString();
        foreach (char c in password.ToCharArray()) passWord.AppendChar(c);
        ctx.Credentials = new SharePointOnlineCredentials(login, passWord);

        try
        {
            int number = 0;
            // This loop will be executed 1000 times, which will cause throttling to occur
            while (number < 1000)
            {
                // Let's try to create new folder based on Ticks to the given list as an example process
                var folder = ctx.Site.RootWeb.GetFolderByServerRelativeUrl(listUrlName);
                ctx.Load(folder);
                folder = folder.Folders.Add(DateTime.Now.Ticks.ToString());
                // Extension method for executing query with throttling checks
                ctx.ExecuteQueryWithIncrementalRetry(5, 30000); //5 retries, with a base delay of 10 secs.
                // Status indication for execution.
                Console.WriteLine("CSOM request successful.");
                // For loop handling.
                number = number + 1;
            }
        }
        catch (MaximumRetryAttemptedException mex)
        {
            // Exception handling for the Maximum Retry Attempted
            Console.WriteLine(mex.Message);
        }
    }
}
```

Following code is actually showing the details related on the ***ExecuteQueryWithIncrementalRetry*** extension method for the **ClientContext** object

```C#
// This is the extension method. 
// The first parameter takes the "this" modifier
// and specifies the type for which the method is defined. 
/// <summary>
/// Extension method to invoke execute query with retry and incremental back off.
/// </summary>
/// <param name="context"></param>
/// <param name="retryCount">Maximum amount of retries before giving up.</param>
/// <param name="delay">Initial delay in milliseconds.</param>
public static void ExecuteQueryWithIncrementalRetry(this ClientContext context, int retryCount, int delay)
{
    int retryAttempts = 0;
    int backoffInterval = delay;
    if (retryCount <= 0)
        throw new ArgumentException("Provide a retry count greater than zero.");

    if (delay <= 0)
        throw new ArgumentException("Provide a delay greater than zero.");

    // Do while retry attempt is less than retry count
    while (retryAttempts < retryCount)
    {
        try
        {
            context.ExecuteQuery();
            return;

        }
        catch (WebException wex)
        {
            var response = wex.Response as HttpWebResponse;
            // Check if request was throttled - http status code 429
            // Check is request failed due to server unavailable - http status code 503
            if (response != null && (response.StatusCode == (HttpStatusCode)429 || response.StatusCode == (HttpStatusCode)503))
            {
                // Output status to console. Should be changed as Debug.WriteLine for production usage.
                Console.WriteLine(string.Format("CSOM request frequency exceeded usage limits. Sleeping for {0} seconds before retrying.", 
                                backoffInterval));

                //Add delay for retry
                System.Threading.Thread.Sleep(backoffInterval);

                //Add to retry count and increase delay.
                retryAttempts++;
                backoffInterval = backoffInterval * 2;
            }
            else
            {
                throw;
            }
        }
    }
    throw new MaximumRetryAttemptedException(string.Format("Maximum retry attempts {0}, has be attempted.", retryCount));
}
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.Throttling" />
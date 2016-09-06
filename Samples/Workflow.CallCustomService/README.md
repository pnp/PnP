# Call custom web services from a workflow #

### Summary ###
This sample shows how to create a workflow that calls a custom web service that updates SharePoint list data.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Workflow.CallCustomService | Todd Baginski, Tyler Lu, Ring Li (**Canviz LLC**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 6th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: CALL CUSTOM WEB SERVICE #
This provider-hosted sample application for SharePoint demonstrates how to create a workflow that calls a custom web service that updates SharePoint list data.

## WEB API SERVICE ##
In this code sample, we use *DataController* controller. It contains a method called Post which is used to handle the workflow’s http post request.

*Post* method calls the [Northwind OData service](http://services.odata.org/V3/Northwind/Northwind.svc) to get the supplier names of the specified country in the list item the workflow is invoked upon. Then, it writes the supplier names back to the *Suppliers* column in the *Part Suppliers* SharePoint list in the add-in-web.

```C#
public class DataController : ApiController
{
    public void Post([FromBody]string country)
    {
        //...
    }
}
```

### CALL NORTHWIND ODATA SERVICE ###
To call the Northwind OData Service, a Service Reference has been added to the web service.  To add a Service Reference to a provider-hosted web project, in the Solution Explorer, right click the **References** node in the provider-hosted web project then click **Add Service Reference**.

![Adding service reference](http://i.imgur.com/MSB7fTU.png)

Then, in the Add Service Reference dialog, type the address of the service you wish to reference.  In this example, the URL is:

http://services.odata.org/V3/Northwind/Northwind.svc.

In this sample, the Namespace is Northwind.

After the URL and Namespace are added, click **Go**. At this point, Visual Studio reads the OData metadata document to discover the entities in the service.

Finally, click **OK** to add the proxy class to your project.

![Proxy class creation](http://i.imgur.com/bz3ZW0i.png)

Here you can see the Northwind OData Service after it has been added to the provider-hosted web project.

![Service referene list in Visual Studio project view](http://i.imgur.com/Pm0KsB8.png)

After the service reference has been added, we can use LINQ to get the supplier names for a specified country.  This is shown in the code snippet below.

```C#
using Workflow.CallCustomServiceWeb.Northwind;

// DataController
private string[] GetSupplierNames(string country)
{
    Uri uri = new Uri("http://services.odata.org/V3/Northwind/Northwind.svc");
    var entities = new NorthwindEntities(uri);
    var names = entities.Suppliers
        .Where(s => s.Country == country)
        .AsEnumerable()
        .Select(s => s.CompanyName)
        .ToArray();
    return names;
}
```

### UPDATE SHAREPOINT LIST ITEM ###
In order to connect to SharePoint and update a list item, the Web API needs a *context token*. The *context token* and *add-in web URL* will be sent to the web API via http header by the workflow.

```C#
// DataController
private void UpdateSuppliers(string country, string[] supplierNames)
{
    var request = HttpContext.Current.Request;
    var authority = request.Url.Authority;
    var spAppWebUrl = request.Headers["SPAppWebUrl"];
    var contextToken = request.Headers["SPContextToken"];

    using (var clientContext = TokenHelper.GetClientContextWithContextToken(spAppWebUrl, contextToken, authority))
    {
        var service = new PartSuppliersService(clientContext);
        service.UpdateSuppliers(country, supplierNames);
    }
}
```

## WORKFLOW ##
### WORKFLOW ARGUMENTS ###
The workflow needs to send the *context token and add-in web URL* to the web API.  To send these values to the Web API these two values are passed to the workflow when it starts. The *web API’s URL* is also passed to the workflow upon startup.  Three arguments are created in the workflow to receive these values. 

![Arguments](http://i.imgur.com/yvsQb1Y.png)

### CALL WEB API SERVICE ###
To call the Web API, the *HttpSend* activity is used.

The *HttpSend* activity’s *Uri* is set to the webServiceUrl variable which is passed to the workflow on startup.

![HttpSend activity](http://i.imgur.com/uCJ6hEJ.png)

The **Method** is set to POST; the **RequestContent** is set to “=” + country.  The country is a variable obtained from the current list item the workflow is interacting with.

![Variables](http://i.imgur.com/IGAhAGs.png)

The **RequestHeaders** are set to pass the appWebUrl and contextToken.  These are the variables that were initially passed into the workflow startup method.  Other header values are set to facilitate the request

![Request headers](http://i.imgur.com/Zp2pb6n.png)

### START THE WORKFLOW ###
In *PartSuppliersController*, the *add-in web URL*, *web service URL* and *context token* variables are packaged into another variable named payload. 

```C#
// PartSuppliersController
[HttpPost]
[SharePointContextFilter]
public ActionResult StartWorkflow(int id, Guid workflowSubscriptionId, string spHostUrl)
{
    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext) as SharePointAcsContext;

    var webServiceUrl = Url.RouteUrl(
        "DefaultApi", 
        new { httproute = "", controller = "Data" },
        Request.Url.Scheme);

    var payload = new Dictionary<string, object>
        {
            { "appWebUrl", spContext.SPAppWebUrl.ToString() },
            { "webServiceUrl", webServiceUrl },
            { "contextToken",  spContext.ContextToken }
        };

    using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
    {
        var service = new PartSuppliersService(clientContext);
        service.StartWorkflow(workflowSubscriptionId, id, payload);
    }
    //…
}
```

The payload variable is passed to the *PartSuppliersService.StartWorkflow* method.

In the *PartSuppliersService*, the workflow is started with the payload variable. The 3 values in the payload variable are passed to the StartWorkflowOnListItem method.

```C#
// PartSuppliersService
public void StartWorkflow(
    Guid subscriptionId, int itemId, Dictionary<string, object> payload)
{
    var workflowServicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);

    var subscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
    var subscription = subscriptionService.GetSubscription(subscriptionId);

    var instanceService = workflowServicesManager.GetWorkflowInstanceService();
    instanceService.StartWorkflowOnListItem(subscription, itemId, payload);
    clientContext.ExecuteQuery();
}
```

# DEPLOYMENT GUIDE #
## DEPLOY THE PROVIDER HOSTED WEB SITE ##
### CREATE A WEB SITE IN MICROSOFT AZURE ###
Open https://manage.windowsazure.com.

Login to you Microsoft azure account.

![Login to Azure Portal](http://i.imgur.com/eZpe3LH.png)

Click **+New** at the bottom left.

![Creation of new web app](http://i.imgur.com/qtszyPe.png)

![Confirmation of details](http://i.imgur.com/JFkHPE2.png)

Click **Computer**, click **WEB SITE**, click **QUICK CREATE**, and input a URL. 

Here, we input *WorkflowCallCustomService*, and the web site will be created at the following URL:

-  *WorkflowCallCustomService.azurewebsites.net*

You will need to input a different url. Please remember the url. You are going to use it later.

Click **CREATE WEB SITE** at the bottom right.

Wait for a while, the new web site will be created.

Click the name of the web site.

![Selection of web site](http://i.imgur.com/iv01yqM.png)

![Web site details](http://i.imgur.com/Ye7OI9I.png)

Click **DOWNLOAD THE PUBLISH PROFILE** under the **PUBLISH YOUR ADD-IN**.

![Download publish profile button](http://i.imgur.com/M0f35qG.png)

Save the file.

### PUBLISH ADD-IN WEB SITE ###
Open the *Workflow.CallCustomService.sln* file with Visual Studio 2013. 

In Solution Explorer, right click the *Workflow.CallCustomService* project.

Click **Publish…**

![Publish selection in context menu](http://i.imgur.com/vcw4dsd.png)

Click the drop down button, then click **<New…>**

![Selection of profile](http://i.imgur.com/wPJP1fF.png)

Select **Import publishing profile**, then click **Browse...**. Choose the publish settings file you previously downloaded. Click **Next**.

![Selecting publish settings file from file system](http://i.imgur.com/agcsNJX.png)

Input the **Client ID** and **Client Secret** shown below:
-  **Client Id:** ce2a0e5b-8497-4ec2-8544-dcbd26eb061a
-  **Client Secret:** QXVIbuyUnwKGWv/zhqPqkQclEV3EDmuvEuiO4Vr9Yl8=

Click **Finish**.

![Confirmed settings](http://i.imgur.com/TisoBy0.png)

Click **Deploy your web project**.

![Deployment of web app](http://i.imgur.com/MIwYcNC.png)

Click **Publish**. 

![Publishing web app](http://i.imgur.com/MZamiKk.png)

![Confirmation](http://i.imgur.com/3Vm985v.png)

 
In a few minutes, the site will be published to Microsoft Azure. 

## DEPLOY THE ADD-IN ##
### PACKAGE THE ADD-IN ###
Click **Package the add-in**.

![Pacakge add-in UI](http://i.imgur.com/2lvhEZt.png)

Modify the URL, add the letter **‘s’** after ‘http’.

Click **Finish**. A Windows Explorer window will pop up and display the .app file you just generated.

![Add-in details](http://i.imgur.com/HgdfaG2.png)

![Confirmation](http://i.imgur.com/FSO2iyB.png)

### REGISTER THE ADD-IN ###
Login to the O365 site where you want to install the add-in.

Change the Url to:

https://tenancy.sharepoint.com/sites/site/_layouts/15/appregnew.aspx

Replace the **tenancy** placeholder in the URL with your tenancy name.
Replace the **site** placeholder with your site collection name.

![appregnew.aspx page](http://i.imgur.com/cYXfPUV.png)

In this step, ```you should use the domain of you Windows Azure web site, and add prefix “https” as the Redirect URL```.

Input these values in the form:
-  **ClientId:** ce2a0e5b-8497-4ec2-8544-dcbd26eb061a
-  **ClientSecret:** QXVIbuyUnwKGWv/zhqPqkQclEV3EDmuvEuiO4Vr9Yl8=
-  **Title:** Workflow.CallCustomService
-  **AppDomain:** WorkflowCallCustomService.azurewebsites.net ```(Use Your AppDomain)```
-  **Redirect URL:** https://WorkflowCallCustomService.azurewebsites.net ```(Use Your AppDomain)```

Then, click **Create**.

![Confirmation of client ID and secret](http://i.imgur.com/zvCqGbG.png)

### CREATE AN ADD-IN CATALOG SITE ###
If you don’t have an add-in Catalog site in your SharePoint Online tenant, you should create one. If there’s already an add-in Catalog in your tenant, please skip this step.

Sign in to the Office 365 admin center with your SharePoint Online admin user name and password.

Click **apps**.

![Apps selection in admin UI](http://i.imgur.com/VYBjOjR.png)

Click **Add-In Catalog**.

![Creation of add-in catalog](http://i.imgur.com/LSQoV7C.png)

Click **OK**.

![Creation details](http://i.imgur.com/q10lFZn.png)

Input the required fields. Click **OK**.

![Confirmation](http://i.imgur.com/d3QY8UV.png)

A few minutes later, the add-in Catalog site will be ready.

### UPLOAD THE  ADD-IN TO ADD-IN CATALOG ###
Login to the add-in catalog site.

Click **Add-ins for SharePoint**.

![Add-in catalog](http://i.imgur.com/dYqSbFo.png)

Click **upload**.

![Uploading UI](http://i.imgur.com/WOe0cTG.png)

Click **Browse…**, and choose the .app file you previously created. Then click **OK**.

![Selection of app file](http://i.imgur.com/FQAbCYR.png)

![Confirmation](http://i.imgur.com/DhdysJ2.png)

Click **Save**.

### INSTALL THE ADD-IN ###
Login to the O365 site where you want to install the add-in.

Click on the wheel at the top right, then click **Add an add-in**.

![Add an add-in UI](http://i.imgur.com/LeIJpli.png)

Click **Workflow.CallCustomService**.

![Selection of right add-in](http://i.imgur.com/hEDxHIA.png)

Click **Trust It**.

![Trust consent](http://i.imgur.com/ZJyXG9q.png)

The add-in will be installed in a few minutes.

![Add-in installed](http://i.imgur.com/t1KpHT9.png)

Once the add-in is installed, click the add-in to load it and follow the instructions in the add-in to run the sample.

![Add-in UI](http://i.imgur.com/lGr6Ts8.png)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Workflow.CallCustomService" />
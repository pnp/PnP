# Call custom web service update SharePoint via web proxy #

### Summary ###
This sample shows how to create a workflow that calls a custom web service that updates SharePoint list data via a web proxy.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Workflow.CallServiceUpdateSPViaProxy | Todd Baginski, Tyler Lu, Romy Ji, (**Canviz LLC**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 16th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO: CALL CUSTOM WEB SERVICE #
This provider-hosted sample application for SharePoint demonstrates how to create a workflow that calls a custom web service that updates SharePoint list data via a web proxy.

## WEB API SERVICE ##

In this code sample, we use the *DataController* controller. It contains a method called Post which is used to handle the workflow’s http post request. 

The *Post* method updates the *Suppliers* column in the *Part Suppliers* SharePoint list in the add-in web.

```C#
public class DataController : ApiController
{
    public void Post(UpdatePartSupplerModel model)
    {
        var request = HttpContext.Current.Request;
        var authority = request.Url.Authority;
        var spAppWebUrl = request.Headers["SPAppWebUrl"];
        var accessToken = request.Headers["X-SP-AccessToken"];

        using (var clientContext = TokenHelper.GetClientContextWithContextToken(spAppWebUrl, accessToken, authority))
        {
            var service = new PartSuppliersService(clientContext);
            service.UpdateSuppliers(
                model.Id,
                model.Suppliers.Select(s => s.CompanyName));
        }
    }
}
```

In the above method, 2 values are required:
1. Add-in web URL.
2. Access token.

The workflow calls this Web API method via a Web Proxy. The Web Proxy adds the *access token* to the http headers. So we only have to send *add-in web URL* to the Web API form workflow.

## WORKFLOW ##
### WORKFLOW ARGUMENTS ###
The workflow needs to send the *add-in web URL* to the web API.  When the workflow starts the *add-in web URL* is passed to it. The *Web API’s URL* is also passed to the workflow.  Two arguments are created in the workflow to receive these values. 

![Arguments](http://i.imgur.com/c1j2UsL.png)

### CALL NORTHWIND ODATA SERVICE ###
The [Northwind OData Service](http://services.odata.org/V3/Northwind/Northwind.svc/Suppliers) supports anonymous access, therefore it may be called without authenticating.

![Nortwind service](http://i.imgur.com/uf6bItN.png)

To call the Northwind OData Service, the *HttpSend* activity is used.  The **Uri** is:

```JavaScript
"http://services.odata.org/V3/Northwind/Northwind.svc/Suppliers/?$filter=Country eq '" + country.Replace("'", "''") + "'&$select=CompanyName"
```

The *Get **Method*** is used to return data from the Northwind Odata Service.

![Selecting Get method](http://i.imgur.com/exO6dn5.png)

To handle the response, an Accept header is added to the **RequestHeaders**:

![Adding request headers](http://i.imgur.com/UiJvgNL.png)

After the call to the Northwind OData Service, the response looks like this:


```JavaScript
{
    value: [
        {
            CompanyName: "Ma Maison"
        },
        {
            CompanyName: "Forêts d'érables"
        }
    ]
}
```

A **GetDynamicValueProperty<DynamicValue>** Activity is added to the workflow to handle the response.  It’s **PropertyName** to ‘value’.

![Property window](http://i.imgur.com/PYE6Dgj.png)

The **suppliers** variable value looks like this:

```JavaScript
[
    {
        CompanyName: "Ma Maison"
    },
    {
        CompanyName: "Forêts d'érables"
    }
]
```

### CALL WEB API SERVICE VIA WEB PROXY ###
The Web Proxy’s URL is:

```JavaScript
appWebUrl + "/_api/SP.WebProxy.invoke"
```

#### Create Custom Service Payload ####
To call the Web Proxy, the workflow uses a **BuildDynamicValue** Activity to build the payload. 

![Activity](http://i.imgur.com/UKlWKPO.png)

The variable **customServicePayload**’s value looks like this:

```JavaScript
{
    Id: 1
    Suppliers: [
        {
            CompanyName: "Ma Maison"
        },
        {
            CompanyName: "Forêts d'érables"
        }
    ]
}
```

#### Create Web Proxy Payload ####
A **BuildDynamicValue** Activity builds the payload.

![Payload activity](http://i.imgur.com/owP6DOU.png)

![Payload details](http://i.imgur.com/MmfVcIw.png)

Its value looks like this:

```JavaScript
{          
    requestInfo: {       
        __metadata: {
            type: "SP.WebRequestInfo" 
        },     
        Url: /* url */,         
        Method: "GET",     
        Headers: {              
            results: [
                {           
                    __metadata: {
                        type: "SP.KeyValue" 
                    },     
                    Key: "SPAppWebUrl",               
                    Value: /* add-in web url */,    
                    ValueType: "Edm.String"                      
                },                
                {           
                    __metadata: {
                        type: "SP.KeyValue" 
                    },     
                    Key: "Accept",               
                    Value: "application/json;odata=nometadata",    
                    ValueType: "Edm.String"                      
                },
                {           
                    __metadata: {
                        type: "SP.KeyValue" 
                    },     
                    Key: "Content-Type",               
                    Value: "application/json;odata=nometadata",    
                    ValueType: "Edm.String"                      
                },
                {           
                    __metadata: {
                        type: "SP.KeyValue" 
                    },     
                    Key: "Content-Length",               
                    Value: /* content length */    
                    ValueType: "Edm.String"                      
                }
            ]               
        }           
    }       
}
```

#### Call Web API via Web Proxy ####

![HttpSend Activity](http://i.imgur.com/TpkoK2R.png)

Set **Method** to POST. Set **RequestContent** to webProxyPayload. Set Uri to webProxyUrl.

Then set **RequestHeaders** as below.

![Request header settings](http://i.imgur.com/Twn7LdO.png)

### START THE WORKFLOW ###
In the *PartSuppliersController*, the add-in web URL and web service URL are set to a variable named payload. 

```C#
// PartSuppliersController
[HttpPost]
[SharePointContextFilter]
public ActionResult StartWorkflow(int id, Guid workflowSubscriptionId, string spHostUrl)
{
    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

    var webServiceUrl = Url.RouteUrl(
        "DefaultApi", 
        new { httproute = "", controller = "Data" },
        Request.Url.Scheme);

    var payload = new Dictionary<string, object>
        {
            { "appWebUrl", spContext.SPAppWebUrl.ToString() },
            { "webServiceUrl", webServiceUrl }
        };

    using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
    {
        var service = new PartSuppliersService(clientContext);
        service.StartWorkflow(workflowSubscriptionId, id, payload);
    }
    //…
}
```

The variable is passed to the *PartSuppliersService.StartWorkflow* method.

In the *PartSuppliersService*, the workflow is started and the payload is passed to it.  The 2 values in the payload are passed to the workflow via startup arguments.

```C#
// PartSuppliersService
public void StartWorkflow(Guid subscriptionId, int itemId, Dictionary<string, object> payload)
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

![Signing in to Azure Portal](http://i.imgur.com/OUQTmQu.png)

Login in to you Microsoft azure account.

![Login UI](http://i.imgur.com/jKXv1Oy.png)

Click **+New** at the bottom left.

![New option](http://i.imgur.com/VIm3JYA.png)
 
Click **Computer**, click **WEB SITE**, click **QUICK CREATE**, and input a URL. 

Here, we input *WorkflowCallCustomService*, and the web site will be created at the following URL:
-  *WorkflowCallCustomService.azurewebsites.net*
You will need to input a different url. Please remember the url. You are going to use it later.

Click **CREATE WEB SITE** at the bottom right.

Wait for a while, the new web site will be created.
Click the name of the web site.

![Web site listed under web sites](http://i.imgur.com/LDfyOnM.png)
 
Click **DOWNLOAD THE PUBLISH PROFILE** under the **PUBLISH YOUR ADD-IN**.

![Download the publishign profile UI button](http://i.imgur.com/u9MLFbK.png)

Save the file.

![Saving the file](http://i.imgur.com/SySE5bP.png)

### PUBLISH ADD-IN WEB SITE ###
Open the *Workflow.CallServiceUpdateSPViaProxy.sln* file with Visual Studio 2013. In Solution Explorer, right click the *Workflow.CallServiceUpdateSPViaProxy* project.

![Publish option](http://i.imgur.com/3eOSe1E.png)

Click **Publish…**

![Current profile section](http://i.imgur.com/3OnWwFJ.png)

Click the drop down button, then click **New…**

Select **Import publishing profile**, then click **Browse...**. Choose the publish settings file you previously downloaded. Click **Next.**

![Chosing right file](http://i.imgur.com/2s9HxFL.png)

Input the **Client ID** and **Client Secret** shown below:
-  **Client Id:** 7ad98516-2cdb-48a3-9238-ce369da3a46d
-  **Client Secret:** wBigHZHqZoAN/92BJPue4Kzhx2lMws6wgHiSZpVkqSA=

Click **Finish**.

![Imported profile](http://i.imgur.com/w2rKZvb.png)

Click **Deploy your web project**.

![Deployment of web project](http://i.imgur.com/jCOwBdq.png)

Click **Publish**. 

![Status of deployment](http://i.imgur.com/4kWII2B.png)

![Finished](http://i.imgur.com/tej60oa.png)

In a few minutes, the site will be published to Microsoft Azure. 

## DEPLOY THE ADD-IN ##
### ADD REMOTE ENDPOINT ###

![AppManifest xml selection](http://i.imgur.com/chqi6uO.png)

In the **Soluction Explorer**, click **AppManifest.xml**.

![Add URL](http://i.imgur.com/VtphdOo.png)

Input the url of your Microsoft Azure website that you previously created.
Make sure the URL starts with **‘https’**. Then click **Add.**

![Set URL as deployed add-in](http://i.imgur.com/AY5Vlwn.png)

### PACKAGE THE ADD-IN ###
Click **Package the add-in**.

![Package wizard](http://i.imgur.com/gXtk1FI.png)

Modify the URL, add the letter **‘s’** after ‘http’.
Click **Finish**. A Windows Explorer window will pop up and display the .app file you just generated.

![Target option](http://i.imgur.com/TyudQux.png)

![Finish deployment](http://i.imgur.com/qKQNJAG.png)

### REGISTER THE ADD-IN ###
Login to the O365 site where you want to install the add-in.
Change the Url to:
https://tenancy.sharepoint.com/sites/site/_layouts/15/appregnew.aspx
Replace the **tenancy** placeholder in the URL with your tenancy name.
Replace the **site** placeholder with your site collection name.

![Appregnew.asxp page](http://i.imgur.com/GoOR6xr.png)

In this step, ```you should use the domain of your Windows Azure web site, and add the prefix “https” to the Redirect URL```.
Input these values in the form:
-  **ClientId:** 7ad98516-2cdb-48a3-9238-ce369da3a46d
-  **ClientSecret:** wBigHZHqZoAN/92BJPue4Kzhx2lMws6wgHiSZpVkqSA=
-  **Title:** Workflow.CallServiceUpdateSPViaProxy
-  **AppDomain:** WorkflowCallCustomService.azurewebsites.net (```Use Your AppDomain```)
-  **Redirect URL:** https://WorkflowCallCustomService.azurewebsites.net (```Use Your AppDomain```)

Then, click **Create**.

![Registration of add-in](http://i.imgur.com/iRkck9z.png)

### CREATE AN ADD-IN CATALOG SITE ###
If you don’t have an add-in catalog site in your SharePoint Online tenant, you should create one. If there’s already an add-in catalog in your tenant, please skip this step.

Sign in to the Office 365 admin center with your SharePoint Online admin user name and password.
Click **apps**.

![Signing in to admin center](http://i.imgur.com/3DbIvth.png)

Click **Add-In Catalog**.

![Add-in catalog creation](http://i.imgur.com/vbHieZc.png)

Click **OK**.

![Cofnirmation of add-in catalog creation](http://i.imgur.com/uOVxPiB.png)

Input the required fields. Click **OK**.

![Catalog created](http://i.imgur.com/nzcc5An.png)

A few minutes later, the add-in catalog site will be ready.

### UPLOAD THE ADD-IN TO ADD-IN CATALOG ###
Login to the add-in catalog site.
Click **Add-Ins for SharePoint**.

![Catalog UI](http://i.imgur.com/2BBp4FS.png)

Click **upload**.

![Upload UI](http://i.imgur.com/5XEAsUO.png)

Click **Browse…**, and choose the .app file you previously created. Then click **OK**.

![Selection of .app file](http://i.imgur.com/zaxYUQV.png)

Click **Save**.

![Uploaded add-in](http://i.imgur.com/kfHqtzh.png)

### INSTALL THE ADD-IN ###
Login to the O365 site where you want to install the add-in.
Click at the wheel at the top right, then click **Add an add-in**.

![Add-in add](http://i.imgur.com/VMKt9Ab.png)

Click **Workflow.CallServiceUpdateSPViaProxy**.

![Selection of right add-in](http://i.imgur.com/uJUkmVw.png)

Click **Trust It**. 

![Trust consent](http://i.imgur.com/Iptta5s.png)

The add-in will be installed in a few minutes.

![Installation](http://i.imgur.com/S5u464d.png)

Once the add-in is installed, click the add-in to load it and follow the instructions in the add-in to run the sample.

![Add-in UI](http://i.imgur.com/SII5XvH.png)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Workflow.CallServiceUpdateSPViaProxy" />
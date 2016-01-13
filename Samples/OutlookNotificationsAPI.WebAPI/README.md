# Outlook Notifications REST API with ASP.NET Web API #

### Summary ###
This is a sample of an ASP.NET Web API project validating and responding to Outlook Notifications - created with the Outlook Notifications REST API.

As always, include a valid authorization header when making the request. You can learn more about the Outlook Notifications REST API and its operations at: <https://msdn.microsoft.com/en-us/office/office365/api/notify-rest-operations>

Using this event driven approach is a much more solid way of dealing with changes in the resources and entities in Outlook. As opposed to polling the Outlook REST APIs directly, this is much more lightweight (especially when the amount of items is large). With scale, this approach becomes essential for a sustainable service architecture.

Read more about this sample at: <http://simonjaeger.com/call-me-back-outlook-notifications-rest-api>

### Applies to ###
-  Exchange Online
-  Office 365
-  Hotmail.com
-  Live.com
-  MSN.com
-  Outlook.com
-  Passport.com

### Prerequisites ###
The Outlook Notifications REST API is available for multiple services. You will need to register your app before you can make any calls towards the Outlook Notifcations REST API. Find more information: <https://dev.outlook.com/RestGettingStarted>

If you are building for Office 365 and you're missing an Office 365 tenant - get yourself a developer account at: <http://dev.office.com/devprogram>

Lastly, you will need to host and deploy your Web API, for instance to a web app on Microsoft Azure: <https://azure.microsoft.com/en-us/documentation/scenarios/web-app/>.

### Solution ###
Solution | Author(s)
---------|----------
OutlookNotificationsAPI.WebAPI | Simon Jäger (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.1  | January 13th 2016 | Added UI
1.0  | December 12th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# How to Use? #
The first step is to create and host your Web API somewhere – it needs to be deployed and validated by the Outlook Notifications REST API before we can get notifications sent to it. In terms of validation, it’s pretty straight forward. When we ask the Outlook Notifications REST API to start sending notifications (by creating a subscription) to your Web API – it will go ahead and send a validation token to it. 

The Web API needs to respond with the same validation token within 5 seconds, if it can achieve that – a subscription for notifications will be created and returned to the client application (creating the subscription).

#### Register in Azure AD ####

Your first step is to register your web application in your Azure AD tenant (associated with your Office 365 tenant). The web application is using OWIN and OpenID Connect to handle authentication and authorization.You can find more details about OWIN and OpenId Connect here, as well as about registering you app on the Azure AD tenant here: <http://www.cloudidentity.com/blog/2014/07/28/protecting-an-mvc4-vs2012-project-with-openid-connect-and-azure-ad/>

When you have registered your web application in Azure AD - you will have to configure the following settings in the Web.config file:

    <add key="ida:ClientId" value="[YOUR APPLICATION CLIENT ID]" />
    <add key="ida:ClientSecret" value="[YOUR APPLICATION CLIENT SECRET]" />
    <add key="ida:Domain" value="[YOUR DOMAIN]" />
    <add key="ida:TenantId" value="[YOUR TENANT ID]" />
    <add key="ida:PostLogoutRedirectUri" value="[YOUR POST LOGOUT REDIRECT URI]" />
    
#### Deploy ####

Deploy your Web API to a hosting provider, for instance a web app on Microsoft Azure: <https://azure.microsoft.com/en-us/documentation/scenarios/web-app/>.

Once you have deployed the sample to a hosting provider; configure a breakpoint to catch and validate the flow in the Web API (NotifyController). After the validation occurs, you will receive notifications and be able to investigate the responses.

You can use Visual Studio 2015 to attach a debugger to an Azure web app (see <https://azure.microsoft.com/sv-se/documentation/articles/web-sites-dotnet-troubleshoot-visual-studio/#remotedebug>)

**Be aware: If you are using remote debugging; delays may cause you to break the 5 second response time when validation notification URLs.**

Navigate to your hosted sample and click on the "Register Subscription" button to start getting notifications.

# Response Models #
The following models are implemented in the sample. They serve to help out when dealing with the notification requests (parsing the received JSON).

The generic ResponseModel class is the main container for the response itself. In the sample it will contain a collection of the NotificationModel class.

```C#
public class ResponseModel<T>
{
    public List<T> Value { get; set; }
}
```
The NotificationModel class represents the notification sent to your listener service (Web API).

```C#
public class NotificationModel
{
    public string SubscriptionId { get; set; }
    public string SubscriptionExpirationDateTime { get; set; }
    public int SequenceNumber { get; set; }
    public string ChangeType { get; set; }
    public string Resource { get; set; }
    public ResourceDataModel ResourceData { get; set; }
}
```
The ResourceDataModel class represents the entity (i.e. mail, contact, event) that has triggered a change. This is a navigation property. 

```C#
public class ResourceDataModel
{
    public string Id { get; set; }
}
```

# Web API Controller #
The NotifyController implements a single POST method. Both the validation and notification requests will be sent as POST messages to your Web API.

As for the validation token, it will accept it as an optional parameter. If it’s present in the request, we know that a validation of the URL (Web API) is happening. If not, we can assume that we’re getting a notification from an active subscription.
So if a validation token parameter is present, we return it right away in the proper way – by setting the content type header to text/plain and return HTTP 200 as the response code.

As for no present validation token in the request, we can start parsing the request body and look for notifications. 

```C#
public async Task<HttpResponseMessage> Post(string validationToken = null)
{
    // If a validation token is present, we need to respond within 5 seconds.
    if (validationToken != null)
    {
        var response = Request.CreateResponse(HttpStatusCode.OK);
        response.Content = new StringContent(validationToken);
        return response;
    }

    // Present only if the client specified the ClientState property in the 
    // subscription request. 
    IEnumerable<string> clientStateValues;
    Request.Headers.TryGetValues("ClientState", out clientStateValues);

    if (clientStateValues != null)
    {
        var clientState = clientStateValues.ToList().FirstOrDefault();
        if (clientState != null)
        {
            // TODO: Use the client state to verify the legitimacy of the notification.
        }
    }

    // Read and parse the request body.
    var content = await Request.Content.ReadAsStringAsync();
    var notifications = JsonConvert.DeserializeObject<ResponseModel<NotificationModel>>(content);

    // TODO: Do something with the notification.

    return new HttpResponseMessage(HttpStatusCode.OK);
}
```

I recommend you to pay attention to the client state header in the request (named ClientState). If you create the subscription with a client state property, it will be passed along with the notification request. This way you can verify the legitimacy of the notification.

# Source Code Files #
The key source code files in this project are the following:

- `OutlookNotificationsAPI.WebAPI\Controllers\NotifyController.cs` - the Web API Controller containing the single POST method (handling both validation and notification requests).
- `OutlookNotificationsAPI.WebAPI\Controllers\HomeController.cs` - the Web API Controller containing the registration action that configures the subscription for notifications.
- `OutlookNotificationsAPI.WebAPI\Models\ResponseModel.cs` - represents the collection of entities sent in the notification request to your listener service (Web API).
- `OutlookNotificationsAPI.WebAPI\Models\NotificationModel.cs` - represents the notification entity sent to your listener service (Web API).
- `OutlookNotificationsAPI.WebAPI\Models\ResourceDataModel.cs` - represents the entity (i.e. mail, contact, event) that has triggered a change. This is a navigation property. 

# More Resources #
- Discover Office development at: <https://msdn.microsoft.com/en-us/office/>
- Get started on Microsoft Azure at: <https://azure.microsoft.com/en-us/>
- Learn about webhooks at: <http://culttt.com/2014/01/22/webhooks/>
- Explore the Outook Notifications REST API and its operations at: <https://msdn.microsoft.com/en-us/office/office365/api/notify-rest-operations> 
- Read more about this sample at: <http://simonjaeger.com/call-me-back-outlook-notifications-rest-api/>